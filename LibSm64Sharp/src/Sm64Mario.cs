using System.Runtime.InteropServices;

using libsm64sharp.lowlevel;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


namespace libsm64sharp {
  public partial class Sm64Context {
    public ISm64Mario CreateMario(short x, short y, short z)
      => new Sm64Mario(this.marioTextureImage_, x, y, z);

    private class Sm64Mario : ISm64Mario {
      private readonly uint id_;
      private readonly Sm64MarioMesh marioMesh_;

      private Sm64Vector3<float> position_ = new();
      private ISm64Vector3<float>? scheduledPosition_;

      private float? scheduledFaceAngle_;

      private Sm64Vector3<float> velocity_ = new();
      private ISm64Vector3<float>? scheduledVelocity_;

      private short? scheduledHealth_;

      public Sm64Mario(Image<Rgba32> marioTextureImage,
                       short x,
                       short y,
                       short z) {
        this.id_ = LibSm64Interop.sm64_mario_create(x, y, z);
        this.marioMesh_ = new Sm64MarioMesh(marioTextureImage);
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
      public ISm64MarioMesh Mesh => this.marioMesh_;


      public ISm64Vector3<float> Position => this.position_;

      public void SchedulePosition(float x, float y, float z)
        => this.scheduledPosition_ = new Sm64Vector3<float> {
            X = x,
            Y = y,
            Z = z
        };

      public float FaceAngle { get; private set; }

      public void ScheduleFaceAngle(float faceAngle)
        => this.scheduledFaceAngle_ = faceAngle;


      public ISm64Vector3<float> Velocity => this.velocity_;

      public void ScheduleVelocity(float xVel, float yVel, float zVel)
        => this.scheduledVelocity_ = new Sm64Vector3<float> {
            X = xVel,
            Y = yVel,
            Z = zVel
        };

      public short Health { get; private set; }

      public void ScheduleHealth(short health)
        => this.scheduledHealth_ = health;

      public void Tick() {
        this.Gamepad.Tick();
        var inputs = new LowLevelSm64MarioInputs {
            buttonA = (byte) (this.Gamepad.IsAButtonDown ? 1 : 0),
            buttonB = (byte) (this.Gamepad.IsBButtonDown ? 1 : 0),
            buttonZ = (byte) (this.Gamepad.IsZButtonDown ? 1 : 0),
            stickX = this.Gamepad.AnalogStick.X,
            stickY = this.Gamepad.AnalogStick.Y,
            camLookX = this.Gamepad.CamLook.X,
            camLookZ = this.Gamepad.CamLook.Y,
        };

        {
          if (this.scheduledPosition_ != null) {
            this.position_.X = this.scheduledPosition_.X;
            this.position_.Y = this.scheduledPosition_.Y;
            this.position_.Z = this.scheduledPosition_.Z;
            this.scheduledPosition_ = null;
          }

          if (this.scheduledFaceAngle_ != null) {
            this.FaceAngle = this.scheduledFaceAngle_.Value;
            this.scheduledFaceAngle_ = null;
          }

          if (this.scheduledVelocity_ != null) {
            this.velocity_.X = this.scheduledVelocity_.X;
            this.velocity_.Y = this.scheduledVelocity_.Y;
            this.velocity_.Z = this.scheduledVelocity_.Z;
            this.scheduledVelocity_ = null;
          }

          if (this.scheduledHealth_ != null) {
            this.Health = this.scheduledHealth_.Value;
            this.scheduledHealth_ = null;
          }
        }
        var outState = new LowLevelSm64MarioState {
            position = new[] {
                this.Position.X,
                this.Position.Y,
                this.Position.Z
            },
            faceAngle = this.FaceAngle,
            velocity = new[] {
                this.Velocity.X,
                this.Velocity.Y,
                this.Velocity.Z
            },
            health = this.Health,
        };

        var marioMesh = this.marioMesh_;
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

        LibSm64Interop.sm64_mario_tick(this.id_,
                                       ref inputs,
                                       ref outState,
                                       ref outBuffers);

        this.position_.X = outState.position[0];
        this.position_.Y = outState.position[1];
        this.position_.Z = outState.position[2];
        this.FaceAngle = outState.faceAngle;
        this.velocity_.X = outState.velocity[0];
        this.velocity_.Y = outState.velocity[1];
        this.velocity_.Z = outState.velocity[2];
        this.Health = outState.health;

        this.marioMesh_.UpdateTriangleDataFromBuffers(
            outBuffers.numTrianglesUsed);

        posHandle.Free();
        normHandle.Free();
        colorHandle.Free();
        uvHandle.Free();
      }
    }
  }
}