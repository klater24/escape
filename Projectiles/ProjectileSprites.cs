using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using escape.Interfaces;

namespace escape.Sprites;
// One place for creating the different projectile types
public static class ProjectileSprites
{
public static ISprite CreateArrow(Texture2D atlas, Vector2 position, float scale)
    {
        var itemArrowFrames = new[]
        {
            new Rectangle(10, 185, 16, 15),
            new Rectangle(53, 185, 7, 15)
        };
        return new AnimatedSprite(atlas, itemArrowFrames, position, 2f, scale);
    }
public static ISprite CreateBomb(Texture2D atlas, Vector2 position, float scale)
    {
        
        var itemBombFrames = new[]
        {
            new Rectangle(129, 185, 8, 16),
            new Rectangle(138, 185, 16, 16)
        };
        return new AnimatedSprite(atlas, itemBombFrames, position, 2f, scale);
        
    }
    
    public static ISprite CreateBoomerang(Texture2D atlas, Vector2 position, float scale)
    {
        var itemBoomerangFrames = new[]
        {
            new Rectangle(64, 185, 8, 16),
            new Rectangle(73, 185, 7, 16),
            new Rectangle(82, 185, 7, 16),
            new Rectangle(73, 185, 7, 16),
            //new Rectangle(53, 185, 7, 15)
        };
        return new AnimatedSprite(atlas, itemBoomerangFrames, position, .5f, scale);
    }
    
    public static ISprite CreateBossProjectile(Texture2D atlas, Vector2 position, float scale)
    {
        var itemBossProjectileFrames = new[]
        {
            new Rectangle(101, 11, 7, 16),
            new Rectangle(110, 11, 8, 16),
            new Rectangle(119, 11, 8, 16),
            new Rectangle(128, 11, 7, 16)
        };
        return new AnimatedSprite(atlas, itemBossProjectileFrames, position, 1f, scale);
    }
}