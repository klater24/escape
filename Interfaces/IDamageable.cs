namespace escape.Interfaces;

public interface IDamageable : ICollidable
{
    int Health { get; }
    bool IsDefeated { get; }
    void TakeDamage();
}
