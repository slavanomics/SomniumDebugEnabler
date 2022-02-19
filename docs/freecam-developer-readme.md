# Developer Readme

## Please Read (17-02-2022)

Originally, I made this mod by directly editing the AI: Somnium Files DLL using DnSpy. However, the code has now been ported to [melonloader](https://github.com/LavaGang/MelonLoader) by [@slavanomics](https://github.com/slavanomics/SomniumDebugEnabler). Please keep this in mind when reading this page, as I may not have properly updated all the information to reflect this. 

I've taken out no-longer relevant information from this page - instead you can check the melonloader website for info on editing/maintaining a melonloader mod. However I've still left in notes which are still relevant on this page.

Note that the users on the Uchikoshi/AITS discord: http://discord.gg/XKMreYw have much more knowledge about the workings of the engine than I do, so please head there unless it's something specific to this mod.

### Rendering

From what I can gather, rendering works like this:

- Various cameras render to a texture
- This gets put on a UI UnityEngine.UI.RawImage type object (thats the base type at least) called "Image" (there are multiple of these named "Image" when viewed in the inspector)
- The camera named "UICamera" then renders the "Image" UI widget (and all the other UI widgets)

As a result, if you disable either the "UICamera" camera or disable the "Image" GUI widget, the 3D part of the game won't be displayed.

### Camera names

- "Right Camera" - main camera for ADV mode?
- "Character Camera" - Somnium camera (interacts with Cinemachine)
- "Camera" - used for character portraits
- Other camera names: BackgroundCamera, UICamera, UICamera2, UICamera3D, ButtonCamer, FrontCamera, MiddleCamera, AIBALL_RENDER_Camera, RightWindow

Camera List in Somnium:

- BackgroundCamera - might be used for UI?
- UICamera - Disabling this makes screen blank in somniums, so maybe it's the final compositing step. Cursor might be rendered also.
- UICamera2 - might be used for UI?
- UICamera3D - might be used for UI?
- MiddleCamera - might be used for UI?
- FrontCamera - might be used for UI?
- RightCamera - might be used for UI?
- RightWindow - might be used for UI?
- Character Camera - used for character portraits
- AIBALL_RENDER_Camera - used for AIBALL vision (I assume)

Camera list in ADV mode:

- BackgroundCamera
- UICamera
- FrontCamera
- ButtonCamera
- UICamera
- Camera - used for character portraits?
- RightCamera
- Camera01 (Cinematic camera?)


### Class info

- "TextController" used to draw the text glyphs on the screen (but not for the portrait window or text background)

## Resources Used

- https://www.unknowncheats.me/forum/unity/285864-beginners-guide-hacking-unity-games.html
- DnSpy
- https://docs.unity3d.com/Packages/com.unity.cinemachine@2.3/manual/CinemachineOverview.html
- https://docs.unity3d.com/Packages/com.unity.cinemachine@2.3/manual/CinemachineFreeLook.html
- https://docs.unity3d.com/Manual/gui-Basics.html
- https://docs.unity3d.com/2018.1/Documentation/ScriptReference/UI.Graphic.html

### Unity Pages

https://docs.unity3d.com/ScriptReference/Camera-allCameras.html

## AI_TheSomniumFiles Arguments

The game has a `Launcher.exe` which calls `AI_TheSomniumFiles.exe` depending on the graphics options you chose. For example:

`"C:\games\Steam\steamapps\common\AI The Somnium Files\AI_TheSomniumFiles.exe"  quality=2 window=0 msaa=2 width=2560 height=1440 vsync=1 filtering=1 hint=0`

## Widescreen Resolution Hacking Attempts

I attempted to change the resolution to widescreen using the above command line arguments. (eg in the above, set `width=2000 height=800`)

If you do this, everything will still be in a 16:9 ratio (letterboxed), and also some GUI will be extremely large.

I then attempted to force the game to the correct ratio by editing the `Game.ScreenScaler` classes `Update()` function. (found by searching `1920` literal in Dnspy (remember to change the search method or you'll get nothing))

I added this code at the bottom of the function:

```csharp
    //Lots of existing code emitted here
    // ??? Maybe set the default screen offset to 0, so drawing begins at the top left of the screen
    this.rectTransform.anchoredPosition = new Vector2(0f, 0f);
    // ??? Maybe scale the already scaled resolution to undo the the 16:9 ratio and then apply the Width/Height ratio
    base.transform.localScale = new Vector3(base.transform.localScale.x / num * (float)Screen.width / (float)Screen.height, base.transform.localScale.y, base.transform.localScale.z);
```

While this fixed the 3D sections of the game work fine, and fixed some menus/UI to scale correctly, the "options menu", scene select menu, and possibly other GUI still remain out of whack (very zoomed in).

I'm not sure if there's some way to scale these particular GUI elements causing a problem. Perhaps by debugging those scenes and finding the cameras, the values could be overridden, but it would be a reasonable amount of work.

For future reference, the following classes appear to reference the aspect ratio:

- Game.CameraScaler (1.77...f)
- Game.ScreenScaler (1.77...f) <- this is the one I modified
- Game.ScreenScalerSafeArea (1.77...f)
- FixAspectRatio(not sure if this is a unity function. Doesn't appear to be used) (1.77...f)
- LauncherArgs (1920)
- OptionMenuRaycaster (1920)

## Game Script Details

The game script uses Lua (specifically, lua-5.1, 64 bit). The lua scripts are pre-compiled into byte code, obfuscated, then saved in the bundle at `AI The Somnium Files\AI_TheSomniumFiles_Data\StreamingAssets\AssetBundles\StandaloneWindows64\luabytecode`.

The files are de-obfuscated in the `Game.ScriptManager.LoaderDelegate()` function (please keep this `ScriptManager` class in mind as it's responsible for various script related things).

You can view the bytecode of the scripts by getting the correct version of the lua binaries (I used lua-5.1.5 **64 bit**) and running `luac -l -l script_name` (yes, use -l twice.).

The scripts are probably not hand written, and instead generated by the library they used, Slua, which you can see here: https://github.com/pangweiwei/slua

## Text Display

The `TextController` class is responsible for displaying glyphs on the screen (but not the actual textbox).

## Game's Pausing function

When I called the game's pausing function, it initally didn't resume (screen would go back). I modified it to not disable any GameObjects, which seemed to make pausing work, except for the eye movement/blinking as below.

The pausing function generally iterates over various types of objects and calls disable/pause on them.

Compare the `RootNode.ModPause` to the `RootNode.Pause` to see more detail.

## Eye Movement and Blinking

The `EyeMove` and `EyeController` classes appear to be responsible for eye rotation and blinking. When I called my modified pause function, it caused the eyes to point in wierd directions, and any issued blinks to close eyes permanently and not reopen. Causing the pause function to disable just these two classes seemed to fix the issue.

### GUI "Graphic" list

There are multiple graphics named "image" - these seem to be used as render targets.
One is used to render the main 3D display.
One might be used for the AIBall display.
I wanted to selectively enable the AIBall display, but since it has the same name it's difficult to pick out.
The "Mask" graphic may also have something to do with the AIBall display

Boss's room example:

- array    {UnityEngine.UI.Graphic[0x00000060]}    UnityEngine.Object[] {UnityEngine.UI.Graphic[]}
- [0]    {TutorialWindow (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [1]    {Image3b (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [2]    {Image2 (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [3]    {IconText (UnityEngine.UI.RawImage)}    UnityEngine.UI.Graphic {UnityEngine.UI.RawImage}
- [4]    {Image01 (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [5]    {Prompt (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [6]    {Text (TMPro.TextMeshProUGUI)}    UnityEngine.UI.Graphic {TMPro.TextMeshProUGUI}
- [7]    {Image_R (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [8]    {TutorialWindow (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [9]    {TutorialWindow (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [10]    {TutorialWindow (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [11]    {TutorialWindow (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [12]    {Image4 (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [13]    {Image1 (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [14]    {Image5 (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [15]    {Image3c (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [16]    {Text (TMPro.TextMeshProUGUI)}    UnityEngine.UI.Graphic {TMPro.TextMeshProUGUI}
- [17]    {Button (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [18]    {Image (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [19]    {Image (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [20]    {Text (TMPro.TextMeshProUGUI)}    UnityEngine.UI.Graphic {TMPro.TextMeshProUGUI}
- [21]    {Folder (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [22]    {Base (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [23]    {Image3 (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [24]    {Button_02 (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [25]    {FilterBlur (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [26]    {Background (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [27]    {BG01 (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [28]    {Filter (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [29]    {Mask (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [30]    {Image2D (UnityEngine.UI.RawImage)}    UnityEngine.UI.Graphic {UnityEngine.UI.RawImage}
- [31]    {LeftFilter (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [32]    {Image (UnityEngine.UI.RawImage)}    UnityEngine.UI.Graphic {UnityEngine.UI.RawImage}
- [33]    {Image04 (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [34]    {Image (UnityEngine.UI.RawImage)}    UnityEngine.UI.Graphic {UnityEngine.UI.RawImage}
- [35]    {ScreenScaler (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [36]    {Image (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [37]    {Outline_back (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [38]    {Normal (UnityEngine.UI.RawImage)}    UnityEngine.UI.Graphic {UnityEngine.UI.RawImage}
- [39]    {FilterTop (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [40]    {Text (TMPro.TextMeshProUGUI)}    UnityEngine.UI.Graphic {TMPro.TextMeshProUGUI}
- [41]    {[Normal] (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [42]    {Frame (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [43]    {Image00 (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [44]    {Button_03 (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [45]    {Image2D (UnityEngine.UI.RawImage)}    UnityEngine.UI.Graphic {UnityEngine.UI.RawImage}
- [46]    {Image (UnityEngine.UI.RawImage)}    UnityEngine.UI.Graphic {UnityEngine.UI.RawImage}
- [47]    {Mask (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [48]    {Button_01 (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [49]    {ui_main_infoW02_hit_add (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [50]    {ui_main_infoW01_add (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [51]    {button_icon (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [52]    {image (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [53]    {image_rest2 (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [54]    {image_rest3 (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [55]    {Base (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [56]    {Mask (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [57]    {ui_main_infoW02_hit_add (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [58]    {ui_main_infoW01_add (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [59]    {button_icon (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [60]    {image (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [61]    {Base (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [62]    {ui_main_infoW02_hit_add (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [63]    {ui_main_infoW01_add (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [64]    {button_icon (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [65]    {image (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [66]    {Base (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [67]    {Mask (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [68]    {ui_main_infoW02_hit_add (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [69]    {ui_main_infoW01_add (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [70]    {button_icon (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [71]    {image (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [72]    {Base (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [73]    {Mask (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [74]    {LoopHit_01 (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [75]    {ColorHit_01 (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [76]    {ui_main_infoW02_hit_add (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [77]    {ui_main_infoW01_add (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [78]    {button_icon (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [79]    {image (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [80]    {Base (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [81]    {Mask (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [82]    {ui_main_infoW02_hit_add (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [83]    {ui_main_infoW01_add (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [84]    {button_icon (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [85]    {image (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [86]    {Base (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [87]    {Mask (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [88]    {ui_main_infoW02_hit_add (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [89]    {ui_main_infoW01_add (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [90]    {button_icon (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [91]    {image (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [92]    {Base (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [93]    {Mask (Game.NonDrawingGraphic)}    UnityEngine.UI.Graphic {Game.NonDrawingGraphic}
- [94]    {BaseImage (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}
- [95]    {EffectBase (UnityEngine.UI.Image)}    UnityEngine.UI.Graphic {UnityEngine.UI.Image}

## TODO

- ~~Add button to hide the UI (for taking screenshots) ... is there already a button for this in the game?~~
- ~~Reduce near field clip of camera to allow being closer to objects without clipping~~ (https://forum.unity.com/threads/recommended-minimum-near-clipping-plane-of-cameras.348620/)
- Allow selectively hiding AIBALL? need to run some tests to see if this will work as intended
- Should this be added? Add momentum/smoothing to movement?