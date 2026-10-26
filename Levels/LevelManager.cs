using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Blocks;
using Microsoft.Xna.Framework.Content;
using escape.Interfaces;

namespace escape.Levels;

public class LevelManager : IGameResettable
{
    private readonly Texture2D _blockAtlas;
    private readonly string _levelsDirectory;
    private readonly ContentManager _content;
    private readonly Texture2D _itemAtlas;
    private readonly Player _player;
    private Vector2 _currentSpawn;

    public Room CurrentRoom { get; private set; }
    public event Action? RoomChanged;

    public LevelManager(
        Texture2D blockAtlas,
        string levelsDirectory,
        ContentManager content,
        Texture2D itemAtlas,
        Player player)
    {
        _blockAtlas = blockAtlas;
        _levelsDirectory = levelsDirectory;
        _content = content;
        _itemAtlas = itemAtlas;
        _player = player;

        LoadRoom("Room1");
    }

    [System.Diagnostics.CodeAnalysis.MemberNotNull(nameof(CurrentRoom))]
    public void LoadRoom(string roomId)
    {
        string filePath = Path.Combine(_levelsDirectory, $"{roomId}.json");
        RoomData data = RoomParser.Load(filePath);

        CurrentRoom = new Room(data, _blockAtlas, _content, _itemAtlas);
        _player.CancelAttack();
        _player.Obstacles = CurrentRoom.Blocks
            .Where(block => block.IsSolid)
            .Cast<ISolidCollider>()
            .ToList();

        _currentSpawn = new Vector2(data.SpawnX, data.SpawnY);
        _player.Position = _currentSpawn;
        RoomChanged?.Invoke();
    }

    public void ChangeRoom(string roomId, Vector2 spawnPosition)
    {
        LoadRoom(roomId);
        _currentSpawn = spawnPosition;
        _player.Position = _currentSpawn;
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
        CurrentRoom.Reset();
        _player.Position = _currentSpawn;
    }
}
