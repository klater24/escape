using System.Linq;
using Microsoft.Xna.Framework;

namespace escape.Interfaces;

public class Room
{
    public string Id { get; }

    public IReadOnlyList<Door> Doors { get; }

    public IReadOnlyDictionary<string, Vector2> Entrances { get; }
    private readonly Dictionary<string, Room> rooms;

    public Room CurrentRoom { get; private set; }


    public Room(string id, IReadOnlyList<Door> doors, IReadOnlyDictionary<string, Vector2> entrances, IEnumerable<Room> rooms,
        string startingRoomId)
    {
        Id = id;
        Doors = doors;
        Entrances = entrances;
        this.rooms = new Dictionary<string, Room>();

        foreach (Room room in rooms)
            this.rooms.Add(room.Id, room);
               if (!this.rooms.TryGetValue(
                startingRoomId, out Room startingRoom))
        {
            throw new ArgumentException(
                $"Unknown starting room: {startingRoomId}");
        }

        CurrentRoom = startingRoom;

        
    }
    public bool TryTransition(IMoveCollider player, Door door)
    {
        if (!rooms.TryGetValue(door.NextRoom, out Room destination))
        {
            return false;
        }

        if (!destination.Entrances.TryGetValue(door.NextDoor, out Vector2 spawnPosition))
        {
            return false;
        }

        CurrentRoom = destination;
        player.Position = spawnPosition;

        return true;
    }
    public bool CheckDoorTransition(IMoveCollider player,Room roomManager)
{
    foreach (Door door in roomManager.CurrentRoom.Doors)
    {
        if (!player.Bounds.Intersects(door.Bounds))
            continue;

        return roomManager.TryTransition(player, door);
    }

    return false;
}
}
