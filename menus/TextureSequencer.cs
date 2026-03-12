using Godot;
using System;
using Manufaktur.menus;

public partial class TextureSequencer : TextureRect
{
	[Export] public TextureStep[] Sequence;
	[Export] public PackedScene NextLevel;

	private int _currentIndex = 0;
	private float _timer = 0.0f;

	public override void _Ready()
	{
		if (Sequence != null && Sequence.Length > 0)
		{
			ApplyCurrentStep();
		}
	}

	public override void _Process(double delta)
	{
		if (Sequence == null || Sequence.Length == 0 || _currentIndex >= Sequence.Length) 
			return;

		_timer += (float)delta;


		if (_timer >= Sequence[_currentIndex].Duration)
		{
			_timer = 0.0f;
			_currentIndex++;

			if (_currentIndex < Sequence.Length)
			{
				ApplyCurrentStep();
			}
			else
			{
				ChangeScene();
			}
		}
	}

	private void ApplyCurrentStep()
	{
		var currentStep = Sequence[_currentIndex];
		if (currentStep != null)
		{
			Texture = currentStep.Image;
		}
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
			{
				ChangeScene();
			}
		}
	}


	private void ChangeScene() {
		GetTree().ChangeSceneToPacked(NextLevel);
	}
}
