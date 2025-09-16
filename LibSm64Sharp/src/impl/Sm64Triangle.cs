namespace libsm64sharp;

public partial class Sm64Context {
  private class Sm64Triangle : ISm64Triangle {
    public Sm64Triangle(Sm64SurfaceType surfaceType,
                        Sm64TerrainType terrainType,
                        IReadOnlyList<IReadOnlySm64Vector3i> vertices) {
      this.SurfaceType = surfaceType;
      this.TerrainType = terrainType;
      this.Vertices = vertices;
    }

    public Sm64SurfaceType SurfaceType { get; private set; }
    public Sm64TerrainType TerrainType { get; private set; }
    public IReadOnlyList<IReadOnlySm64Vector3i> Vertices { get; private set; }
  }
}