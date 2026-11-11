# Dungeon room data contract

Rooms are JSON text files in `Content/Levels/<Id>.json`, loaded by
`RoomParser` and instantiated by `Room`. JSON property names are read without
case sensitivity. Use the field names and value spelling below when authoring
files. This document describes the Sprint 4 contract proposed for review by the
inventory/map and camera owners.

## Room fields

| Field | Meaning |
| --- | --- |
| `Id` | Unique, stable room identifier matching the filename without `.json`, such as `Room1`. |
| `Name` | Display name; it may differ from the ID. Connections use the ID. |
| `MapX`, `MapY` | Integer dungeon map coordinates. Positive X is right; positive Y is down. Each ordinary adjacent room is one grid step away. |
| `RoomType` | `TopDown` or `SideScrolling`. All current rooms use `TopDown`. |
| `SpawnX`, `SpawnY` | Default player position in room-local pixels when loading the room directly. |
| `TileMap` | Seven rows of twelve parenthesized tile symbols, described below. |
| `Doors` | Door definitions with destination connections and arrival positions. |

Dungeon map coordinates are separate from room-local pixel coordinates and are
not camera positions. Current arrangement:

| Room | MapX | MapY | Connections |
| --- | --- | --- | --- |
| Room1 | 0 | 0 | Top to Room2.Bottom; Right to Room3.Left |
| Room2 | 0 | -1 | Bottom to Room1.Top |
| Room3 | 1 | 0 | Left to Room1.Right |

`Id`, map coordinates, and `RoomType` are metadata at this stage. The current
loader still selects room files by the supplied room ID, and does not dispatch
movement behavior based on `RoomType`. Side-scrolling behavior is a later issue.

## Tile maps and pixel coordinates

The current top-down room interior is 12 columns by 7 rows. Each tile is 48 by
48 pixels (32-pixel artwork at scale 1.5), with the interior starting at (96, 96).
Column and row indices start at zero:

```text
X = 96 + column * 48
Y = 96 + row * 48
```

Positive pixel X points right and positive pixel Y points down. Spawn positions
refer to `Player.Position`, the top-left of the player's body hitbox, not its
center. The current player body is 48 by 48 pixels. Place spawns clear of solid
tiles and exterior walls.

Each row uses adjacent parenthesized symbols, for example:

```json
"(0)(0)(0)(0)(0)(E)(0)(0)(K)(0)(0)(0)"
```

| Symbol | Object |
| --- | --- |
| `0` | Empty floor |
| `B` | Solid brick block |
| `E` | Enemy A (Run/Knight) |
| `F` | Flying demon |
| `M` | Forester (ManaSeed) |
| `X` | Boss |
| `K` | Key |
| `D` | Book |
| `W` | Watch |
| `H` | Heart |
| `P` | Potion |

The floor is generated separately. `TileMap` is the supported placement source
for blocks, enemies, and items. Although `RoomData` retains `Blocks`, `Enemies`,
and `Items` lists, the current `Room` constructor does not instantiate their
contents. Treat those lists as legacy; omit them from new room files.

## Door fields and arrival semantics

| Field | Meaning |
| --- | --- |
| `Id` | Globally unique stable door ID, using `<RoomId>.<Direction>`, such as `Room1.Top`. |
| `Type` | `Open`, `Locked`, or `Bombable`. Room1.Right and Room3.Left form the example bombable passage. |
| `Direction` | This room's doorway side: `Top`, `Bottom`, `Left`, or `Right`. Define at most one door per side. |
| `DestinationRoom` | Destination room ID, matching its filename. |
| `DestinationDoor` | Destination doorway's **direction**, not its `Id`. |
| `SpawnX`, `SpawnY` | Arrival position inside **this room** when entering through this doorway. |

For example, this door belongs to Room1:

```json
{
  "Id": "Room1.Top",
  "Type": "Open",
  "Direction": "Top",
  "DestinationRoom": "Room2",
  "DestinationDoor": "Bottom",
  "SpawnX": 360,
  "SpawnY": 102
}
```

Leaving through it loads Room2 and looks up Room2's Bottom door to obtain the
arrival position. The (360, 102) above is used when returning into Room1 through
its Top door. Room-level spawns instead apply to direct room loads.

Normal connections are reciprocal: Room2.Bottom points back to Room1.Top.
Author both sides and ensure their dungeon map positions agree with their
directions. Door trigger bounds currently come from `Room.CreateDoors()`:

| Direction | X | Y | Width | Height |
| --- | --- | --- | --- | --- |
| Top | 336 | 48 | 96 | 48 |
| Bottom | 336 | 432 | 96 | 48 |
| Left | 48 | 216 | 48 | 96 |
| Right | 672 | 216 | 48 | 96 |

These are room-local rectangles; they are not configurable in JSON. Existing
navigation prevents another transition until the player leaves doorway bounds.

`Locked` passages require and consume a key when traversed. `Bombable` passages
draw as solid exterior walls and block player movement, projectiles, and room
transitions until a bomb explosion overlaps their doorway bounds. Configure
both reciprocal doors as `Bombable`. Opening either side reveals both sides
for the rest of the run, including room revisits; reset restores the walls.
Enemies remain blocked at all doorways, even after a passage opens.
Interior destructible blocks require their own data representation.

To try the example, stand against the middle of Room1's right wall and press
`2` (the existing bomb test control). After the two-second fuse the centered
96-pixel blast reveals the passage. Walk right to transition to Room3, then
return to confirm the opening persists. A distant bomb, arrow, or key does
not reveal the wall. Equipped bombs used with `X` use the same explosion path
and consume inventory through the existing item-use logic.

## Integration boundaries

- Person 1 owns room IDs, map positions, layouts, and connection data.
- Person 2 can use IDs, map coordinates, and connections for the dungeon map.
  Locked-door logic will consume keys through the agreed inventory interface;
  a temporary key provider can support independent development.
- Person 4 can use source/destination room information for camera transitions.
  Dungeon grid coordinates require an explicit conversion to camera coordinates.
  Existing immediate navigation remains usable before animation integration.

Review these fields with the other owners before treating the proposed contract
as agreed. Future schema extensions should preserve existing room files where
possible.

## Authoring checks

Before adding a room, check unique IDs and map positions, seven rows of twelve
valid tile symbols, valid destination files and doorway directions, reciprocal
connections, and unobstructed default and doorway spawns. The current parser
deserializes JSON but does not enforce all these rules. Automated runtime
validation belongs to the room-validation issue.

Build the project and manually traverse each connection in both directions.
Check that arriving does not immediately transition back. Metadata alone does
not establish that locked-door, bombable-wall, or side-scrolling behavior works.
