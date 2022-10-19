using libsm64sharp;


namespace demo.camera {
  public class MarioOrbitingCamera : ICamera {
    private readonly ISm64Mario mario_;

    private const float MARIO_HEIGHT_ = 150f;

    public MarioOrbitingCamera(ISm64Mario mario) {
      this.mario_ = mario;
    }

    public float EyeX => this.FocusX - this.Distance * this.XNormal;
    public float EyeY => this.FocusY - this.Distance * this.YNormal;
    public float EyeZ => this.FocusZ - this.Distance * this.ZNormal;

    public float FocusX => this.mario_.Position.X;
    public float FocusY => this.mario_.Position.Y + MARIO_HEIGHT_;
    public float FocusZ => this.mario_.Position.Z;

    public float UpX => 0;
    public float UpY => 1;
    public float UpZ => 0;

    public float FovY => Constants.FOV;

    public float Distance { get; set; } = 800;

    /// <summary>
    ///   The left-right angle of the camera, in degrees.
    /// </summary>
    public float Yaw { get; set; }

    /// <summary>
    ///   The up-down angle of the camera, in degrees.
    /// </summary>
    public float Pitch { get; set; }


    public float HorizontalNormal => MathF.Cos(this.Pitch / 180 * MathF.PI);
    public float VerticalNormal => MathF.Sin(this.Pitch / 180 * MathF.PI);


    public float XNormal
      => this.HorizontalNormal * MathF.Cos(this.Yaw / 180 * MathF.PI);

    public float YNormal => this.VerticalNormal;

    public float ZNormal
      => this.HorizontalNormal * MathF.Sin(this.Yaw / 180 * MathF.PI);
  }
}