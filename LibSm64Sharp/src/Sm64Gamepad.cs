namespace libsm64sharp {
  public partial class Sm64Context {
    private class Sm64Gamepad : ISm64Gamepad {
      private readonly Sm64Vector2<float> analogStick_ = new();
      private Sm64Vector2<float>? scheduledAnalogStick_;

      private readonly Sm64Vector2<float> camLook_ = new();
      private Sm64Vector2<float>? scheduledCamLook_;

      private bool? scheduledIsAButtonDown_;
      private bool? scheduledIsBButtonDown_;
      private bool? scheduledIsZButtonDown_;

      public IReadOnlySm64Vector2<float> AnalogStick => this.analogStick_;

      public void ScheduleAnalogStick(float x, float y)
        => this.scheduledAnalogStick_ = new Sm64Vector2<float> {X = x, Y = y};

      public IReadOnlySm64Vector2<float> CamLook => this.camLook_;

      public void ScheduleCamLook(float x, float y)
        => this.scheduledCamLook_ = new Sm64Vector2<float> {X = x, Y = y};

      public bool IsAButtonDown { get; private set; }

      public void ScheduleAButton(bool isAButtonDown)
        => this.scheduledIsAButtonDown_ = isAButtonDown;

      public bool IsBButtonDown { get; private set; }

      public void ScheduleBButton(bool isBButtonDown)
        => this.scheduledIsBButtonDown_ = isBButtonDown;

      public bool IsZButtonDown { get; private set; }

      public void ScheduleZButton(bool isZButtonDown)
        => this.scheduledIsZButtonDown_ = isZButtonDown;

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

        if (this.scheduledIsAButtonDown_ != null) {
          this.IsAButtonDown = this.scheduledIsAButtonDown_.Value;
          this.scheduledIsAButtonDown_ = null;
        }

        if (this.scheduledIsBButtonDown_ != null) {
          this.IsBButtonDown = this.scheduledIsBButtonDown_.Value;
          this.scheduledIsBButtonDown_ = null;
        }

        if (this.scheduledIsZButtonDown_ != null) {
          this.IsZButtonDown = this.scheduledIsZButtonDown_.Value;
          this.scheduledIsZButtonDown_ = null;
        }
      }
    }
  }
}