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
}

public class DoorData
{
    public string Direction { get; set; } = "";
    public string DestinationRoom { get; set; } = "";
    public string DestinationDoor { get; set; } = "";
    public float SpawnX { get; set; }
    public float SpawnY { get; set; }
}

public class GameObjectData
{
    public string Type { get; set; } = "";
    public float X { get; set; }
    public float Y { get; set; }
}
