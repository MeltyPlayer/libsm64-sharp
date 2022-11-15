using System.Numerics;


namespace demo.common.math {
  public class HandlerVector2<TNumber> : IReadOnlyVector2<TNumber>
      where TNumber : INumber<TNumber> {
    private readonly Func<TNumber> getX_;
    private readonly Func<TNumber> getY_;

    public HandlerVector2(Func<TNumber> getX, Func<TNumber> getY) {
      this.getX_ = getX;
      this.getY_ = getY;
    }

    public TNumber X => this.getX_();
    public TNumber Y => this.getY_();
  }
}