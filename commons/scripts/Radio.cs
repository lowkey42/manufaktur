using Godot;
using System;

[GlobalClass]
public partial class Radio : Node
{
	public static Radio Instance { get; private set; }

	[Export]
	public string MusicBus { get; set; } = "Music";

	[Export]
	public float MusicFadeDuration { get; private set; } = 0.5f;

	private AudioStream _currentlyPlayingSong = null;
	private int _currentMusicVolume = 100;

	private AudioStreamPlayer _currentMusicPlayer = null;
	private Tween _currentTween = null;
	private bool _currentTweenHasCallbacks = false;

	public override void _Ready()
	{
		Instance = this;
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
}
