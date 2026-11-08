using escape.GameStates;
using Microsoft.Xna.Framework;

namespace escape.Interfaces;

public interface IGameTransition
{
    bool IsTransitioning { get; }
    void Begin(GameState from, GameState to, string? destinationRoom = null);
    void Update(GameTime gameTime);
    void Cancel();
}
