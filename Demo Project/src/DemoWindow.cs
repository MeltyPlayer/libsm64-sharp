using demo.camera;
using demo.controller;
using demo.gl;
using demo.mesh;

using libsm64sharp;

using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace demo;

public class DemoWindow : GameWindow {
  private readonly ISm64Context sm64Context_;
  private readonly ISm64Mario sm64Mario_;

  private bool isGlInit_;

  private readonly MarioMeshRenderer marioMeshRenderer_;
  private readonly StaticAssimpSceneRenderer staticAssimpSceneRenderer_;
  private readonly StaticCollisionMeshRenderer staticCollisionMeshRenderer_;

  private MarioOrbitingCamera camera_;
  private MarioOrbitingCameraController cameraController_;

  public DemoWindow() {
    var sm64RomBytes = File.ReadAllBytes("rom\\sm64.z64");

    this.sm64Context_ = new Sm64Context(sm64RomBytes);

    var (assimpScene, staticCollisionMesh) =
        new LevelMeshLoader().LoadAndCreateCollisionMesh(this.sm64Context_);
    this.staticAssimpSceneRenderer_ =
        new StaticAssimpSceneRenderer(assimpScene);
    this.staticCollisionMeshRenderer_ =
        new StaticCollisionMeshRenderer(staticCollisionMesh);

    this.sm64Mario_ = this.sm64Context_.CreateMario(0, 900, 0);
    this.marioMeshRenderer_ = new MarioMeshRenderer(this.sm64Mario_.Mesh);

    this.camera_ = new MarioOrbitingCamera(this.sm64Mario_);
    this.cameraController_ =
        new MarioOrbitingCameraController(this.camera_, this);
  }

  private void InitGL_() {
    if (this.isGlInit_) {
      return;
    }

    this.isGlInit_ = true;

    GL.ShadeModel(ShadingModel.Smooth);
    GL.Enable(EnableCap.PointSmooth);
    GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);

    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

    GL.ClearDepth(5.0F);

    GL.DepthFunc(DepthFunction.Lequal);
    GL.Enable(EnableCap.DepthTest);
    GL.DepthMask(true);

    GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

    GL.Enable(EnableCap.Texture2D);
    GL.Enable(EnableCap.Normalize);

    GL.Enable(EnableCap.CullFace);
    GL.CullFace(CullFaceMode.Back);

    GL.Enable(EnableCap.Blend);
    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

    GL.ClearColor(.5f, .5f, .5f, 1);
  }

  protected override void OnUpdateFrame(FrameEventArgs args) {
    base.OnUpdateFrame(args);

    this.sm64Mario_.Tick();
  }

  protected override void OnRenderFrame(FrameEventArgs args) {
    base.OnRenderFrame(args);
    this.InitGL_();

    var width = this.Width;
    var height = this.Height;
    GL.Viewport(0, 0, width, height);

    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    this.RenderPerspective_();

    GL.Flush();
    this.SwapBuffers();
  }

  private void RenderPerspective_() {
    var width = this.Width;
    var height = this.Height;

    {
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadIdentity();
      GlUtil.Perspective(this.camera_.FovY, 1.0 * width / height, 1, 10000);
      GlUtil.LookAt(this.camera_);

      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadIdentity();
    }

    this.marioMeshRenderer_.Render();
    this.staticAssimpSceneRenderer_.Render();
    //this.staticCollisionMeshRenderer_.Render();
  }
}