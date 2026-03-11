shader_type canvas_item;

uniform sampler2D screen_tex : hint_screen_texture, filter_linear_mipmap;
uniform sampler2D noise : repeat_enable;

uniform float seed = 0.0;

uniform float shatter_progress : hint_range(0.0,1.0) = 0.0;

uniform float refraction_strength = 0.3;

uniform vec3 glass_color : source_color = vec3(2,2,2);

uniform vec3 vignette_color_inner : source_color = vec3(0.2,0,0);
uniform vec3 vignette_color_outer : source_color = vec3(0,0,0);
uniform float vignette_size = 0.1;

const float cell_jitter = 0.9;
const float angular_segs = 13.0;

vec2 hash(vec2 p) {
	p = mod(p, angular_segs);
	return texture(noise, (p + 0.5) / 200.0 + seed*432.0).xy;
}

vec3 voronoi(in vec2 x, out vec2 cpId) {
	vec2 n = floor(x);
	vec2 f = fract(x);

	vec2 mg, mr;
	float md = 8.0;

	// nearest cell
	for(int j=-1;j<=1;j++) {
		for(int i=-1;i<=1;i++) {
			vec2 g = vec2(float(i),float(j));
			vec2 o = cell_jitter * hash(n + g);
			vec2 r = g + o - f;
			float d = dot(r,r);

			if(d < md) {
				md = d;
				mr = r;
				mg = g;
			}
		}
	}

	// border distance
	md = 8.0;
	for(int j=-2;j<=2;j++) {
		for(int i=-2;i<=2;i++) {
			vec2 g = mg + vec2(float(i),float(j));
			vec2 o = cell_jitter * hash(n + g);
			vec2 r = g + o - f;

			if(dot(mr-r,mr-r) > 0.000001) {
				float d = dot(0.5*(mr+r), normalize(r-mr));
				md = min(md, d);
			}
		}
	}

	cpId = n + mg;
	return vec3(md, mr);
}

float rand(float n){
	return fract(sin(n + seed)*43758.5453123);
}

float my_noise(float p) {
    float fl = floor(p);
    float fc = fract(p);
    return mix(rand(fl), rand(fl + 1.0), fc);
}

float lowfreqnoise(float x) {
	x = mod(x,1.0);
	float res = 50.;
	float ia = floor(x * res);
	float ib = floor(x * res) + 1.;

	float a = my_noise(mod(ia, res));
	float b = my_noise(mod(ib, res));

	float t = fract(x * res);
	return mix(a, b, t) * 2.0 - 1.;
}

vec3 calcLighting(vec3 N, float opacity, vec3 background) {
	const vec3 lightDir = vec3(0.57735026919, -0.57735026919, 0.57735026919);
	vec3 viewDir = vec3(0,0,1);
	vec3 halfwayDir = normalize(lightDir + viewDir);

	float diff = max(dot(N, lightDir), 0.0);
	vec3 diffuse = diff * vec3(1,1,1);

	float spec = pow(max(dot(N, halfwayDir), 0.0), 128.0);
	vec3 specular = vec3(1.0) * spec;

	float fresnel = pow(1.0 - max(dot(N, viewDir), 0.0), 3.0);

	float alpha = mix(opacity, 1.0, fresnel);
	return mix(background, glass_color * diffuse + 4.0 * specular + 4.0 * (vec3(1,1,1) * fresnel * 0.5), alpha);
}

