# Sandland.SceneTools

Enhance your Unity scene management workflow with this powerful set of editor tools.

![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)

## Table of Contents

- [Features](#features)
- [Quick Start Guide](#quick-start-guide)
    - [Installation](#installation)
        - [Git URL](#git-url)
        - [Unity Package](#unity-package)
    - [Usage](#usage)
        - [Scene Selector Tool](#scene-selector-tool)
        - [Scene Class Generation Tool](#scene-class-generation-tool)
- [Example of Generated Class Usage](#example-of-generated-class-usage)
- [Theme Support](#theme-support)
- [License](#license)

## Features

1. [Scene Selector Tool](#scene-selector-tool) - quickly navigate between scenes in your project.
2. [Scene Class Generation Tool](#scene-class-generation-tool) - automatically generate a C# class containing your scene names and indexes.

## Quick Start Guide

### Installation

#### Git URL

1. Open Unity and navigate to `Window > Package Manager`.
2. Click the `+` button and select `Add package from git URL...`.
3. Enter the following URL: `https://github.com/migus88/Sandland.SceneTools.git?path=Assets/SceneTools`.

To use a specific version, append a hashtag followed by the version number in a `v0.0.0` format. Example: `https://github.com/migus88/Sandland.SceneTools.git?path=Assets/SceneTools#v0.5.0`.

#### Unity Package

1. Visit the [releases page](https://github.com/migus88/Sandland.SceneTools/releases) and download the latest `SceneTools.unitypackage`.
2. Open Unity and navigate to `Assets > Import Package > Custom Package...`.
3. Select the downloaded `SceneTools.unitypackage` and click `Import`.

### Usage

#### Scene Selector Tool

1. Press `Ctrl+G` on Windows or `Cmd+G` on Mac to open the Scene Selector Tool, or navigate to `Tools > Sandland Games > Scene Selector`.
2. Click on a scene icon to reveal its asset in the Project window.
3. Click on a scene name to load it in the editor.
4. Use the search bar to filter scenes by name.
5. Toggle the star icon next to a scene to add or remove it from your favorites. Favorited scenes will appear at the top of the list.

#### Scene Class Generation Tool

1. Navigate to `Tools > Sandland Games > Setup Scene Tools` to enable and configure the tool.
2. Select the namespace, class name, and location for the generated class.
3. The class will be re-generated automatically whenever the "Scenes In Build" list changes.
    - If Addressables support is enabled in the "Setup Scene Tools" window, the class will also be regenerated every time there is a scene-related change in the Addressables.

To enable or disable Addressables support, open the "Setup Scene Tools" window, and toggle the corresponding option.

## Example of Generated Class Usage

Here's an example of how to use the generated `SceneList` class:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;
using Sandland;

public class SceneLoader : MonoBehaviour
{
    public void LoadSampleScene()
    {
        SceneManager.LoadScene(SceneList.Names.SampleScene);
    }

    public void LoadYetAnotherScene()
    {
        SceneManager.LoadScene(SceneList.Indexes.YetAnotherScene);
    }
}
```
## Theme Support

Sandland.SceneTools now supports custom themes. You can choose from built-in themes or create your own. To select a theme, open the "Setup Scene Tools" window.

To create your own theme, follow these steps:

1. Create a USS file anywhere in your project (preferably inside one of the Editor folders, because you don't need it in the build). Name this file as you wish.
2. Add a label to the USS file by selecting the file in the Project window, and in the Inspector window, click the "Labels" icon (a tag icon). Type `Sandland-theme` into the input field and press Enter to add the label.
3. Edit the content of the USS file to look like this:

```css
:root {
    --title-color: #B85C38;
    --background-color: #6b5246;
    --border-color: black;
    --primary-text-color: #E0C097;
    --secondary-text-color: #ffffff;
    --button-color: #5C3D2E;
    --accent-color: #e0723f;
    --focus-color: #c48664;
}
```

4. Replace the color values with your preferred colors.
5. Restart the "Setup Scene Tools" window, and you should see your color palette as an option in the themes list.

## License
This project is licensed under the MIT License. See the [license file](https://github.com/migus88/Sandland.SceneTools/blob/main/LICENSE) for details.