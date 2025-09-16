using demo.common.math;


namespace demo.common.gamepad;

public class ButtonsAnalogStick : IAnalogStick {
  private readonly IButton up_;
  private readonly IButton down_;
  private readonly IButton left_;
  private readonly IButton right_;

  public ButtonsAnalogStick(IButton up,
                            IButton down,
                            IButton left,
                            IButton right) {
    this.up_ = up;
    this.down_ = down;
    this.left_ = left;
    this.right_ = right;
  }

  public IReadOnlyVector2<float> Axes =>
      new HandlerVector2<float>(
          () => ButtonsAnalogStick.GetAxis_(
              this.up_.IsDown, this.down_.IsDown),
          () => ButtonsAnalogStick.GetAxis_(
              this.left_.IsDown, this.right_.IsDown));

  private static float GetAxis_(bool negative, bool positive)
    => (negative ? -1 : 0) + (positive ? 1 : 0);
}