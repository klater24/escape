using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Projectiles;

// Owns active shots and clears them when the game resets.
public class ProjectileManager : IGameResettable
{
    private readonly List<IProjectile> _projectiles = new();
    private readonly ProjectileFactory _factory;

    public ProjectileManager(ProjectileFactory factory)
    {
        _factory = factory;
    }

    public void Add(IProjectile projectile) => _projectiles.Add(projectile);
    public int Count => _projectiles.Count;

    public void ResolveCombat(Player player, IEnumerable<IDamageable> enemies)
    {
        var targets = enemies.ToArray();
        foreach (var projectile in _projectiles.OfType<ICombatProjectile>())
        {
            if (projectile.HitsPlayer)
            {
                if (projectile.TryHit(player)) player.Damage();
            }
            else
            {
                foreach (var enemy in targets)
                    if (projectile.TryHit(enemy)) enemy.TakeDamage();
            }
        }
        _projectiles.RemoveAll(projectile => !projectile.IsActive);
    }

    public void SpawnArrow(Vector2 position, Vector2 direction) => Add(_factory.CreateArrow(position, direction));
    public void SpawnBomb(Vector2 position) => Add(_factory.CreateBomb(position));
    public void SpawnBoomerang(Vector2 position, Vector2 direction) => Add(_factory.CreateBoomerang(position, direction));
    public void SpawnBossProjectile(Vector2 position, Vector2 direction) => Add(_factory.CreateBossProjectile(position, direction));

    public void Update(GameTime gameTime)
    {
        foreach (var projectile in _projectiles)
            projectile.Update(gameTime);

        _projectiles.RemoveAll(projectile => !projectile.IsActive);
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        foreach (var projectile in _projectiles)
            projectile.Draw(spriteBatch, gameTime);
    }

    // Boss shots travel right; discard them once their left edge exits the view.
    public void RemoveOffscreenBossProjectiles(int screenWidth)
    {
        _projectiles.RemoveAll(projectile =>
            projectile is BossProjectile && projectile.Position.X >= screenWidth);
    }

    public void Reset()
    {
        _projectiles.Clear();
    }
}
