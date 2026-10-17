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

    public Room CurrentRoom { get; private set; }

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

    public void LoadRoom(string roomId)
    {
        string filePath = Path.Combine(
            _levelsDirectory,
            $"{roomId}.json");

        RoomData data = RoomParser.Load(filePath);
        CurrentRoom = new Room(data, _blockAtlas, _content, _itemAtlas);
    }

    public void ChangeRoom(string roomId, Vector2 spawnPosition)
    {
        LoadRoom(roomId);
        _player.Position = spawnPosition;
    }
    public void Update(GameTime gameTime)
    {
        CurrentRoom.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        CurrentRoom.Draw(spriteBatch);
    }

    public void ResetCurrentRoom()
    {
        CurrentRoom.Reset();
    }
    public void Reset()
    {
        ResetCurrentRoom();
    }
}
