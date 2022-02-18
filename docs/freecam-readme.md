# AI: The Somnium Files FPS Camera Mod

See this video for a brief demonstration of the mod: https://www.youtube.com/watch?v=iR8HZhfthF0 (Please turn on Close Captions/Subtitles to see keys pressed).

## What does the mod do?

This mod adds the following features to the game:

- A "Noclip Mode" FPS style camera allowing you to explore ADV mode scenes and Somniums. You freely rotate and translate the camera, and can move faster with `SHIFT` key to get around large levels (see above video)
- Screenshot helper features like:
  - the ability to entirely hide the game GUI (and the mod's shortcuts cheatsheet)
  - the ability to pause and resume the game, while still being able to move the camera freely
  - the ability to override the zoom level, and adjust the zoom level using the mouse wheel
  - (extra feature: the ability to slow down the game by 10x (doesn't work in all scenes))
- A rudimentary GUI which shows the keyboard shortcuts and which features are enabled.

This mod may be useful for taking screenshots or videos of the game. You can also explore the maps of the game - but note that the game appears to be fairly "clean" - there are a few wierd T-posing/strangely posed characters and such, but overall everything is as expected. I don't recommend your first playthrough have this mod enabled, as various things may break unexpectedly.

## Tips from the developer

- Please try out all the major hotkeys (displayed in-game), even if you don't think you'll use them - you might find them handy at some point
- Please see the "Known bugs and wierd behaviors" section of this page if you have problems with the mod.
- In ADV mode, when you enter FPS mode via the `F8` key, the current state of the cameras is saved, and when you leave with the `F9` key, it is restored. If you enter FPS mode (`F8`) at a bad time (such as during a cinematic), when you hit `F9` you might be restored to a bad state. Therefore, it's best to enter FPS mode when in the "resting" position, although it cannot always be avoided.
- The base game (without the mod) might have a bug when your FPS is unlocked - it's probably best to enable vsync to make sure the game works correctly.

## How to Use the Mod / Controls

**Please note that this mod only works with mouse and keyboard** (... and is PC only)

Update: The keyboard shortcuts are now shown in-game. To hide the shortcuts cheat-sheet, press F11.

### Movement Controls

| Key | Action |
| --- | --- |
| `F8` | Enable Noclip/FPS Mode |
| `F9` | Disable Noclip/FPS Mode |
| `P` | Move Forward (Hold `SHIFT` to move faster) |
| `;` | Move Backward (Hold `SHIFT` to move faster) |
| `L` | Strafe Left (Hold `SHIFT` to move faster) |
| `'` | Strafe Right (Hold `SHIFT` to move faster) |
| `SHIFT` | While held, you move faster ("Sprint" key) |
| Mouse Rotation | Rotate the camera (while in FPS mode) |

### Screenshot Controls

These shortcuts are useful when taking screenshots to hide the GUI or slow down the action.

| Key | Action |
| --- | --- |
| `F11` | Hide shortcuts menu (for taking screenshots) |
| `F10` | Hide Game GUI (for taking screenshots) |
| `O`   | Toggle Pause/Resume of the game. The camera can be freely moved while the game is paused. |
| `K` or `F7` | Toggle (Camera Zoom Override and Near Clip mode) |
| `Mouse Scrollwheel` | Adjust camera zoom level (only works while camera zoom is enabled by pressing the `K` key) |
| `F3`  | 10x Slow Motion (doesn't work in all scenes) |

### Optional / Rarely Used Controls

| Key | Action |
| --- | --- |
| `F6` | Enable Noclip/FPS Mode with Magenta Box under player |
| `[` | Move vertically upwards (lift player in somnium with Magenta Box) |
| `]` | Move vertically downwards |

### Common input problems

- Menus might not operate properly, or be invisible while in FPS mode. If this happens **Hit `F9` to revert to normal mode**.
- In Somniums, don't hold right click to rotate - use only the mouse. If you right click, you'll rotate in two different ways at once.

### Camera Zoom

- In some scenes, cameras may zoom in unexpectedly due to the cinematic having a zoom in effect. Activate Camera Zoom Override mode by pressing the `K` key, then adjust the zoom to your liking using the `scrollwheel`.

### Pause/Unpause Feature

- The game can be entirely paused/unpaused, which is useful for taking a screenshot of a particular moment in time:
  - Press `O` to pause the game. Press `O` again to unpause the game.
  - While the game is paused, you can still move the camera using FPS mode, adjust camera zoom etc.

### Clip Distance

- In some scenes, the camera will clip into objects very easily. By pressing `F7` you can toggle to a shorter clip distance. Please note that doing this may cause graphical artifacts with shadows. In addition, ADV mode deliberately uses clipping to avoid seeing the characters own head - with this mode enabled and the camera in the default ADV position, you'll see the inside of the character's head.

### F6 mode / magenta box

- You can press `F6` instead of `F8` to enter FPS mode. If you do this, a magenta box will spawn directly underneath the player (it's the standard Unity cube).
- In somniums, the box can be used to lift and push the player around.
- If the magenta box gets in the way of the camera, hold right click while moving the mouse and you can rotate the camera around the magenta box.

#### Known bugs and wierd behaviors

As this is a just a hack of the game and a side project for me, some "bugs" might not be worth fixing. I'll state the known bugs below, but might not ever fix them:

- Cameras don't revert to their original position when you press F9 while in a Cinematic
  - Workaround: None
- The camera may starts rotating randomly in somniums
  - Workaround: Disable the FPS camera, move a bit, then enable it again. You can also try entering and leaving the main menu.
- When a new version of the game is released, probably everything will break until a new version of the mod is issued.
  - Workaround: wait for a new release of the mod
- A few scenes look like they're in-game, but they are actually videos. Since it's a video, you cannot move the camera.
  - Workaround: None
- Menus Disappear in FPS mode
  - Press `F9` to disable FPS mode when attempting to navigate menus
  - Press `F10` to toggle GUI hiding, incase you forgot you left it on
- In somniunms, the "choice" buttons and some other GUI are not hidden while making the choice. Immediately after making the choice, they go away, so this is not really a problem.
  - Workaround: None
- The slow-motion modes don't work in some cutscenes
  - Workaround: Repeatedly hit the pause/play button `O` to get to the part you want

## Reporting Bugs

Please report bugs by raising an issue on this github repository. If you don't want to create a Github account, you can PM me @drojf0 on twitter.

## Developer's Instructions / Reproduction Instructions

See the [Developer Readme](Developer_Readme.md) for reproduction instructions and various other useful information. You may be able to port the code to other Unity games.
