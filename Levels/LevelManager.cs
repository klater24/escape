using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Blocks;
using escape.Collisions;
using Microsoft.Xna.Framework.Content;
using escape.Interfaces;
using System.Linq;

namespace escape.Levels;

public class LevelManager : IGameResettable
{
    public const string StartingRoom = "Room1";
    private readonly Texture2D _blockAtlas;
    private readonly string _levelsDirectory;
    private readonly ContentManager _content;
    private readonly Texture2D _itemAtlas;
    private readonly Player _player;
    private Vector2 _currentSpawn;
    private readonly Texture2D _exteriorTexture;
    private readonly IInventory _inventory;
    private readonly HashSet<string> _collectedItems = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _bombedPassages = new(StringComparer.OrdinalIgnoreCase);

    private static string PassageKey(string roomId, Door door)
    {
        string source = $"{roomId}:{door.Direction}";
        string destination = $"{door.NextRoom}:{door.NextDoor}";
        return StringComparer.OrdinalIgnoreCase.Compare(source, destination) < 0
            ? $"{source}|{destination}" : $"{destination}|{source}";
    }

    public void BombWalls(Rectangle blastBounds)
    {
        bool changed = false;
        foreach (Door door in CurrentRoom.Doors)
        {
            if (!door.TryBomb(blastBounds)) continue;
            _bombedPassages.Add(PassageKey(CurrentRoom.RoomId, door));
            changed = true;
        }
        if (changed) RefreshPlayerObstacles();
    }

    public Room CurrentRoom { get; private set; }
    public event Action? RoomChanged;
    public event Action<string>? RoomChanging;
    private readonly Action<int> _addRupees;

    private void RefreshPlayerObstacles()
    {
        _player.Obstacles = CurrentRoom.Blocks
            .Where(block => block.IsSolid)
            .Cast<ISolidCollider>()
            .Concat(CurrentRoom.ExteriorWalls)
            .Concat(CurrentRoom.Doors
                .Where(door => !door.CanTraverse(_inventory))
                .Select(door => (ISolidCollider)new RoomWallCollider(door.Bounds)))
            .ToList();
    }

    public LevelManager(Texture2D blockAtlas, string levelsDirectory, ContentManager content,
                            Texture2D itemAtlas, Player player, Texture2D exteriorTexture, IInventory inventory, Action<int> addRupees)
    {
        _blockAtlas = blockAtlas;
        _levelsDirectory = levelsDirectory;
        _content = content;
        _itemAtlas = itemAtlas;
        _player = player;
        _exteriorTexture = exteriorTexture;
        _inventory = inventory;
        _addRupees = addRupees;

        LoadRoom(StartingRoom);
    }

    [System.Diagnostics.CodeAnalysis.MemberNotNull(nameof(CurrentRoom))]
    public void LoadRoom(string roomId)
    {
        string filePath = Path.Combine(_levelsDirectory, $"{roomId}.json");
        RoomData data = RoomParser.Load(filePath);

        CurrentRoom = new Room(data, _blockAtlas, _content, _itemAtlas, _exteriorTexture, _player, _inventory, _addRupees, roomId, _collectedItems);
        foreach (Door door in CurrentRoom.Doors)
            if (door.IsBombable && _bombedPassages.Contains(PassageKey(roomId, door)))
                door.RestoreOpening();

        _currentSpawn = new Vector2(data.SpawnX, data.SpawnY);
        _player.Position = _currentSpawn;
        _player.CancelAttack();
        _player.SetSideScrolling(CurrentRoom.IsSideScrolling);
        RefreshPlayerObstacles();

        _currentSpawn = new Vector2(data.SpawnX, data.SpawnY);
        _player.Position = _currentSpawn;
        RoomChanged?.Invoke();
    }

    public void ChangeRoom(string roomId, Vector2 spawnPosition)
    {
        if (_player.IsDefeated) return;
        RoomChanging?.Invoke(roomId);
        LoadRoom(roomId);
        _currentSpawn = spawnPosition;
        _player.Position = _currentSpawn;
    }

    public Vector2 GetDoorSpawn(string roomId, string doorDirection)
    {
        string filePath = Path.Combine(
            _levelsDirectory,
            $"{roomId}.json");

        RoomData data = RoomParser.Load(filePath);

        foreach (DoorData door in data.Doors)
        {
            if (door.Direction.Equals(
                doorDirection,
                StringComparison.OrdinalIgnoreCase))
            {
                return new Vector2(door.SpawnX, door.SpawnY);
            }
        }

        throw new InvalidOperationException(
            $"Room '{roomId}' has no door in direction '{doorDirection}'.");
    }

    public Vector2 GetTransitionDirection(string roomId)
    {
        var data = RoomParser.Load(Path.Combine(_levelsDirectory, $"{roomId}.json"));
        int dx = data.MapX - CurrentRoom.MapPosition.X;
        int dy = data.MapY - CurrentRoom.MapPosition.Y;
        return dx != 0 ? new Vector2(Math.Sign(dx), 0) : new Vector2(0, Math.Sign(dy));
    }

    public bool TryTransition(Door door)
    {
        if (_player.IsDefeated)
        {
            return false;
        }

        if (!door.CanTraverse(_inventory)) return false;

        try
        {
            Vector2 spawnPosition = GetDoorSpawn(
                door.NextRoom,
                door.NextDoor);

            // Validate the destination before spending a key.
            if (!door.TryUnlock(_inventory)) return false;

            ChangeRoom(door.NextRoom, spawnPosition);
            return true;
        }
        catch (InvalidOperationException)
        {
            // The destination room or matching door is invalid.
            return false;
        }
    }


private bool _doorTransitioned;

public void CheckDoorTransitions()
{
    if (_player.IsDefeated) return;
    // Prevent another transition until the player leaves the doorway.
    if (_doorTransitioned)
    {
        foreach (Door door in CurrentRoom.Doors)
        {
            if (_player.Bounds.Intersects(door.Bounds))
            {
                return;
            }
        }

        _doorTransitioned = false;
    }

    foreach (Door door in CurrentRoom.Doors)
    {
        if (!_player.Bounds.Intersects(door.Bounds))
        {
            continue;
        }

        if (TryTransition(door))
        {
            _doorTransitioned = true;
        }

        return;
    }
}

    public void Update(GameTime gameTime)
    {
        CurrentRoom.Update(gameTime);
        // Pickups can change whether a locked doorway blocks the player.
        RefreshPlayerObstacles();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        CurrentRoom.Draw(spriteBatch);
    }

    public void Reset()
    {
        _doorTransitioned = false;
        _collectedItems.Clear();
        _bombedPassages.Clear();
        LoadRoom(StartingRoom);
    }
}
