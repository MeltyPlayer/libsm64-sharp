using demo.camera;

using OpenTK.Input;
using OpenTK.Platform;


namespace demo.controller {
  public class MarioOrbitingCameraController {
    private bool isMouseDown_ = false;
    private (int, int)? prevMousePosition_ = null;

    public MarioOrbitingCameraController(
        MarioOrbitingCamera marioOrbitingCamera,
        IGameWindow gameWindow) {
      gameWindow.MouseDown += (_, args) => {
        if (args.Button == MouseButton.Right) {
          isMouseDown_ = true;
          this.prevMousePosition_ = null;
        }
      };
      gameWindow.MouseUp += (_, args) => {
        if (args.Button == MouseButton.Right) {
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

            var fovY = marioOrbitingCamera.FovY;
            var fovX = fovY / gameWindow.Height * gameWindow.Width;

            var deltaXFrac = 1f * deltaMouseX / gameWindow.Width;
            var deltaYFrac = 1f * deltaMouseY / gameWindow.Height;

            var mouseSpeedX = 1;
            var mouseSpeedY = 1;

            marioOrbitingCamera.Pitch += deltaYFrac * fovY * mouseSpeedY;
            marioOrbitingCamera.Yaw -= deltaXFrac * fovX * mouseSpeedX;
          }

          this.prevMousePosition_ = mouseLocation;
        }
      };
    }
  }
}