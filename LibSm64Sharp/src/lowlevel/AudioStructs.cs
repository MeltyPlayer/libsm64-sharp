using System.Runtime.InteropServices;


namespace libsm64sharp.lowlevel {
  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64AudioBanks {
    public int numCtlEntries;
    public IntPtr ctlEntries;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64CtlEntry {
    public byte unused;
    public byte numInstruments;
    public byte numDrums;
    public IntPtr instruments;
    public IntPtr drums;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64Drum {
    public byte releaseRate;
    public byte pan;
    public byte loaded;
    public LowLevelSm64AudioBankSound sound;
    public IntPtr envelope;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64Instrument {
    public byte loaded;
    public byte normalRangeLo;
    public byte normalRangeHi;
    public byte releaseRate;
    public IntPtr envelope;
    public LowLevelSm64AudioBankSound lowNotesSound;
    public LowLevelSm64AudioBankSound normalNotesSound;
    public LowLevelSm64AudioBankSound highNotesSound;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64AudioBankSound {
    public IntPtr sample;
    public float tuning;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64AudioBankSample {
    public byte unused;
    public byte loaded;
    public IntPtr sampleAddr;
    public IntPtr loop;
    public IntPtr book;
    public uint sampleSize;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64AdpcmLoop {
    public uint start;
    public uint end;
    public uint count;
    public uint pad;
    // state
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct LowLevelSm64AdpcmBook {
    public int order;
    public int npredictors;
    public IntPtr book;
  }
}