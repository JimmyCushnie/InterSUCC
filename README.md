# InterSUCC
This library is an extension for [SUCC](https://github.com/JimmyCushnie/SUCC) which adds generic `DataFile` types. These new types let you save and load data without using any magic strings.

**Before:**

```csharp
const string KEY_RESOLUTION_X = "resolution x";
const string KEY_RESOLUTION_Y = "resolution y";
const string KEY_FULLSCREEN = "fullscreen";

DataFile GraphicsSettingsFile = new DataFile("graphics settings");

void SaveGraphicsSettings()
{
    GraphicsSettingsFile.Set(KEY_RESOLUTION_X, Graphics.ResolutionX);
    GraphicsSettingsFile.Set(KEY_RESOLUTION_Y, Graphics.ResolutionY);
    GraphicsSettingsFile.Set(KEY_FULLSCREEN, Graphics.Fullscreen);
}

void LoadGraphicsSettings()
{
    Graphics.ResolutionX = GraphicsSettingsFile.Get<int>(KEY_RESOLUTION_X);
    Graphics.ResolutionY = GraphicsSettingsFile.Get<int>(KEY_RESOLUTION_Y);
    Graphics.Fullscreen = GraphicsSettingsFile.Get<bool>(KEY_FULLSCREEN);
}
```

**After:**

```csharp
public interface IGraphicsSettings
{
    int ResolutionX { get; set; }
    int ResolutionY { get; set; }
    bool Fullscreen { get; set; }
}

DataFile<IGraphicsSettings> GraphicsSettingsFile = new DataFile<IGraphicsSettings>("graphics settings");
IGraphicsSettings GraphicsData => GraphicsSettingsFile.Data;

void SaveGraphicsSettings()
{
    GraphicsData.ResolutionX = Graphics.ResolutionX;
    GraphicsData.ResolutionY = Graphics.ResolutionY;
    GraphicsData.Fullscreen = Graphics.Fullscreen;
}

void LoadGraphicsSettings()
{
    Graphics.ResolutionX = GraphicsData.ResolutionX;
    Graphics.ResolutionY = GraphicsData.ResolutionY;
    Graphics.Fullscreen = GraphicsData.Fullscreen;
}
```

## Installing

InterSUCC requires [SUCC](https://github.com/JimmyCushnie/SUCC) and [ClassImpl](https://github.com/pipe01/classimpl).

InterSUCC can be installed via the Unity Package Manager. To do so, add the following to the `dependencies` array in your `manifest.json`:

```json
"com.jimmycushnie.intersucc": "https://github.com/JimmyCushnie/InterSUCC.git#unity"
```

## Limitations

Due to the nature of InterSUCC's implementation, there's no way to set a default value as you're loading a value. Therefore, it is recommended that you set default values for your files using the [Default Files](https://github.com/JimmyCushnie/SUCC/wiki/Additional-DataFile-Functionality#default-files) feature.
