using Microsoft.Xna.Framework.Input;
using escape.Interfaces;
using Microsoft.Xna.Framework;
using escape.Projectiles;
using escape.Items;

namespace escape.Inputs;

public class UseItemCommand : ICommand
{
    private int _itemSlot;
    private Player _player;
    private ProjectileManager _projectiles;
    private readonly IInventory _inventory;

    public UseItemCommand(int itemSlot, Player player, ProjectileManager projectiles, IInventory inventory)
    {
        _itemSlot = itemSlot;
        _player = player;
        _projectiles = projectiles;
        _inventory = inventory;
    }
    public void Execute()
    {
        if (_player.IsDefeated) return;
        Vector2 position = _player.getPosit();
        Vector2 direction = GetDirection();
        if (_itemSlot == 1)
        {
            _projectiles.SpawnArrow(position, direction);
        }
        else if (_itemSlot == 2)
        {
            if (!_inventory.RemoveItem(ItemType.Bomb)) return;
            _projectiles.SpawnBomb(position);
        }
        else if (_itemSlot == 3)
        {
            _projectiles.SpawnBoomerang(position, direction);
        }
    }
    private Vector2 GetDirection()
    {
        if (_player.GetFacingDirection() == Direction.Up)
            return new Vector2(0, -1);

        if (_player.GetFacingDirection() == Direction.Down)
            return new Vector2(0, 1);

        if (_player.GetFacingDirection() == Direction.Left)
            return new Vector2(-1, 0);

        return new Vector2(1, 0);
    }
}
