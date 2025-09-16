using libsm64sharp.lowlevel;

using System.Numerics;


namespace libsm64sharp;

// TODO: Fork libsm64 to expose more features:
// TODO: - Mario's enum state
// TODO: - Mario's animation enum
// TODO: - when sounds should play
// TODO: - loading sounds from game
// TODO: - loading other data from game?
// TODO: - events?
// TODO: - support for picking things up
public interface ISm64Mario : IDisposable {
  ISm64Gamepad Gamepad { get; }
  ISm64MarioMesh Mesh { get; }

  Vector3 Position { get; set; }
  float FaceAngle { get; set; }
  Vector3 Velocity { get; set; }
  float ForwardVelocity { get; set; }
  MarioAction Action { get; set; }
  MarioAnimId AnimId { get; set; }
  short AnimFrame { get; set; }
  ushort Health { get; set; }

  void Tick();
}