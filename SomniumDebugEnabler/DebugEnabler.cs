namespace SomniumDebugEnabler
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;
    using UnityEngine.Playables;
    using RogoDigital;
    using Cinemachine;
    using Game;
    using HarmonyLib;
    using MelonLoader;
    using SLua;
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Reflection;

    [HarmonyPatch(typeof(ScriptManager), "GetLuaFile")]
    internal class LuaFile
    {
        public static void Prefix(LuaState __instance)
        {
            if (DebugEnabler.forceAsset.Value)
            {
                Traverse.Create(__instance).Field("useAssetBundle").SetValue(false);
            }
        }
    }

    [HarmonyPatch(typeof(ScriptManager), "Start")]
    internal class LuaOverride
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {

            var code = new List<CodeInstruction>(instructions);
            for (int i = 0; i < code.Count - 1; i++)
            {
                if (code[i].opcode == OpCodes.Ldc_I4_1)
                {

                    code[i].opcode = OpCodes.Ldc_I4_0;
                }
            }

            return code;
        }

        public static void Prefix(LuaState __instance)
        {
            if (DebugEnabler.forceAsset.Value)
            {

                Traverse.Create(__instance).Field("useAssetBundle").SetValue(false);
                DebugEnabler.logger.Msg("Asset Bundle False");


            }
        }

        public static void Postfix(bool ___useAssetBundle)
        {
            DebugEnabler.logger.Msg(___useAssetBundle);
        }
    }

    [HarmonyPatch(typeof(LuaState), "setObject", new Type[] {
    typeof (string), typeof (object), typeof (bool), typeof (bool)
  })]
    internal class Patch
    {
        public static void Prefix(LuaState __instance, string key, ref object v)
        {
            if (key == "UNITY_EDITOR")
            {
                v = DebugEnabler.EDITOR.Value;
            }
            if (key == "UNITY_STANDALONE_WIN")
            {
                v = DebugEnabler.STANDALONE_WIN.Value;
            }
            if (key == "UNITY_PS4")
            {
                v = DebugEnabler.PS4.Value;
            }
            if (key == "UNITY_XBOXONE")
            {
                v = DebugEnabler.XBOXONE.Value;
            }
            if (key == "UNITY_SWITCH")
            {
                v = DebugEnabler.SWITCH.Value;
            }
            if (key == "BUILD_RELEASE")
            {
                v = DebugEnabler.BUILD_RELEASE.Value;
            }
            if (key == "BUILD_REGION")
            {
                v = DebugEnabler.BUILD_REGION.Value;
            }
            if (key == "USER_NAME")
            {
                v = DebugEnabler.USERNAME.Value;
                foreach (var item in DebugEnabler.Cust.Value.custDict)
                {
                    int foo;
                    if (int.TryParse(item.Value, out foo))
                    {
                        LuaState.main[item.Key] = foo;
                    }
                }

            }
        }

        public static void Postfix()
        {
            DebugEnabler.logger.Msg("GONZO!!!!!!!");
        }
    }

    public class DebugEnabler : MelonMod
    {
        public class ModPauseClass : RootNode
        {
            //public static RootNode inst;
            public void ModPause(RootNode inst)
            {
                logger.Msg("runmodpause");
                if(inst == null)
                {
                    logger.Warning("FUCK");
                }
                Type rootType = typeof(RootNode);
                FieldInfo pauseBool = rootType.GetField("pause", BindingFlags.NonPublic | BindingFlags.Instance);
                if(pauseBool == null)
                {
                    logger.Warning("Null Pausebool");
                }
                FieldInfo pauseStateObj = rootType.GetField("pauseState", BindingFlags.NonPublic | BindingFlags.Instance);
                if (pauseStateObj == null)
                {
                    logger.Warning("Null Pausebool");
                }
                logger.Msg("b1");
                if (!(bool)pauseBool.GetValue(inst))
                {
                    logger.Msg("b1");
                    pauseStateObj.SetValue(inst, new RootNode.PauseState());
                    pauseBool.SetValue(inst, true);
                    PauseState pauseStateAcc = (PauseState)pauseStateObj.GetValue(inst);
                    pauseStateAcc.monoBehaviours.Clear();
                    logger.Msg("b2");
                    //Modified so that the character's eyes and eyelids are frozen, but all other monobehaviors still run
                    foreach (MonoBehaviour monoBehaviour in inst.GetComponentsInChildren<MonoBehaviour>(true))
                    {
                        logger.Msg("b2");
                        if (monoBehaviour != null && (monoBehaviour.GetType() == typeof(EyeMove) || monoBehaviour.GetType() == typeof(EyeController)) && monoBehaviour.enabled)
                        {
                            monoBehaviour.enabled = false;
                            pauseStateAcc.monoBehaviours.Add(monoBehaviour);
                        }
                    }
                    logger.Msg("b2");
                    DynamicBone[] componentsInChildren2 = inst.GetComponentsInChildren<DynamicBone>(true);
                    for (int i = 0; i < componentsInChildren2.Length; i++)
                    {
                        componentsInChildren2[i].pause = true;
                    }
                    logger.Msg("b2");
                    pauseStateAcc.cameras.Clear();
                    foreach (Animator animator in inst.GetComponentsInChildren<Animator>(true))
                    {
                        if (animator != null && animator.enabled)
                        {
                            animator.enabled = false;
                            pauseStateAcc.animators.Add(animator);
                        }
                    }
                    logger.Msg("b2");
                    pauseStateAcc.audioSources.Clear();
                    foreach (AudioSource audioSource in inst.GetComponentsInChildren<AudioSource>(true))
                    {
                        if (audioSource != null && audioSource.enabled && audioSource.isPlaying)
                        {
                            audioSource.Pause();
                            pauseStateAcc.audioSources.Add(audioSource);
                        }
                    }
                    logger.Msg("b2");
                    pauseStateAcc.customAnimators.Clear();
                    foreach (CustomAnimator customAnimator in inst.GetComponentsInChildren<CustomAnimator>(true))
                    {
                        if (customAnimator != null)
                        {
                            customAnimator.Pause(true);
                            pauseStateAcc.customAnimators.Add(customAnimator);
                        }
                    }
                    logger.Msg("b2");
                    pauseStateAcc.playableDirectors.Clear();
                    foreach (PlayableDirector playableDirector in inst.GetComponentsInChildren<PlayableDirector>(true))
                    {
                        if (playableDirector != null && playableDirector.gameObject.activeInHierarchy && playableDirector.enabled && playableDirector.state == PlayState.Playing)
                        {
                            CutSceneHelper component = playableDirector.GetComponent<CutSceneHelper>();
                            if (component != null)
                            {
                                if (!component.pause)
                                {
                                    component.ui_pause = true;
                                    component.ui_time = playableDirector.time;
                                    component.enabled = true;
                                }
                            }
                            else
                            {
                                playableDirector.Pause();
                            }
                            pauseStateAcc.playableDirectors.Add(playableDirector);
                        }
                    }
                    logger.Msg("b2");
                    pauseStateAcc.particleSystems.Clear();
                    foreach (ParticleSystem particleSystem in inst.GetComponentsInChildren<ParticleSystem>(true))
                    {
                        if (particleSystem != null && particleSystem.gameObject.activeSelf && particleSystem.isPlaying)
                        {
                            particleSystem.Pause();
                            pauseStateAcc.particleSystems.Add(particleSystem);
                        }
                    }
                    logger.Msg("b2");
                    pauseStateAcc.rigidbodys.Clear();
                    foreach (Rigidbody rigidbody in inst.GetComponentsInChildren<Rigidbody>(true))
                    {
                        if (rigidbody != null)
                        {
                            RootNode.RigidbodyState rigidbodyState = new RootNode.RigidbodyState();
                            rigidbodyState.rigidbody = rigidbody;
                            rigidbodyState.velocity = rigidbody.velocity;
                            rigidbodyState.angularVelocity = rigidbody.angularVelocity;
                            rigidbody.Sleep();
                            pauseStateAcc.rigidbodys.Add(rigidbodyState);
                        }
                    }
                }
            }
        }
        struct CameraBackupState
        {
            public Vector3 position;
            public Vector3 eulerAngles;
        }

        struct CameraClipState
        {
            public float near;
            public float far;
            public float fieldOfView;
        }

        bool custom_clip;
        bool immediateGUIHidden;
        float rotX;
        float rotY;
        bool fpsEnabled;
        float userFov;
        bool isPaused;
        bool debugMenuActive;   //Set to true when Debug menu and debug info is being collected
        bool danceMode;         //Set to true if F8 is pressed during dance mode
        bool manualCameraOverrideIsActive; //Set to true if the debug menu buttons should be used to control camera override (instead of automatically)

        //Used to save/restore the clip settings of the GUI cameras
        Dictionary<Camera, CameraClipState> backupClipState;
        //Used to save/restore the "enabled" state of the GUI cameras
        Dictionary<Camera, Vector3> backupGUICameraState;
        //Used to save the position/rotation of each 'normal' camera
        Dictionary<Camera, CameraBackupState> backupCameraPositions;
        //Used to save whether collision is enabled/disabled of each cinemachine collider (which is attached to a cinemachine camera)
        Dictionary<CinemachineCollider, bool> backupCollisionStates;
        //Debug only - used to manually override cameras by name
        Dictionary<string, bool> manualCameraOverrideActive;

        GameObject cube;
        CinemachineFreeLook customFreeLook;
        Vector3 camPosOverride;

        private void SetTimescale(float newTimeScale)
        {
            Time.timeScale = newTimeScale;
            // Adjust fixed delta time according to timescale
            // The fixed delta time will now be 0.02 frames per real-time second
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        // Game.InputProc
        // Tip: Use the Unity Editor to prototype the GUI rather than trying to do it in-game
        public override void OnGUI()
        {
            if (!immediateGUIHidden)
            {
                //GUI.Box(new Rect(10f, 10f, 450f, 90f), "Mod Menu");
                //GUI.Button(new Rect(20f, 100f, 400f, 20f), "Button 1");
                //GUI.Button(new Rect(20f, 120f, 400f, 20f), "Button 2");

                // Make a background box
                Rect backgroundArea = new Rect(Screen.width - 420f, 10f, 400f, Screen.height - 20f) ;

                Rect contentArea = new Rect(backgroundArea.x + 10f,
                                            backgroundArea.y + 20f,
                                            backgroundArea.width - 20f,
                                            backgroundArea.height - 20f);

                GUI.Box(backgroundArea, "Keyboard Shortcuts");

                GUILayout.BeginArea(contentArea);

                GUILayout.Label(
    @"Press F10 & F11 to toggle the GUIs! (for taking screenshots)
Movement Controls:
F8 - Enable Noclip/FPS mode
F9 - Disable Noclip/FPS mode
P and ; - Move forward and back (hold SHIFT to move faster)
L  and ' - Move left and right (hold SHIFT to move faster)
SHIFT - Hold to move faster
Mouse Rotation - Rotate the camera (while in FPS mode)
Screenshot Controls:
F11 - Toggle this window
F10 - Toggle Game GUI
K or F7 - Toggle (Camera Zoom and Near Clip mode)
Scroll Wheel - Adjust camera zoom level (must enable Camera Zoom first)
NOTE: Camera Zoom is especially useful if the game decides to zoom in too much.
Slow Motion/Pause Controls:
O - Toggle Pause/Resume of the game
F3 - 10x Slow Motion (note: doesn't always work)
NOTE: in all pause/slowmo modes, you can move the camera freely
Rarely Used Controls:
F6 - Enable Noclip/FPS Mode with Magenta Box
Press F10 & F11 to toggle the GUIs! (for taking screenshots)
https://github.com/Slavanomics/SomniumDebugEnabler



creds to https://github.com/drojf/ai_somnium_fps_camera for the original freecam code.
----------------------------------------------------
");
                GUILayout.Label($"Zoom/Fov: {(backupClipState == null ? "disabled" : "ENABLED")} (Press 'K' to toggle)");
                if (backupClipState != null)
                {
                    GUILayout.Label($"User Fov: {userFov} (Adjust with scroll wheel)");
                }

                if (fpsEnabled)
                {
                    GUILayout.Label($"FPS Mode Enabled - Press 'F9' to disable");
                }

                if (backupGUICameraState != null)
                {
                    GUILayout.Label($"GAME GUI IS HIDDEN - Press 'F10' to show");
                }

                if (isPaused)
                {
                    GUILayout.Label($"GAME IS PAUSED - Press 'O' to unpause");
                }

                // Draw the debug menu
                debugMenuActive = GUILayout.Toggle(debugMenuActive, "Advanced Debug Tools", new GUILayoutOption[0]);
                if (debugMenuActive)
                {
                    if (manualCameraOverrideActive == null)
                    {
                        manualCameraOverrideActive = new Dictionary<string, bool>();
                    }

                    // Display the scene name, and all the cameras in the current scene
                    GUILayout.Label($"Current Scene: [{SceneManager.GetActiveScene().name}]");
                    GUILayout.Label($"Cameras in scene:");

                    manualCameraOverrideIsActive = GUILayout.Toggle(manualCameraOverrideIsActive, $"Manual Override Enabled");

                    foreach (Camera camera in Camera.allCameras)
                    {
                        string cameraIsOverridenString = "";
                        if (backupCameraPositions != null && backupCameraPositions.ContainsKey(camera))
                        {
                            cameraIsOverridenString = " - Automatic Override Active";
                        }

                        // Create a toggle allowing you override the camera
                        bool currentToggleValue = false;
                        if (manualCameraOverrideActive.TryGetValue(camera.name, out bool value))
                        {
                            currentToggleValue = value;
                        }
                        manualCameraOverrideActive[camera.name] = GUILayout.Toggle(currentToggleValue, $"{camera.name}{cameraIsOverridenString}");
                    }
                }

                GUILayout.EndArea();
            }
        }

        bool cameraMightBeActiveCamera(Camera camera)
        {
            return !camera.name.Contains("UICamera") && !camera.name.Contains("Button") && !camera.name.Contains("Background") && camera.name != "Camera";
        }

        void forceGraphicAlpha(Graphic graphic, float newAlpha)
        {
            Color color = graphic.canvasRenderer.GetColor();
            color.a = newAlpha;
            graphic.canvasRenderer.SetColor(color);
        }

        // Game.InputProc
        public override void OnLateUpdate()
        {
            // Set user fov to 45 if it's zero (would do in constructor but easier to just do here)
            if (userFov == 0f)
            {
                userFov = 45f;
            }

            // Prevent fov getting too small or too large
            userFov = Mathf.Clamp(userFov, 1f, 180f);

            if (backupClipState == null)
            {
                // Enable custom fov/clip distance mode via F7 key or mouse wheel scroll
                if (Input.GetKeyDown(KeyCode.F7) || Input.GetKeyDown(KeyCode.K))
                {
                    userFov = 45f;
                    backupClipState = new Dictionary<Camera, CameraClipState>();
                }
            }
            else
            {
                if (Input.mouseScrollDelta.y > 0f)
                {
                    // Zoom in when scrolling up
                    userFov *= 0.9f;
                }
                else if (Input.mouseScrollDelta.y < 0f)
                {
                    // Zoom out when scrolling down
                    userFov *= 1.1f;
                }

                // Disable custom fov/clip distance mode via F7 key
                if (Input.GetKeyDown(KeyCode.F7) || Input.GetKeyDown(KeyCode.K))
                {
                    // Restore clip settings
                    foreach (KeyValuePair<Camera, CameraClipState> kvp in backupClipState)
                    {
                        kvp.Key.near = kvp.Value.near;
                        kvp.Key.far = kvp.Value.far;
                        kvp.Key.fieldOfView = kvp.Value.fieldOfView;
                    }
                    backupClipState = null;
                }
            }

            if (backupClipState != null)
            {
                foreach (Camera camera in Camera.allCameras)
                {
                    // If a new camera is found, backup clip settings, then force clip plane to .1 and FOV to 60 degrees
                    if (!backupClipState.ContainsKey(camera))
                    {
                        backupClipState[camera] = new CameraClipState()
                        {
                            near = camera.near,
                            far = camera.far,
                            fieldOfView = camera.fieldOfView,
                        };
                    }

                    camera.near = .1f;
                    camera.fieldOfView = userFov;
                }
            }

            // Press F11 to toggle the keyboard shortcuts window
            if (Input.GetKeyDown(KeyCode.F11))
            {
                immediateGUIHidden = !immediateGUIHidden;
            }

            // Press F10 to toggle GUI on/off
            if (Input.GetKeyDown(KeyCode.F10))
            {
                if (backupGUICameraState == null)
                {
                    // This can be replaced with a bool later
                    backupGUICameraState = new Dictionary<Camera, Vector3>();
                }
                else
                {
                    // Set all "UnityEngine.UI.Graphic" objects not named "Image" to 1 alpha
                    // This doesn't properly restore the UI's alpha value (if it was not originally '1') but should be good enough
                    foreach (Graphic graphic in UnityEngine.Object.FindObjectsOfType(typeof(Graphic)))
                    {
                        if (graphic.name != "Image")
                        {
                            forceGraphicAlpha(graphic, 1f);
                        }
                    }

                    backupGUICameraState = null;
                }
            }

            // Every keyframe, rotate the bustshot camera way out of the way so the character isn't rendered
            // We can't disable the camera, as the game will just spawn a new camera if it detects the normal camera is disabled.
            foreach (Camera camera2 in Camera.allCameras)
            {
                if (camera2.name == "Camera")
                {
                    camera2.transform.rotation = ((this.backupGUICameraState == null) ?
                    new Quaternion(0f, -1f, 0f, 0f) : // This is the normal position, camera pointing forwards
                    new Quaternion(0f, 0f, -1f, 0f)); // This disables the camera by pointing the camera backwards
                }
            }

            // This must be done every frame (or periodically) in case you hover over an item in ADV mode which causes a new UI widget to spawn
            //For all UI elements except those named "Image":
            // - Backup the UI color values
            // - Set 0 alpha
            if (backupGUICameraState != null)
            {
                foreach (Graphic graphic in UnityEngine.Object.FindObjectsOfType(typeof(Graphic)))
                {
                    // Set graphics not named "Image" to 0 alpha
                    if (graphic.name != "Image")
                    {
                        forceGraphicAlpha(graphic, 0f);
                    }
                }
            }

            // Game slowdown, pause, and resume
            // see https://docs.unity3d.com/ScriptReference/Time-timeScale.html)
            {
                // Press "O" key to toggle pausing the game
                if (Input.GetKeyDown(KeyCode.O))
                {
                    if (isPaused)
                    {
                        SetTimescale(1f);
                        foreach (RootNode rootNode in UnityEngine.Object.FindObjectsOfType<RootNode>())
                        {
                            rootNode.UnPause();
                        }
                    }
                    else
                    {
                        SetTimescale(0);
                        foreach (RootNode rootNode in UnityEngine.Object.FindObjectsOfType<RootNode>())
                        {
                            var test = new ModPauseClass();
                            test.ModPause(rootNode);
                        }
                    }
                    isPaused = !isPaused;
                }

                // Press "F3" to slow time by 10x (only works in certain scenes)
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    SetTimescale(.1f);
                    foreach (RootNode rootNode in UnityEngine.Object.FindObjectsOfType<RootNode>())
                    {
                        rootNode.UnPause();
                    }
                }
            }


            if (this.fpsEnabled)
            {
                float mouseSensitivity = 100f;

                //Add FPS camera rotation (independent of timescale)
                this.rotX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.unscaledDeltaTime;
                this.rotY += Input.GetAxis("Mouse Y") * mouseSensitivity * Time.unscaledDeltaTime;
                this.rotY = Mathf.Clamp(this.rotY, -90f, 90f);

                foreach (Camera camera in Camera.allCameras)
                {
                    bool shouldOverrideCamera = false;
                    if (manualCameraOverrideIsActive && manualCameraOverrideActive.TryGetValue(camera.name, out bool value))
                    {
                        shouldOverrideCamera = value;
                    }
                    else if (danceMode)
                    {
                        // Dance mode is special - need to override the camera called "Camera"
                        shouldOverrideCamera = camera.name == "Camera";
                    }
                    else
                    {
                        // Only move the right camera (used in ADV mode) and character camera (used in somniums)
                        shouldOverrideCamera = cameraMightBeActiveCamera(camera) || camera.name == "Character Camera";
                    }

                    if (shouldOverrideCamera)
                    {
                        camera.transform.eulerAngles = new Vector3(-this.rotY, this.rotX, 0f);

                        //Backup the camera if it hasn't already been backed up
                        if (!backupCameraPositions.ContainsKey(camera))
                        {
                            backupCameraPositions[camera] = new CameraBackupState()
                            {
                                position = camera.transform.position,
                                eulerAngles = camera.transform.eulerAngles,
                            };
                        }

                        //Add FPS camera movement (independent of timescale)
                        Vector3 moveDir = default(Vector3);
                        float speed = (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) ? 20f : 1.5f;
                        speed *= Time.unscaledDeltaTime;

                        //Move right and forward relative to the camera
                        if (Input.GetKey(KeyCode.P))
                        {
                            moveDir += speed * camera.transform.forward;
                        }
                        if (Input.GetKey(KeyCode.Semicolon))
                        {
                            moveDir -= speed * camera.transform.forward;
                        }
                        if (Input.GetKey(KeyCode.Quote))
                        {
                            moveDir += speed * camera.transform.right;
                        }
                        if (Input.GetKey(KeyCode.L))
                        {
                            moveDir -= speed * camera.transform.right;
                        }

                        //Always move up/down the Z axis
                        if (Input.GetKey(KeyCode.LeftBracket))
                        {
                            moveDir += speed * Vector3.up;
                        }
                        if (Input.GetKey(KeyCode.RightBracket))
                        {
                            moveDir -= speed * Vector3.up;
                        }

                        if (camera.name == "Character Camera")
                        {
                            this.cube.transform.position += moveDir;
                        }
                        else
                        {
                            //move camera itself, only if not using a customFreeLookCamera
                            if (customFreeLook == null)
                            {
                                // Each 'normal' camera has an overridePosition where translation is accumulated
                                camPosOverride += moveDir;
                                camera.transform.position = camPosOverride;
                            }
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.F9))
                {
                    this.fpsEnabled = false;

                    if (customFreeLook != null)
                    {
                        //Set the custom freelook camera to the lowest priority so it becomes disabled
                        customFreeLook.m_Priority = int.MinValue;
                        customFreeLook = null;
                    }

                    if (cube != null)
                    {
                        UnityEngine.Object.Destroy(cube);
                        cube = null;
                    }

                    //attempt to restore each camera's transform. If you enter a menu or change scene without exiting, this might break.
                    foreach (KeyValuePair<Camera, CameraBackupState> kvp in backupCameraPositions)
                    {
                        kvp.Key.transform.position = kvp.Value.position;
                        kvp.Key.transform.eulerAngles = kvp.Value.eulerAngles;
                    }

                    // Restore collider states
                    foreach (KeyValuePair<CinemachineCollider, bool> kvp in backupCollisionStates)
                    {
                        kvp.Key.m_AvoidObstacles = kvp.Value;
                    }

                    // Clear backups
                    this.backupCameraPositions.Clear();
                    backupCollisionStates.Clear();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.F6) | Input.GetKeyDown(KeyCode.F8))
                {
                    this.fpsEnabled = true;
                    this.rotX = 0;
                    this.rotY = 0;

                    // Record if fps enabled during "Dance" mode
                    danceMode = SceneManager.GetActiveScene().name == "Dance";

                    if (backupCameraPositions == null)
                    {
                        backupCameraPositions = new Dictionary<Camera, CameraBackupState>();
                    }

                    if (backupCollisionStates == null)
                    {
                        backupCollisionStates = new Dictionary<CinemachineCollider, bool>();
                    }

                    //////////////// Modify somium camera so it can be moved properly ////////////////
                    //Disable camera collisions on all cinemachine coliders
                    foreach (CinemachineCollider collider in UnityEngine.Object.FindObjectsOfType<CinemachineCollider>())
                    {
                        backupCollisionStates[collider] = collider.m_AvoidObstacles;
                        collider.m_AvoidObstacles = false;
                    }

                    //Set initial camera override position to the first "might be active camera" camera
                    camPosOverride = new Vector3();
                    foreach (Camera camera in Camera.allCameras)
                    {
                        if (cameraMightBeActiveCamera(camera))
                        {
                            camPosOverride = camera.transform.position;
                            break;
                        }
                    }

                    // Spawn a unity object for the CinemachineFreeLook to follow, save it to the class
                    if (Input.GetKeyDown(KeyCode.F8))
                    {
                        cube = new GameObject();
                    }
                    else
                    {
                        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    }

                    //Clone the first existing CinemachineFreeLook camera (if it exists)
                    foreach (CinemachineFreeLook freelook in UnityEngine.Object.FindObjectsOfType<CinemachineFreeLook>())
                    {
                        if (freelook.name != "hackedFPSFreeLook")
                        {
                            //Set the cube's location to the last freelook follow's position, to give a decent initial position
                            cube.transform.position = freelook.Follow.position;

                            if (customFreeLook == null)
                            {
                                customFreeLook = (CinemachineFreeLook)UnityEngine.Object.Instantiate(freelook);
                            }

                            customFreeLook.name = "hackedFPSFreeLook";

                            //set Follow to cube
                            customFreeLook.Follow = cube.transform;

                            //give it the highest priority
                            customFreeLook.m_Priority = int.MaxValue;
                            break;

                        }
                    }
                    //////////////// Modify somium camera so it can be moved properly ////////////////
                }
            }
        }
        public static MelonLogger.Instance logger;

        public static MelonPreferences_Category overCat;

        public static MelonPreferences_Category customCat;

        public static MelonPreferences_Entry<int> EDITOR;

        public static MelonPreferences_Entry<int> STANDALONE_WIN;

        public static MelonPreferences_Entry<int> PS4;

        public static MelonPreferences_Entry<int> XBOXONE;

        public static MelonPreferences_Entry<int> SWITCH;

        public static MelonPreferences_Entry<int> BUILD_RELEASE;

        public static MelonPreferences_Entry<string> BUILD_REGION;

        public static MelonPreferences_Entry<string> USERNAME;

        public static MelonPreferences_Entry<bool> forceAsset;

        public static MelonPreferences_Entry<CustomType> Cust;

        public class CustomType
        {
            public Dictionary<string, string> custDict = new Dictionary<string, string> { { "DEVELOP_MODE", "1" } };
        }

        public override void OnApplicationStart()
        {

            logger = LoggerInstance;
            overCat = MelonPreferences.CreateCategory("Overrides");
            EDITOR = overCat.CreateEntry("UNITY_EDITOR", 0);
            STANDALONE_WIN = overCat.CreateEntry("UNITY_STANDALONE_WIN", 1);
            PS4 = overCat.CreateEntry("UNITY_PS4", 0);
            XBOXONE = overCat.CreateEntry("UNITY_XBOXONE", 0);
            SWITCH = overCat.CreateEntry("UNITY_SWITCH", 0);
            BUILD_RELEASE = overCat.CreateEntry("BUILD_RELEASE", 0);
            BUILD_REGION = overCat.CreateEntry("BUILD_REGION", "BUILD_WORLDWIDE");
            USERNAME = overCat.CreateEntry("USER_NAME", "gonzo");
            forceAsset = overCat.CreateEntry("FORCE_ASSET_BUNDLE", true);
            Cust = overCat.CreateEntry<CustomType>("CUSTOM_OVERRIDES", new CustomType());

            overCat.SaveToFile();


            logger.Msg("Mod Loaded.");
        }
    }
}
