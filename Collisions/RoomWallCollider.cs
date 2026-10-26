using Microsoft.Xna.Framework;
using escape.Interfaces;

namespace escape.Collisions;

public class RoomWallCollider : ISolidCollider
{
    public Rectangle Bounds { get; }

    public Collision Category => Collision.Environment;

    public bool IsCollisionEnabled => true;

    public bool IsSolid => true;

    public RoomWallCollider(Rectangle bounds)
    {
        Bounds = bounds;
    }
}