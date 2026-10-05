using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace escape.Interfaces;
// The shared rules for an item
public interface IItem
{
    //the items position on screen
    Vector2 Position { get; set;}

    //Update the item
    void Update(GameTime gameTime);

    //Draw the item
    void Draw(SpriteBatch spriteBatch);

    //Reset the item
    void Reset();

   
}
