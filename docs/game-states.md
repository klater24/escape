# Sprint 4 - Game States (Samiya)

## Behavior and acceptance checks

- Gameplay updates the player, enemies, projectiles, pickups, and collisions.
- P toggles pause. Pause freezes gameplay and its timers; P resumes without resetting the room.
- Enter opens/closes inventory. Gameplay stays frozen while inventory or a transition is active.
- Zero health opens Game Over. Movement, attacks, items, and room shortcuts stay disabled until restart.
- Collecting a Triforce opens Victory and freezes gameplay. Room JSON uses tile `T`; Room 2 contains one near the top for integration testing.
- R cancels transitions and starts a fresh run in Room 1: full health, initial enemies/pickups, no projectiles, no collected/equipped items, and zero starting keys. Q quits in every state.
- Game over takes priority if death and victory are detected together.

## Integration contracts

`Game1.States` owns the current state and exposes `CanUpdateGameplay`, `Current`, and the `Changed` event. Player animation states remain separate.

Person 2 can replace `Game1.InventoryScreen` with an `IInventoryScreen` implementation. It receives `Update(GameTime)` only while inventory is open and `Draw(SpriteBatch, Rectangle)` within an active sprite batch. `Reset()` clears screen input on entry and restart and highlights the equipped item. The current basic view lists equipment quantities; Up/Down selects and Space equips an owned item using `IInventory`. The final HUD, map, and original-game inventory layout remain Person 2's work.

Person 4 can replace `Game1.Transitions` with an `IGameTransition` implementation. `Begin(from, to, destinationRoom)` starts a transition; `Update(GameTime)` continues it even while gameplay is frozen. `IsTransitioning == false` resumes the destination state. `Cancel()` must stop animation on restart. Room transitions pass the destination ID and begin before the room loads, so outgoing render data must be captured then. Camera rendering belongs to Person 4 and must be connected to the draw pipeline. The default transition finishes immediately and does not implement camera animation.

The coordinator resets inventory and player before rebuilding the starting room. Room changes reconnect boss firing and clear old projectiles. The dungeon loader still recreates rooms on each visit; persistent dungeon progress is separate from game-state management.

## Validation

An external integration harness exercises the real game update loop, pickup collisions, reset, death, victory, and a delayed transition driver. Manual checks: P freezes/resumes shots; Enter opens/closes inventory; E reduces health after each cooldown until Game Over; R restarts; collecting `T` in Room 2 shows Victory. Verify camera/UI integrations again when their implementations replace the defaults.

## Inventory-to-gameplay connection

X uses the equipped item through the shared IInventory data. Bombs consume one unit; Bow/Arrow requires owning both items and spends one rupee per shot without consuming either item; boomerangs remain owned. A potion restores full health and consumes one potion only when health is missing. Unsupported/unowned items do nothing. Use is blocked outside gameplay and after death. The existing 1-3 projectile demo shortcuts remain separate from equipped-item use.

Reopening inventory highlights the equipped item. Trying to equip an unowned item displays feedback and preserves the previous equipment. Current room data provides a potion in Room 2 for testing: collect it, open inventory, select Potion, press Space, return with Enter, take damage, then press X.

Current main contains inventory data and a HUD, but no separate final inventory screen. The basic screen therefore remains until Person 2 supplies an IInventoryScreen implementation. Room placements for other equipment and the final inventory layout/map remain separate work.

Validation: 57 automated integration assertions pass, including real potion pickup, keyboard input blocking, equipment use/quantities, reopening selection, death, victory, and reset. Camera/audio code was not changed.
