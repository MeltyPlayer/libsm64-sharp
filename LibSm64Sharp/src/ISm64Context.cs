namespace libsm64sharp {
  public interface ISm64Context : IDisposable {
    ISm64Mario CreateMario(short x, short y, short z);

    ISm64StaticCollisionMesh ChangeStaticCollisionMesh(
        IReadOnlyList<ISm64Triangle> triangles);

    ISm64StaticCollisionMesh CreateDynamicCollisionMesh(
        IReadOnlyList<ISm64Triangle> triangles);
  }
}