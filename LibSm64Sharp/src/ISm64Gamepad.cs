namespace libsm64sharp;

public interface ISm64Gamepad {
  ISm64Vector2f AnalogStick { get; }
  ISm64Vector2f CameraNormal { get; }

  bool IsAButtonDown { get; set; }
  bool IsBButtonDown { get; set; }
  bool IsZButtonDown { get; set; }
}