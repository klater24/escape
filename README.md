# Escape — Sprint 2

CSE 3902 team game prototype. This sprint demonstrates a controllable player, three enemies and a boss, five pickups, four projectile types, ten stationary block types, and coordinated reset.

See [DESIGN.md](DESIGN.md) for architecture, team responsibilities, reset behavior, and the manual verification checklist. The team requirements link is in [REQUIREMENTS.md](REQUIREMENTS.md).

## Build and run

Use the .NET 9 SDK. The current content project targets Windows; other platforms have not been verified. Initial setup requires network access to restore NuGet packages and MonoGame tools.

Extract the release ZIP or clone the repository. Open a terminal in the folder containing `escape.csproj` and run:

```powershell
dotnet tool restore
dotnet build
dotnet run
```

The build compiles the assets listed in `Content/Content.mgcb` into XNB files. Keep the source assets, project file, and tool manifest together. Generated `bin` and `obj` folders are not required in the submission.

Click the game window to give it keyboard focus. Close the running game before rebuilding if Windows reports that `escape.exe` is locked.

## Controls

| Key | Action |
| --- | --- |
| W / Up arrow | Move up |
| A / Left arrow | Move left |
| S / Down arrow | Move down |
| D / Right arrow | Move right |
| Z / N | Attack |
| E | Trigger temporary player damage feedback |
| 1 | Fire an arrow from the player's position and facing direction |
| 2 | Place a bomb at the player's position |
| 3 | Throw a boomerang in the player's facing direction |
| O / P | Previous / next enemy |
| U / I | Previous / next pickup |
| T / Y | Previous / next block |
| R | Reset the scene and clear active projectiles |
| Q | Quit |

Movement keys repeat while held. Other actions occur once per press. Use the number row for 1–3; keys 4–5 have no assigned action. The window title also lists controls, but it may be clipped by the window width.

## What to demonstrate

- **Player:** starts at (100, 100), moves, faces left/right/up/down, and displays idle, walking, attack, and damaged states. Attack lasts 0.48 seconds; damage normally lasts 0.96 seconds.
- **Enemies:** O/P cycles among Knight (random cardinal movement), Flying Demon (eight-direction movement with varying speed), Forester (alternating walking and pausing), and Sorcerer (horizontal patrol). Only the selected enemy updates and draws. Selecting another pauses the previous enemy.
- **Boss:** the selected Sorcerer requests a rightward projectile every two seconds. Existing shots continue after switching enemies until removed or reset.
- **Pickups:** Book, Key, Watch, Heart, and Potion cycle at the middle-right of the screen. They bob six pixels around their starting positions on a two-second cycle. Heart and Potion also change image frames.
- **Blocks:** grass, dirt, stone, brick, water, wood, sand, ice, metal, and platform. Only the selected block is shown; blocks remain stationary. All three selectors wrap at either end.
- **Projectiles:** the scene starts empty. Create shots with 1–3; boss shots are created by the selected boss. Arrow moves for two seconds and expires after three. Bomb has a two-second fuse and a one-second explosion display. Boomerang travels outward for three seconds and returns to its launch point over three seconds. Boss shots expire after six seconds or when they leave the right edge.
- **Reset:** R restores block/item/enemy selections and their initial state, restores the player's position/facing/state, and clears every active projectile. It does not create demo shots.

The sprint does not require pickup collection, inventory effects, projectile collisions, or enemies reacting to the player/environment. These are not implemented.

## Known limitations

- Player reset does not yet rewind all sprite frames, sprite effects, and attack/damage timers.
- Repeated or interrupted attacks/damage do not explicitly restart their animation frames. Attack can interrupt damage, and item use is not blocked during either state.
- Holding equivalent movement keys together (such as W and Up) doubles movement commands; simultaneous directions can produce diagonal movement. Player movement is per command rather than scaled by elapsed time.
- Arrow artwork does not rotate to match travel direction. Variable-size player attack frames may need alignment adjustments; visual verification remains pending.
- The bomb uses a static fuse image followed by a static explosion image. The boomerang returns to its launch point, not to a moving player.

## Validation and submission

A development build was verified with zero warnings and errors during the documentation review. This does not certify all visual behaviors or a clean-machine setup. Record actual gameplay results using the checklist in [DESIGN.md](DESIGN.md); do not treat planned checks as passed tests.

For the sprint submission, merge the intended work into `main`/`master`, create the GitHub release from that branch, and test the downloaded source ZIP separately before uploading it to Carmen. Each member submits their own peer review, including self-review. Coordinate the grader meeting or permitted task-board evidence separately; these tasks are not verified by this repository.
