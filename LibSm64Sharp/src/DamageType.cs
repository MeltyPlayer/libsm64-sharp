namespace libsm64sharp;

public enum DamageType : uint {
  DELAY_INVINCIBILITY = 0x00000002,

  // Used by Bowser, sets Mario's forward velocity to 40 on hit
  BIG_KNOCKBACK = 0x00000008
}