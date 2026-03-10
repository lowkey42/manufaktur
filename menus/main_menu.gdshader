shader_type canvas_item;

// ========================
// Eingabetexturen
// ========================

uniform sampler2D to_tex;
uniform float progress : hint_range(0.0, 1.0) = 0.0;
uniform int dir = 0;
// ========================
// Hexagon-Parameter
// ========================
uniform float hex_size : hint_range(0.01, 0.5) = 0.08;
uniform float randomness = 0.05;                       
uniform float edge_softness = 0.01;                    

// ========================
// Hilfsfunktionen
// ========================
float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123);
}

vec2 hextile(inout vec2 p) {
    const vec2 s = vec2(1.0, 1.7320508);
    const vec2 hs = s * 0.5;

    vec2 a = mod(p, s) - hs;
    vec2 b = mod(p - hs, s) - hs;

    vec2 c = dot(a, a) < dot(b, b) ? a : b;
    vec2 id = (c - p + hs) / s;

    p = c;
    return floor(id);
}

float hex(vec2 p, float r) {
    p = abs(p);
    return max(p.x * 0.866025 + p.y * 0.5, p.y) - r;
}

// ========================
// Fragment-Shader
// ========================
void fragment() {
    vec2 uv = UV;
    
    
    vec2 p = uv - 0.5;
    float aspect = (1.0 / SCREEN_PIXEL_SIZE).x / (1.0 / SCREEN_PIXEL_SIZE).y;
    p.x *= aspect * 0.9;

    vec2 hp = p / hex_size;
    vec2 id = hextile(hp);

	float wipe = 0.0;
    if (dir == 0) {
        wipe = (uv.x + uv.y) * 0.5;                 
    } else if (dir == 1) {
        wipe = (1.0 - uv.x + uv.y) * 0.5;           
    } else if (dir == 2) {
        wipe = (uv.x + 1.0 - uv.y) * 0.5;        
    } else {
        wipe = (1.0 - uv.x + 1.0 - uv.y) * 0.5;     
    }
    float spread = 0.2; 
    

    float sweep = progress * (1.0 + spread * 2.0) - spread;
    

    float hex_progress = 1.0 - smoothstep(sweep - spread, sweep + spread, wipe);
    hex_progress = clamp(hex_progress, 0.0, 1.0);
    // -------------------------------------

    float r = hash(id);
    float t = clamp(hex_progress - r * randomness, 0.0, 1.0);
    float h = hex(hp, t * 0.9);
    float mask = smoothstep(edge_softness, -edge_softness, h);


    vec4 from_col = texture(TEXTURE, uv);
    vec4 to_col   = texture(to_tex, uv);
    vec4 final_col = mix(from_col, to_col, mask);

    COLOR = final_col;
}