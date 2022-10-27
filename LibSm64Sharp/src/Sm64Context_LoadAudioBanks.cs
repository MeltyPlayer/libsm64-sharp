using libsm64sharp.lowlevel;

using System.Runtime.InteropServices;


namespace libsm64sharp {
  public partial class Sm64Context {
    public ISm64AudioBanks LoadAudioBanks() {
      var romHandle = GCHandle.Alloc(this.romBytes_, GCHandleType.Pinned);

      var lowLevelAudioBanks =
          LibSm64Interop.sm64_asset_load_audio_banks(
              romHandle.AddrOfPinnedObject());
      var audioBanks = new Sm64AudioBanks(lowLevelAudioBanks);

      romHandle.Free();

      return audioBanks;
    }

    private class Sm64AudioBanks : ISm64AudioBanks {
      private LowLevelSm64AudioBanks lowLevelImpl_;

      public Sm64AudioBanks(LowLevelSm64AudioBanks lowLevelImpl) {
        this.lowLevelImpl_ = lowLevelImpl;

        var ctlEntries = new List<ISm64CtlEntry>();

        var lowLevelCtlEntries =
            MarshalUtil.MarshalArray<LowLevelSm64CtlEntry>(
                lowLevelImpl.ctlEntries, lowLevelImpl.numCtlEntries);
        foreach (var lowLevelCtlEntry in lowLevelCtlEntries) {
          ctlEntries.Add(new Sm64CtlEntry(lowLevelCtlEntry));
        }

        this.CtlEntries = ctlEntries;
      }

      public IReadOnlyList<ISm64CtlEntry> CtlEntries { get; }
    }

    private class Sm64CtlEntry : ISm64CtlEntry {
      private LowLevelSm64CtlEntry lowLevelImpl_;

      public Sm64CtlEntry(LowLevelSm64CtlEntry lowLevelImpl) {
        this.lowLevelImpl_ = lowLevelImpl;

        {
          var instruments = new List<ISm64Instrument>();
          var lowLevelInstruments =
              MarshalUtil.MarshalArrayOfRefs_<LowLevelSm64Instrument>(
                  lowLevelImpl.instruments, lowLevelImpl.numInstruments);
          foreach (var lowLevelInstrument in lowLevelInstruments) {
            instruments.Add(new Sm64Instrument(lowLevelInstrument));
          }
          this.Instruments = instruments;
        }

        {
          var drums = new List<ISm64Drum>();
          var lowLevelDrums =
              MarshalUtil.MarshalArrayOfRefs_<LowLevelSm64Drum>(
                  lowLevelImpl.drums, lowLevelImpl.numDrums);
          foreach (var lowLevelDrum in lowLevelDrums) {
            drums.Add(new Sm64Drum(lowLevelDrum));
          }
          this.Drums = drums;
        }
      }

      public IReadOnlyList<ISm64Instrument> Instruments { get; }
      public IReadOnlyList<ISm64Drum> Drums { get; }
    }

    private class Sm64Drum : ISm64Drum {
      private LowLevelSm64Drum lowLevelImpl_;

      public Sm64Drum(LowLevelSm64Drum lowLevelImpl) {
        this.lowLevelImpl_ = lowLevelImpl;
      }

      public bool Loaded { get; }
      public byte ReleaseRate { get; }
      public byte Pan { get; }
      public ISm64AudioBankSound Sound { get; }
    }

    private class Sm64Instrument : ISm64Instrument {
      private LowLevelSm64Instrument lowLevelImpl_;

