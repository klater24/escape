using Microsoft.Xna.Framework;
using escape.Collisions;

namespace escape.Interfaces;

// A game object that can collide with other objects
public interface ICollidable
{
    // World coordinates, including rendering scale; independent of animation frames
    Rectangle Bounds { get; }
    Collision Category { get; }
    bool IsCollisionEnabled { get; }
}
