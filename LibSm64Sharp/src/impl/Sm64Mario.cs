using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using libsm64sharp.lowlevel;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


namespace libsm64sharp;

public partial class Sm64Context {
  public ISm64Mario CreateMario(float x, float y, float z)
    => new Sm64Mario(this.marioTextureImage_, x, y, z);

  private partial class Sm64Mario : ISm64Mario {
    private readonly int id_;
    private readonly Sm64MarioMesh mesh_;

    private Vector3 position_;
    private Vector3 velocity_;
    private float faceAngle_;
    private float forwardVelocity_;
    private MarioAction action_;
    private MarioAnimId animId_;
    private short animFrame_;
    private ushort health_;

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

    public Vector3 Position {
      get => this.position_;
      set {
        this.position_ = value;
        LibSm64Interop.sm64_set_mario_position(
            this.id_,
            value.X,
            value.Y,
            value.Z);
      }
    }

    public Vector3 Velocity {
      get => this.velocity_;
      set {
        this.velocity_ = value;
        LibSm64Interop.sm64_set_mario_velocity(
            this.id_,
            value.X,
            value.Y,
            value.Z);
      }
    }

    public float FaceAngle {
      get => this.faceAngle_;
      set {
        this.faceAngle_ = value;
        LibSm64Interop.sm64_set_mario_faceangle(this.id_, value);
      }
    }

    public float ForwardVelocity {
      get => this.forwardVelocity_;
      set {
        this.forwardVelocity_ = value;
        LibSm64Interop.sm64_set_mario_forward_velocity(this.id_, value);
      }
    }

    public MarioAction Action {
      get => this.action_;
      set {
        this.action_ = value;
        LibSm64Interop.sm64_set_mario_action(this.id_, value);
      }
    }

    public MarioAnimId AnimId {
      get => this.animId_;
      set {
        this.animId_ = value;
        LibSm64Interop.sm64_set_mario_animation(this.id_, value);
      }
    }

    public short AnimFrame {
      get => this.animFrame_;
      set {
        this.animFrame_ = value;
        LibSm64Interop.sm64_set_mario_anim_frame(this.id_, value);
      }
    }

    public ushort Health {
      get => this.health_;
      set {
        this.health_ = value;
        LibSm64Interop.sm64_set_mario_health(this.id_, value);
      }
    }

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

      LowLevelSm64MarioOutState outState = default;
      LibSm64Interop.sm64_mario_tick(this.id_,
                                     ref inputs,
                                     ref outState,
                                     ref outBuffers);

      this.position_ = outState.position;
      this.velocity_ = outState.velocity;
      this.faceAngle_ = outState.faceAngle;
      this.forwardVelocity_ = outState.forwardVelocity;
      this.action_ = outState.action;
      this.animId_ = outState.animId;
      this.animFrame_ = outState.animFrame;
      this.health_ = outState.health;

      this.mesh_.UpdateTriangleDataFromBuffers(
          outBuffers.numTrianglesUsed);

      posHandle.Free();
      normHandle.Free();
      colorHandle.Free();
      uvHandle.Free();
    }
  }
}