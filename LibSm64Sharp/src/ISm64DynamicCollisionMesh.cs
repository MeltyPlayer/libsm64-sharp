namespace libsm64sharp {
  public interface IMutablePositionAndRotation<TSelf>
      where TSelf : IMutablePositionAndRotation<TSelf> {
    TSelf SetPosition(
        float x,
        float y,
        float z);

    TSelf SetEulerRotation(
        float xDegrees,
        float yDegrees,
        float zDegrees);
  }

  public interface ISm64DynamicCollisionMeshBuilder
      : ISm64CollisionMeshBuilder<
              ISm64DynamicCollisionMeshBuilder,
              ISm64DynamicCollisionMesh>,
          IMutablePositionAndRotation<ISm64DynamicCollisionMeshBuilder> { }

  public interface ISm64DynamicCollisionMesh
      : ISm64CollisionMesh,
          IMutablePositionAndRotation<ISm64DynamicCollisionMesh>,
          IDisposable {
    ISm64ObjectTransform Transform { get; }
  }

  public interface ISm64ObjectTransform {
    IVector3<float> Position { get; }
    IVector3<float> EulerRotation { get; }
  }
}