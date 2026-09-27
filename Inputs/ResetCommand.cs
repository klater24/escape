using Microsoft.Xna.Framework;
using escape.Interfaces;

namespace escape.Inputs;

public class ResetCommand : ICommand
{
    private Game _game;

    public ResetCommand(Game game)
    {
        _game = game;
    }
    public void Execute()
    {
        ((Game1)_game).Reset();
    }
}