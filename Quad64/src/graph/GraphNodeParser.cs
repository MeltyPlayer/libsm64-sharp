namespace Quad64.graph;

public class GraphNodeParser : IGraphNodeParser {
  public enum GraphCommand {
    G_0x00,
    G_0x01,
    G_0x02,
    G_0x03,
    G_0x04,
    G_0x05,
    G_0x06,
    G_0x07,
    G_0x08,
    G_0x09,
    G_0x0A,
    G_0x0B,
    G_0x0C,
    G_0x0D,
    G_0x0E,
    G_0x0F,
    G_0x10,
    G_0x11,
    G_0x12,
    G_0x13,
    G_0x14,
    G_0x15,
    G_0x16,
    G_0x17,
    G_0x18,
    G_0x19,
    G_0x1A,
    G_0x1B,
    G_0x1C,
    G_0x1D,
    G_0x1E,
    G_0x1F,
    G_0x20,
  }

  public IGraphNode Parse(uint address,
                          byte? areaId,
                          out GraphReturnType graphReturnType) {
    BitLogic.SplitAddress(
        address,
        out var graphNodeSegment,
        out var graphNodeOffset);
    ROM rom = ROM.Instance;
    byte[] data = rom.getSegment(graphNodeSegment, areaId);

    IGraphNode node = new ParentGraphNode();
    graphReturnType = GraphReturnType.UNDEFINED;

    while (graphReturnType == GraphReturnType.UNDEFINED) {
      var cmdLen = GetCmdLength_(data[graphNodeOffset]);
      var cmd = rom.getSubArray_safe(data, graphNodeOffset, cmdLen);

      switch ((GraphCommand) cmd[0]) {
        // 0x00: Branch and link
        case GraphCommand.G_0x00: {
          var branchAddress = BitLogic.BytesToInt(cmd, 4, 4);
          var child = Parse(branchAddress, areaId, out var branchReturnType);
          node.AddChildToEnd(child);

          if (branchReturnType == GraphReturnType.END) {
            graphReturnType = GraphReturnType.END;
          }
          break;
        }
        // 0x01: Terminate geo layout
        case GraphCommand.G_0x01: {
          graphReturnType = GraphReturnType.END;
          break;
        }
        // 0x02: Branch
        case GraphCommand.G_0x02: {
          var branchAddress = BitLogic.BytesToInt(cmd, 4, 4);
          var child = Parse(branchAddress, areaId, out var branchReturnType);
          node.AddChildToEnd(child);
          graphReturnType = branchReturnType;

          if (cmd[1] == 1) {
            // TODO: How does this work??
          }

          break;
        }
        // 0x03: Return from branch
        case GraphCommand.G_0x03: {
          graphReturnType = GraphReturnType.RETURN;
          break;
        }
        // 0x04: Open node
        case GraphCommand.G_0x04: {
          var newNode = new ParentGraphNode();
          node.AddChildToEnd(newNode);
          node = newNode;
          break;
        }
        // 0x05: Close node
        case GraphCommand.G_0x05: {
          node = node.Parent;
          break;
        }
        // 0x1A: Noop
        case GraphCommand.G_0x1A: {
          node.AddChildToEnd(new NoopGraphNode("Noop"));
          break;
        }
        // 0x1B: Copy view
        case GraphCommand.G_0x1B: {
          node.AddChildToEnd(new NoopGraphNode("Copy view"));
          break;
        }
        // 0x1C: Held object
        case GraphCommand.G_0x1C: {
          node.AddChildToEnd(new NoopGraphNode("Held object"));
          break;
        }
        // 0x20: Radius for frustum culling
        case GraphCommand.G_0x20: {
          node.AddChildToEnd(new NoopGraphNode("Radius for frustum culling"));
          break;
        }
      }

      graphNodeOffset += cmdLen;
    }

    return node;
  }

  private static byte GetCmdLength_(byte cmd) {
    switch (cmd) {
      case 0x00:
      case 0x02:
      case 0x0D:
      case 0x0E:
      case 0x11:
      case 0x12:
      case 0x14:
      case 0x15:
      case 0x16:
      case 0x18:
      case 0x19:
      case 0x1A:
      case 0x1D:
      case 0x1E:
        return 0x08;
      case 0x08:
      case 0x0A:
      case 0x13:
      case 0x1C:
        return 0x0C;
      case 0x1F:
        return 0x10;
      case 0x0F:
      case 0x10:
        return 0x14;
      default:
        return 0x04;
    }
  }

  private abstract class BGraphNode : IGraphNode {
    public IGraphNode Parent { get; private set; }
    public IGraphNode? FirstChild { get; private set; }
    public IGraphNode? NextSibling { get; private set; }

    public void AddChildToEnd(IGraphNode node) {
      if (this.FirstChild == null) {
        this.FirstChild = node;
        return;
      }

      var lastChild = this.FirstChild;
      while (lastChild.NextSibling != null) {
        lastChild = lastChild.NextSibling;
      }
      lastChild.InsertSiblingAfter(node);
    }

    public void InsertSiblingAfter(IGraphNode node) {
      if (this.NextSibling == null) {
        this.NextSibling = node;
        return;
      }

      var prevNextSibling = this.NextSibling;
      this.InsertSiblingAfter(node);
      node.InsertSiblingAfter(prevNextSibling);
    }
  }

  private class ParentGraphNode : BGraphNode { }

  private class NoopGraphNode : BGraphNode {
    private readonly string label_;

    public NoopGraphNode(string label) {
      this.label_ = label;
    }

    public override string ToString() => this.label_;
  }
}