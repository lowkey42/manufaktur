using Godot;

namespace Manufaktur.menus;

[GlobalClass]
public partial class TextureStep: Resource {
	[Export] public Texture2D Image;
	[Export] public float Duration = 1.0f;
}
