using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace escape.Levels;

public class MinimapLayout
{
    private const int HorizontalSpacing = 24;
    private const int VerticalSpacing = 12;

    private static readonly Rectangle RoomSource =
        new(663, 108, 7, 3);

    public class Tile
    {
        public Point Position { get; }
        public Rectangle Source { get; }

        public Tile(Point position, Rectangle source)
        {
            Position = position;
            Source = source;
        }
    }

    public IReadOnlyList<Tile> Tiles { get; }

    public MinimapLayout(DungeonMap dungeonMap)
    {
        List<Tile> tiles = new();

        int minX = dungeonMap.Rooms.Values.Min(room => room.X);
        int minY = dungeonMap.Rooms.Values.Min(room => room.Y);

        foreach (var room in dungeonMap.Rooms)
        {
            int x = (room.Value.X - minX) * HorizontalSpacing;
            int y = (room.Value.Y - minY) * VerticalSpacing;

            tiles.Add(new Tile(
                new Point(x, y),
                RoomSource));
        }

        Tiles = tiles;
    }
}