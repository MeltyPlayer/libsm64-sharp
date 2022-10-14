namespace libsm64sharp {
  public interface ISm64Gamepad {
    ISm64Vector2<float> AnalogStick { get; }
    void ScheduleAnalogStick(float x, float y);

    ISm64Vector2<float> CamLook { get; }
    void ScheduleCamLook(float x, float y);

    bool IsAButtonDown { get; }
    void ScheduleAButton(bool isAButtonDown);

    bool IsBButtonDown { get; }
    void ScheduleBButton(bool isBButtonDown);

    bool IsZButtonDown { get; }
    void ScheduleZButton(bool isZButtonDown);
   
    void Tick();
  }
}