vec4 scatter_effect(vec2 uv)
{
	vec2 uvCenter = vec2(0.5,0.5);
	vec2 p = uv - uvCenter;
	vec2 screen_size = vec2(textureSize(screen_tex,0));
	p.x *= screen_size.x / screen_size.y;
	p /= 1.2;

	float r = length(p);

	vec2 cyl = vec2(max(0.5,pow(r,0.1)), atan(p.y, p.x) / 6.2831853 + 0.5);
	cyl.x += 0.008 * abs(lowfreqnoise(cyl.y)) * mix(0.5, 1.0, r);
	cyl.y += 0.01 * abs(lowfreqnoise(cyl.x*2.0 + 0.1356)) * mix(0.5, 1.0, r);

	vec2 freq = vec2(17.0, angular_segs);

	vec2 cpid;
	vec3 c = voronoi(cyl * freq, cpid);

	cpid = mod(cpid, angular_segs);

	float borderDist = c.x;

	float e0 = mix(.2,.0,pow(r,.1));
	float edge = smoothstep(e0,e0+0.001,borderDist);

	vec2 dirToCenter = normalize(c.yz);
	float slope = smoothstep(e0*0.1, e0*0.1+0.02, borderDist);
	vec3 N = normalize(vec3(dirToCenter * (1.0 - slope*slope), 1.0));

	vec3 normOffset = vec3(
		rand(cpid.x*7.0),
		rand(cpid.y*13.0),
		rand(27.0*(cpid.x-cpid.y))
	) * 2.0 - 1.0;
	vec3 refractionN = normalize(vec3(0,0,1) + normalize(mix(N, normOffset, 0.5)) * 0.5);
	float normal_progress = smoothstep(0.25, 0.5, shatter_progress);
	N = mix(vec3(0,0,1), N, step(0.5, pow(1.0 - r, mix(10.0, 1.0, normal_progress)) + 0.5*normal_progress));
	N = mix(N, refractionN, 0.7*smoothstep(0.5, 1.0, shatter_progress));

	vec3 Ndx = dFdx(N);
	vec3 Ndy = dFdy(N);
	vec3 neighborAvgN = (
		(N + Ndx) + 
		(N - Ndx) + 
		(N + Ndy) + 
		(N - Ndy)
	) * 0.25;
	N = mix(N, neighborAvgN, 0.5);

	// convert normal to screen refraction
	vec2 refractUV = uv + N.xy * refraction_strength * (1.0 - smoothstep(0.9, 1.0, length(uv)));

	vec3 col = texture(screen_tex, refractUV).rgb;

	// lighting
	float opacity = texture(noise, 1.5 * uv * vec2(screen_size.x / screen_size.y, 1.0) + seed*432.0).x;
	col = calcLighting(N, mix(0.02, 0.1, opacity), col);

	return vec4(col, smoothstep(0.0, 0.25, shatter_progress));
}

void fragment() {
	vec2 screen_size = vec2(textureSize(screen_tex, 0));
	
	// sample scatter_effect with blur/smoothing
	const int L = 3;
	const float Lf = float(L);
	vec4 sum = vec4(0.);
	for(int i = 0; i < L; i++) {
		for(int j = 0; j < L; j++) {
			vec2 ofs = vec2(float(i), float(j)) / screen_size;
			sum += scatter_effect(SCREEN_UV + ofs);
		}
	}
	COLOR = sum/(Lf*Lf);
	
	// add a vignette at the screen edge
	float vignette_round = 1.0 - length(SCREEN_UV*2.0 - 1.0)*0.7;
	float vignette_square = 1.0 - max(abs(SCREEN_UV.x*2.0 - 1.0), abs(SCREEN_UV.y*2.0 - 1.0));
	float vignette = mix(vignette_round, vignette_square, 0.6);
	vignette -= texture(noise, 1.8 * SCREEN_UV * vec2(screen_size.x / screen_size.y, 1.0) + seed*432.0).y * (1.2 - smoothstep(0.0, vignette_size, vignette));
	float splatter = 4.0 * (pow(texture(noise, 0.1 * SCREEN_UV * vec2(screen_size.x / screen_size.y, 1.0) + seed*432.0).z, 2.0) + 0.2)
			* (pow(texture(noise, 3.0 * SCREEN_UV * vec2(screen_size.x / screen_size.y, 1.0) + seed*432.0).x, 2.0) + 0.2)
			* pow(vignette_round, 2.0);
	vec3 vignette_color = mix(vignette_color_inner, vignette_color_outer, clamp(abs(vignette)*10.0 - splatter*10.0, 0.0, 1.0));
	vignette -= splatter;
	vignette = 1.0 - pow(smoothstep(0.0, vignette_size, vignette), 0.4);
	COLOR = mix(COLOR, vec4(vignette_color, 1.0), vignette * smoothstep(0.5, 0.8, shatter_progress));
}