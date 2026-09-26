using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Blocks;

// A block holds the block type and the sprite that shows it
public class Block : IBlock
{
    private readonly string _blockType;
    private readonly Vector2 _initialPosition;
    private readonly ISprite _sprite;
    private Vector2 _position;

    public Block(Texture2D atlas, string blockType, Vector2 position, float scale = 1f)
    {
        _blockType = blockType;
        _initialPosition = position;
        _sprite = escape.Sprites.SpriteFactory.CreateBlockSprite(atlas, blockType, position, scale);
        Position = position;
        IsActive = true;
    }

    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            _sprite.Position = value;
        }
    }

    public string BlockType => _blockType;
    public bool IsStationary => true;
    public bool IsActive { get; set; }

    // Blocks stay in place but their sprites may still update
    public void Update(GameTime gameTime)
    {
        _sprite.Update(gameTime);
    }

    // Draw the block sprite
    public void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Draw(spriteBatch);
    }

    // Put the block back to its starting state
    public void Reset()
    {
        Position = _initialPosition;
        IsActive = true;
    }
}
