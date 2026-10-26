using escape.Collisions;
using escape.Interfaces;

namespace escape.Projectiles;

public sealed class ProjectileHitTracker
{
    private readonly HashSet<ICollidable> _targets = new();

    public bool TryHit(ICollidable projectile, ICollidable target) =>
        AABBCollisionDetector.IsColliding(projectile, target) && _targets.Add(target);

    public void Reset() => _targets.Clear();
}
