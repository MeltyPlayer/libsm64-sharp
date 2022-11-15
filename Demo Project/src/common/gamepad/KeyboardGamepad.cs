using demo.camera;
using demo.common.math;

using OpenTK.Input;
using OpenTK.Platform;


namespace demo.common.gamepad {
  using KeyId = Key;
  using MouseButtonId = MouseButton;

  public class KeyboardGamepad : IGamepad {
    public KeyboardGamepad(IGameWindow gameWindow,
                           ICamera camera) {
      this.MovementAnalogStick =
          new ButtonsAnalogStick(
              new KeyboardButton(gameWindow, KeyId.W),
              new KeyboardButton(gameWindow, KeyId.S),
              new KeyboardButton(gameWindow, KeyId.A),
              new KeyboardButton(gameWindow, KeyId.D));
      this.CameraAnalogStick = new MouseAnalogStick(gameWindow, camera);

      this.JumpButton = new KeyboardButton(gameWindow, KeyId.Space);
      this.PunchButton = new MouseButton(gameWindow, MouseButtonId.Left);
      this.CrouchButton = new KeyboardButton(gameWindow, KeyId.ControlLeft);
    }

    public IAnalogStick MovementAnalogStick { get; }
    public IAnalogStick CameraAnalogStick { get; }

    public IButton JumpButton { get; }
    public IButton PunchButton { get; }
    public IButton CrouchButton { get; }

    public class MouseAnalogStick : IAnalogStick {
      private readonly Vector2<float> axes_ = new();
      private readonly IButton dragButton_;
      private (int, int)? prevMousePosition_ = null;

      public MouseAnalogStick(IGameWindow gameWindow, 
                              ICamera camera) {
        this.dragButton_ = new MouseButton(gameWindow, MouseButtonId.Right);

        gameWindow.MouseMove += (_, args) => {
          if (this.dragButton_.IsDown) {
            var mouseLocation = (args.X, args.Y);

            if (this.prevMousePosition_ != null) {
              var (prevMouseX, prevMouseY) = this.prevMousePosition_.Value;
              var (mouseX, mouseY) = mouseLocation;

              var deltaMouseX = mouseX - prevMouseX;
              var deltaMouseY = mouseY - prevMouseY;

              var fovY = camera.FovY;
              var fovX = fovY / gameWindow.Height * gameWindow.Width;

              var deltaXFrac = 1f * deltaMouseX / gameWindow.Width;
              var deltaYFrac = 1f * deltaMouseY / gameWindow.Height;

              var mouseSpeedX = 1;
              var mouseSpeedY = 1;

              this.axes_.X = -deltaXFrac * fovX * mouseSpeedX;
              this.axes_.Y = deltaYFrac * fovY * mouseSpeedY;
            }

            this.prevMousePosition_ = mouseLocation;
          }
        };
      }

      public IReadOnlyVector2<float> Axes => this.axes_;
    }

    public class MouseButton : IButton {
      public MouseButton(IGameWindow gameWindow, MouseButtonId id) {
        gameWindow.MouseDown += (_, args) => {
          if (args.Button == id) {
            this.IsDown = true;
          }
        };
        gameWindow.MouseUp += (_, args) => {
          if (args.Button == id) {
            this.IsDown = false;
          }
        };
      }

      public bool IsDown { get; private set; }
    }

    public class KeyboardButton : IButton {
      public KeyboardButton(IGameWindow gameWindow, KeyId id) {
        gameWindow.KeyDown += (_, args) => {
          if (args.Key == id) {
            this.IsDown = true;
          }
        };
        gameWindow.KeyUp += (_, args) => {
          if (args.Key == id) {
            this.IsDown = false;
          }
        };
      }

      public bool IsDown { get; private set; }
    }
  }
}