using System.Numerics;


namespace libsm64sharp;

public interface IReadOnlySm64Vector2<TNumber>
    where TNumber : INumber<TNumber> {
  TNumber X { get; }
  TNumber Y { get; }
}

public interface ISm64Vector2<TNumber> : IReadOnlySm64Vector2<TNumber>
    where TNumber : INumber<TNumber> {
  new TNumber X { get; set; }
  new TNumber Y { get; set; }
}