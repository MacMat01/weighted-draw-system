# CDS — Choice Driven System

> A reusable Unity plugin for event management, built for Unity 6.

**Package:** `com.macmat01.choice-driven-system`  
**Version:** `1.0.0`  
**Author:** I tre Moschettieri  
**Minimum Unity Version:** 6000.4

---

## Overview

The **Choice Driven System (CDS)** is a lightweight, reusable Unity plugin designed to streamline event management within your project. It provides a structured way to define, trigger, and respond to in-game choices and events, keeping your codebase decoupled and maintainable.

---

## Requirements

- Unity **6000.4** or later
- No additional dependencies required

---

## Installation

### Via Unity Package Manager (Recommended)

1. Open your project in Unity.
2. Go to **Window → Package Manager**.
3. Click the **+** button in the top-left corner and select **Add package from git URL…** (or **Add package from disk…** if installing locally).
4. Enter the package URL or path and confirm.

Unity will resolve and install the package automatically.

### Manual Installation

1. Clone or download this repository.
2. In the Unity Package Manager, click **+ → Add package from disk…**.
3. Navigate to the folder containing `package.json` and select it.

---

## Package Structure

```
com.macmat01.choice-driven-system/
├── package.json
├── README.md
├── CHANGELOG.md
├── LICENSE.md
├── Runtime/                  # Core runtime scripts (included in builds)
│   ├── CDS.Runtime.asmdef
│   └── ...
├── Editor/                   # Editor-only utilities (stripped from builds)
│   ├── CDS.Editor.asmdef
│   └── ...
├── Tests/
│   ├── Runtime/
│   └── Editor/
└── Samples~/                 # Optional samples (import via Package Manager)
    └── BasicExample/
```

> **Note:** The `Samples~/` folder uses Unity's tilde convention — its contents are not imported automatically. See the [Samples](#samples) section below.

---

## Getting Started

Once the package is installed, you can reference CDS types in your scripts by adding the `CDS.Runtime` assembly reference to your project's `.asmdef` file, or by using it directly if you are not using Assembly Definitions.

```csharp
// Example: subscribing to a CDS event
using CDS.Runtime;

public class MyListener : MonoBehaviour
{
    void OnEnable()
    {
        // Subscribe to an event channel
        EventChannel.Subscribe("OnChoiceMade", HandleChoice);
    }

    void OnDisable()
    {
        EventChannel.Unsubscribe("OnChoiceMade", HandleChoice);
    }

    void HandleChoice(object data)
    {
        Debug.Log("Choice received: " + data);
    }
}
```

> ⚠️ The exact API surface depends on your runtime implementation. Replace the snippet above with your actual classes and method signatures.

---

## Samples

CDS ships with an optional sample scene demonstrating basic usage.

**To import a sample:**

1. Open **Window → Package Manager**.
2. Select **CDS — Choice Driven System** from the list.
3. Open the **Samples** tab.
4. Click **Import** next to the sample you want.

| Sample | Description |
|---|---|
| Basic CDS Example | Simple usage example demonstrating core event management features |

Imported samples are placed under `Assets/Samples/CDS - Choice Driven System/<version>/`.

---

## Contributing

This package is maintained by **I tre Moschettieri**. If you encounter a bug or have a feature request, please open an issue in the project repository.

---

## License

See [LICENSE.md](LICENSE.md) for full terms.
