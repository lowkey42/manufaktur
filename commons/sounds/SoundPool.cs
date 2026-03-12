using Godot;
using System;

[GlobalClass]
public partial class SoundPool : Resource
{
	[Export] public AudioStream[] pool;
}
