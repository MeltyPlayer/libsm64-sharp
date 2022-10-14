namespace libsm64sharp {
  public partial class Sm64Context {
    private class Sm64Triangle : ISm64Triangle {
      public Sm64SurfaceType SurfaceType { get; set; }
      public Sm64TerrainType TerrainType { get; set; }
      public IReadOnlyList<ISm64Vector3<short>> Vertices { get; set; }
    }
  }
}