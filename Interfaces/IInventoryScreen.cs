using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace escape.Interfaces;

public interface IInventoryScreen
{
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch, Rectangle viewport);
    void Reset();
}
