# Escape — Sprint 2 Design

This document describes the current implementation, its relationship to the team's sprint responsibilities, and remaining limitations. It does not claim that every planned behavior has been tested.

## Scope and ownership

| Owner | Responsibility | Main implementation |
| --- | --- | --- |
| Person 1 — Nesrin | Player movement, facing, states, animation, reset | `player/Player.cs` |
| Person 2 — Wyatt | Keyboard commands and player combat/item-use integration | `Inputs/`, player state calls |
| Person 3 — Samiya | Three enemy behaviors, boss, O/P cycling, firing requests | `Enemies/` |
| Person 4 — Rosa | Five pickups, U/I cycling, projectile creation, animation, movement, lifetime, reset | `Items/`, `Projectiles/` |
| Person 5 — Katelynn | Shared sprites/factories, ten blocks, T/Y cycling, reset coordination and integration | `Sprites/`, `Blocks/`, `GameResetCoordinator.cs`, `Game1.cs` |

Subsystem owners implement their own state restoration. Person 5 connects those systems to the shared reset flow. Player item use and boss firing are shared integration points with person 4.

This milestone demonstrates objects independently. Collision, collection effects, inventory, and reactions to other objects are outside the supplied sprint scope. `IPlayer` was optional in the ownership plan and is not implemented.

## Overall structure

`Game1` loads assets, constructs objects, connects commands/events, and delegates update/draw work. Update advances behavior and timers; draw displays the resulting state.

```text
KeyboardController -> ICommand -> Player / manager / Game1
Game1 -> Player, BlockManager, EnemyManager, ItemManager, ProjectileManager
Gameplay object -> sprite -> SpriteBatch
R -> ResetCommand -> Game1.Reset -> GameResetCoordinator -> registered systems
```

The block, enemy, and item managers update and draw only their selected object for the demonstration. The projectile manager updates and draws every active shot. Switching selection pauses an unselected enemy/item; cycling does not reset it.

## Shared rendering and assets

`ISprite` provides Position, Update, Draw, and Reset (through `IGameResettable`). Gameplay objects generally contain sprites; not every drawable gameplay object implements `ISprite`. `IBlock` currently extends `ISprite`, while enemies and items use their own interfaces.

`StaticSprite` draws one source rectangle. `AnimatedSprite` advances through rectangles using elapsed time and a frame duration. Its loop catches up when an update spans multiple frames, and frame selection wraps. Both support scaling and sprite effects; the player uses horizontal flipping for left-facing artwork. `PointClamp` keeps pixel art sharp.

`SpriteFactory` creates shared sprites, maps block names to atlas cells, and configures enemy sheets. `ManaSeedSprite` draws synchronized body, outfit, and hair layers for the Forester and chooses standing/walking rows from movement. Player frame definitions remain inside Player; moving them to a factory would improve separation of artwork setup from behavior.

Register textures in `Content/Content.mgcb`. MonoGame compiles them to XNB assets, loaded with extensionless names:

```csharp
var texture = Content.Load<Texture2D>("Textures/Blocks/PixelPack_Block_Atlas");
```

ContentManager caches and owns these textures. Sprites and projectile factories borrow them rather than disposing them. `Game1` disposes its SpriteBatch on unload. Most artwork is under `Content/Textures`; the item/projectile atlases currently also include `Content/zeldaitems.png`, `Content/zelda.png`, and `Content/boses.png`.

Use explicit source rectangles matching the asset. Frame dimensions need not be identical, but differently sized frames need a consistent visual anchor. Keep premultiplied alpha enabled; the content configuration applies color-key transparency where required. General sprite construction still needs defensive validation of frame arrays and durations.

## Player states

Player owns position, initial position, facing, and an enum state: Idle, Walking, Attacking, or Damaged. This is a state machine implemented with conditionals, not separate State-pattern classes.

