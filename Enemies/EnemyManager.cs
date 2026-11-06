using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace escape.Enemies;

// Enemy Manager purpose is to manage the enemies in the game, allowing for cycling through them, updating their state, and drawing them on the screen
// It implements the IGameResettable interface to allow for resetting the state of all managed enemies.
public sealed class EnemyManager : escape.Interfaces.IGameResettable
{
    private readonly IReadOnlyList<IEnemy> _enemies;
    public int SelectedIndex { get; private set; }
    public IEnemy? Current => _enemies.Count == 0 ? null : _enemies[SelectedIndex]; 

    public EnemyManager(IReadOnlyList<IEnemy> enemies)
    {
        _enemies = enemies;
    }

    public void Cycle(int direction)
    {
        if (_enemies.Count == 0)
        {
            return;
        }

        SelectedIndex = ((SelectedIndex + direction) % _enemies.Count + _enemies.Count) % _enemies.Count;
    }

    public void Update(GameTime gameTime) => Current?.Update(gameTime);
    public void Draw(SpriteBatch spriteBatch) => Current?.Draw(spriteBatch);

    public void Reset()
    {
        foreach (var enemy in _enemies) enemy.Reset();
        SelectedIndex = 0;
    }
}
