using escape.Levels;
using Microsoft.Xna.Framework;

namespace escape.Interfaces;

public class Door
{
    public Rectangle Bounds { get; }

    public Room NextRoom { get; }

    public Door NextDoor { get; }

    public Door(Rectangle bounds, Door nextDoor, Room nextRoom)
    {
        Bounds = bounds;
        NextRoom = nextRoom;
        NextDoor = nextDoor;
    }
}
