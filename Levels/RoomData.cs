using System.Collections.Generic;

namespace escape.Levels;
public class RoomData
{
    public string Name { get; set; } = "";
    public float SpawnX { get; set; } = 100;
    public float SpawnY { get; set; } = 100;

    public List<string> TileMap { get; set; } = new();
    public List<GameObjectData> Blocks { get; set; } = new();
    public List<GameObjectData> Enemies { get; set; } = new();
    public List<GameObjectData> Items { get; set; } = new();
    public List<DoorData> Doors { get; set; } = new();

    public string Id { get; set; } = "";                // Unique identifier for the room - matches filename without any extension
    public int MapX { get; set; }                       // X coordinate of the room in the map grid - positive X is right, positive Y is down
    public int MapY { get; set; }
    public string RoomType { get; set; } = "TopDown";   // Distinguishes rooms with different gameplay styles, e.g. "TopDown" or "SideScrolling"
}

public class DoorData
{
    public string Direction { get; set; } = "";         // Can be "Top", "Bottom", "Left", or "Right"
    public string DestinationRoom { get; set; } = "";   // Destination room ID
    public string DestinationDoor { get; set; } = "";   // Destination doorway's direction as the current loader expects
    public float SpawnX { get; set; }                   // Player position when arriving through this doorway into this room
    public float SpawnY { get; set; }
    public string Id { get; set; } = "";                // Unique identifier for the door - ex: Room1.Top
    public string Type { get; set; } = "Open";          // Can be open, locked, bombable
}

public class GameObjectData
{
    public string Type { get; set; } = "";
    public float X { get; set; }
    public float Y { get; set; }
}
