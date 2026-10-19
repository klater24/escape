using Microsoft.Xna.Framework;

namespace escape.Interfaces;

public class Door
{
    public Rectangle Bounds { get; }

    public string NextRoom { get; }

    public string NextDoor { get; }

    public Door(Rectangle bounds, string nextDoor, string nextRoom)
    {
        Bounds = bounds;
        NextRoom = nextRoom;
        NextDoor = nextDoor;
    }
}