
using escape.Interfaces;
using Microsoft.Xna.Framework.Input;

namespace escape.Inputs;

public class AttackCommand : ICommand
{
    private Player _player;

    public AttackCommand(Player player)
    {
        _player = player;
    }
    public void Execute()
    {
        
    }
}