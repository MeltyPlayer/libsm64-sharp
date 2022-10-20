using System.Runtime.InteropServices;


namespace libsm64sharp.lowlevel {
  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64Surface {
    public short type;
    public short force;
    public ushort terrain;
    public int v0x, v0y, v0z;
    public int v1x, v1y, v1z;
    public int v2x, v2y, v2z;
  };

  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64MarioInputs {
    public float camLookX, camLookZ;
    public float stickX, stickY;
    public byte buttonA, buttonB, buttonZ;
  };

  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64MarioOutState {
    public LowLevelSm64Vector3f position;
    public LowLevelSm64Vector3f velocity;
    public float faceAngle;
    public short health;
  };

  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64MarioGeometryBuffers {
    public IntPtr position;
    public IntPtr normal;
    public IntPtr color;
    public IntPtr uv;
    public ushort numTrianglesUsed;
  };

  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64ObjectTransform {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public float[] position;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public float[] eulerRotation;
  };

  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64SurfaceObject {
    public LowLevelSm64ObjectTransform transform;
    public uint surfaceCount;
    public IntPtr surfaces;
  }


  [StructLayout(LayoutKind.Sequential)]
  public readonly struct LowLevelSm64Vector2f : IReadOnlySm64Vector2<float> {
    public float X { get; }
    public float Y { get; }

    public override string ToString() => $"({X}, {Y})";
  }

  [StructLayout(LayoutKind.Sequential)]
  public readonly struct LowLevelSm64Vector3f : IReadOnlySm64Vector3<float> {
    public float X { get; }
    public float Y { get; }
    public float Z { get; }

    public override string ToString() => $"({X}, {Y}, {Z})";
  }

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void DebugPrintFuncDelegate(string str);

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void PlaySoundFuncDelegate(
      uint soundBits,
      ref LowLevelSm64Vector3f position);
}