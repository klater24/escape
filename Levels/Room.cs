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

namespace escape.Levels;

public class Room
{
    public string Name { get; }

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

    public Room(
        RoomData data,
        Texture2D blockAtlas,
        ContentManager content,
        Texture2D itemAtlas)
    {
        Name = data.Name;
        _data = data;
        _blockAtlas = blockAtlas;
        _content = content;
        _itemAtlas = itemAtlas;

        CreateBlocks();
        CreateEnemies();
        CreateItems();
    }

    private void CreateBlocks()
    {
        foreach (GameObjectData blockData in _data.Blocks)
        {
            Vector2 position = new(blockData.X, blockData.Y);

            bool isSolid = blockData.Type is "stone" or "brick" or "metal";

            _blocks.Add(new Block(
                _blockAtlas, blockData.Type, position, 2f, isSolid));
        }
    }

    private void CreateEnemies()
    {
        foreach (GameObjectData enemyData in _data.Enemies)
        {
            Vector2 position = new(enemyData.X, enemyData.Y);

            ISprite sprite = SpriteFactory.CreateEnemySprite(
                _content,
                enemyData.Type,
                position);

            IEnemy enemy = enemyData.Type switch
            {
                "Run" => new EnemyA(sprite, position),
                "DemonFlying" => new EnemyB(sprite, position),
                "ManaSeed" => new EnemyC(sprite, position),
                "SorcererAttack" => new Boss(sprite, position),
                _ => throw new ArgumentException(
                    $"Unknown enemy type: {enemyData.Type}")
            };

            _enemies.Add(enemy);
        }
    }


private void CreateItems()
{
    foreach (GameObjectData itemData in _data.Items)
    {
        Vector2 position = new(itemData.X, itemData.Y);

        IItem item = itemData.Type switch
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
                $"Unknown item type: {itemData.Type}")
        };

        _items.Add(item);
    }
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
