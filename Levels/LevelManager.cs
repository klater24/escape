using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Blocks;
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

    public Room CurrentRoom { get; private set; }
    public event Action? RoomChanged;
    public event Action<string>? RoomChanging;
    private readonly Action<int> _addRupees;

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

        CurrentRoom = new Room(data, _blockAtlas, _content, _itemAtlas, _exteriorTexture, _player, _inventory, _addRupees);

        _currentSpawn = new Vector2(data.SpawnX, data.SpawnY);
        _player.Position = _currentSpawn;
        _player.CancelAttack();
        _player.Obstacles = CurrentRoom.Blocks
            .Where(block => block.IsSolid)
            .Cast<ISolidCollider>()
            .Concat(CurrentRoom.ExteriorWalls)
            .ToList();

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

    public Vector2 GetTransitionDirection(string roomId)
    {
        var data = RoomParser.Load(Path.Combine(_levelsDirectory, $"{roomId}.json"));
        int dx = data.MapX - CurrentRoom.MapPosition.X;
        int dy = data.MapY - CurrentRoom.MapPosition.Y;
        return dx != 0 ? new Vector2(Math.Sign(dx), 0) : new Vector2(0, Math.Sign(dy));
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


public bool TryTransition(Door door)
{
    if (_player.IsDefeated)
    {
        return false;
    }

    try
    {
        Vector2 spawnPosition = GetDoorSpawn(
            door.NextRoom,
            door.NextDoor);

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
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        CurrentRoom.Draw(spriteBatch);
    }

    public void Reset()
    {
        _doorTransitioned = false;
        LoadRoom(StartingRoom);
    }
}
