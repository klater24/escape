using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace escape.GameStates;

public sealed class StateScreen : IDisposable
{
    private readonly Texture2D _pixel;
    private readonly SpriteFont _font;

    public StateScreen(GraphicsDevice graphics, SpriteFont font)
    {
        _font = font;
        _pixel = new Texture2D(graphics, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public void Draw(SpriteBatch batch, Rectangle viewport, GameState state)
    {
        if (state is GameState.Gameplay or GameState.Inventory or GameState.Transition) return;
        batch.Draw(_pixel, viewport, Color.Black * 0.85f);
        string title = state switch
        {
            GameState.Paused => "PAUSED",
            GameState.GameOver => "GAME OVER",
            GameState.Victory => "TRIFORCE RECOVERED!",
            _ => ""
        };
        string controls = state == GameState.Paused ? "P: Resume   R: Restart   Q: Quit" : "R: Play again   Q: Quit";
        Center(batch, title, viewport.Center.X, viewport.Center.Y - 45, Color.Gold);
        Center(batch, controls, viewport.Center.X, viewport.Center.Y + 20, Color.White);
    }

    private void Center(SpriteBatch batch, string text, float x, float y, Color color)
        => batch.DrawString(_font, text, new Vector2(x - _font.MeasureString(text).X / 2, y), color);

    public void Dispose() => _pixel.Dispose();
}
