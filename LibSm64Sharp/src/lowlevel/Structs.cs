using System.Runtime.InteropServices;


namespace libsm64sharp.lowlevel {
  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64Surface {
    public short type;
    public short force;
    public ushort terrain;
    public short v0x, v0y, v0z;
    public short v1x, v1y, v1z;
    public short v2x, v2y, v2z;
  };

  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64MarioInputs {
    public float camLookX, camLookZ;
    public float stickX, stickY;
    public byte buttonA, buttonB, buttonZ;
  };

  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64MarioState {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public float[] position;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public float[] velocity;

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
  public struct LowLevelSm64Vector2f : ISm64Vector2<float> {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    float[] values;

    public float X => this.values[0];
    public float Y => this.values[1];
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64Vector3f : ISm64Vector3<float> {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    float[] values;

    public float X => this.values[0];
    public float Y => this.values[1];
    public float Z => this.values[2];
  }

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public delegate void DebugPrintFuncDelegate(string str);
}