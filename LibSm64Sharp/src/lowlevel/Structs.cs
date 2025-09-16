using System.Runtime.InteropServices;


namespace libsm64sharp.lowlevel;

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

[StructLayout(LayoutKind.Sequential)]
public readonly struct LowLevelSm64Vector3i : IReadOnlySm64Vector3<int> {
  public int X { get; }
  public int Y { get; }
  public int Z { get; }

  public int this[int index]
    => index switch {
        0 => X,
        1 => Y,
        2 => Z,
    };

  public override string ToString() => $"({X}, {Y}, {Z})";
}

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void DebugPrintFuncDelegate(string str);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void PlaySoundFuncDelegate(
    uint soundBits,
    ref LowLevelSm64Vector3f position);


[StructLayout(LayoutKind.Sequential)]
public struct LowLevelSm64SurfaceInternal {
  public Sm64SurfaceType type;
  public short force;
  public sbyte flags;
  public sbyte room;
  public int lowerY; // libsm64: 32 bit
  public int upperY; // libsm64: 32 bit
  public LowLevelSm64Vector3i vertex1; // libsm64: 32 bit
  public LowLevelSm64Vector3i vertex2; // libsm64: 32 bit
  public LowLevelSm64Vector3i vertex3; // libsm64: 32 bit

  public LowLevelSm64Vector3f normal;

  public float originOffset;
  //struct Object *object;

  public byte isValid; // libsm64: added field

  public LowLevelSm64SurfaceObjectTransform transform; // libsm64: added field
  public ushort terrain; // libsm64: added field
}

[StructLayout(LayoutKind.Sequential)]
public struct LowLevelSm64SurfaceObjectTransform {
  public float aPosX, aPosY, aPosZ;
  public float aVelX, aVelY, aVelZ;

  public short aFaceAnglePitch;
  public short aFaceAngleYaw;
  public short aFaceAngleRoll;

  public short aAngleVelPitch;
  public short aAngleVelYaw;
  public short aAngleVelRoll;
};

[StructLayout(LayoutKind.Sequential)]
public struct LowLevelSm64WallCollisionData {
  /*0x00*/
  public float x, y, z;

  /*0x0C*/
  public float offsetY;

  /*0x10*/
  public float radius;

  /*0x14*/
  public short unk14;

  /*0x16*/
  public short numWalls;

  /*0x18*/
  [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
  public LowLevelSm64SurfaceInternal[] walls;
}

[StructLayout(LayoutKind.Sequential)]
public struct LowLevelSm64FloorGeometry {
  public float unused1;
  public float unused2;
  public float unused3;
  public float unused4;

  public float normalX;
  public float normalY;
  public float normalZ;
  public float originOffset;
}