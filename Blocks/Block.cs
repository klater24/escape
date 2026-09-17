using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Blocks;

// A block holds the block type and the sprite that shows it
public class Block : IBlock
{
    private readonly string _blockType;
    private readonly ISprite _sprite;
    private readonly Vector2 _initialPosition;
    private readonly int _cycleIndex;

    public Block(string blockType, ISprite sprite, Vector2 position, int cycleIndex = 0)
    {
        _blockType = blockType;
        _sprite = sprite;
        _initialPosition = position;
        _cycleIndex = cycleIndex;
        Position = position;
        IsActive = true;
    }

    public Vector2 Position { get; set; }
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
        _sprite.Reset();
        Position = _initialPosition;
        IsActive = true;
    }

    // Leave this ready for future block cycling controls
    public void Cycle()
    {
        _sprite.Reset();
    }
}
