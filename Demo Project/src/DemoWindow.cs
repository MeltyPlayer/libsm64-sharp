using demo.audio;
using demo.common.audio;
using demo.common.audio.impl.al;
using demo.camera;
using demo.controller;
using demo.common.gl;
using demo.level;
using demo.mesh;

using libsm64sharp;

using OpenTK;

using Quad64;

using BlendingFactor = OpenTK.Graphics.OpenGL.BlendingFactor;
using ClearBufferMask = OpenTK.Graphics.OpenGL.ClearBufferMask;
using CullFaceMode = OpenTK.Graphics.OpenGL.CullFaceMode;
using DepthFunction = OpenTK.Graphics.OpenGL.DepthFunction;
using EnableCap = OpenTK.Graphics.OpenGL.EnableCap;
using GL = OpenTK.Graphics.OpenGL.GL;
using HintMode = OpenTK.Graphics.OpenGL.HintMode;
using HintTarget = OpenTK.Graphics.OpenGL.HintTarget;
using MaterialFace = OpenTK.Graphics.OpenGL.MaterialFace;
using MatrixMode = OpenTK.Graphics.OpenGL.MatrixMode;
using PolygonMode = OpenTK.Graphics.OpenGL.PolygonMode;
using ShadingModel = OpenTK.Graphics.OpenGL.ShadingModel;


namespace demo;

public class DemoWindow : GameWindow {
  private readonly ISm64Context sm64Context_;

  private readonly ISm64Mario? sm64Mario_;
  private readonly MarioController? marioController_;
  private readonly MarioMeshRenderer? marioMeshRenderer_;

  private List<Sm64Object> objects_ = new();

  private readonly IRenderable meshRenderer_;
  private readonly IRenderable collisionMeshRenderer_;

  private ICamera camera_;
  private ICameraController cameraController_;

  private IAudioManager<short> audioManager_;
  private IActiveMusic<short> activeMusic_;

  private bool isGlInit_;

  public DemoWindow() {
    Sm64Context.RegisterDebugPrintFunction(text => {
      //Debug.WriteLine(text);
    });

    this.Closed += (_, _) => {
      this.sm64Context_.Dispose();
      this.audioManager_.Dispose();
    };

    Sm64Context.RegisterPlaySoundFunction(
        args => {
          // TODO: Play sounds
          ;
        });

    var sm64RomBytes = File.ReadAllBytes("rom\\sm64.z64");
    this.sm64Context_ = Sm64Context.InitFromRom(sm64RomBytes);

    this.audioManager_ = new AlAudioManager();

    {
      var musicIntroBuffer =
          new OggAudioLoader().LoadAudio(this.audioManager_,
                                         "resources/music_intro.ogg");
      var musicLoopBuffer =
          new OggAudioLoader().LoadAudio(this.audioManager_,
                                         "resources/music_loop.ogg");

      this.activeMusic_ = this.audioManager_.CreateAudioSource()
                              .CreateMusic(musicIntroBuffer, musicLoopBuffer);
      this.activeMusic_.Play();
    }

    Sm64Audio.Start(this.sm64Context_, this.audioManager_);

    var shouldLoadViaRom = true;
    if (shouldLoadViaRom) {
      var level = Quad64LevelMeshLoader.LoadLevel(LevelId.WHOMPS_FORTRESS);
      var area = level.Areas[0];

      var staticCollisionMesh =
          Quad64LevelMeshLoader.UpdateCollisionMesh(this.sm64Context_, area);
      this.collisionMeshRenderer_ =
          new StaticCollisionMeshRenderer(staticCollisionMesh);

      this.meshRenderer_ = new Quad64AreaRenderer(area);

      var marioStart =
          area.Objects.First(
              obj => obj.Behavior_Name.ToLower()
                        .StartsWith("warp (mario start"));

      this.sm64Mario_ =
          this.sm64Context_.CreateMario(
              marioStart.xPos, marioStart.yPos, marioStart.zPos);

      var camera = new MarioOrbitingCamera(this.sm64Mario_);
      var cameraController =
          new MarioOrbitingCameraController(camera, this);

      this.marioController_ =
          new MarioController(this.sm64Mario_, camera, this);
      this.marioMeshRenderer_ = new MarioMeshRenderer(
          this.sm64Context_, this.sm64Mario_);

      this.camera_ = camera;
      this.cameraController_ = cameraController;

      var objects =
          Array.Empty<Object3D>()
               .Concat(area.MacroObjects)
               .Concat(area.Objects)
               .Concat(area.SpecialObjects);

      foreach (var obj in objects) {
        this.objects_.Add(
            new Sm64Object(this.sm64Context_, level, obj, this.camera_));
      }
    } else {
      var (assimpSceneData, staticCollisionMesh) =
          new LevelMeshLoader().LoadAndCreateCollisionMesh(this.sm64Context_);
      this.meshRenderer_ =
          new StaticAssimpSceneRenderer(assimpSceneData);
      this.collisionMeshRenderer_ =
          new StaticCollisionMeshRenderer(staticCollisionMesh);

      this.sm64Mario_ = this.sm64Context_.CreateMario(0, 900, 0);

      var camera = new MarioOrbitingCamera(this.sm64Mario_);
      var cameraController =
          new MarioOrbitingCameraController(camera, this);

      this.marioController_ =
          new MarioController(this.sm64Mario_, camera, this);
      this.marioMeshRenderer_ = new MarioMeshRenderer(
          this.sm64Context_, this.sm64Mario_);

      this.camera_ = camera;
      this.cameraController_ = cameraController;
    }
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

    GL.ClearDepth(1.0F);

    GL.DepthFunc(DepthFunction.Less);
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

    this.marioController_?.BeforeTick();
    this.cameraController_.Tick();

    this.sm64Mario_?.Tick();
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
      GlUtil.Perspective(this.camera_.FovY, 1.0 * width / height, 1, 50000);
      GlUtil.LookAt(this.camera_);

      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadIdentity();
    }

    this.meshRenderer_.Render();
    //this.collisionMeshRenderer_.Render();

    foreach (var objectRenderer in this.objects_) {
      objectRenderer.Render();
    }

    this.marioMeshRenderer_?.Render();
  }
}