      public Sm64Instrument(LowLevelSm64Instrument lowLevelImpl) {
        this.lowLevelImpl_ = lowLevelImpl;
        this.Loaded = lowLevelImpl.loaded != 0;
        this.ReleaseRate = lowLevelImpl.releaseRate;

        this.NormalRangeHi = lowLevelImpl.normalRangeHi;
        this.NormalRangeLo = lowLevelImpl.normalRangeLo;

        if (lowLevelImpl.normalNotesSound.sample.ToInt64() != 0) {
          this.NormalNotesSound =
              new Sm64AudioBankSound(lowLevelImpl.normalNotesSound);
        }
        if (lowLevelImpl.highNotesSound.sample.ToInt64() != 0) {
          this.HighNotesSound =
              new Sm64AudioBankSound(lowLevelImpl.highNotesSound);
        }
        if (lowLevelImpl.lowNotesSound.sample.ToInt64() != 0) {
          this.LowNotesSound =
              new Sm64AudioBankSound(lowLevelImpl.lowNotesSound);
        }
      }

      public bool Loaded { get; }
      public byte ReleaseRate { get; }
      public byte NormalRangeLo { get; }
      public byte NormalRangeHi { get; }
      public ISm64AudioBankSound? LowNotesSound { get; }
      public ISm64AudioBankSound NormalNotesSound { get; }
      public ISm64AudioBankSound? HighNotesSound { get; }
    }

    private class Sm64AudioBankSound : ISm64AudioBankSound {
      private LowLevelSm64AudioBankSound lowLevelImpl_;

      public Sm64AudioBankSound(LowLevelSm64AudioBankSound lowLevelImpl) {
        this.lowLevelImpl_ = lowLevelImpl;

        if (lowLevelImpl.sample.ToInt64() == 0) {
          throw new ArgumentException("Sound pointer is null.");
        }

        this.Sample = new Sm64AudioBankSample(
            MarshalUtil.MarshalRef<LowLevelSm64AudioBankSample>(
                lowLevelImpl.sample));
        this.Tuning = lowLevelImpl.tuning;
      }

      public ISm64AudioBankSample Sample { get; }
      public float Tuning { get; }
    }

    private class Sm64AudioBankSample : ISm64AudioBankSample {
      private LowLevelSm64AudioBankSample lowLevelImpl_;

      public Sm64AudioBankSample(LowLevelSm64AudioBankSample lowLevelImpl) {
        this.lowLevelImpl_ = lowLevelImpl;

        this.Loaded = lowLevelImpl.loaded != 0;
        this.Loop =
            new Sm64AdpcmLoop(
                MarshalUtil
                    .MarshalRef<LowLevelSm64AdpcmLoop>(lowLevelImpl.loop));
        this.Book =
            new Sm64AdpcmBook(
                MarshalUtil
                    .MarshalRef<LowLevelSm64AdpcmBook>(lowLevelImpl.book));
        this.Samples =
            MarshalUtil.MarshalArray<byte>(lowLevelImpl.sampleAddr,
                                           (int) lowLevelImpl.sampleSize);
      }

      public bool Loaded { get; }
      public byte[] Samples { get; }
      public ISm64AdpcmLoop Loop { get; }
      public ISm64AdpcmBook Book { get; }
    }

    public class Sm64AdpcmLoop : ISm64AdpcmLoop {
      public Sm64AdpcmLoop(LowLevelSm64AdpcmLoop lowLevelImpl) {
        this.Start = lowLevelImpl.start;
        this.End = lowLevelImpl.end;
        this.Count = lowLevelImpl.count;
        this.Pad = lowLevelImpl.pad;
        this.State = lowLevelImpl.state;
      }

      public uint Start { get; }
      public uint End { get; }
      public uint Count { get; }
      public uint Pad { get; }
      public short[] State { get; }
    }

    public class Sm64AdpcmBook : ISm64AdpcmBook {
      public Sm64AdpcmBook(LowLevelSm64AdpcmBook lowLevelImpl) {
        this.Order = lowLevelImpl.order;
        this.NPredictors = lowLevelImpl.npredictors;
        this.Predictors = lowLevelImpl.book;
      }

      public int Order { get; }
      public int NPredictors { get; }
      public short[] Predictors { get; }
    }
  }
}