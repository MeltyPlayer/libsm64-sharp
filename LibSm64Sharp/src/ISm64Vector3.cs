namespace libsm64sharp;

public interface IReadOnlySm64Vector3s {
  short X { get; }
  short Y { get; }
  short Z { get; }
}

public interface IReadOnlySm64Vector3i {
  int X { get; }
  int Y { get; }
  int Z { get; }
}

public interface IReadOnlySm64Vector3f {
  float X { get; }
  float Y { get; }
  float Z { get; }
}

public interface ISm64Vector3s : IReadOnlySm64Vector3s {
  new short X { get; set; }
  new short Y { get; set; }
  new short Z { get; set; }
}

public interface ISm64Vector3i : IReadOnlySm64Vector3i {
  new int X { get; set; }
  new int Y { get; set; }
  new int Z { get; set; }
}

public interface ISm64Vector3f : IReadOnlySm64Vector3f {
  new float X { get; set; }
  new float Y { get; set; }
  new float Z { get; set; }
}