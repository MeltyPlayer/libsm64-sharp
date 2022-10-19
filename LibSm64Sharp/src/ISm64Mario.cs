namespace libsm64sharp {
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

    IReadOnlySm64Vector3<float> Position { get; }
    float FaceAngle { get; }
    IReadOnlySm64Vector3<float> Velocity { get; }
    short Health { get; }

    void Tick();
  }
}