using demo.common.math;


namespace demo.common.gamepad;

public interface IGamepad {
  IAnalogStick MovementAnalogStick { get; }
  IAnalogStick CameraAnalogStick { get; }

  IButton JumpButton { get; }
  IButton PunchButton { get; }
  IButton CrouchButton { get; }
}

public interface IAnalogStick {
  IReadOnlyVector2<float> Axes { get; }
}

public interface IButton {
  bool IsDown { get; }
}