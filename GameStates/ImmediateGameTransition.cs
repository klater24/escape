using escape.Interfaces;
using Microsoft.Xna.Framework;

namespace escape.GameStates;

public sealed class ImmediateGameTransition : IGameTransition
{
    public bool IsTransitioning => false;
    public void Begin(GameState from, GameState to, string? destinationRoom = null) { }
    public void Update(GameTime gameTime) { }
    public void Cancel() { }
}
