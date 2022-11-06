namespace demo.camera.sm64 {
  public partial class Sm64Camera {
    uint unused8033B30C;
    uint unused8033B310;
    short sSelectionFlags;
    short unused8033B316;
    short s2ndRotateFlags;
    short unused8033B31A;
    short sCameraSoundFlags;
    ushort sCButtonsPressed;
    short sCutsceneDialogID;
    short unused8033B3E8;
    short sAreaYaw;
    short sAreaYawChange;
    short sLakituDist;
    short sLakituPitch;
    float sZoomAmount;
    short sCSideButtonYaw;
    short sBehindMarioSoundTimer;
    float sZeroZoomDist;
    short sCUpCameraPitch;
    short sModeOffsetYaw;
    short sSpiralStairsYawOffset;
    short s8DirModeBaseYaw;
    short s8DirModeYawOffset;
    float sPanDistance;
    float sCannonYOffset;
    short sCutsceneShot;
    short gCutsceneTimer;
    byte sObjectCutscene = 0;
    byte gRecentCutscene = 0;
    short gCameraMovementFlags;
    short sStatusFlags;
    Object? gCutsceneFocus;
    Object? gSecondCameraFocus;
    bool gObjCutsceneDone;
    uint gCutsceneObjSpawn;
    TransitionInfo sModeTransition;

    /**
     * Reset all the camera variables to their arcane defaults
     */
    void reset_camera(Camera c) {
      gCamera = c;
      gCameraMovementFlags = 0;
      s2ndRotateFlags = 0;
      sStatusFlags = 0;
      gCutsceneTimer = 0;
      sCutsceneShot = 0;
      gCutsceneObjSpawn = 0;
      gObjCutsceneDone = false;
      gCutsceneFocus = null;
      gSecondCameraFocus = null;
      sCButtonsPressed = 0;
      vec3f_copy(sModeTransition.marioPos, sMarioCamState.pos);
      sModeTransition.framesLeft = 0;
      gCameraMovementFlags = 0;
      gCameraMovementFlags |= (short) CamMove.CAM_MOVE_INIT_CAMERA;
      unused8033B316 = 0;
      sStatusFlags = 0;
      unused8033B31A = 0;
      sCameraSoundFlags = 0;
      sCUpCameraPitch = 0;
      sModeOffsetYaw = 0;
      sSpiralStairsYawOffset = 0;
      sLakituDist = 0;
      sLakituPitch = 0;
      sAreaYaw = 0;
      sAreaYawChange = 0;
      sPanDistance = 0f;
      sCannonYOffset = 0f;
      sZoomAmount = 0f;
      sZeroZoomDist = 0f;
      sBehindMarioSoundTimer = 0;
      sCSideButtonYaw = 0;
      s8DirModeBaseYaw = 0;
      s8DirModeYawOffset = 0;
      c.doorStatus = DoorStatus.DOOR_DEFAULT;
      sMarioCamState.headRotation[0] = 0;
      sMarioCamState.headRotation[1] = 0;
      sLuigiCamState.headRotation[0] = 0;
      sLuigiCamState.headRotation[1] = 0;
      sMarioCamState.cameraEvent = 0;
      sMarioCamState.usedObj = null;
      gLakituState.shakeMagnitude[0] = 0;
      gLakituState.shakeMagnitude[1] = 0;
      gLakituState.shakeMagnitude[2] = 0;
      gLakituState.unusedVec2[0] = 0;
      gLakituState.unusedVec2[1] = 0;
      gLakituState.unusedVec2[2] = 0;
      gLakituState.unusedVec1[0] = 0f;
      gLakituState.unusedVec1[1] = 0f;
      gLakituState.unusedVec1[2] = 0f;
      gLakituState.lastFrameAction = 0;
      set_fov_function(CameraFovFunc.CAM_FOV_DEFAULT);
      sFOVState.fov = 45f;
      sFOVState.fovOffset = 0f;
      sFOVState.unusedIsSleeping = 0;
      sFOVState.shakeAmplitude = 0f;
      sFOVState.shakePhase = 0;
      sObjectCutscene = 0;
      gRecentCutscene = 0;
      unused8033B30C = 0;
      unused8033B310 = 0;
    }
  }
}
