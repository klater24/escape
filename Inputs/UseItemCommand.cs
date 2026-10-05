using Microsoft.Xna.Framework.Input;
using escape.Interfaces;
using Microsoft.Xna.Framework;
using escape.Projectiles;

namespace escape.Inputs;

public class UseItemCommand : ICommand
{
    private int _itemSlot;
    private Player _player;
    private Arrow _arrow;
    private Bomb _bomb;
    private Boomerang _boomerang;

    public UseItemCommand(int itemSlot, Player player, Arrow arrow, Bomb bomb, Boomerang boomerang)
    {
        _itemSlot = itemSlot;
        _player = player;
        _arrow = arrow;
        _bomb = bomb;
        _boomerang = boomerang;
        
        
    }
    public void Execute()
    {
        if (_itemSlot == 1)
        {
            _arrow.Shoot(_player.getPosit());
        }
        else if (_itemSlot == 2)
        {
            _bomb.Throw(_player.getPosit());
        }
        else if (_itemSlot == 3)
        {
            _boomerang.Throw(_player.getPosit());
        }
    }
}