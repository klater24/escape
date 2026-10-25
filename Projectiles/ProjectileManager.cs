using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;
using escape.Collisions;

namespace escape.Projectiles;

// Owns active shots and clears them when the game resets.
public class ProjectileManager : IGameResettable
{
    private readonly List<IProjectile> _projectiles = new();
    private readonly ProjectileFactory _factory;
    private const double MaximumStepSeconds = 1.0 / 120.0;
    public IReadOnlyList<ISolidCollider> Obstacles { get; set; } = Array.Empty<ISolidCollider>();

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
            if (projectile is not Bomb { State: BombState.Exploding } &&
                Obstacles.Any(wall => wall.IsSolid && wall.IsCollisionEnabled &&
                projectile.Bounds.Intersects(wall.Bounds)))
            {
                projectile.HitWall();
                continue;
            }
            if (projectile.HitsPlayer)
            {
                if (projectile.TryHit(player)) player.Damage();
            }
            else
            {
                foreach (var enemy in targets)
                    if (!Obstacles.Any(wall => ProjectileWallCollision.BlocksPath(
                            projectile.Bounds.Center.ToVector2(), enemy.Bounds.Center.ToVector2(), wall)) &&
                        projectile.TryHit(enemy)) enemy.TakeDamage();
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
        {
            double remaining = gameTime.ElapsedGameTime.TotalSeconds;
            while (remaining > 0 && projectile.IsActive)
            {
                double step = Math.Min(remaining, MaximumStepSeconds);
                Vector2 previous = projectile.Position;
                projectile.Update(new GameTime(gameTime.TotalGameTime, TimeSpan.FromSeconds(step)));
                if (projectile is ICombatProjectile combat &&
                    projectile is not Bomb { State: BombState.Exploding } &&
                    Obstacles.Any(wall => wall.IsSolid && wall.IsCollisionEnabled &&
                        combat.Bounds.Intersects(wall.Bounds)))
                {
                    projectile.Position = previous;
                    combat.HitWall();
                    projectile.Update(new GameTime(gameTime.TotalGameTime, TimeSpan.Zero));
                }
                remaining -= step;
            }
        }

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
