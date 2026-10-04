# Shared Sprite, Block, and Reset Design

## Sprite System

Gameplay objects keep their behavior separate from drawing. Anything drawn by the game implements `ISprite`, which provides a screen position, per-frame update, draw, and reset operation. `ISprite` also implements `IGameResettable` so a sprite can be registered with the game-wide reset flow.

`StaticSprite` draws one rectangle from a texture. `AnimatedSprite` cycles through an ordered list of rectangles using a frame duration. `SpriteFactory` creates these sprite types and maps the ten named block types to cells in `PixelPack_Block_Atlas.png`.

Texture files belong under `Content/Textures`, grouped into `Blocks`, `Player`, `Enemies`, `Items`, or `Projectiles`. The project copies PNG files from that tree to the build output. Load a texture with a path relative to the output root:

```csharp
var texture = TextureLoader.Load(
    GraphicsDevice,
    "Content/Textures/Player/player.png");
```

Sprite sheets use explicit source rectangles. Keep cell dimensions consistent within a sheet and pass animation frames to `CreateAnimatedSprite` in playback order. The block atlas uses 32 by 32 pixel cells arranged left to right as grass, dirt, stone, brick, water, wood, sand, ice, metal, and platform.

## Blocks

`IBlock` extends `ISprite` with a block name, stationary flag, and active flag. Each `Block` represents one fixed tile type, position, and visual sprite. Blocks do not move or interact with other objects.

`BlockManager` owns the list of available blocks. It updates and draws only the selected active block, selects the previous or next block, and resets every block and the selected index. `Game1` creates the demo blocks through `BlockManager` rather than storing them as unrelated sprites. A newly added block category should continue to use the shared block factory and should not add movement or collision behavior unless the assignment requires it.

The starter demo binds `T` to select the previous block and `Y` to select the next block. Only one block is shown at a time. Selection wraps around at the beginning and end of the list. Key presses are edge-triggered, so holding a key does not repeat the selection every frame.

## Reset Integration

`IGameResettable` is the shared reset contract. A subsystem owner implements `Reset()` on the subsystem or its manager and restores that subsystem's own objects. `GameResetCoordinator` calls `Reset()` on each registered object once. `Game1` registers `BlockManager` during setup and exposes `RegisterResettable` for other teammates' systems.

When a system is ready, register its manager or top-level object from `Game1` setup:

```csharp
RegisterResettable(player);
RegisterResettable(enemyManager);
RegisterResettable(itemManager);
RegisterResettable(projectileManager);
```

`Player` can implement `ISprite` and inherit the reset contract. Managers can implement `IGameResettable` directly. Register either a manager or its contained objects, not both, to avoid resetting the same state twice. The full game reset is complete only after all participating systems have been registered.

## Manual Checks

Build and launch the game from the repository root:

```powershell
dotnet build
dotnet run
```

In the game window, check that only one block appears at a time, `T` selects the previous block, and `Y` selects the next block. Confirm selection wraps at both ends. Call `Game1.Reset()` after changing the selection to confirm it returns to the first block. Check reset integration again as each teammate registers their system.

##Player 
Player class manages the states of the players through Direction and State Enums. Based on the enum value an object of the Player class is set to, the player will be displayed either an idle, walking, attacking, or damaged state. The direction enum dictates what direction the player is facing. The class also contains logic that allows the player to move in the direction it is set to. Player class uses the Enum structures to display the player on a specific position on the game window. The implementation of the Player class allows for transition between different states and movement in four cardinal directions. 
