# SomniumDebugEnabler
A quick and easy tool for modifying debug settings in AI: The Somnium Files

## Install
Head over to the [relases page](https://github.com/slavanomics/SomniumDebugEnabler/releases) and download the latest one, or compile it yourself.

Download and install [MelonLoader](https://melonwiki.xyz/#/?id=requirements) into your game directory

Move `SomniumDebugEnabler.dll` into your `AI The Somnium Files\Mods` folder

Launch!

## Configuration

By default, the mod will launch into the debug menu.

If you'd like to configure the mod more heavily, head to your `AI The Somnium Files\UserData\` folder and open `MelonPreferences.cfg`

By default your file should look like this:
```TOML
[Overrides]
UNITY_EDITOR = 0
UNITY_STANDALONE_WIN = 1
UNITY_PS4 = 0
UNITY_XBOXONE = 0
UNITY_SWITCH = 0
BUILD_RELEASE = 0
BUILD_REGION = "BUILD_WORLDWIDE"
USER_NAME = "gonzo"
FORCE_ASSET_BUNDLE = true
[Overrides.CUSTOM_OVERRIDES]
custDict = { DEVELOP_MODE = "1" }
```

Feel free to experiment with these values, currently the only known values to trigger the debug mode are `BUILD_RELEASE` being set to 0, and `USER_NAME` being set to `"gonzo"`

custDict can be edited to include whatever other variables you want to forcibly inject into the script manager.

## Freecam Instructions and Tips

The freecam mode included in this tool adds a "Noclip Mode" FPS style camera allowing you to explore ADV mode scenes and Somniums, plus some other extra features. **It is highly recommended you look at the [Freecam Instructions and Tips](docs/freecam-readme.md) page before using the freecam for tips and known issues.**

If you're a developer working on the freecam code, check out the [Freecam Developer Readme](docs/freecam-developer-readme.md) first.

## Credits

HUGE THANK YOU TO [@robotortoise](https://github.com/robotortoise) for finding [this in the first place](https://www.reddit.com/r/aithesomniumfiles/comments/sltzx1/i_have_discovered_a_fully_functional_debug_mode/)

Thank you to [@drojf](https://github.com/drojf) for letting me use [his freecam code in this.](https://github.com/drojf/ai_somnium_fps_camera)

another thank you to [@GarnetSunset](https://github.com/GarnetSunset) for coming up with the idea in the first place.
