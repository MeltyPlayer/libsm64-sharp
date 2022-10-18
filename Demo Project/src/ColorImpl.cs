namespace demo {
  public interface IColor {
    float Rf { get; }
    float Gf { get; }
    float Bf { get; }
    float Af { get; }

    byte Rb { get; }
    byte Gb { get; }
    byte Bb { get; }
    byte Ab { get; }
  }

  public class ColorImpl : IColor {
    private static Random RANDOM_ = new();

    private ColorImpl(byte rb, byte gb, byte bb, byte ab) {
      this.Rb = rb;
      this.Gb = gb;
      this.Bb = bb;
      this.Ab = ab;
    }

    public static IColor FromRgbBytes(byte rb, byte gb, byte bb)
      => FromRgbaBytes(rb, gb, bb, 255);

    public static IColor FromRgbaBytes(byte rb, byte gb, byte bb, byte ab)
      => new ColorImpl(rb, gb, bb, ab);


    public static IColor FromRgbFloats(float rf, float gf, float bf)
      => FromRgbaFloats(rf, gf, bf, 1);

    public static IColor FromRgbaFloats(float rf, float gf, float bf, float af)
      => FromRgbaBytes((byte) (rf * 255),
                       (byte) (gf * 255),
                       (byte) (bf * 255),
                       (byte) (af * 255));


    public static IColor FromIntensityByte(byte ib)
      => FromRgbBytes(ib, ib, ib);

    public static IColor FromIntensityFloat(float iF)
      => FromIntensityByte((byte) (iF * 255));


    public static IColor FromSystemColor(Color color)
      => FromRgbaBytes(color.R, color.G, color.B, color.A);


    public static IColor FromHsv(
        double hDegrees,
        double sFraction,
        double vFraction) {
      var sharpColor = ColorImpl.ColorFromHSV(hDegrees, sFraction, vFraction);
      return ColorImpl.FromRgbaBytes(sharpColor.R,
                                     sharpColor.G,
                                     sharpColor.B,
                                     sharpColor.A);
    }

    public static IColor Random() {
      return ColorImpl.FromHsv(360 * ColorImpl.RANDOM_.NextDouble(),
                               1,
                               1);
    }

    public float Rf => this.Rb / 255f;
    public float Gf => this.Gb / 255f;
    public float Bf => this.Bb / 255f;
    public float Af => this.Ab / 255f;

    public byte Rb { get; }
    public byte Gb { get; }
    public byte Bb { get; }
    public byte Ab { get; }

    public static void ColorToHSV(
        Color color,
        out double hue,
        out double saturation,
        out double value) {
      int max = Math.Max(color.R, Math.Max(color.G, color.B));
      int min = Math.Min(color.R, Math.Min(color.G, color.B));

      hue = color.GetHue();
      saturation = (max == 0) ? 0 : 1d - (1d * min / max);
      value = max / 255d;
    }

    public static Color ColorFromHSV(
        double hue,
        double saturation,
        double value) {
      int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
      double f = hue / 60 - Math.Floor(hue / 60);

      value = value * 255;
      int v = Convert.ToInt32(value);
      int p = Convert.ToInt32(value * (1 - saturation));
      int q = Convert.ToInt32(value * (1 - f * saturation));
      int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

      if (hi == 0)
        return Color.FromArgb(255, v, t, p);
      else if (hi == 1)
        return Color.FromArgb(255, q, v, p);
      else if (hi == 2)
        return Color.FromArgb(255, p, v, t);
      else if (hi == 3)
        return Color.FromArgb(255, p, q, v);
      else if (hi == 4)
        return Color.FromArgb(255, t, p, v);
      else
        return Color.FromArgb(255, v, p, q);
    }
  }
}