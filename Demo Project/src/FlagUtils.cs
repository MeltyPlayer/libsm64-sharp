namespace demo;

public static class FlagUtils {
  public static bool MatchesFlag<T>(this T enumValue, T flag) where T : Enum
    => enumValue.HasFlag(flag);
}