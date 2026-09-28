using Microsoft.Xna.Framework;
using escape.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace escape.Inputs;

public class QuitCommand : ICommand
{
    private Game game;

    public QuitCommand(Game game)
    {
        this.game = game;
    }
    public void Execute()
    {
        game.Exit();
    }
}