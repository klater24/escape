using Microsoft.Xna.Framework;
using escape.Items;
using escape.Interfaces;

namespace escape.Interfaces;

public enum DoorType
{
    Open,
    Locked,
    Bombable
}

public class Door
{
    public Rectangle Bounds { get; }
    public string NextRoom { get; }
    public string NextDoor { get; }
    public string Id { get; }
    public string Direction { get; }
    public DoorType Type { get; private set; }
    public bool IsLocked => Type == DoorType.Locked;
    public bool IsBombable => Type == DoorType.Bombable;

    public Door(Rectangle bounds, string nextDoor, string nextRoom, string id = "", string type = "Open", string direction = "")
    {
        Bounds = bounds;
        NextDoor = nextDoor;
        NextRoom = nextRoom;
        Id = id;
        Direction = direction;
        Type = ParseType(type);
    }

    public bool CanTraverse(IInventory inventory)
    {
        if (IsBombable) return false;
        if (Type != DoorType.Locked)
            return true;

        return inventory.HasItem(ItemType.Key);
    }

    public bool TryUnlock(IInventory inventory)
    {
        if (IsBombable) return false;
        if (Type != DoorType.Locked)
            return true;

        if (!inventory.RemoveItem(ItemType.Key))
            return false;

        Type = DoorType.Open;
        return true;
    }

    public bool TryBomb(Rectangle blastBounds)
    {
        if (!IsBombable || !Bounds.Intersects(blastBounds)) return false;
        RestoreOpening();
        return true;
    }

    public void RestoreOpening() => Type = DoorType.Open;

    public static DoorType ParseType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return DoorType.Open;

        return type.Trim().ToLowerInvariant() switch
        {
            "open" => DoorType.Open,
            "locked" => DoorType.Locked,
            "bombable" => DoorType.Bombable,
            _ => DoorType.Open
        };
    }
}

