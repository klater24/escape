using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Blocks;

// Keeps track of every block in the level
public class BlockManager : IGameResettable
{
    private readonly List<IBlock> _blocks = new();
    private int _selectedIndex;

    public IReadOnlyList<IBlock> Blocks => _blocks;
    public IBlock? CurrentBlock => _blocks.Count == 0 ? null : _blocks[_selectedIndex];

    // Add a block to the level
    public void Add(IBlock block)
    {
        _blocks.Add(block);
    }

    // Select the previous block in the list
    public void SelectPrevious()
    {
        if (_blocks.Count > 0)
        {
            _selectedIndex = (_selectedIndex - 1 + _blocks.Count) % _blocks.Count;
        }
    }

    // Select the next block in the list
    public void SelectNext()
    {
        if (_blocks.Count > 0)
        {
            _selectedIndex = (_selectedIndex + 1) % _blocks.Count;
        }
    }

    // Update every block
    public void Update(GameTime gameTime)
    {
        if (CurrentBlock is { IsActive: true } block)
        {
            block.Update(gameTime);
        }
    }

    // Draw blocks that are turned on
    public void Draw(SpriteBatch spriteBatch)
    {
        if (CurrentBlock is { IsActive: true } block)
        {
            block.Draw(spriteBatch);
        }
    }

    // Reset every block
    public void Reset()
    {
        foreach (var block in _blocks)
        {
            block.Reset();
        }

        _selectedIndex = 0;
    }
}
