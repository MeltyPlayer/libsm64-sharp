namespace libsm64sharp {
  public partial class Sm64Context {
    private class Sm64Gamepad : ISm64Gamepad {
      public ISm64Vector2<float> AnalogStick { get; } =
        new Sm64Vector2<float>();

      public ISm64Vector2<float> CameraNormal { get; } =
        new Sm64Vector2<float>();

      public bool IsAButtonDown { get; set; }
      public bool IsBButtonDown { get; set; }
      public bool IsZButtonDown { get; set; }
    }
  }
}