using escape.Interfaces;
using escape.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Sprites;
public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
public enum State
{
    Idle,
    Walking,
    Attacking,
    Damaged
}

public class Player : IGameResettable, IMoveCollider
{
    public Rectangle Bounds => new Rectangle((int)position.X, (int)position.Y, 48, 48);
    public Collision Category => Collision.Player;
    public const int MaxHealth = 3;
    public int Health { get; private set; } = MaxHealth;
    public bool IsDefeated => Health == 0;
    public bool IsCollisionEnabled => !IsDefeated;
    public Vector2 Position
    {
        get => position;
        set
        {
            position = value;
            UpdateSpritePositions();
        }
    }
    // The obstacles that the player can collide with
    public IReadOnlyList<ISolidCollider> Obstacles { get; set; }
        = Array.Empty<ISolidCollider>();
    private Direction facingDirection;
    public Vector2 position, initialPosition;
    //Index 0 for Up, Index 1 for down, Index 2 for side
    private AnimatedSprite[] attackUDS = new AnimatedSprite[3];
    private AnimatedSprite[] walkingUDS = new AnimatedSprite[3];
    private StaticSprite[] idleUDS = new StaticSprite[3];
    private AnimatedSprite damaged;
    private State currentState;
    private float attackTimer, damageTimer;
    private const float AttackDuration = 0.48f;
    private const float MovementSpeed = 1.5f;
    private const float damageDuration = 0.72f;
    private const float DamageCooldown = 5f;
    private float damageCooldownTimer;
    private const float SwordActiveStart = 0.12f;
    private const float SwordActiveEnd = 0.36f;
    public SwordHitbox Sword { get; }
    public bool IsAttacking => !IsDefeated && currentState == State.Attacking;
    public bool IsSwordActive => IsAttacking &&
        attackTimer >= SwordActiveStart && attackTimer < SwordActiveEnd;

        public Player(Vector2 intiPos, Texture2D spriteSheet, Texture2D attackSheet)
        {
        facingDirection = Direction.Down;
        position = intiPos;
        currentState = State.Idle;
        initialPosition = intiPos;
        Sword = new SwordHitbox(this);

        //walk
        walkingUDS[0] = new AnimatedSprite(spriteSheet, Frames(0, 4, 5), intiPos);
        walkingUDS[1] = new AnimatedSprite(spriteSheet, Frames(0, 0, 1), intiPos);
        walkingUDS[2] = new AnimatedSprite(spriteSheet, Frames(0, 2, 3), intiPos);
        Texture2D swordSheet = attackSheet;
        Rectangle[] attackUpFrame = { new(1, 109, 16, 16), new(18, 97, 16, 28), new(35, 98, 16, 27), new(52, 106, 16, 19) };
        Rectangle[] attackDownFrame = { new(1, 47, 16, 16), new(18, 47, 16, 27), new(35, 47, 16, 23), new(52, 47, 16, 19) };
        Rectangle[] attackSideFrame = { new(1, 77, 16, 16), new(18, 77, 27, 16), new(46, 77, 23, 16), new(70, 77, 19, 16) };
        //attack
        attackUDS[0] = new AnimatedSprite(swordSheet, attackUpFrame, intiPos);
        attackUDS[1] = new AnimatedSprite(swordSheet, attackDownFrame, intiPos);
        attackUDS[2] = new AnimatedSprite(swordSheet, attackSideFrame, intiPos);
        foreach (var sprite in attackUDS)
            sprite.AnchorBodySize = new Vector2(16, 16);
        attackUDS[0].FrameAnchor = new Vector2(0, 1);
        //idle
        idleUDS[0] = new StaticSprite(spriteSheet, Frame(4, 0), intiPos);
        idleUDS[1] = new StaticSprite(spriteSheet, Frame(0, 0), intiPos);
        idleUDS[2] = new StaticSprite(spriteSheet, Frame(2, 0), intiPos);
        //damage
        damaged = new AnimatedSprite(spriteSheet, Frames(8, 1, 3), intiPos);
    }

    private Rectangle Frame(int column, int row)
    {
        return new Rectangle(1 + column * 17, 1 + row * 17, 16, 16);
    }

    private Rectangle[] Frames(int row, int startColumn, int endColumn)
    {
        Rectangle[] frames = new Rectangle[endColumn - startColumn + 1];

        for (int x = startColumn; x <= endColumn; x++)
        {
            frames[x - startColumn] = Frame(x, row);
        }

        return frames;
    }

