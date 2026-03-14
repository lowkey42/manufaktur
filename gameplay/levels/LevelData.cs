using Godot;

namespace Manufaktur.gameplay.levels;

[GlobalClass]
public partial class LevelData : Resource
{
	[Export] 
	public string LevelName { get; set; } = "Neues Level";
    
	[Export(PropertyHint.File, "*.tscn")] 
	public string ScenePath { get; set; } = "";
    
	[Export] 
	public string HighscoreKey { get; set; } = "level_key";
    
	[Export] 
	public Texture2D PreviewImage { get; set; }

	[Export]
	public string UniqueListName { get; set; } = "";
}
