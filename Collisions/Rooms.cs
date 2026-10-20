using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace escape.Interfaces; 

public class Rooms
{
    public string Id { get; }

    public IReadOnlyList<Door> Doors { get; }

    public IReadOnlyDictionary<string, Vector2> Entrances { get; }
    private readonly Dictionary<string, Rooms> rooms;

    public Rooms CurrentRoom { get; private set; }


    public Rooms(string id, IReadOnlyList<Door> doors, IReadOnlyDictionary<string, Vector2> entrances, IEnumerable<Rooms> rooms,
        string startingRoomId)
    {
        Id = id;
        Doors = doors;
        Entrances = entrances;
        this.rooms = new Dictionary<string, Rooms>();

        foreach (Rooms room in rooms)
            this.rooms.Add(room.Id, room);
               if (!this.rooms.TryGetValue(
                startingRoomId, out Rooms startingRoom))
        {
            throw new ArgumentException(
                $"Unknown starting room: {startingRoomId}");
        }

        CurrentRoom = startingRoom;

        
    }
    public bool TryTransition(IMoveCollider player, Door door)
    {
        if (!rooms.TryGetValue(door.NextRoom, out Rooms destination))
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
    public bool CheckDoorTransition(Player player, Rooms room, MouseState currentMouse, MouseState prevMouse)
{
    foreach (Door door in room.CurrentRoom.Doors)
    {
        if(currentMouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
        {
           Point mousePosition = new Point(currentMouse.X, currentMouse.Y);
                if (door.Bounds.Contains(mousePosition))
                 return room.TryTransition(player, door);
        }
        if (!player.Bounds.Intersects(door.Bounds))
            continue;
        return room.TryTransition(player, door);
    }

    return false;
}
}
