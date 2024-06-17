# GameMusic Manager

This Unity script manages the background music for a game, including tension, danger, and death states. It provides smooth transitions between different types of music based on game events.

## Features

- Singleton pattern to ensure only one instance of `GameMusic` exists.
- Controls multiple audio sources for different types of game music.
- Smooth volume transitions for tension, danger, and death music.
- Adjustable maximum volume based on player preferences.

## Installation

1. Copy the `GameMusic` script into your Unity project.
2. Attach the `GameMusic` script to a GameObject in your scene.
3. Assign the required audio sources and audio clips in the Inspector.

## Usage

### Initialization

The script automatically initializes itself and sets up the audio sources. Ensure the audio sources and clips are correctly assigned in the Inspector.

### Methods

- **SenseTension()**: Starts the tension music.
- **ZeroTension()**: Stops the tension music.
- **Ingozi()**: Starts the danger music.
- **Death()**: Plays the death music.
- **ForceChill()**: Forces the music to calm down immediately.

### Example

```csharp
// Start tension music
GameMusic.instance.SenseTension();

// Stop tension music
GameMusic.instance.ZeroTension();

// Start danger music
GameMusic.instance.Ingozi();

// Play death music
GameMusic.instance.Death();

// Force the music to calm down
GameMusic.instance.ForceChill();
```

### Customization
You can customize the music transition speeds and maximum volume in the Inspector or by modifying the script.

```csharp
// Example of setting the maximum volume in the script
GameMusic.instance.SetMaxVolume(0.8f);
```

### License
This project is licensed under the MIT License. See the LICENSE file for details.
