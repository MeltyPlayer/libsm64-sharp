namespace demo.camera;

public interface ICamera {
  float EyeX { get; }
  float EyeY { get; }
  float EyeZ { get; }

  float FocusX { get; }
  float FocusY { get; }
  float FocusZ { get; }

  float UpX { get; }
  float UpY { get; }
  float UpZ { get; }

  float XNormal { get; }
  float YNormal { get; }
  float ZNormal { get; }

  float FovY { get; }
}