shader_type canvas_item;

uniform vec4 bg_color : source_color = vec4(0.05, 0.1, 0.4, 1.0);
uniform vec4 line_color : source_color = vec4(0.9, 0.9, 1.0, 1.0);
uniform float grid_size : hint_range(10.0, 500.0) = 100.0; // Größe eines Quadrats in Pixeln
uniform float speed : hint_range(0.0, 0.5) = 0.02;

void fragment() {
    // DER TRICK: fwidth berechnet die Änderung der UV pro Pixel.
    // Damit finden wir heraus, wie viele Pixel breit/hoch das Rect wirklich ist.
    vec2 pixel_pos = UV / fwidth(UV); 
    
    // Animation hinzufügen
    vec2 p = pixel_pos + (TIME * speed * grid_size);
    
    // Hauptgitter (große Quadrate)
    vec2 grid = abs(fract(p / grid_size - 0.5) - 0.5);
    // Linienstärke stabilisieren (immer ca. 1.5 Pixel breit)
    float thick_line = smoothstep(1.5, 0.0, min(grid.x, grid.y) / fwidth(p / grid_size).x);
    
    // Feingitter (5 Unterteilungen)
    vec2 sub_grid = abs(fract(p / (grid_size / 5.0) - 0.5) - 0.5);
    float thin_line = smoothstep(1.0, 0.0, min(sub_grid.x, sub_grid.y) / fwidth(p / (grid_size / 5.0)).x);
    
    // Hintergrundfarbe
    vec3 color = bg_color.rgb;
    
    // Vignette (basierend auf UV, damit sie immer zentriert im Rect bleibt)
    float vignette = smoothstep(0.8, 0.2, length(UV - 0.5));
    color *= (0.8 + 0.2 * vignette);
    
    // Gitter mischen
    color = mix(color, line_color.rgb, thin_line * 0.2); // Dünne Linien 20% Deckkraft
    color = mix(color, line_color.rgb, thick_line * 0.6); // Dicke Linien 60% Deckkraft
    
    COLOR = vec4(color, 1.0);
}