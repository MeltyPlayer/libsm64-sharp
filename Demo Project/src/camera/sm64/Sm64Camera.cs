using libsm64sharp;

namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    private readonly ISm64Mario sm64Mario_;

    private Vec3f position = new();
    private Vec3f focus = new();

    private float sPanDistance;

    public Sm64Camera(ISm64Mario sm64Mario) {
      this.sm64Mario_ = sm64Mario;
    }
  }
}
