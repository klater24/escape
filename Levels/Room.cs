using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using escape.Blocks;
using escape.Enemies;
using escape.Interfaces;
using escape.Sprites;
using escape.Items;
using escape.Collisions;

namespace escape.Levels;

public class Room
{
    public string Name { get; }
    private readonly RoomExterior _exterior;
    private readonly RoomData _data;
    private readonly Texture2D _blockAtlas;
    private readonly ContentManager _content;
    private readonly List<Block> _blocks = new();
    private readonly List<IEnemy> _enemies = new();
    private readonly Texture2D _itemAtlas;
    private readonly List<IItem> _items = new();
    public IReadOnlyList<IItem> Items => _items;
    public IReadOnlyList<Block> Blocks => _blocks;
    public IReadOnlyList<IEnemy> Enemies => _enemies;
    private readonly List<RoomWallCollider> _exteriorWalls = new();
    public IReadOnlyList<RoomWallCollider> ExteriorWalls => _exteriorWalls;
    private readonly List<Block> _floorTiles = new();
    

    public Room(
        RoomData data,
        Texture2D blockAtlas,
        ContentManager content,
        Texture2D itemAtlas,
        Texture2D exteriorTexture)
    {
        Name = data.Name;
        _data = data;
        _blockAtlas = blockAtlas;
        _content = content;
        _itemAtlas = itemAtlas;

        _exterior = new RoomExterior(exteriorTexture, Vector2.Zero, 3f);

        CreateExteriorWalls();
        CreateFloor();
        CreateTileMap();
    }


