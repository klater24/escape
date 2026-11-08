using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace escape.Enemies;

// Updates, draws, and resets all enemies in a room
public sealed class EnemyManager : escape.Interfaces.IGameResettable
{
    private readonly IReadOnlyList<IEnemy> _enemies;

    public EnemyManager(IReadOnlyList<IEnemy> enemies)
    {
        _enemies = enemies;
    }

    public void Update(GameTime gameTime)
    {
        foreach (var enemy in _enemies)
        {
            enemy.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var enemy in _enemies)
        {
            enemy.Draw(spriteBatch);
        }
    }

    public void Reset()
    {
        foreach (var enemy in _enemies) enemy.Reset();
    }
}
