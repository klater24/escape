using escape.Interfaces;
using Microsoft.Xna.Framework;

namespace escape.Collisions;

// Detects whether two objects with axis-aligned bounding boxes (AABBs) are colliding
// Does not handle collision resolution, only detection
public static class AABBCollisionDetector
{
    public static bool IsColliding(ICollidable a, ICollidable b)
    {
        // An object cannot collide with itself
        if (ReferenceEquals(a, b))
            return false;

        // If either object is not enabled for collision, they cannot collide
        if (!a.IsCollisionEnabled || !b.IsCollisionEnabled)
            return false;
        
        // The bounds of the two objects used to determine if they overlap
        Rectangle boundsA = a.Bounds;
        Rectangle boundsB = b.Bounds;

        // Empty or invalid hitboxes cannot collide
        if (boundsA.Width <= 0 || boundsA.Height <= 0 ||
            boundsB.Width <= 0 || boundsB.Height <= 0)
            return false;

        // Return if there is an overlap between the two AABBs
        return boundsA.Left < boundsB.Right &&
               boundsA.Right > boundsB.Left &&
               boundsA.Top < boundsB.Bottom &&
               boundsA.Bottom > boundsB.Top;
    }
}