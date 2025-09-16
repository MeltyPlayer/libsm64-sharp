namespace libsm64sharp;

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
  ISm64AudioBankSound Sound { get; }
}

public interface ISm64Instrument {
  bool Loaded { get; }
  byte ReleaseRate { get; }
  byte NormalRangeLo { get; }
  byte NormalRangeHi { get; }
  ISm64AudioBankSound? LowNotesSound { get; }
  ISm64AudioBankSound NormalNotesSound { get; }
  ISm64AudioBankSound? HighNotesSound { get; }
}

public interface ISm64AudioBankSound {
  ISm64AudioBankSample Sample { get; }
  float Tuning { get; }
}

public interface ISm64AudioBankSample {
  bool Loaded { get; }
  byte[] Samples { get; }
  ISm64AdpcmLoop Loop { get; }
  ISm64AdpcmBook Book { get; }
}

public interface ISm64AdpcmLoop {
  uint Start { get; }
  uint End { get; }
  uint Count { get; }
  uint Pad { get; }
  short[] State { get; }
}

public interface ISm64AdpcmBook {
  int Order { get; }
  int NPredictors { get; }
  public short[] Predictors { get; }
}