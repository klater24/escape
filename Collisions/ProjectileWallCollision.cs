using escape.Interfaces;
using Microsoft.Xna.Framework;

namespace escape.Collisions;

public static class ProjectileWallCollision
{
    public static bool BlocksPath(Vector2 start, Vector2 end, ISolidCollider wall)
    {
        if (!wall.IsSolid || !wall.IsCollisionEnabled) return false;
        Rectangle bounds = wall.Bounds;
        float enter = 0f, exit = 1f;
        Vector2 delta = end - start;
        return Clip(start.X, delta.X, bounds.Left, bounds.Right, ref enter, ref exit) &&
            Clip(start.Y, delta.Y, bounds.Top, bounds.Bottom, ref enter, ref exit);
    }

    private static bool Clip(float start, float delta, float min, float max,
        ref float enter, ref float exit)
    {
        if (MathF.Abs(delta) < 0.00001f) return start >= min && start <= max;
        float first = (min - start) / delta;
        float last = (max - start) / delta;
        if (first > last) (first, last) = (last, first);
        enter = MathF.Max(enter, first);
        exit = MathF.Min(exit, last);
        return enter <= exit;
    }
}
