namespace libsm64sharp;

public partial class Sm64Context {
  private class Sm64Vector3i : ISm64Vector3i {
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
  }

  private class Sm64Vector3f : ISm64Vector3f {
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
  }
}