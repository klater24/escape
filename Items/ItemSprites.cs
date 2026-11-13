using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using escape.Interfaces;

namespace escape.Sprites;
// One place for creating the different projectile types
public static class ItemSprites
{
public static ISprite CreateHeart(Texture2D atlas, Vector2 position, float scale)
    {
        var itemHeartFrames = new[]
        {
            new Rectangle(0, 0, 7, 8),
            new Rectangle(0, 7, 7, 9)
        };
        return new AnimatedSprite(atlas, itemHeartFrames, position, .5f, scale);
    }
public static ISprite CreatePotion(Texture2D atlas, Vector2 position, float scale)
    {
        
        var itemPotionFrames = new[]
        {
             new Rectangle(80, 0, 8, 16),
            new Rectangle(80, 16, 8, 16)
        };
        return new AnimatedSprite(atlas, itemPotionFrames, position, .5f, scale);
        
    }
    
    public static ISprite CreateWatch(Texture2D atlas, Vector2 position, float scale)
    {
        var itemWatchFrames = new[]
        {
           new Rectangle(58, 0, 11, 16)
        };
        return new AnimatedSprite(atlas, itemWatchFrames, position, .5f, scale);
    }
    
    public static ISprite CreateKey(Texture2D atlas, Vector2 position, float scale)
    {
        var itemKeyFrames = new[]
        {
            new Rectangle(240, 0, 7, 15)
        };
        return new AnimatedSprite(atlas, itemKeyFrames, position, 1f, scale);
    }

    public static ISprite CreateBook(Texture2D atlas, Vector2 position, float scale)
    {
        var itemBookFrames = new[]
        {
            new Rectangle(231, 0, 8, 16)
        };
        return new AnimatedSprite(atlas, itemBookFrames, position, 1f, scale);
    }

    public static ISprite CreateHeartContainer(
        Texture2D atlas, Vector2 position, float scale)
    {
        var frames = new[]
        {
            new Rectangle(25, 0, 13, 15)
        };

        return new AnimatedSprite(atlas, frames, position, .5f, scale);
    }

    public static ISprite CreateRupee(
        Texture2D atlas, Vector2 position, float scale)
    {
        var frames = new[]
        {
            new Rectangle(72, 0, 8, 16)
        };

        return new AnimatedSprite(atlas, frames, position, .5f, scale);
    }
    public static ISprite CreateBomb(Texture2D atlas, Vector2 position, float scale)
    {
        return new StaticSprite(
            atlas, new Rectangle(136, 0, 8, 16), position, scale);
    }

    public static ISprite CreateArrow(Texture2D atlas, Vector2 position, float scale)
    {
        return new StaticSprite(
            atlas, new Rectangle(152, 0, 8, 16), position, scale);
    }

    public static ISprite CreateBoomerang(Texture2D atlas, Vector2 position, float scale)
    {
        var frames = new[]
        {
            new Rectangle(128, 0, 8, 16)
        };

        return new AnimatedSprite(atlas, frames, position, 0.5f, scale);
    }

    public static ISprite CreateBow(Texture2D atlas, Vector2 position, float scale)
    {
        return new StaticSprite(
            atlas, new Rectangle(144, 0, 8, 16), position, scale);
    }
}