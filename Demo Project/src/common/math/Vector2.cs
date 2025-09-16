using System.Numerics;


namespace demo.common.math;

public class Vector2<TNumber> : IVector2<TNumber>
    where TNumber : INumber<TNumber> {
  public TNumber X { get; set; }
  public TNumber Y { get; set; }
}