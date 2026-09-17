using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace escape.Interfaces;

// The shared rules for anything the game can draw
public interface ISprite
{
    // The sprite position on screen
    Vector2 Position { get; set; }

    // Update the sprite once per frame
    void Update(GameTime gameTime);

    // Draw the sprite
    void Draw(SpriteBatch spriteBatch);

    // Return the sprite to its starting state
    void Reset();
}