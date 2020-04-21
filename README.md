# InterSUCC
This library is an extension for [SUCC](https://github.com/JimmyCushnie/SUCC) which adds generic `DataFile` types. These new types let you save and load data without using any magic strings.

**Before:**

```csharp
DataFile File = new DataFile("game.succ");

void SaveHighscore(int score)
    => File.Set("highscore", score);

int LoadHighscore()
    => File.Get<int>("highscore");
```

**After:**

```csharp
public interface IFileData
{
    int Highscore { get; set; }
}

DataFile<IFileData> File = new DataFile<IFileData>("game.succ");

void SaveHighscore(int score)
    => File.Data.Highscore = score;

int LoadHighscore()
    => File.Data.Highscore;
```

## Installing

InterSUCC requires [SUCC](https://github.com/JimmyCushnie/SUCC) and [ClassImpl](https://github.com/pipe01/classimpl).

InterSUCC can be installed via the Unity Package Manager. To do so, add the following to the `dependencies` array in your `manifest.json`:

```json
"com.jimmycushnie.intersucc": "https://github.com/JimmyCushnie/InterSUCC.git#unity"
```

## Limitations

Due to the nature of InterSUCC's implementation, there's no way to set a default value as you're loading a value. Therefore, it is recommended that you set default values for your files using the [Default Files](https://github.com/JimmyCushnie/SUCC/wiki/Additional-DataFile-Functionality#default-files) feature.

## ConfigWithOverride

Todo document this feature
