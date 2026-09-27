using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace escape.Enemies;

// The shared rules for ENEMIES
public interface IEnemy
{
    // The senemy's position on screen
    Vector2 Position { get; set; }

    // Update the enemy once per frame
    void Update(GameTime gameTime);

    // Draw the enemy
    void Draw(SpriteBatch spriteBatch);

    // Return the enemy to its starting state
    void Reset();
}