using Microsoft.Xna.Framework;

namespace escape.Interfaces;

// A collidable object whose position can be moved
public interface IMoveCollider : ICollidable
{
    Vector2 Position { get; set; }
}