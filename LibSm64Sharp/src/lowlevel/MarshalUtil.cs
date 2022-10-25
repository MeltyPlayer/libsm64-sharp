using System.Runtime.InteropServices;


namespace libsm64sharp.lowlevel {
  public static class MarshalUtil {
    public static T? MarshalRef<T>(IntPtr ptr)
      => ptr.ToInt64() != 0 ? Marshal.PtrToStructure<T>(ptr) : default;

    public static T[] MarshalArray<T>(IntPtr ptrToArray, int count) {
      var size = Marshal.SizeOf<T>();

      var array = new T?[count];
      for (var i = 0; i < count; i++) {
        var ptr = new IntPtr(ptrToArray.ToInt64() + i * size);
        array[i] = MarshalRef<T>(ptr);
      }

      return array;
    }

    public static T?[] MarshalArrayOfRefs_<T>(IntPtr ptrToArray, int count) {
      var ptrSize = Marshal.SizeOf<IntPtr>();

      var array = new T?[count];
      for (var i = 0; i < count; i++) {
        var ptrToPtr = new IntPtr(ptrToArray.ToInt64() + i * ptrSize);
        var ptr = Marshal.PtrToStructure<IntPtr>(ptrToPtr);
        array[i] = MarshalRef<T>(ptr);
      }

      return array;
    }
  }
}