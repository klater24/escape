
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;

namespace escape.Levels;

public class DungeonMap
{
    private readonly Dictionary<string, Point> _rooms = new();
    private readonly Dictionary<string, int> _roomTileIndices = new();
    private readonly Dictionary<string, HashSet<string>> _connectedRooms = new();
    public IReadOnlyDictionary<string, HashSet<string>> Connections => _connectedRooms;
    public IReadOnlyDictionary<string, Point> Rooms => _rooms;
    private readonly HashSet<string> _exploredRooms = new();
    public IReadOnlyCollection<string> ExploredRooms => _exploredRooms;
    public string? TriforceRoomId { get; private set; }
    public bool ShowAlternateTriforceMarker { get; private set; }

    public Rectangle GetPlayerMarkerSource()
    {
        return new Rectangle(537, 126, 3, 3);
    }
    public void MarkExplored(string roomId)
    {
        if (_rooms.ContainsKey(roomId))
        {
            _exploredRooms.Add(roomId);
        }
    }

    public bool IsExplored(string roomId)
    {
        return _exploredRooms.Contains(roomId);
    }

    public void Reset() => _exploredRooms.Clear();

    public Rectangle GetRoomTileSource(string roomId)
    {
        int tileIndex = _roomTileIndices[roomId];

        return new Rectangle(
            519 + (tileIndex * 9),
            108,
            8,
            8);
    }

    public void Update(GameTime gameTime)
    {
        ShowAlternateTriforceMarker =
            (int)(gameTime.TotalGameTime.TotalMilliseconds / 500) % 2 == 0;
    }

    public DungeonMap(string levelsDirectory)
    {
        string[] files = Directory.GetFiles(levelsDirectory, "*.json");

        foreach (string file in files)
        {
            RoomData data = RoomParser.Load(file);

            if (string.IsNullOrWhiteSpace(data.Id))
            {
                throw new InvalidDataException(
                    $"Room file '{file}' has no room ID.");
            }

            if (_rooms.ContainsKey(data.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate room ID '{data.Id}'.");
            }

            _rooms.Add(data.Id, new Point(data.MapX, data.MapY));
            if (data.TileMap.Any(row => row.Contains("(T)")))
            {
                TriforceRoomId = data.Id;
            }

            _connectedRooms.Add(data.Id, data.Doors.Select(door => door.DestinationRoom)
                    .Where(destination => !string.IsNullOrWhiteSpace(destination)).ToHashSet());
            
            int doorMask = 0;

            foreach (DoorData door in data.Doors)
            {
                doorMask |= door.Direction switch
                {
                    "Right" => 1,
                    "Left" => 2,
                    "Bottom" => 4,
                    "Top" => 8,
                    _ => 0
                };
            }

            _roomTileIndices.Add(data.Id, doorMask);
        }
    }
}
