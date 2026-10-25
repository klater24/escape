using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using escape.Levels;
using System.Numerics;

namespace escape.Interfaces; 

public class Rooms
{
    public Room CurrentRoom { get; private set; }


    public Rooms(Room room)
    {
        CurrentRoom = room;     
    }
    public bool TryTransition(Player player, Door door)
    {
        if (CurrentRoom._data.Doors.Count()<CurrentRoom._data.Doors.IndexOf(door)+1)
        {
            return false;
        }
        CurrentRoom = door.NextRoom;
        player.position.X = CurrentRoom._data.SpawnX;
        player.position.Y = CurrentRoom._data.SpawnY;
        return true;
    }
    public bool CheckDoorTransition(Player player, MouseState currentMouse, MouseState prevMouse)
{
    foreach (Door door in CurrentRoom._data.Doors)
    {
        if(currentMouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
        {
           Point mousePosition = new Point(currentMouse.X, currentMouse.Y);
                if (door.Bounds.Contains(mousePosition))
                 return this.TryTransition(player, door);
        }
        if (!player.Bounds.Intersects(door.Bounds))
            continue;
        return this.TryTransition(player, door);
    }

    return false;
}
}
