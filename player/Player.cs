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
    public bool IsCollisionEnabled => true;
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
    private Vector2 position, initialPosition;
    //Index 0 for Up, Index 1 for down, Index 2 for side
    private AnimatedSprite[] attackUDS = new AnimatedSprite[3];
    private AnimatedSprite[] walkingUDS = new AnimatedSprite[3];
    private StaticSprite[] idleUDS = new StaticSprite[3];
    private AnimatedSprite damaged;
    private State currentState;
    private float attackTimer, damageTimer;
    private const float AttackDuration = 0.48f;
    private const float MovementSpeed = 1.5f;
    private const float damageDuration = 0.96f;
    private const float DamageCooldown = 5f;
    private float damageCooldownTimer;
    private const float SwordActiveStart = 0.12f;
    private const float SwordActiveEnd = 0.36f;
    public SwordHitbox Sword { get; }
    public bool IsSwordActive => currentState == State.Attacking &&
        attackTimer >= SwordActiveStart && attackTimer < SwordActiveEnd;
    
        public Player(Vector2 intiPos, Texture2D spriteSheet)
        {
        facingDirection = Direction.Down;
        position = intiPos; 
        currentState = State.Idle;
        initialPosition = intiPos;
        Sword = new SwordHitbox(this);

        Rectangle idleDownFrame = new Rectangle(1, 11, 16, 16); 
        Rectangle idleSideFrame = new Rectangle(35, 11, 16, 16);
        Rectangle idleUpFrame = new Rectangle(69, 11, 16, 16);  

        Rectangle[] walkUpFrame =
        {
            new Rectangle(69, 11, 16, 16), 
            new Rectangle(86, 11, 16, 16)  
        };
        Rectangle[] walkDownFrame =
        {
            new Rectangle(1, 11, 16, 16),   
            new Rectangle(18, 11, 16, 16)   
        };
        Rectangle[] walkSideFrame =
        {
            new Rectangle(35, 11, 16, 16), 
            new Rectangle(52, 11, 16, 16)   
        };
        Rectangle[] attackUpFrame =
        {
            new Rectangle(1, 109, 16, 16),   
            new Rectangle(18, 97, 16, 28),  
            new Rectangle(35, 98, 16, 27),  
            new Rectangle(52, 106, 16, 19)   
        };
        Rectangle[] attackDownFrame =
        {
            new Rectangle(1, 47, 16, 16),   
            new Rectangle(18, 47, 16, 27),  
            new Rectangle(35, 47, 16, 23),  
            new Rectangle(52, 47, 16, 19)   
            
        };
        Rectangle[] attackSideFrame =
        {
            new Rectangle(1, 77, 16, 16),   
            new Rectangle(18, 77, 27, 16),  
            new Rectangle(46, 77, 23, 16),  
            new Rectangle(70, 77, 19, 16)   
        };
        //walk
        Rectangle[] damagedFrame =
        {
            new Rectangle(1, 232, 16, 16), 
            new Rectangle(109, 241, 16, 16),  
            new Rectangle(200, 241, 16, 16),
            new Rectangle(223, 241, 16, 16),
            new Rectangle(109, 241, 16, 16),  
            new Rectangle(200, 241, 16, 16),
            new Rectangle(223, 241, 16, 16),
            new Rectangle(1, 232, 16, 16)     
        };

        //walk
        walkingUDS[0] = new AnimatedSprite(spriteSheet, walkUpFrame, intiPos);
        walkingUDS[1] = new AnimatedSprite(spriteSheet, walkDownFrame, intiPos);
        walkingUDS[2] = new AnimatedSprite(spriteSheet, walkSideFrame, intiPos);
        //attack
        attackUDS[0] = new AnimatedSprite(spriteSheet, attackUpFrame, intiPos);
        attackUDS[1] = new AnimatedSprite(spriteSheet, attackDownFrame, intiPos);
        attackUDS[2] = new AnimatedSprite(spriteSheet, attackSideFrame, intiPos);
        foreach (var sprite in attackUDS)
            sprite.AnchorBodySize = new Vector2(16, 16);
        attackUDS[0].FrameAnchor = new Vector2(0, 1);
        //idle
        idleUDS[0] = new StaticSprite(spriteSheet, idleUpFrame, intiPos);
        idleUDS[1] = new StaticSprite(spriteSheet, idleDownFrame, intiPos);
        idleUDS[2] = new StaticSprite(spriteSheet, idleSideFrame, intiPos);
        //damage
        damaged = new AnimatedSprite(spriteSheet, damagedFrame, intiPos);
    }
    
    public Vector2 getPosit()
    {
        return position;
    }
    public void Move(Direction direct)
    {
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
        currentState = State.Walking; 
    }
    public void Idle()
    {
        currentState = State.Idle;
    }
    public void Damage()
    {
        if (damageCooldownTimer > 0f)
            return;
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

    public void Update(GameTime gameTime)
    {
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

