shader_type canvas_item;

uniform vec4 bg_color : source_color = vec4(0.95, 0.95, 0.95, 1.0);
uniform vec4 accent_color : source_color = vec4(0.0, 0.4, 0.8, 1.0); 
uniform float speed : hint_range(0.0, 0.5) = 0.02;
uniform float angle : hint_range(0.0, 3.14) = 0.5;
uniform float line_width : hint_range(0.01, 1.0) = 0.1;

void fragment() {
    vec2 uv = UV;
    // Wir nutzen SCREEN_PIXEL_SIZE für eine perfekte Kantenglättung (Antialiasing)
    vec2 pixel_size = fwidth(uv); 
    float time = TIME * speed;
    
    vec2 dir1 = vec2(cos(angle), sin(angle));
    vec2 dir2 = vec2(cos(angle + 1.5), sin(angle + 1.5));
    
    // Wir nutzen große Offsets, damit die Flächen weit auseinander liegen
    float val1 = dot(uv, dir1) + time;
    float val2 = dot(uv, dir2) - time * 0.8;

    // --- Der Anti-Flicker-Trick ---
    // Statt step(0.5, val) nutzen wir smoothstep basierend auf der Pixelgröße.
    // Das ergibt eine perfekte 1-Pixel-Kante.
    float aa = length(pixel_size) * 1.5; 
    float mask1 = smoothstep(0.5, 0.5 + aa, fract(val1 * 0.3));
    float mask2 = smoothstep(0.4, 0.4 + aa, fract(val2 * 0.2));
    
    // --- Ebenen-Logik ---
    vec3 color = bg_color.rgb;
    
    // Ebene 1: Eine dezente graue Schattenfläche (für Tiefe)
    // Wir dunkeln das Weiß nur leicht ab, damit man sieht, dass sich Flächen überlagern
    float shadow = smoothstep(0.3, 0.3 + aa, fract(val1 * 0.3 + 0.05));
    color = mix(color, bg_color.rgb * 0.92, shadow * 0.5);
    
    // Ebene 2: Die Akzentfarbe
    // Wir nutzen "max", damit sie über dem Schatten liegt
    color = mix(color, accent_color.rgb, mask1);
    
    // Ebene 3: Eine zweite Akzent-Nuance oder weißes Highlight
    // Hier mischen wir die zweite Maske ein, um geometrische Kreuzungen zu erzeugen
    color = mix(color, accent_color.rgb * 1.1, mask2 * mask1); 
    
    // Damit Weiß nicht auf Weiß liegt: Ein dezenter dunkler Rand an den Kanten
    float line_edge = smoothstep(0.5 + aa, 0.5, fract(val1 * 0.3)) * smoothstep(0.49, 0.49 + aa, fract(val1 * 0.3));
    color -= line_edge * 0.05;

    COLOR = vec4(color, 1.0);
}