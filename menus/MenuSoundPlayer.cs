using Godot;
using System;

public partial class MenuSoundPlayer : AudioStreamPlayer {
	[Export] public AudioStream[] SoundPool;

	private int _lastIndex = -1;
	private Random _random = new Random();

	public void PlayRandomSound() {
		if (SoundPool == null || SoundPool.Length == 0) return;
		int randomIndex = _random.Next(0, SoundPool.Length);
		while (randomIndex == _lastIndex && SoundPool.Length > 1) {
			randomIndex = _random.Next(0, SoundPool.Length);
		}

		_lastIndex  = randomIndex;
		this.Stream = SoundPool[randomIndex];
		this.Play();
	}
}
