using OpenTK.Platform;


namespace demo.common.gamepad;

public class JoystickGamepad : IGamepad {
  public JoystickGamepad(IGameWindow gameWindow) {
  }

  public IAnalogStick MovementAnalogStick { get; }
  public IAnalogStick CameraAnalogStick { get; }

  public IButton JumpButton { get; }
  public IButton PunchButton { get; }
  public IButton CrouchButton { get; }
}