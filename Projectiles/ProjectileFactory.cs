using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Sprites;

namespace escape.Projectiles;

// Borrows loaded textures; every call creates independent sprite and projectile state
public class ProjectileFactory
{
    private readonly Texture2D _projectileAtlas;
    private readonly Texture2D _bossAtlas;

    public ProjectileFactory(Texture2D projectileAtlas, Texture2D bossAtlas)
    {
        _projectileAtlas = projectileAtlas;
        _bossAtlas = bossAtlas;
    }

    public Arrow CreateArrow(Vector2 position, Vector2 direction) =>
        new Arrow(ProjectileSprites.CreateArrow(_projectileAtlas, position, 2f), position, direction);

    public Bomb CreateBomb(Vector2 position) =>
        new Bomb(ProjectileSprites.CreateBomb(_projectileAtlas, position, 2f),
            ProjectileSprites.CreateBombExplosion(_projectileAtlas, position, 2f), position);

    public Boomerang CreateBoomerang(Vector2 position, Vector2 direction) =>
        new Boomerang(ProjectileSprites.CreateBoomerang(_projectileAtlas, position, 2f), position, direction);

    public BossProjectile CreateBossProjectile(Vector2 position, Vector2 direction) =>
        new BossProjectile(ProjectileSprites.CreateBossProjectile(_bossAtlas, position, 2f), position, direction);

    internal static Vector2 NormalizeDirection(Vector2 direction)
    {
        float length = direction.Length();
        if (!float.IsFinite(length) || length <= 0f)
            throw new ArgumentException("Direction must be a finite, nonzero vector.", nameof(direction));
        return direction / length;
    }
}