    public Vector2 getPosit()
    {
        return position;
    }
    public void Move(Direction direct)
    {
        if (IsDefeated) return;
        if (currentState == State.Attacking || currentState == State.Damaged)
        {
            return;
        }
        currentState = State.Walking;
        facingDirection = direct;

        // Now movement is based on a requested direction
        Vector2 movement = direct switch
        {
            Direction.Up => new Vector2(0, -1),
            Direction.Down => new Vector2(0, 1),
            Direction.Left => new Vector2(-1, 0),
            Direction.Right => new Vector2(1, 0),
            _ => Vector2.Zero
        };

        // Directional movement speed
        if (direct == Direction.Left || direct == Direction.Right)
        {
            // Adjust the movement speed for horizontal movement
            SpriteEffects effects = direct == Direction.Left
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;

            walkingUDS[2].SetSpriteEffects(effects);
            attackUDS[2].SetSpriteEffects(effects);
            attackUDS[2].FrameAnchor = direct == Direction.Left ? Vector2.UnitX : Vector2.Zero;
            idleUDS[2].SetSpriteEffects(effects);
        }

        // Move the player while resolving collisions with obstacles
        EnvironmentCollisionResolver.Move(this, movement * MovementSpeed, Obstacles);
    }
    public Direction GetFacingDirection()
    {
        return facingDirection;
    }
    public void Attack()
    {
        if (IsDefeated) return;
        if (currentState == State.Attacking || currentState == State.Damaged)
            return;

        currentState = State.Attacking;
        attackTimer = 0f;
        Sword.Reset();
        foreach (var sprite in attackUDS)
            sprite.Reset();
        UpdateSpritePositions();
    }
    public void Walk()
    {
        if (IsDefeated) return;
        currentState = State.Walking;
    }
    public void Idle()
    {
        if (IsDefeated) return;
        currentState = State.Idle;
    }
    public void Damage()
    {
        if (IsDefeated || damageCooldownTimer > 0f)
            return;
        Health--;
        CancelAttack();
        damageCooldownTimer = DamageCooldown;
        currentState = State.Damaged;
        damageTimer = 0f;
        damaged.Reset();
        damaged.Position = position;
    }
    public void StopMoving()
    {
        if (currentState == State.Walking)
        {
            currentState = State.Idle;
        }
    }
    public void Reset()
    {
        Health = MaxHealth;
        position = initialPosition;
        facingDirection = Direction.Down;
        currentState = State.Idle;
        damageCooldownTimer = 0f;
        damageTimer = 0f;
        attackTimer = 0f;
        Sword.Reset();
        foreach (var sprite in attackUDS)
        {
            sprite.Reset();
            sprite.SetSpriteEffects(SpriteEffects.None);
        }
        foreach (var sprite in walkingUDS)
        {
            sprite.Reset();
            sprite.SetSpriteEffects(SpriteEffects.None);
        }
        damaged.Reset();
        attackUDS[2].FrameAnchor = Vector2.Zero;
        UpdateSpritePositions();
    }

    public void CancelAttack()
    {
        attackTimer = 0f;
        Sword.Reset();
        if (currentState == State.Attacking)
            currentState = State.Idle;
    }

    public void Update(GameTime gameTime)
    {
        if (IsDefeated) return;
        damageCooldownTimer = MathF.Max(
            0f,
            damageCooldownTimer - (float)gameTime.ElapsedGameTime.TotalSeconds
        );

        if (currentState == State.Walking)
        {
            if (facingDirection == Direction.Up)
            {
                walkingUDS[0].Update(gameTime);
            }
            else if (facingDirection == Direction.Down)
            {
                walkingUDS[1].Update(gameTime);
            }
            else
            {
                walkingUDS[2].Update(gameTime);
            }
        }
        else if (currentState == State.Attacking)
        {
            attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (facingDirection == Direction.Up)
            {
                attackUDS[0].Update(gameTime);
            }
            else if (facingDirection == Direction.Down)
            {
                attackUDS[1].Update(gameTime);
            }
            else
            {
                attackUDS[2].Update(gameTime);
            }

            if (attackTimer >= AttackDuration)
            {
                attackTimer = 0f;
                currentState = State.Idle;
            }
        }
        else if (currentState == State.Damaged)
        {
            damageTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            damaged.Update(gameTime);

            if (damageTimer >= damageDuration)
            {
                damageTimer = 0f;
                currentState = State.Idle;
            }
        }
    }

    private void UpdateSpritePositions()
    {
        idleUDS[0].Position = position;
        idleUDS[1].Position = position;
        idleUDS[2].Position = position;

        walkingUDS[0].Position = position;
        walkingUDS[1].Position = position;
        walkingUDS[2].Position = position;

        attackUDS[0].Position = position;
        attackUDS[1].Position = position;
        attackUDS[2].Position = position;

        damaged.Position = position;
    }
    public void Draw(SpriteBatch spriteBatch){
       if(currentState == State.Idle)
       {
           if (facingDirection == Direction.Up)
           {
                idleUDS[0].Draw(spriteBatch);
           }

            else if (facingDirection == Direction.Down)
            {
                idleUDS[1].Draw(spriteBatch);
            }
            else
            {
                idleUDS[2].Draw(spriteBatch);
            }
       }
       else if(currentState == State.Walking)
       {
           if (facingDirection == Direction.Up)
           {
                walkingUDS[0].Draw(spriteBatch);
           }
            else if (facingDirection == Direction.Down)
            {
                walkingUDS[1].Draw(spriteBatch);
            }
            else
            {
                walkingUDS[2].Draw(spriteBatch);
            }
       }
       else if(currentState == State.Attacking)
       {
           if (facingDirection == Direction.Up)
           {
                attackUDS[0].Draw(spriteBatch);
           }
            else if (facingDirection == Direction.Down)
            {
                attackUDS[1].Draw(spriteBatch);
            }
            else
            {
                attackUDS[2].Draw(spriteBatch);
            }
        }
        else if (currentState == State.Damaged)
        {
            damaged.Draw(spriteBatch);
        }
    }
}
