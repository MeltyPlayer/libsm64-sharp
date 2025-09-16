namespace libsm64sharp;

public partial class Sm64Context {
  private class Sm64Triangle : ISm64Triangle {
    public Sm64SurfaceType SurfaceType { get; init; }
    public Sm64TerrainType TerrainType { get; init; }
    public IReadOnlyList<IReadOnlySm64Vector3<int>> Vertices { get; init; }
  }
}