namespace libsm64sharp;

public interface IReadOnlySm64Vector2f {
  float X { get; }
  float Y { get; }
}

public interface ISm64Vector2f : IReadOnlySm64Vector2f {
  new float X { get; set; }
  new float Y { get; set; }
}