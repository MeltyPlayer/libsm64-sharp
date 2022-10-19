namespace libsm64sharp {
  public partial class Sm64Context {
    private class Sm64Gamepad : ISm64Gamepad {
      private readonly Sm64Vector2<float> analogStick_ = new();
      private Sm64Vector2<float>? scheduledAnalogStick_;

      private readonly Sm64Vector2<float> camLook_ = new();
      private Sm64Vector2<float>? scheduledCamLook_;

      public IReadOnlySm64Vector2<float> AnalogStick => this.analogStick_;

      public void ScheduleAnalogStick(float x, float y)
        => this.scheduledAnalogStick_ = new Sm64Vector2<float> {X = x, Y = y};

      public IReadOnlySm64Vector2<float> CamLook => this.camLook_;

      public void ScheduleCamLook(float x, float y)
        => this.scheduledCamLook_ = new Sm64Vector2<float> {X = x, Y = y};

      public bool IsAButtonDown { get; set; }
      public bool IsBButtonDown { get; set; }
      public bool IsZButtonDown { get; set; }

      public void Tick() {
        if (this.scheduledAnalogStick_ != null) {
          this.analogStick_.X = this.scheduledAnalogStick_.X;
          this.analogStick_.Y = this.scheduledAnalogStick_.Y;
          this.scheduledAnalogStick_ = null;
        }

        if (this.scheduledCamLook_ != null) {
          this.camLook_.X = this.scheduledCamLook_.X;
          this.camLook_.Y = this.scheduledCamLook_.Y;
          this.scheduledCamLook_ = null;
        }
      }
    }
  }
}