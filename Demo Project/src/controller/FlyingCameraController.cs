using demo.camera;

using OpenTK.Input;
using OpenTK.Platform;


namespace demo.controller {
  public class FlyingCameraController : ICameraController {
    private readonly FlyingCamera flyingCamera_;

    private bool isMouseDown_ = false;
    private (int, int)? prevMousePosition_ = null;

    private bool isForwardDown_ = false;
    private bool isBackwardDown_ = false;
    private bool isLeftwardDown_ = false;
    private bool isRightwardDown_ = false;

    public FlyingCameraController(FlyingCamera flyingCamera,
                                  IGameWindow gameWindow) {
      this.flyingCamera_ = flyingCamera;

      gameWindow.MouseDown += (_, args) => {
        if (args.Button == MouseButton.Left) {
          isMouseDown_ = true;
          this.prevMousePosition_ = null;
        }
      };
      gameWindow.MouseUp += (_, args) => {
        if (args.Button == MouseButton.Left) {
          isMouseDown_ = false;
        }
      };
      gameWindow.MouseMove += (_, args) => {
        if (this.isMouseDown_) {
          var mouseLocation = (args.X, args.Y);

          if (this.prevMousePosition_ != null) {
            var (prevMouseX, prevMouseY) = this.prevMousePosition_.Value;
            var (mouseX, mouseY) = mouseLocation;

            var deltaMouseX = mouseX - prevMouseX;
            var deltaMouseY = mouseY - prevMouseY;

            var fovY = flyingCamera.FovY;
            var fovX = fovY / gameWindow.Height * gameWindow.Width;

            var deltaXFrac = 1f * deltaMouseX / gameWindow.Width;
            var deltaYFrac = 1f * deltaMouseY / gameWindow.Height;

            var mouseSpeedX = 1;
            var mouseSpeedY = 1;

            flyingCamera.Pitch += deltaYFrac * fovY * mouseSpeedY;
            flyingCamera.Yaw -= deltaXFrac * fovX * mouseSpeedX;
          }

          this.prevMousePosition_ = mouseLocation;
        }
      };

      gameWindow.KeyDown += (_, args) => {
        switch (args.Key) {
          case Key.W: {
            this.isForwardDown_ = true;
            break;
          }
          case Key.S: {
            this.isBackwardDown_ = true;
            break;
          }
          case Key.A: {
            this.isLeftwardDown_ = true;
            break;
          }
          case Key.D: {
            this.isRightwardDown_ = true;
            break;
          }
        }
      };

      gameWindow.KeyUp += (_, args) => {
        switch (args.Key) {
          case Key.W: {
            this.isForwardDown_ = false;
            break;
          }
          case Key.S: {
            this.isBackwardDown_ = false;
            break;
          }
          case Key.A: {
            this.isLeftwardDown_ = false;
            break;
          }
          case Key.D: {
            this.isRightwardDown_ = false;
            break;
          }
        }
      };
    }

    public void Tick() {
      var forwardVector =
          (this.isForwardDown_ ? 1 : 0) - (this.isBackwardDown_ ? 1 : 0);
      var rightwardVector =
          (this.isRightwardDown_ ? 1 : 0) - (this.isLeftwardDown_ ? 1 : 0);
      this.flyingCamera_.Move(forwardVector, rightwardVector, 100);
    }
  }
}