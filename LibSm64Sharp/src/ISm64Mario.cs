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

    ISm64Vector3<float> Position { get; }
    void SchedulePosition(float x, float y, float z);

    float FaceAngle { get; }
    void ScheduleFaceAngle(float faceAngle);

    ISm64Vector3<float> Velocity { get; }
    void ScheduleVelocity(float xVel, float yVel, float zVel);
    
    short Health { get; }
    void ScheduleHealth(short health);

    void Tick();
  }
}