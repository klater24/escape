using escape.Interfaces;
using escape.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Sprites;
using System.Security.Cryptography;
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
    private const float damageDuration = 0.72f;
    
        public Player(Vector2 intiPos, Texture2D spriteSheet)
        {
        facingDirection = Direction.Down;
        position = intiPos; 
        currentState = State.Idle;
        initialPosition = intiPos;

        //walk
        walkingUDS[0] = new AnimatedSprite(spriteSheet, Frames(0, 4, 5), intiPos);
        walkingUDS[1] = new AnimatedSprite(spriteSheet, Frames(0, 0, 1), intiPos);
        walkingUDS[2] = new AnimatedSprite(spriteSheet, Frames(0, 2, 3), intiPos);
        //attack
        attackUDS[0] = new AnimatedSprite(spriteSheet, Frames(6, 0, 3), intiPos);
        attackUDS[1] = new AnimatedSprite(spriteSheet, Frames(2, 0, 3), intiPos);
        attackUDS[2] = new AnimatedSprite(spriteSheet, Frames(4, 0, 3), intiPos);
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
            idleUDS[2].SetSpriteEffects(effects);
        }

        // Move the player while resolving collisions with obstacles
        EnvironmentCollisionResolver.Move(this, movement, Obstacles);
    }
    public Direction GetFacingDirection()
    {
        return facingDirection;
    }
    public void Attack()
    {
        if (currentState == State.Attacking)
            return;

        currentState = State.Attacking; 
        attackTimer = 0f;
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
        if (currentState == State.Damaged)
            return;
        currentState = State.Damaged;   
        damageTimer = 0f;
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
        UpdateSpritePositions();
    }

    public void Update(GameTime gameTime)
    {
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

