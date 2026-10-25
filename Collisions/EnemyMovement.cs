using escape.Interfaces;
using Microsoft.Xna.Framework;

namespace escape.Collisions;

public static class EnemyMovement
{
    private const float PositionTolerance = 0.001f;

    public static Vector2 Move(IEnvironmentEnemy enemy, Vector2 direction, float distance)
    {
        Vector2 expected = enemy.Position + direction * distance;
        EnvironmentCollisionResolver.Move(enemy, direction * distance, enemy.Obstacles);
        if (MathF.Abs(enemy.Position.X - expected.X) > PositionTolerance)
            direction.X = -direction.X;
        if (MathF.Abs(enemy.Position.Y - expected.Y) > PositionTolerance)
            direction.Y = -direction.Y;
        return direction;
    }
}
