using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Blocks;
using Microsoft.Xna.Framework.Content;

namespace escape.Levels;

public class LevelManager
{
    private readonly Texture2D _blockAtlas;
    private readonly string _levelsDirectory;
    private readonly ContentManager _content;
    private readonly Texture2D _itemAtlas;

    public Room CurrentRoom { get; private set; }

    public LevelManager(
        Texture2D blockAtlas,
        string levelsDirectory,
        ContentManager content,
        Texture2D itemAtlas)
    {
        _blockAtlas = blockAtlas;
        _levelsDirectory = levelsDirectory;
        _content = content;
        _itemAtlas = itemAtlas;

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
}
