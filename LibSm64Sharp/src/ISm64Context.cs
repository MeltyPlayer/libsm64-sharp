namespace libsm64sharp {
  public interface ISm64Context : IDisposable {
    ISm64Mario CreateMario(float x, float y, float z);

    // TODO: Are builders the right design for this?
    ISm64DynamicCollisionMeshBuilder CreateDynamicCollisionMesh();
    ISm64StaticCollisionMeshBuilder CreateStaticCollisionMesh();
  }
}