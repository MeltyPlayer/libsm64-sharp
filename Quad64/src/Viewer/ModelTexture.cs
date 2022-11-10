namespace Quad64 {
  public class Texture2D {
    public Bitmap Bmp { get; set; }

    private int id;
    private int width, height;

    public int ID {
      get { return id; }
    }

    public int Width {
      get { return width; }
    }

    public int Height {
      get { return height; }
    }

    public int TextureParamS { get; set; }
    public int TextureParamT { get; set; }

    public Texture2D(Bitmap bmp, int id, int width, int height) {
      this.Bmp = bmp;
      this.id = id;
      this.width = width;
      this.height = height;
    }
  }
}