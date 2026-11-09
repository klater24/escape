using escape.Interfaces;
using escape.Projectiles;
using Microsoft.Xna.Framework;

namespace escape.Items;

public sealed class EquippedItemUse
{
    private readonly Player _player;
    private readonly IInventory _inventory;
    private readonly ProjectileManager _projectiles;
    private readonly Func<bool> _canUse;

    public EquippedItemUse(Player player, IInventory inventory, ProjectileManager projectiles, Func<bool> canUse)
    {
        _player = player;
        _inventory = inventory;
        _projectiles = projectiles;
        _canUse = canUse;
    }

    public bool TryUse()
    {
        if (!_canUse() || _player.IsDefeated) return false;
        var equipped = _inventory.GetEquippedItem();
        if (equipped is not ItemType item || !_inventory.HasItem(item)) return false;
        var position = _player.Position;
        var direction = _player.GetFacingDirection() switch
        {
            Direction.Up => -Vector2.UnitY,
            Direction.Down => Vector2.UnitY,
            Direction.Left => -Vector2.UnitX,
            _ => Vector2.UnitX
        };

        switch (item)
        {
            case ItemType.Bomb:
                if (!_inventory.RemoveItem(ItemType.Bomb)) return false;
                _projectiles.SpawnBomb(position);
                return true;
            case ItemType.Arrow:
            case ItemType.Bow:
                if (!_inventory.RemoveItem(ItemType.Arrow)) return false;
                _projectiles.SpawnArrow(position, direction);
                return true;
            case ItemType.Boomerang:
                _projectiles.SpawnBoomerang(position, direction);
                return true;
            case ItemType.Potion:
                if (_player.Health >= _player.MaxHealth || !_inventory.RemoveItem(ItemType.Potion)) return false;
                _player.RestoreHealth(_player.MaxHealth);
                return true;
            default:
                return false;
        }
    }
}
