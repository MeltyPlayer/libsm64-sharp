using System.Numerics;


namespace libsm64sharp {
  public interface IReadOnlySm64Vector3<TNumber>
      where TNumber : INumber<TNumber> {
    TNumber X { get; }
    TNumber Y { get; }
    TNumber Z { get; }
  }

  public interface ISm64Vector3<TNumber> : IReadOnlySm64Vector3<TNumber>
      where TNumber : INumber<TNumber> {
    new TNumber X { get; set; }
    new TNumber Y { get; set; }
    new TNumber Z { get; set; }
  }
}