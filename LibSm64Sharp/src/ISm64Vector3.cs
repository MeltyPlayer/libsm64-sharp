using System.Numerics;


namespace libsm64sharp {
  public interface ISm64Vector3<TNumber> where TNumber : INumber<TNumber> {
    TNumber X { get; }
    TNumber Y { get; }
    TNumber Z { get; }
  }
}