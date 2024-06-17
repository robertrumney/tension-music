# Game Music Manager

A Unity script to manage background music transitions and states (normal, tension, danger, death) dynamically.

## Description

This script handles background music transitions in a Unity game. It supports different states such as normal gameplay, tension, danger, and death, allowing for smooth transitions between these states.

## Features

- Singleton pattern for easy access
- Supports tension and calm music states
- Handles transitions to danger and death music
- Adjustable maximum volume based on player preferences
- Coroutine-based volume fading for smooth transitions

## How to Use

1. Attach the `GameMusic` script to a GameObject in your Unity scene.
2. Assign the `Music1`, `Music2`, and `DeathMusic` AudioSource and AudioClip references in the inspector.
3. Use the public methods `SenseTension()`, `ZeroTension()`, `Ingozi()`, and `Death()` to trigger the respective music states.

## Public Methods

- `SenseTension()`: Start playing tension music.
- `ZeroTension()`: Stop playing tension music and return to normal.
- `Ingozi()`: Trigger the danger music state.
- `Death()`: Trigger the death music state.

## Example

```csharp
// Example usage
GameMusic.instance.SenseTension();
GameMusic.instance.Ingozi();
GameMusic.instance.Death();
GameMusic.instance.ZeroTension();
```
