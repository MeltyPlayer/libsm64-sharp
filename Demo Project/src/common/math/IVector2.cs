using System.Numerics;


namespace demo.common.math {
  public interface IReadOnlyVector2<TNumber>
      where TNumber : INumber<TNumber> {
    TNumber X { get; }
    TNumber Y { get; }
  }

  public interface IVector2<TNumber> : IReadOnlyVector2<TNumber>
      where TNumber : INumber<TNumber> {
    new TNumber X { get; set; }
    new TNumber Y { get; set; }
  }
}