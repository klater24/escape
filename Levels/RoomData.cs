
using System.Collections.Generic;
using escape.Collisions;
using escape.Interfaces;

namespace escape.Levels;

public class RoomData
{
    public string Name { get; set; } = "";
    public float SpawnX { get; set; } = 100;
    public float SpawnY { get; set; } = 100;

    public List<GameObjectData> Blocks { get; set; } = new();

    public List<GameObjectData> Enemies { get; set; } = new();

    public List<GameObjectData> Items { get; set; } = new();

    public List<Door> Doors { get; set; } = new();
}

public class GameObjectData
{
    public string Type { get; set; } = "";

    public float X { get; set; }

    public float Y { get; set; }
}
