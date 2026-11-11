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
    public DoorType Type { get; private set; }
    public bool IsLocked => Type == DoorType.Locked;

    public Door(Rectangle bounds, string nextDoor, string nextRoom, string id = "", string type = "Open")
    {
        Bounds = bounds;
        NextDoor = nextDoor;
        NextRoom = nextRoom;
        Id = id;
        Type = ParseType(type);
    }

    public bool CanTraverse(IInventory inventory)
    {
        if (Type != DoorType.Locked)
            return true;

        return inventory.HasItem(ItemType.Key);
    }

    public bool TryUnlock(IInventory inventory)
    {
        if (Type != DoorType.Locked)
            return true;

        if (!inventory.RemoveItem(ItemType.Key))
            return false;

        Type = DoorType.Open;
        return true;
    }

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

