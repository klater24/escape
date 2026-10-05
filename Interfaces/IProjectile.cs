using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace escape.Interfaces;
// The shared rules for a projectile
public interface IProjectile
{
    //the projectiles position on screen
    Vector2 Position { get; set;}

    //Update the projectile
    void Update(GameTime gameTime);

    //Draw the projectile
    void Draw(SpriteBatch spriteBatch, GameTime gameTime);

    //Reset the projectile
    void Reset();
}
