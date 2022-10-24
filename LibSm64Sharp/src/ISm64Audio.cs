namespace libsm64sharp {
  public interface ISm64AudioBanks {
    IReadOnlyList<ISm64CtlEntry> CtlEntries { get; }
  }

  public interface ISm64CtlEntry {
    IReadOnlyList<ISm64Instrument> Instruments { get; }
    IReadOnlyList<ISm64Drum> Drums { get; }
  }

  public interface ISm64Drum {
    bool Loaded { get; }
    byte ReleaseRate { get; }
    byte Pan { get; }
    ISm64AudioBankSample Sound { get; }
  }

  public interface ISm64Instrument {
    bool Loaded { get; }
    byte ReleaseRate { get; }
    byte NormalRangeLo { get; }
    byte NormalRangeHi { get; }
    ISm64AudioBankSample? LowNotesSound { get; }
    ISm64AudioBankSample NormalNotesSound { get; }
    ISm64AudioBankSample? HighNotesSound { get; }
  }

  public interface ISm64AudioBankSound {
    ISm64AudioBankSample Sample { get; }
    float Tuning { get; }
  }

  public interface ISm64AudioBankSample {
    bool Loaded { get; }
    short[] Samples { get; }
  }
}