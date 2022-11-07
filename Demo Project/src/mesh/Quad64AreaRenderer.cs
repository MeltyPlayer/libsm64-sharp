using Quad64.src.LevelInfo;


namespace demo.mesh {
  public class Quad64AreaRenderer : IRenderable {
    private readonly Quad64ModelRenderer impl_;

    public Quad64AreaRenderer(Area area) {
      this.impl_ = new Quad64ModelRenderer(area.AreaModel);
    }

    public void Render() => this.impl_.Render();
  }
}