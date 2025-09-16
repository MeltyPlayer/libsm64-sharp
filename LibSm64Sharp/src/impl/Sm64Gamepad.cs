namespace libsm64sharp;

public partial class Sm64Context {
  private class Sm64Gamepad : ISm64Gamepad {
    public ISm64Vector2f AnalogStick { get; } = new Sm64Vector2f();
    public ISm64Vector2f CameraNormal { get; } = new Sm64Vector2f();

    public bool IsAButtonDown { get; set; }
    public bool IsBButtonDown { get; set; }
    public bool IsZButtonDown { get; set; }
  }
}