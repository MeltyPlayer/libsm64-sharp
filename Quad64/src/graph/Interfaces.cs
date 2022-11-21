using libsm64sharp;

using Quad64.f3d;


namespace Quad64.graph {
  public interface IGraphNode {
    IReadOnlyList<IGraphNode> Children { get; }
  }

  public interface ILevelOfDetailGraphNode : IGraphNode {
    short MinDistance { get; }
    short MaxDistance { get; }
  }

  public interface ISwitchCaseGraphNode : IGraphNode {
    short NumCases { get; }
    short SelectedCase { get; }
  }

  public interface ITranslationRotationGraphNode : IGraphNode {
    IF3dDisplayList DisplayList { get; }
    IReadOnlySm64Vector3<short> Translation { get; }
    IReadOnlySm64Vector3<short> Rotation { get; }
  }

  public interface ITranslationGraphNode : IGraphNode {
    IF3dDisplayList DisplayList { get; }
    IReadOnlySm64Vector3<short> Translation { get; }
  }

  public interface IRotationGraphNode : IGraphNode {
    IF3dDisplayList DisplayList { get; }
    IReadOnlySm64Vector3<short> Rotation { get; }
  }

  public interface IAnimatedPartGraphNode : IGraphNode {
    IF3dDisplayList DisplayList { get; }
    IReadOnlySm64Vector3<short> Translation { get; }
  }

  public interface IBillboardGraphNode : IGraphNode {
    IF3dDisplayList DisplayList { get; }
    IReadOnlySm64Vector3<short> Translation { get; }
  }

  public interface IDisplayListGraphNode : IGraphNode {
    IF3dDisplayList DisplayList { get; }
  }

  public interface IScaleGraphNode : IGraphNode {
    IF3dDisplayList DisplayList { get; }
    IReadOnlySm64Vector3<float> Scale { get; }
  }
}