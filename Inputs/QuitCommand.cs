using Microsoft.Xna.Framework;
using escape.Interfaces;

namespace escape.Inputs;

public class QuitCommand : ICommand
{
    private Game _game;

    public QuitCommand(Game game)
    {
        _game = game;
    }
    public void Execute()
    {
        _game.Exit();
    }
}