namespace demo.camera {
  public class FlyingCamera : ICamera {
    public float EyeX { get; set; }
    public float EyeY { get; set; }
    public float EyeZ { get; set; }

    public float FocusX => this.EyeX + this.XNormal;
    public float FocusY => this.EyeY + this.YNormal;
    public float FocusZ => this.EyeZ + this.ZNormal;

    public float UpX => 0;
    public float UpY => 1;
    public float UpZ => 0;

    public float FovY => Constants.FOV;


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


    // TODO: These negative signs and flipped cos/sin don't look right but they
    // work???
    public void Move(float forwardVector, float rightVector, float speed) {
      this.EyeY += speed * this.VerticalNormal * forwardVector;

      var forwardYawRads = this.Yaw / 180 * MathF.PI;
      var rightYawRads = (this.Yaw - 90) / 180 * MathF.PI;

      this.EyeX += speed *
                   this.HorizontalNormal *
                   (forwardVector * MathF.Cos(forwardYawRads) -
                    rightVector * MathF.Cos(rightYawRads));

      this.EyeZ += speed *
                   this.HorizontalNormal *
                   (forwardVector * MathF.Sin(forwardYawRads) -
                    rightVector * MathF.Sin(rightYawRads));
    }
  }
}