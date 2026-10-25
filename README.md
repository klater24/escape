# Escape — Sprint 3

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

## Sprint 3 - Combat (Samiya)

- Z/N attacks enemies; regular enemies take three hits and the boss takes six. Link is protected from contact during sword attacks.
- 1 fires an arrow (one target), 2 places a bomb (damage only during explosion), and 3 throws a boomerang (one hit per enemy per throw). Each hit deals one damage. Player weapons do not hurt Link.
- F2 includes the demon and Enemy C; F3 includes the boss and a demon. The boss fires toward Link every two seconds and stops when defeated.
- Link has 3 health with a 5-second damage cooldown. Boss shots hurt Link, including during attacks. R restores health and enemies; changing rooms clears shots.
- Test all weapons, flashing, defeat, cooldown, reset, and room switching. Enemies turn at solid walls and cannot leave through doorways; Link can still use doors. Projectile-wall collision is not implemented.
- Z/N attacks Enemy A with the sword. It flashes when hit and is defeated after three hits.
- Link has 3 health and loses one on enemy contact, with damage feedback and a 5-second cooldown. Health appears in the window title.
- At zero health, Link cannot move, attack, or use items. R restores player/enemy health and clears combat timers.
- Test attacks in all four directions, repeated contact, defeat, and reset. Combat applies only to Enemy A.

## Level Files and Room Configuration

### Overview

Room layouts are defined in JSON files stored in `Content/Levels`. This allows developers to add or modify room layouts without changing the C# code responsible for constructing rooms.

Each room file defines the room's name, default player spawn position, tile map, and door connections. The `RoomParser` reads the JSON file, and the level-loading system uses the resulting data to construct and manage the room.

### JSON File Format

Each room JSON file contains the following properties:

| Property | Description |
|---|---|
| `Name` | The room's identifier. |
| `SpawnX` | The default horizontal player spawn coordinate. |
| `SpawnY` | The default vertical player spawn coordinate. |
| `TileMap` | An array of strings representing rows of tiles in the room. |
| `Doors` | An array describing the room's doorways and their destinations. |

Each entry in `Doors` contains:

| Property | Description |
|---|---|
| `Direction` | The direction of the doorway in the current room: `Top`, `Bottom`, `Left`, or `Right`. |
| `DestinationRoom` | The identifier of the room the door leads to. |
| `DestinationDoor` | The direction of the corresponding doorway in the destination room. |
| `SpawnX` | The horizontal coordinate where the player appears when entering this room through this door. |
| `SpawnY` | The vertical coordinate where the player appears when entering this room through this door. |

The top-level spawn coordinates are used for the room's default spawn. Door-specific spawn coordinates determine where the player appears when entering through a particular doorway.

### Tile Map Symbols

The tile map uses symbols to determine which objects are created at each tile position.

| Symbol | Object created |
|---|---|
| `0` | Empty tile; no object is created at that position. |
| `B` | Brick block. |
| `E` | Run enemy. |
| `F` | Flying Demon enemy. |
| `M` | Mana Seed enemy. |
| `X` | Boss enemy. |
| `K` | Key item. |
| `D` | Book item. |
| `W` | Watch item. |
| `H` | Heart item. |
| `P` | Potion item. |

Each tile symbol represents a position in the room's tile grid. The room constructor translates the symbols into the corresponding game objects.

### Door Connections and Room Transitions

Doors connect rooms using room identifiers and doorway directions. For example, a door in `Room1` may lead to `Room3` through its `Left` doorway.

When the player touches a door, the level manager loads the destination room and uses the destination doorway's spawn coordinates to position the player. Door connections should reference valid room files and matching doorway directions so that transitions work correctly.

### Adding or Modifying Rooms

To add a room:

1. Create a new JSON file in `Content/Levels`.
2. Set the room's name and default spawn coordinates.
3. Define the tile map using the supported symbols.
4. Add door definitions for the room's connections.
5. Set each destination room, destination doorway, and door-specific spawn position.
6. Verify that the referenced destination rooms and doorway directions exist.
7. Run the game and test the room's transitions and spawn positions.

When modifying a room, ensure that tile symbols are supported by the room constructor and that door spawn positions place the player safely inside the destination room.

### Level Loading and Management

The game uses three main components to load and manage rooms:

- **`RoomParser`** reads a room's JSON file and converts it into a `RoomData` object.
- **`Room`** uses the parsed data to create the room's floor, blocks, enemies, items, exterior walls, and door collision objects.
- **`LevelManager`** loads rooms, tracks the active room, positions the player, and handles transitions between rooms.

When a room is loaded, the level manager constructs the room and initializes the player's spawn position and collision obstacles. When the player touches a door, the manager identifies the destination room and doorway, loads the destination room, and positions the player at the corresponding spawn coordinates.

The room-loading system separates level data from game logic. As a result, room layouts and connections can be modified through JSON files without requiring changes to the code that constructs and loads rooms.