- Move changes position/facing and synchronizes the contained sprites. Movement is blocked while attacking or damaged.
- The controller calls StopMoving when movement keys are released, returning Walking to Idle.
- Attack starts a 0.48-second timer and selects a directional attack animation.
- Damage starts a 0.96-second timer and displays damage frames.
- Attack and damage normally return to Idle when their timers finish.
- Reset restores position, facing Down, and Idle, but does not yet rewind every animation or clear all timers/effects.

Known transition gaps: Attack can interrupt Damaged; Damage can interrupt Attacking; item use bypasses these state restrictions. Starting an animation does not explicitly restart its frame index. Movement uses one pixel per command, and overlapping inputs can double movement or allow diagonals. These are remaining implementation issues, not completed guarantees.

## Input and commands

`KeyboardController.Initialize()` builds held-key and pressed-key command dictionaries. `Game1` constructs the controller and invokes initialization; it does not register each binding itself. Movement commands execute while held. Other commands execute on a transition from released to pressed.

Bindings are listed in README: WASD/arrows, Z/N attack, E damage, 1 arrow, 2 bomb, 3 boomerang, O/P enemies, U/I pickups, T/Y blocks, R reset, Q quit. There are no bindings for 4–5.

`UseItemCommand` reads the player's current position and facing, then calls a spawn method on ProjectileManager. It does not require the player to construct textures or sprites. Projectiles currently spawn at the player's top-left position; there is no separate item-use state or animation.

## Blocks

The ten types are grass, dirt, stone, brick, water, wood, sand, ice, metal, and platform. `SpriteFactory.CreateBlockSprite` maps each to a 32x32 atlas region. Game1 displays blocks at scale 2.

`Block` holds type, position, active status, and a sprite. Updating Position also updates the sprite. Blocks have no autonomous movement or collision. `BlockManager` stores all ten, selects one using T/Y, and wraps selection. Reset restores every block and selects the first. This is a demonstration selector, not a complete level map.

## Enemies and boss

All four implement `IEnemy`, including update, draw, position, and reset. They receive `ISprite` instances so movement logic does not need texture coordinates.

| Enemy | Defined behavior |
| --- | --- |
| A — Knight | Random movement in four cardinal directions |
| B — Flying Demon | Eight-direction random movement with varying speed |
| C — Forester | Alternates walking and pausing, with directional layered animation |
| Boss — Sorcerer | Horizontal patrol and a firing request every two seconds while selected |

EnemyManager handles O/P wraparound and resets all enemies, including hidden ones. Boss raises `FireRequested` with a spawn position. Game1 handles the event through `ProjectileManager.SpawnBossProjectile`, currently using a rightward direction. Switching away pauses the boss timer; already spawned shots continue. Boss reset clears the firing timer. Random enemies reset their behavior state but do not reseed their random generators for an identical replay.

## Items

Book, Key, Watch, Heart, and Potion implement `IItem` and each contain a sprite. ItemManager displays the selected item and handles U/I wraparound. All five bob six pixels around their initial position on a two-second cycle using elapsed time. Heart and Potion additionally animate between image frames; the other three use single-frame sprites.

Each item's Reset restores its position, bobbing timer, and sprite animation. ItemManager resets all five and selects Book. Items do not yet interact with the player. Their shared movement code is duplicated and could be extracted later without changing this sprint's behavior.

## Projectiles

The responsibilities are split as follows:

| Component | Responsibility |
| --- | --- |
| `ProjectileSprites` | Create the visual sprite for a shot or bomb phase |
| `ProjectileFactory` | Combine independent sprites with projectile behavior |
| `ProjectileManager` | Store, spawn, update, draw, remove, and clear active shots |
| `IProjectile` | Expose Position, IsActive, Update, Draw, and Reset |

Each projectile has its own age, position, and sprite state. Textures may be shared, but sprite instances are not. Directions are validated and normalized so diagonals do not increase speed. Public Position is synchronized to the sprite.

