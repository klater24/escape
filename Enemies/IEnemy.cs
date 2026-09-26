using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;

namespace escape.Enemies;

// The shared rules for ENEMIES
public interface IEnemy : IGameResettable
{
    // The senemy's position on screen
    Vector2 Position { get; set; }

    // Update the enemy once per frame
    void Update(GameTime gameTime);

    // Draw the enemy
    void Draw(SpriteBatch spriteBatch);

}