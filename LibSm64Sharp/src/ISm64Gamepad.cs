namespace libsm64sharp {
  public interface ISm64Gamepad {
    IReadOnlySm64Vector2<float> AnalogStick { get; }
    void ScheduleAnalogStick(float x, float y);

    IReadOnlySm64Vector2<float> CamLook { get; }
    void ScheduleCamLook(float x, float y);

    bool IsAButtonDown { get; set; }
    bool IsBButtonDown { get; set; }
    bool IsZButtonDown { get; set; }
   
    void Tick();
  }
}