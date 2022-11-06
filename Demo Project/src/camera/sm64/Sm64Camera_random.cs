namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    ushort gRandomSeed16;

    // Generate a pseudorandom integer from 0 to 65535 from the random seed, and update the seed.
    ushort random_ushort() {
      ushort temp1, temp2;

      if (gRandomSeed16 == 22026) {
        gRandomSeed16 = 0;
      }

      temp1 = (ushort)((gRandomSeed16 & 0x00FF) << 8);
      temp1 = (ushort)(temp1 ^ gRandomSeed16);

      gRandomSeed16 = (ushort)(((temp1 & 0x00FF) << 8) + ((temp1 & 0xFF00) >> 8));

      temp1 = (ushort)(((temp1 & 0x00FF) << 1) ^ gRandomSeed16);
      temp2 = (ushort)((temp1 >> 1) ^ 0xFF80);

      if ((temp1 & 1) == 0) {
        if (temp2 == 43605) {
          gRandomSeed16 = 0;
        } else {
          gRandomSeed16 = (ushort)(temp2 ^ 0x1FF4);
        }
      } else {
        gRandomSeed16 = (ushort)(temp2 ^ 0x8180);
      }

      return gRandomSeed16;
    }

    // Generate a pseudorandom float in the range [0, 1).
    float random_float() {
      float rnd = random_ushort();
      return (float)(rnd / (double)0x10000);
    }

    // Return either -1 or 1 with a 50:50 chance.
    int random_sign() {
      if (random_ushort() >= 0x7FFF) {
        return 1;
      } else {
        return -1;
      }
    }

    /**
     * Generate a vector with all three values about zero. The
     * three ranges determine how wide the range about zero.
     */
    void random_vec3s(Vec3s dst, short xRange, short yRange, short zRange) {
      float randomFloat;
      float tempXRange;
      float tempYRange;
      float tempZRange;

      randomFloat = random_float();
      tempXRange = xRange;
      dst[0] = (short) (randomFloat * tempXRange - tempXRange / 2);

      randomFloat = random_float();
      tempYRange = yRange;
      dst[1] = (short)(randomFloat * tempYRange - tempYRange / 2);

      randomFloat = random_float();
      tempZRange = zRange;
      dst[2] = (short)(randomFloat * tempZRange - tempZRange / 2);
    }
  }
}
