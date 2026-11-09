using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using escape.Interfaces;

namespace escape.Items;

// Updates, draws, and resets all items in a room
public sealed class ItemManager : escape.Interfaces.IGameResettable
{
    private readonly IReadOnlyList<IItem> _items;

    public ItemManager(IReadOnlyList<IItem> items)
    {
        _items = items;
    }

    public void Update(GameTime gameTime)
    {
        foreach (var item in _items)
        {
            item.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var item in _items)
        {
            item.Draw(spriteBatch);
        }
    }

    public void Reset()
    {
        foreach (var item in _items) item.Reset();
    }
}
