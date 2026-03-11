shader_type canvas_item;


uniform sampler2D noise: repeat_enable;
uniform vec4 line_color: source_color;
uniform float line_count: hint_range(0.0, 2.0, 0.05) = 0.5;
uniform float line_density: hint_range(0.0, 1.0) = 0.5;
uniform float line_faloff: hint_range(0.0, 1.0) = 0.25;
uniform float mask_size: hint_range(0.0, 1.0) = 0.1;
uniform float mask_edge: hint_range(0.0, 1.0) = 0.5;
uniform float animation_speed: hint_range(1.0, 20.0) = 0.5;

uniform float distortion_power: hint_range(0.0, 0.1) = 0.034;
uniform float line_falloff: hint_range(0.0, 1.0) = 1.0;
uniform float blur_strength: hint_range(0.0, 0.01) = 0.01;
uniform float effect_power: hint_range(0.0, 1.0) = 0.5;

uniform vec3 blur_direction = vec3(0.0, 0.0, 0.0);
uniform int motion_blur_samples : hint_range(4, 32) = 32;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

float inv_lerp(float from, float to, float value){
  return (value - from) / (to - from);
}
vec2 polar_coordinates(vec2 uv, vec2 center, float zoom, float repeat)
{
	vec2 dir = uv - center;
	float radius = length(dir) * 2.0;
	float angle = atan(dir.y, dir.x) * 1.0/(PI * 2.0);
	return mod(vec2(radius * zoom, angle * repeat), 1.0);
}

vec2 rotate_uv(vec2 uv, vec2 pivot, float rotation) {
    float cosa = cos(rotation);
    float sina = sin(rotation);
    uv -= pivot;
    return vec2(
        cosa * uv.x - sina * uv.y,
        cosa * uv.y + sina * uv.x 
    ) + pivot;
}

void fragment(){
	vec2 polar_uv = polar_coordinates(rotate_uv(UV, vec2(0.5), floor(fract(TIME) * animation_speed) ) , vec2(0.5), 0.01, line_count);
	vec3 lines = texture(noise, polar_uv).rgb;
	
	float mask_value = length(UV - vec2(0.5));
	float mask = inv_lerp(mask_size, mask_edge, mask_value * mix(0.7, 1.0, effect_power));
	float result = 1.0 - (mask * line_density);
	result = smoothstep(result, result + line_faloff, lines.r);
	
	vec2 center = vec2(0.5);
	vec2 direction = normalize(UV - center);
	vec2 distorted_uv = UV + direction * result;
	
	// Sample the original texture with distortion
	vec3 base_color = texture(SCREEN_TEXTURE, distorted_uv).rgb;
	
	vec3 blur_color = vec3(0.0);
	float blur_samples = 8.0;
	for(float i = 0.0; i < blur_samples; i++){
		float angle = (i / blur_samples) * PI * 2.0;
		vec2 offset = vec2(cos(angle), sin(angle)) * blur_strength * result;
		blur_color += texture(SCREEN_TEXTURE, distorted_uv + offset).rgb;
	}
	blur_color /= blur_samples;
	
	COLOR.rgb = line_color.rgb*0.8 + mix(base_color, blur_color, result * 0.5) * 0.8;
	COLOR.a = min(line_color.a, result*0.9 * effect_power);
	
	vec2 effective_blur_direction = (blur_direction.xy*0.5 + (UV*2.0-1.0) * blur_direction.z);
	effective_blur_direction *= effect_power * 0.5;
	effective_blur_direction *= length(UV*2.0-1.0);

    vec4 color = vec4(0.0);
    float total_weight = 0.0;
    for (int i = 0; i < motion_blur_samples; i++) {
        // Offset samples along the direction of movement
        float offset = float(i) / float(motion_blur_samples - 1) - 0.5;
        vec2 uv_offset = effective_blur_direction * offset;
        color += texture(SCREEN_TEXTURE, SCREEN_UV + uv_offset);
        total_weight += 1.0;
    }
    COLOR.rgb = mix(color.rgb / total_weight, COLOR.rgb, COLOR.a);
	COLOR.a = 1.0;
}
