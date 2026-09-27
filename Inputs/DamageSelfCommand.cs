using escape.Interfaces;

namespace escape.Inputs;

public class DamageSelfCommand : ICommand
{
    private Player _player;

    public DamageSelfCommand(Player player)
    {
        _player = player;
    }
    public void Execute()
    {
        
    }
}