﻿using demo.camera;

using libsm64sharp;

using OpenTK.Input;
using OpenTK.Platform;


namespace demo.controller {
  public class MarioController {
    private readonly ISm64Mario mario_;
    private readonly ICamera camera_;

    private bool isForwardDown_ = false;
    private bool isBackwardDown_ = false;
    private bool isLeftwardDown_ = false;
    private bool isRightwardDown_ = false;

    public MarioController(
        ISm64Mario mario,
        ICamera camera,
        IGameWindow gameWindow) {
      this.mario_ = mario;
      this.camera_ = camera;

      var gamepad = mario.Gamepad;

      gameWindow.MouseDown += (_, args) => {
        switch (args.Button) {
          case MouseButton.Left: {
            gamepad.ScheduleBButton(true);
            break;
          }
        }
      };
      gameWindow.MouseUp += (_, args) => {
        switch (args.Button) {
          case MouseButton.Left: {
            gamepad.ScheduleBButton(false);
            break;
          }
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
          case Key.Space: {
            gamepad.ScheduleAButton(true);
            break;
          }
          case Key.ControlLeft: {
            gamepad.ScheduleZButton(true);
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
          case Key.Space: {
            gamepad.ScheduleAButton(false);
            break;
          }
          case Key.ControlLeft: {
            gamepad.ScheduleZButton(false);
            break;
          }
        }
      };
    }

    public void BeforeTick() {
      var forwardVector =
          (this.isForwardDown_ ? 1f : 0) - (this.isBackwardDown_ ? 1 : 0);
      var rightwardVector =
          (this.isRightwardDown_ ? 1f : 0) - (this.isLeftwardDown_ ? 1 : 0);

      var length = MathF.Sqrt(forwardVector * forwardVector +
                                  rightwardVector * rightwardVector);

      forwardVector /= length;
      rightwardVector /= length;

      this.mario_.Gamepad.ScheduleCamLook(-this.camera_.ZNormal,
                                          this.camera_.XNormal);
      this.mario_.Gamepad.ScheduleAnalogStick(-forwardVector,
                                              -rightwardVector);
    }
  }
}