using Godot;
using System;

[GlobalClass]
public partial class Radio : Node
{
	public static Radio Instance { get; private set; }

	[Export]
	public string MusicBus { get; set; } = "Music";

	[Export]
	public string UiSfxBus { get; set; } = "UI";

	[Export]
	public float MusicFadeDuration { get; private set; } = 0.5f;
	
	private int _lastIndex = -1;
	private Random _random = new Random();

	private AudioStream _currentlyPlayingSong = null;
	private int _currentMusicVolume = 100;

	private AudioStreamPlayer _currentMusicPlayer = null;
	private Tween _currentTween = null;
	private bool _currentTweenHasCallbacks = false;

	public override void _Ready()
	{
		Instance = this;
	}

	public void PlayUiSfx(AudioStream sfx, int volume = 100) {
		PlaySfx(sfx, volume, UiSfxBus);
	}

	/// <summary>
	/// Set playing song. Volume in range 0 - 100.
	/// </summary>
	/// <param name="song"></param>
	/// <param name="volume"></param>
	public void PlayMusic(AudioStream song, int volume = 100) {
		if (_currentlyPlayingSong != song) {
			StartSong(song, volume);
		} else if (_currentlyPlayingSong == song && volume != _currentMusicVolume) {
			SetMusicVolume(volume);
		}
	}

	/// <summary>
	/// Volume in range 0 - 100
	/// </summary>
	/// <param name="volume"></param>
	public void SetMusicVolume(int volume) {
		if (volume != _currentMusicVolume) {
			ClearTween();
			var tween = CreateTween();
			tween.TweenProperty(_currentMusicPlayer, "volume_linear", (float) volume / 100f, MusicFadeDuration);
			_currentMusicVolume = volume;
			_currentTween = tween;
		}
	}

	private void StartSong(AudioStream song, int volume) {
		if (_currentMusicPlayer != null) {
			ClearTween();
			var tween = _currentMusicPlayer.CreateTween();
			tween.TweenProperty(_currentMusicPlayer, "volume_linear", 0, MusicFadeDuration);
			tween.TweenCallback(Callable.From(_currentMusicPlayer.Stop));
			tween.TweenCallback(Callable.From(_currentMusicPlayer.QueueFree));
			_currentTween = tween;
			_currentMusicPlayer = null;
			_currentTweenHasCallbacks = true;
		}

		AudioStreamPlayer player = new();

		player.Stream = song;
		player.Bus = MusicBus;
		player.PlaybackType = AudioServer.PlaybackType.Stream;
		player.Autoplay = true;
		player.ProcessMode = ProcessModeEnum.Always;

		AddChild(player);
		_currentMusicPlayer = player;
		_currentlyPlayingSong = song;
	}

	private void ClearTween() {
		if (_currentTween != null) {
			if (_currentTweenHasCallbacks) {
				_currentTween.Pause();
				_currentTween.CustomStep(10);
				_currentTweenHasCallbacks = false;
			}
			_currentTween.Stop();
			_currentTween.Kill();
			_currentTween = null;
		}
	}

	private async void PlaySfx(AudioStream sfx, int volume, string bus) {
		AudioStreamPlayer player = new();

		player.Stream = sfx;
		player.Bus = bus;
		player.Autoplay = true;
		player.ProcessMode = ProcessModeEnum.Always;

		if (volume != 100) {
			player.VolumeLinear = volume / 100f;
		}

		AddChild(player);

		var duration = sfx.GetLength();
		await ToSignal(GetTree().CreateTimer(duration + 0.1f), "timeout");
		player.QueueFree();
	}
	
	public void PlayRandomSound(AudioStream[] pool, string bus, int volume = 100) {
		if (pool == null || pool.Length == 0) return;
		int randomIndex = _random.Next(0, pool.Length);
		while (randomIndex == _lastIndex && pool.Length > 1) {
			randomIndex = _random.Next(0, pool.Length);
		}

		_lastIndex  = randomIndex;
		var stream = pool[randomIndex];
		PlaySfx(stream, volume, bus);
	}
	
	public void PlayUiSound(AudioStream stream, string bus, int volume = 100) {
		if (stream == null) return;
		PlaySfx(stream, volume, bus);
	}
}
