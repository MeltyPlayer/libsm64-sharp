using libsm64sharp.lowlevel;


namespace libsm64sharp {
  public partial class Sm64Context {
    public ISm64StaticCollisionMeshBuilder CreateStaticCollisionMesh()
      => new Sm64StaticCollisionMeshBuilder();

    private class Sm64StaticCollisionMeshBuilder
        : ISm64StaticCollisionMeshBuilder {
      private List<ISm64Triangle> triangles_ = new();

      public ISm64StaticCollisionMesh Build()
        => new Sm64StaticCollisionMesh(this.triangles_);

      public ISm64StaticCollisionMeshBuilder AddTriangle(
          Sm64SurfaceType surfaceType,
          Sm64TerrainType terrainType,
          IReadOnlyList<(short x, short y, short z)> vertices) {
        this.triangles_.Add(new Sm64Triangle {
            SurfaceType = surfaceType,
            TerrainType = terrainType,
            Vertices = vertices.Select(xyz => new Sm64Vector3<short> {
                                   X = xyz.x,
                                   Y = xyz.y,
                                   Z = xyz.z,
                               })
                               .ToArray(),
        });
        return this;
      }
    }

    private class Sm64StaticCollisionMesh : ISm64StaticCollisionMesh {
      public Sm64StaticCollisionMesh(
          IReadOnlyList<ISm64Triangle> triangles) {
        this.Triangles = triangles;

        var surfaces =
            triangles.Select(triangle => {
                       var vertices = triangle.Vertices;
                       return new LowLevelSm64Surface {
                           type = (short) triangle.SurfaceType,
                           terrain = (ushort) triangle.TerrainType,
                           v0x = vertices[0].X,
                           v0y = vertices[0].Y,
                           v0z = vertices[0].Z,
                           v1x = vertices[1].X,
                           v1y = vertices[1].Y,
                           v1z = vertices[1].Z,
                           v2x = vertices[2].X,
                           v2y = vertices[2].Y,
                           v2z = vertices[2].Z,
                       };
                     })
                     .ToArray();
        LibSm64Interop.sm64_static_surfaces_load(
            surfaces, (uint) surfaces.Length);
      }

      public IReadOnlyList<ISm64Triangle> Triangles { get; }
    }
  }
}