    private void CreateBlock(string type, Vector2 position, float scale)
    {
        bool isSolid = type is "stone" or "brick" or "metal";

        _blocks.Add(new Block(
            _blockAtlas,
            type,
            position,
            scale,
            isSolid));
    }


private void CreateTileItem(string type, Vector2 position)
{
    IItem item = type switch
    {
        "Book" => new Book(
            ItemSprites.CreateBook(_itemAtlas, position, 2f),
            position),

        "Key" => new Key(
            ItemSprites.CreateKey(_itemAtlas, position, 2f),
            position),

        "Watch" => new Watch(
            ItemSprites.CreateWatch(_itemAtlas, position, 2f),
            position),

        "Heart" => new Heart(
            ItemSprites.CreateHeart(_itemAtlas, position, 2f),
            position),

        "Potion" => new Potion(
            ItemSprites.CreatePotion(_itemAtlas, position, 2f),
            position),

        _ => throw new ArgumentException(
            $"Unknown item type: {type}")
    };

    _items.Add(item);
}

private void CreateTileEnemy(string type, Vector2 position)
{
    string animation = type == "Boss" ? "SorcererAttack" : type;

    ISprite sprite = SpriteFactory.CreateEnemySprite(
        _content, animation, position);

    IEnemy enemy = type switch
    {
        "Run" => new EnemyA(sprite, position),
        "DemonFlying" => new EnemyB(sprite, position),
        "ManaSeed" => new EnemyC(sprite, position),
        "Boss" => new Boss(sprite, position),
        _ => throw new ArgumentException($"Unknown enemy type: {type}")
    };

    _enemies.Add(enemy);
}




private void CreateExteriorWalls()
{
    const int wallThickness = 96;

    const int leftWall = 96;
    const int rightWall = 672;
    const int topWall = 96;
    const int bottomWall = 432;

    const int doorGapStart = 336;
    const int doorGapSize = 96;
    const int sideGapStart = 216;
    const int sideGapSize = 96;

    // Top wall.
    _exteriorWalls.Add(new RoomWallCollider(
        new Rectangle(0, topWall - wallThickness,
            doorGapStart, wallThickness)));

    _exteriorWalls.Add(new RoomWallCollider(
        new Rectangle(doorGapStart + doorGapSize,
            topWall - wallThickness,
            768 - (doorGapStart + doorGapSize), wallThickness)));

    if (!HasDoor("Top"))
    {
        _exteriorWalls.Add(new RoomWallCollider(
            new Rectangle(doorGapStart, topWall - wallThickness,
                doorGapSize, wallThickness)));
    }

    // Bottom wall.
    _exteriorWalls.Add(new RoomWallCollider(
        new Rectangle(0, bottomWall,
            doorGapStart, wallThickness)));

    _exteriorWalls.Add(new RoomWallCollider(
        new Rectangle(doorGapStart + doorGapSize, bottomWall,
            768 - (doorGapStart + doorGapSize), wallThickness)));

    if (!HasDoor("Bottom"))
    {
        _exteriorWalls.Add(new RoomWallCollider(
            new Rectangle(doorGapStart, bottomWall,
                doorGapSize, wallThickness)));
    }

    // Left wall.
    _exteriorWalls.Add(new RoomWallCollider(
        new Rectangle(leftWall - wallThickness, 0,
            wallThickness, sideGapStart)));

    _exteriorWalls.Add(new RoomWallCollider(
        new Rectangle(leftWall - wallThickness,
            sideGapStart + sideGapSize,
            wallThickness, 528 - (sideGapStart + sideGapSize))));

    if (!HasDoor("Left"))
    {
        _exteriorWalls.Add(new RoomWallCollider(
            new Rectangle(leftWall - wallThickness, sideGapStart,
                wallThickness, sideGapSize)));
    }

    // Right wall.
    _exteriorWalls.Add(new RoomWallCollider(
        new Rectangle(rightWall, 0,
            wallThickness, sideGapStart)));

    _exteriorWalls.Add(new RoomWallCollider(
        new Rectangle(rightWall, sideGapStart + sideGapSize,
            wallThickness, 528 - (sideGapStart + sideGapSize))));

    if (!HasDoor("Right"))
    {
        _exteriorWalls.Add(new RoomWallCollider(
            new Rectangle(rightWall, sideGapStart,
                wallThickness, sideGapSize)));
    }
}


private void CreateFloor()
{
    const int columns = 12;
    const int rows = 7;
    const float tileSize = 48f;
    const float tileScale = 1.5f;

    const float startX = 96f;
    const float startY = 96f;

    for (int row = 0; row < rows; row++)
    {
        for (int column = 0; column < columns; column++)
        {
            Vector2 position = new(
                startX + column * tileSize,
                startY + row * tileSize);

            _floorTiles.Add(new Block(
                _blockAtlas,
                "stone",
                position,
                tileScale,
                false));
        }
    }
}


private void CreateTileMap()
{
    const float tileSize = 48f;
    const float tileScale = 1.5f;
    const float startX = 96f;
    const float startY = 96f;

    for (int row = 0; row < _data.TileMap.Count; row++)
    {
        string[] tiles = _data.TileMap[row]
            .Split(')', StringSplitOptions.RemoveEmptyEntries);

        for (int column = 0; column < tiles.Length; column++)
        {
            string tile = tiles[column].Trim('(');

            Vector2 position = new(
                startX + column * tileSize,
                startY + row * tileSize);

            switch (tile)
            {
                case "0":
                    break;

                case "B":
                    CreateBlock("brick", position, tileScale);
                    break;
                //enemies
                case "E":
                    CreateTileEnemy("Run", position);
                    break;

                case "F":
                    CreateTileEnemy("DemonFlying", position);
                    break;

                case "M":
                    CreateTileEnemy("ManaSeed", position);
                    break;

                case "X":
                    CreateTileEnemy("Boss", position);
                    break;

                //items
                case "K":
                    CreateTileItem("Key", position);
                    break;

                case "D":
                    CreateTileItem("Book", position);
                    break;

                case "W":
                    CreateTileItem("Watch", position);
                    break;

                case "H":
                    CreateTileItem("Heart", position);
                    break;

                case "P":
                    CreateTileItem("Potion", position);
                    break;

                default:
                    throw new ArgumentException(
                        $"Unknown tile '{tile}' at row {row}, column {column}.");
            }
        }
    }
}


private void CreateExterior()
{
    Vector2 topPosition = new(320, 0);
    Vector2 leftPosition = new(0, 320);
    Vector2 rightPosition = new(640, 320);
    Vector2 bottomPosition = new(320, 640);
}
private bool HasDoor(string direction)
{
    foreach (DoorData door in _data.Doors)
    {
        if (door.Direction.Equals(
            direction, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
    }

    return false;
}

    public void Update(GameTime gameTime)
    {
        foreach (Block block in _blocks)
            block.Update(gameTime);

        foreach (IEnemy enemy in _enemies)
            enemy.Update(gameTime);

        foreach (IItem item in _items)
            item.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _exterior.Draw(spriteBatch);

        bool hasUpDoor = HasDoor("Top");
        bool hasLeftDoor = HasDoor("Left");
        bool hasRightDoor = HasDoor("Right");
        bool hasDownDoor = HasDoor("Bottom");

        _exterior.DrawOpening(spriteBatch, _blockAtlas, hasUpDoor, "up");
        _exterior.DrawOpening(spriteBatch, _blockAtlas, hasLeftDoor, "left");
        _exterior.DrawOpening(spriteBatch, _blockAtlas, hasRightDoor, "right");
        _exterior.DrawOpening(spriteBatch, _blockAtlas, hasDownDoor, "down");

        foreach (Block tile in _floorTiles)
            tile.Draw(spriteBatch);

        foreach (Block block in _blocks)
            block.Draw(spriteBatch);

        foreach (IEnemy enemy in _enemies)
            enemy.Draw(spriteBatch);

        foreach (IItem item in _items)
            item.Draw(spriteBatch);
    }

    public void Reset()
    {
        foreach (Block block in _blocks)
            block.Reset();

        foreach (IEnemy enemy in _enemies)
            enemy.Reset();
    }
}
