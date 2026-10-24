using escape.Enemies;

namespace escape.Collisions;

public static class CombatCollisionHandler
{
    public static void Resolve(Player player, EnemyA enemy)
    {
        if (player.Sword.TryHit(enemy))
            enemy.TakeDamage();

        if (!player.IsAttacking && AABBCollisionDetector.IsColliding(player, enemy))
            player.Damage();
    }
}
