using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace escape.Enemies;

// Only the selected enemy runs in the sprint demonstration.
public sealed class EnemyManager : escape.Interfaces.IGameResettable
{
    private readonly IReadOnlyList<IEnemy> _enemies;
    public int SelectedIndex { get; private set; }
    public IEnemy Current => _enemies[SelectedIndex];

    public EnemyManager(IReadOnlyList<IEnemy> enemies)
    {
        if (enemies.Count == 0) throw new ArgumentException("At least one enemy is required.", nameof(enemies));
        _enemies = enemies;
    }

    public void Cycle(int direction) => SelectedIndex =
        ((SelectedIndex + direction) % _enemies.Count + _enemies.Count) % _enemies.Count;

    public void Update(GameTime gameTime) => Current.Update(gameTime);
    public void Draw(SpriteBatch spriteBatch) => Current.Draw(spriteBatch);

    public void Reset()
    {
        foreach (var enemy in _enemies) enemy.Reset();
        SelectedIndex = 0;
    }
}
