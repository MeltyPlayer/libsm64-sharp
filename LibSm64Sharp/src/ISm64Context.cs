namespace libsm64sharp {
  public interface ISm64Context : IDisposable {
    ISm64Mario CreateMario(short x, short y, short z);

    ISm64DynamicCollisionMeshBuilder CreateDynamicCollisionMesh();
  }
}