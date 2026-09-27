# game_prototype
Game Design class CSE 3902

See [DESIGN.md](DESIGN.md) for the sprite, block, texture, and reset conventions

## Texture and Sprite Sheet Rules

Put PNG art under `Content/Textures` and group it by object type

```text
Content/Textures/
	Blocks/
	Player/
	Enemies/
	Items/
	Projectiles/
```

The project copies PNG files from this folder beside the game when it builds. Load them with `TextureLoader.Load` and give it a path starting at `Content`, using forward slashes

```csharp
var texture = TextureLoader.Load(
		GraphicsDevice,
		"Content/Textures/Blocks/PixelPack_Block_Atlas.png");
```

Keep each sprite sheet on a regular grid. The current block atlas uses 32 by 32 pixel cells arranged left to right in this order: grass, dirt, stone, brick, water, wood, sand, ice, metal, platform. `SpriteFactory.CreateBlockSprite` maps each name to its cell

For other sheets, give `SpriteFactory` the exact source rectangle for each sprite. Animated sprites take an ordered list of rectangles, so frames can share a row or come from different rows

```csharp
var frames = new[]
{
		new Rectangle(0, 0, 32, 32),
		new Rectangle(32, 0, 32, 32)
};

var sprite = SpriteFactory.CreateAnimatedSprite(texture, frames, position);
```

Use the same cell size and frame order consistently within a sheet.
