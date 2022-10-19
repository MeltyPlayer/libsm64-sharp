namespace libsm64sharp {
  public interface ISm64Gamepad {
    ISm64Vector2<float> AnalogStick { get; }
    ISm64Vector2<float> CameraNormal { get; }

    bool IsAButtonDown { get; set; }
    bool IsBButtonDown { get; set; }
    bool IsZButtonDown { get; set; }
  }
}