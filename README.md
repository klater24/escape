# Escape enemy demo

Run with `dotnet run` (.NET 9). Enemy PNGs are copied into the build/publish output, so the game does not depend on a Downloads folder.

## Controls

- O: previous enemy; P: next enemy. Both wrap around the four choices.
- R: reset all enemies and block sprites, and select Enemy A.
- Q: quit.

Each press triggers one command; holding a key does not repeatedly cycle. Shift/Caps Lock do not affect the bindings. The window title identifies the selected enemy. Only the selected enemy updates and draws; other enemies resume where they stopped when selected again.

## Samiya's enemy behavior

- Enemy A: knight run animation, cardinal movement at 100 pixels/second, random direction every 2 seconds.
- Enemy B: flying demon animation, eight possible directions, speed chosen from 60–140 pixels/second every second. Diagonals are normalized.
- Enemy C: knight shield animation, moves for 1 second at 100 pixels/second, rests for 0.75 seconds, then chooses a cardinal direction.
- Boss: sorcerer casting animation (10 frames, 0.15 seconds per frame), horizontal patrol 150 pixels either side of its start at 75 pixels/second. The attack is visual only.

Projectiles are intentionally omitted at Samiya's request. Enemy A and Enemy C still share the knight design; one more distinct standard enemy design is needed to fully satisfy the assignment. Enemy B is a flying demon, and the boss is a sorcerer. Blocks remain placeholders. Player, items, and their controls are not implemented in this checkout.

## Design

`IEnemy` separates behavior from rendering through `ISprite`. Enemy classes own movement and timers; `EnemySpriteFactory` loads transparent PNG strips and selects frame rectangles through the shared `SpriteFactory`. Run uses 8 frames of 96x64, Roll 15 of 180x64, Shield 7 of 96x64, and Attack 22 of 144x64. The demon uses 4 frames of 81x71; the sorcerer uses 10 frames of 200x200. The sorcerer's flat background color is made transparent at load time; its casting effects are part of the animation, not projectiles. Textures are disposed when content unloads.

`EnemyManager` handles selection, wrapping, and reset of every enemy, including hidden enemies. Reset restores positions, movement directions, speed/state timers, and animation frames; future random choices can differ. `KeyboardController` detects new presses and executes `ICommand` objects. Wyatt can move these bindings into the team's controller when integrating. `Game1.Reset()` connects the managers/sprites currently present; future player/item managers must be registered there too.

## Manual verification

1. Start the game: only Enemy A appears alongside the existing blocks; its running frames advance.
2. Press P three times: observe flying demon Enemy B, shielding Enemy C, then the casting/patrolling sorcerer Boss.
3. Press P again to wrap to A, then O to wrap to Boss. Hold either key: selection changes once.
4. Leave each standard enemy active for several seconds to observe its direction/speed/rest behavior. Check the full sprite remains on screen.
5. Move several enemies, then press R: selection returns to A. Cycle through the others to verify their starting positions and reset animations/timers.
6. Check Shift+O/P and Caps Lock; press Q to exit.

Assets: four unmodified `noBKG_` PNGs supplied from the user's Knight folder, the sorcerer attack sheet, and the flying demon FLYING sheet. No license or creator metadata was included in that folder; retain the original asset attribution/license when available.
