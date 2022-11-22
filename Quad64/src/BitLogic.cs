namespace Quad64 {
  public static class BitLogic {
    public static void SplitAddress(uint address,
                                    out byte segment,
                                    out uint offset) {
      segment = (byte) (address >> 24);
      offset = address & 0xffffff;
    }

    public static uint BytesToInt(byte[] b, int offset, int length) {
      switch (length) {
        case 1: return b[0 + offset];
        case 2: return (uint) (b[0 + offset] << 8 | b[1 + offset]);
        case 3:
          return (uint) (b[0 + offset] << 16 | b[1 + offset] << 8 |
                         b[2 + offset]);
        default:
          return (uint) (b[0 + offset] << 24 | b[1 + offset] << 16 |
                         b[2 + offset] << 8 | b[3 + offset]);
      }
    }
  }
}