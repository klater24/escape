
using System.Collections.Generic;

namespace escape.Levels;

public class RoomData
{
    public string Name { get; set; } = "";

    public List<GameObjectData> Blocks { get; set; } = new();

    public List<GameObjectData> Enemies { get; set; } = new();

    public List<GameObjectData> Items { get; set; } = new();

    public List<DoorData> Doors { get; set; } = new();
}

public class GameObjectData
{
    public string Type { get; set; } = "";

    public float X { get; set; }

    public float Y { get; set; }
}

public class DoorData
{
    public float X { get; set; }

    public float Y { get; set; }

    public string DestinationRoom { get; set; } = "";

    public float SpawnX { get; set; }

    public float SpawnY { get; set; }
}
