using libsm64sharp;

using Quad64.f3d;


namespace Quad64.graph;

public enum GraphReturnType {
  UNDEFINED,
  RETURN,
  END,
}

public interface IGraphNodeParser {
  IGraphNode Parse(uint address,
                   byte? areaId,
                   out GraphReturnType graphReturnType);
}

public interface IGraphNode {
  IGraphNode? Parent { get; }

  IGraphNode? FirstChild { get; }
  IGraphNode? NextSibling { get; }

  void AddChildToEnd(IGraphNode node);
  void InsertSiblingAfter(IGraphNode node);
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