using Godot;

namespace Manufaktur.menus;

public partial class ExitButton : Button {
	public override void _Ready() {
		ButtonDown  += _onButton_Down;
	}
	

	private void _onButton_Down() {
		Radio.Instance.PlayUiSfx(GD.Load<AudioStream>("res://commons/vfx/MenuClose.wav"));
	}
}
