using System.Runtime.InteropServices;


namespace libsm64sharp.lowlevel {
  public static class LibSm64Interop {
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
      float[] position;

      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      float[] eulerRotation;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct LowLevelSm64SurfaceObject {
      public LowLevelSm64ObjectTransform transform;
      public uint surfaceCount;
      public IntPtr surfaces;
    }

    [DllImport("sm64.dll")]
    public static extern void sm64_global_init(
        IntPtr rom,
        IntPtr outTexture,
        IntPtr debugPrintFunctionPtr);

    [DllImport("sm64.dll")]
    public static extern void sm64_global_terminate();

    [DllImport("sm64.dll")]
    public static extern void sm64_static_surfaces_load(
        LowLevelSm64Surface[] surfaces,
        ulong numSurfaces);

    [DllImport("sm64.dll")]
    public static extern uint sm64_mario_create(
        short marioX,
        short marioY,
        short marioZ);

    [DllImport("sm64.dll")]
    public static extern void sm64_mario_tick(
        uint marioId,
        ref LowLevelSm64MarioInputs inputs,
        ref LowLevelSm64MarioState outState,
        ref LowLevelSm64MarioGeometryBuffers outBuffers);

    [DllImport("sm64.dll")]
    public static extern void sm64_mario_delete(uint marioId);

    [DllImport("sm64.dll")]
    public static extern uint sm64_surface_object_create(
        ref LowLevelSm64SurfaceObject surfaceObject);

    [DllImport("sm64.dll")]
    public static extern void sm64_surface_object_move(
        uint objectId,
        ref LowLevelSm64ObjectTransform transform);

    [DllImport("sm64.dll")]
    public static extern void sm64_surface_object_delete(uint objectId);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DebugPrintFuncDelegate(string str);
  }
}