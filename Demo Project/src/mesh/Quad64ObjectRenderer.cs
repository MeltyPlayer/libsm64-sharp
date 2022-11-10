using OpenTK.Graphics.OpenGL;

using Quad64;
using Quad64.src.LevelInfo;


namespace demo.mesh {
  public class Quad64ObjectRenderer : IRenderable {
    private readonly Quad64ModelRenderer? impl_;
    private readonly Object3D object_;

    public Quad64ObjectRenderer(Level level, Object3D obj) {
      this.object_ = obj;

      if (level.ModelIDs.TryGetValue(obj.ModelID, out var model)) {
        this.impl_ = new Quad64ModelRenderer(model.HighestLod);
      }
    }

    public void Render() {
      if (this.impl_ != null) {
        GL.LoadIdentity();
        GL.Translate(this.object_.xPos, this.object_.yPos, this.object_.zPos);
        Quad64ObjectRenderer.Rotate_(this.object_.xRot,
                                     this.object_.yRot,
                                     this.object_.zRot);

        this.impl_.Render();

        GL.LoadIdentity();
      }
    }

    private static void Rotate_(short xRot, short yRot, short zRot) {
      // TODO: This doesn't seem to rotate correctly??
      GL.Rotate(xRot, 1, 0, 0);
      GL.Rotate(yRot, 0, 1, 0);
      GL.Rotate(zRot, 0, 0, 1);

      /*var quaternion = new Quaternion(xRot, yRot, zRot, 1.0f);
      var matrix = Matrix4.CreateFromQuaternion(quaternion);

      GL.MultMatrix(ref matrix);*/
    }
  }
}