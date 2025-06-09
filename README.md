# Tuick UI Framework

A Tiny component-oriented UI framework for Unity UI Toolkit. 
This framework allows you to create reusable UI components with a clean separation of markup (UXML), styles (USS), and logic (C#).

## Installation

### Using Unity Package Manager (UPM)

1. Open your Unity project
2. Go to Window > Package Manager
3. Click the "+" button in the top-left corner
4. Select "Add package from git URL..."
5. Enter the following URL:
   ```
   https://github.com/mutsuyuki/Tuick.git
   ```
6. Click "Add"

### Manual Installation

1. Clone or download this repository
2. Copy the Assets/Tuick folder into your Unity project's Assets folder

## Features

- Tiny component-oriented UI development for Unity UI Toolkit
- Clean separation of markup (UXML), styles (USS), and logic (C#)
- Template-based component creation
- Automatic UXML and USS processing
- Easy to use API
- Sample components included

## Usage

### Exploring Samples

The package includes a sample scene and example components directly within its folder structure to demonstrate how to use the framework. You can find these in:

`Packages/Tuick UI Framework/Samples`

To use or experiment with these samples:
1.  Navigate to the `Packages/Tuick UI Framework/Samples` folder in your Project window.
2.  You can open the `SampleScene.unity` directly from here to see it in action.
3.  If you wish to modify the samples, it's recommended to copy them into your project's `Assets` folder (e.g., `Assets/Tuick Samples`) to avoid losing your changes when updating the package.

### Creating a New Component

1. Right-click in the Project window
2. Select "Create > Tuick > Component"
3. Enter a name for your component
4. Choose whether to create a folder for the component
5. Click "Create"

This will create three files:
- YourComponent.cs - The C# script for your component
- YourComponent.uxml - The UXML template for your component
- YourComponent.uss - The USS stylesheet for your component

### Using Components

```csharp
using UnityEngine;
using UnityEngine.UIElements;
using Tuick;

public class ExampleUsage : MonoBehaviour
{
    private void OnEnable()
    {
        // Get the UIDocument component
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        // Create and add your component
        var myComponent = new YourComponent();
        root.Add(myComponent);
    }
}
```

## Requirements

- Unity 2022.3 or later
- UI Toolkit package (com.unity.modules.uielements)

This dependency will be automatically installed when you add the package through the Package Manager.

## Notes

- This package generates intermediate build files within the `Assets/Tuick/Build/` directory in your project's Assets folder. A .gitignore file is automatically created inside Assets/Tuick/Build/ to exclude its contents from version control
- Make sure to refresh the UXML/USS lists after creating new components
- The framework will automatically create all necessary directories in the Assets folder when needed
- The package includes a build preprocessor that automatically regenerates all necessary files before building your project, so you don't need to worry about missing files in CI/CD environments
- This package is designed to work with Unity's UI Toolkit, so make sure you have UI Toolkit enabled in your project

## License

This package is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
