using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Blocks;

// Keeps track of every block in the level
public class BlockManager
{
    private readonly List<IBlock> _blocks = new();

    public IReadOnlyList<IBlock> Blocks => _blocks;

    // Add a block to the level
    public void Add(IBlock block)
    {
        _blocks.Add(block);
    }

    // Update every block
    public void Update(GameTime gameTime)
    {
        foreach (var block in _blocks)
        {
            block.Update(gameTime);
        }
    }

    // Draw blocks that are turned on
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var block in _blocks)
        {
            if (block.IsActive)
            {
                block.Draw(spriteBatch);
            }
        }
    }

    // Reset every block
    public void Reset()
    {
        foreach (var block in _blocks)
        {
            block.Reset();
        }
    }
}
