namespace escape.Interfaces;

public interface ICombatProjectile : IProjectile, ICollidable
{
    bool HitsPlayer { get; }
    bool TryHit(ICollidable target);
}
