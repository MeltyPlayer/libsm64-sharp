using Quad64.src.Scripts;


namespace Quad64.Scripts {
  public static class CollisionMapLoader {
    public static CollisionMap Load(uint address) {
      var cmap = new CollisionMap();
      LoadInto(cmap, address);
      return cmap;
    }

    public static void LoadInto(CollisionMap cmap, uint address) {
      var rom = ROM.Instance;

      var segment = (ushort) (address >> 24);
      uint off = address & 0xFFFFFF;
      byte[] data = rom.getSegment(segment, null);
      var sub_cmd = (ushort) BitLogic.BytesToInt(data, (int) off, 2);

      // Check if the data is actually collision data.
      if (data[off] != 0x00 || data[off + 1] != 0x40)
        return;

      uint num_verts = (ushort) BitLogic.BytesToInt(data, (int) off + 2, 2);

      off += 4;
      for (int i = 0; i < num_verts; i++) {
        short x = (short) BitLogic.BytesToInt(data, (int) off + 0, 2);
        short y = (short) BitLogic.BytesToInt(data, (int) off + 2, 2);
        short z = (short) BitLogic.BytesToInt(data, (int) off + 4, 2);
        cmap.AddVertex(new OpenTK.Vector3(x, y, z));
        off += 6;
      }

      while (sub_cmd != 0x0041) {
        sub_cmd = (ushort) BitLogic.BytesToInt(data, (int) off, 2);
        //Console.WriteLine(sub_cmd.ToString("X8"));
        if (sub_cmd == 0x0041) break;
        //rom.printArraySection(data, (int)off, 4 + (int)collisionLength(sub_cmd));
        cmap.NewTriangleList((int) BitLogic.BytesToInt(data, (int) off, 2));
        uint num_tri = (ushort) BitLogic.BytesToInt(data, (int) off + 2, 2);
        uint col_len = GetLengthOfSubCommand(sub_cmd);
        off += 4;
        for (int i = 0; i < num_tri; i++) {
          uint a = BitLogic.BytesToInt(data, (int) off + 0, 2);
          uint b = BitLogic.BytesToInt(data, (int) off + 2, 2);
          uint c = BitLogic.BytesToInt(data, (int) off + 4, 2);
          cmap.AddTriangle(a, b, c);
          off += col_len;
        }
      }
      cmap.buildCollisionMap();
      off += 2;
      bool end = false;
      while (!end) {
        sub_cmd = (ushort) BitLogic.BytesToInt(data, (int) off, 2);
        switch (sub_cmd) {
          case 0x0042:
            end = true;
            break;
          case 0x0043:
            throw new NotImplementedException();
          case 0x0044:
            // Also skipping water boxes. Will come back to it later.
            uint num_boxes =
                (ushort) BitLogic.BytesToInt(data, (int) off + 2, 2);
            off += 4 + (num_boxes * 0xC);
            break;
          default:
            throw new NotImplementedException();
        }
      }
    }

    public static uint GetLengthOfSubCommand(int type) {
      switch (type) {
        case 0x0E:
        case 0x24:
        case 0x25:
        case 0x27:
        case 0x2C:
        case 0x2D:
        case 0x40:
          return 8;
        default:
          return 6;
      }
    }
  }
}