| Type | Current lifecycle |
| --- | --- |
| Arrow | Moves at 100 pixels/second for two seconds; remains visible until three seconds, then expires |
| Bomb | Stationary two-second fuse, one-second explosion display, then finished; each phase uses a static image |
| Boomerang | Moves at 50 pixels/second for three seconds, returns for three seconds, then finishes at its launch point |
| Boss projectile | Moves at 100 pixels/second; expires at six seconds, or is removed after passing the right viewport edge |

Creation and registration are available through:

```csharp
projectileManager.SpawnArrow(position, direction);
projectileManager.SpawnBomb(position);
projectileManager.SpawnBoomerang(position, direction);
projectileManager.SpawnBossProjectile(position, direction);
```

Spawn methods use the factory and add the result to the active collection. Updates use time since creation, never total time since game startup. The manager removes inactive shots after iteration. Arrow artwork does not yet rotate with travel direction. Boomerang returns to the launch point rather than tracking a moving player. Collision and damage effects are not implemented.

## Reset integration

GameResetCoordinator registers the block manager, player, enemy manager, item manager, and projectile manager. It calls each registered system once; managers reset their own objects.

| System | Current result of R |
| --- | --- |
| Blocks | Initial positions/active state; first block selected |
| Player | Initial position, facing, and Idle; animation/timer restoration remains incomplete |
| Enemies | Initial positions, animation and behavior state; Enemy A selected; boss firing timer cleared |
| Items | Initial positions, bobbing/animation state; Book selected |
| Projectiles | All active shots removed; no demo projectiles created |

The scene starts with no projectiles. New shots require player input or the selected boss's firing event. Clearing the collection on reset handles shots that have already expired as well as currently active ones; it does not require replaying their individual Reset methods. The player reset gap means full initial-state equivalence is not yet guaranteed.

## Verification checklist

The development build was verified with zero errors/warnings during the documentation review. The following are manual acceptance checks to perform and record, not claims of completed tests. For each, record date, tester, commit, observed result, and any issue number. No gameplay passes are recorded here.

| Check | Expected result / known issue to observe | Status |
| --- | --- | --- |
| Fresh release ZIP | Extract separately, restore tools, build and run with all textures present | Pending |
| Startup | Player and selected block/enemy/item visible; no automatic projectiles | Pending |
| Player movement | WASD/arrows work; release returns to idle; facing changes | Pending |
| Combined keys | W+Up must not double speed; perpendicular keys must follow the agreed cardinal rule | Known code issue; retest after fix |
| Attack | Z/N starts and completes a directional attack; repeat and interrupt to check frame restart | Pending; restart limitation known |
| Damage | E shows feedback and normally returns to idle after 0.96 seconds; test Z/N and item use during damage | Pending; interruption limitation known |
| Blocks | Cycle all ten, wrap both directions, hold T/Y without repeated cycling; no movement | Pending |
| Enemies | Cycle all four, observe distinct movement/animation and paused hidden enemies | Pending |
| Boss | While selected, fires every two seconds; independent shots expire; reset clears timer/shots | Pending |
| Items | Cycle five pickups; bob around spawn point; reset selection and animation | Pending |
| Player shots | After waiting ten seconds, press 1–3 in each facing direction; each starts a fresh lifecycle | Pending; arrow orientation limitation known |
| Multiple shots | Shots have independent positions and timers, and expire without affecting each other | Pending |
| Bomb/boomerang | Fuse/explosion timing works; boomerang returns to launch point | Pending |
| Full reset | Test during attack, damage, explosion, and return; clear shots and restore all selections | Pending; player reset limitation known |
| Quit | Q exits the game | Pending |

## Team workflow and submission

Use feature branches, small PRs, and review before merging into main. Build and exercise affected controls after integration. Subsystem tests and documented manual checks remain each owner's responsibility.

One teammate creates the final GitHub release from main/master, verifies its source ZIP, and uploads it to the sprint assignment on Carmen. Every teammate submits the separate peer review including themselves. Coordinate the grader meeting or permitted task-board evidence. Repository documentation does not establish that these external submissions are complete. Any approved extension should be documented accurately in the release without assuming it changes other deadlines.
