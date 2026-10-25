using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Blocks;
using Microsoft.Xna.Framework.Content;
using escape.Interfaces;
using System.Linq;
using escape.Projectiles;
using escape.Enemies;

namespace escape.Levels;

public class LevelManager : IGameResettable
{
    private readonly Texture2D _blockAtlas;
    private readonly string _levelsDirectory;
    private readonly ContentManager _content;
    private readonly Texture2D _itemAtlas;
    private readonly Player _player;
    private Vector2 _currentSpawn;
    private readonly Texture2D _exteriorTexture;
    private readonly ProjectileManager _projectiles;

    public Room CurrentRoom { get; private set; }

    public LevelManager(Texture2D blockAtlas, string levelsDirectory, ContentManager content,
                            Texture2D itemAtlas, Player player, Texture2D exteriorTexture, ProjectileManager projectiles)
    {
        _blockAtlas = blockAtlas;
        _levelsDirectory = levelsDirectory;
        _content = content;
        _itemAtlas = itemAtlas;
        _player = player;
        _exteriorTexture = exteriorTexture;
        _projectiles = projectiles;

        LoadRoom("Room1");
    }

    [System.Diagnostics.CodeAnalysis.MemberNotNull(nameof(CurrentRoom))]
    public void LoadRoom(string roomId)
    {
        string filePath = Path.Combine(_levelsDirectory, $"{roomId}.json");
        RoomData data = RoomParser.Load(filePath);

        CurrentRoom = new Room(data, _blockAtlas, _content, _itemAtlas, _exteriorTexture);

        foreach (Boss boss in CurrentRoom.Enemies.OfType<Boss>())
        {
            boss.FireRequested += position =>
                _projectiles.SpawnBossProjectile(position, new Vector2(1, 0));
        }

        _currentSpawn = new Vector2(data.SpawnX, data.SpawnY);
        _player.Position = _currentSpawn;
        _player.CancelAttack();
        _player.Obstacles = CurrentRoom.Blocks
            .Where(block => block.IsSolid)
            .Cast<ISolidCollider>()
            .Concat(CurrentRoom.ExteriorWalls)
            .ToList();
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
