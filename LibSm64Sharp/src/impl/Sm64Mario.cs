using System.Runtime.InteropServices;

using libsm64sharp.lowlevel;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


namespace libsm64sharp {
  public partial class Sm64Context {
    public ISm64Mario CreateMario(float x, float y, float z)
      => new Sm64Mario(this.marioTextureImage_, x, y, z);

    private class Sm64Mario : ISm64Mario {
      private readonly int id_;
      private LowLevelSm64MarioOutState outState_;
      private readonly Sm64MarioMesh mesh_;

      public Sm64Mario(Image<Rgba32> marioTextureImage,
                       float x,
                       float y,
                       float z) {
        this.id_ = LibSm64Interop.sm64_mario_create(x, y, z);
        if (this.id_ == -1) {
          throw new NullReferenceException(
              "Failed to create Mario. " +
              "Have you created a floor for him to stand on yet?");
        }

        this.mesh_ = new Sm64MarioMesh(marioTextureImage);
      }

      ~Sm64Mario() {
        this.ReleaseUnmanagedResources_();
      }

      public void Dispose() {
        this.ReleaseUnmanagedResources_();
        GC.SuppressFinalize(this);
      }

      private void ReleaseUnmanagedResources_()
        => LibSm64Interop.sm64_mario_delete(this.id_);

      public ISm64Gamepad Gamepad { get; } = new Sm64Gamepad();
      public ISm64MarioMesh Mesh => this.mesh_;

      public IReadOnlySm64Vector3<float> Position => this.outState_.position;
      public IReadOnlySm64Vector3<float> Velocity => this.outState_.velocity;
      public float FaceAngle => this.outState_.faceAngle;
      public short Health => this.outState_.health;

      public void Tick() {
        var inputs = new LowLevelSm64MarioInputs {
            buttonA = (byte) (this.Gamepad.IsAButtonDown ? 1 : 0),
            buttonB = (byte) (this.Gamepad.IsBButtonDown ? 1 : 0),
            buttonZ = (byte) (this.Gamepad.IsZButtonDown ? 1 : 0),
            stickX = this.Gamepad.AnalogStick.X,
            stickY = this.Gamepad.AnalogStick.Y,
            camLookX = this.Gamepad.CameraNormal.X,
            camLookZ = this.Gamepad.CameraNormal.Y,
        };

        var outState = this.outState_;

        var marioMesh = this.mesh_;
        var posHandle =
            GCHandle.Alloc(marioMesh.PositionsBuffer, GCHandleType.Pinned);
        var normHandle =
            GCHandle.Alloc(marioMesh.NormalsBuffer, GCHandleType.Pinned);
        var colorHandle =
            GCHandle.Alloc(marioMesh.ColorsBuffer, GCHandleType.Pinned);
        var uvHandle = GCHandle.Alloc(marioMesh.UvsBuffer, GCHandleType.Pinned);
        var outBuffers = new LowLevelSm64MarioGeometryBuffers() {
            position = posHandle.AddrOfPinnedObject(),
            normal = normHandle.AddrOfPinnedObject(),
            color = colorHandle.AddrOfPinnedObject(),
            uv = uvHandle.AddrOfPinnedObject()
        };

        // TODO: Crashes here when sliding, need to investigate.
        LibSm64Interop.sm64_mario_tick(this.id_,
                                       ref inputs,
                                       ref outState,
                                       ref outBuffers);

        this.outState_ = outState;

        this.mesh_.UpdateTriangleDataFromBuffers(
            outBuffers.numTrianglesUsed);

        posHandle.Free();
        normHandle.Free();
        colorHandle.Free();
        uvHandle.Free();
      }
    }
  }
}