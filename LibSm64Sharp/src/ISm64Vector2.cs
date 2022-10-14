using System.Numerics;


namespace libsm64sharp {
  public interface ISm64Vector2<TNumber> where TNumber : INumber<TNumber> {
    TNumber X { get; }
    TNumber Y { get; }
  }
}