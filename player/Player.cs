using escape.Interfaces;
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

public class Player : IGameResettable
{  
    private Direction facingDirection;
    private Vector2 position;
    //walking
    private AnimatedSprite walkingUp;
    private AnimatedSprite walkingDown;
    private AnimatedSprite walkingSide;
    //side
    private AnimatedSprite attackUp;
    private AnimatedSprite attackDown;
    private AnimatedSprite attackSide;
    //idle
    private StaticSprite idleDown;
    private StaticSprite idleUp;
    private StaticSprite idleSide;
    //damage
    private AnimatedSprite damaged;
    private Vector2 initialPosition;
    private State currentState;
    private float attackTimer;
    private float damageTimer;
    private const float AttackDuration = 0.48f;
    private const float damageDuration = 0.72f;
    private const float PlayerScale = 2f;
    private const float AnimationSpeed = 0.12f;
    
        public Player(Vector2 intiPos, Texture2D spriteSheet)
        {
        facingDirection = Direction.Down;
        position = intiPos; 
        currentState = State.Idle;
        initialPosition = intiPos;

        //walk
        walkingUp = new AnimatedSprite(spriteSheet, Frames(0, 4, 5), intiPos, AnimationSpeed, PlayerScale);
        walkingDown = new AnimatedSprite(spriteSheet, Frames(0, 0, 1), intiPos, AnimationSpeed, PlayerScale);
        walkingSide = new AnimatedSprite(spriteSheet, Frames(0, 2, 3), intiPos, AnimationSpeed, PlayerScale);
        //attack
        attackUp = new AnimatedSprite(spriteSheet, Frames(6, 0, 3), intiPos, AnimationSpeed, PlayerScale);
        attackDown = new AnimatedSprite(spriteSheet, Frames(2, 0, 3), intiPos, AnimationSpeed, PlayerScale);
        attackSide = new AnimatedSprite(spriteSheet, Frames(4, 0, 3), intiPos, AnimationSpeed, PlayerScale);
        //idle
        idleUp = new StaticSprite(spriteSheet, Frame(4, 0), intiPos, PlayerScale);
        idleDown = new StaticSprite(spriteSheet, Frame(0, 0), intiPos, PlayerScale);
        idleSide = new StaticSprite(spriteSheet, Frame(2, 0), intiPos, PlayerScale);
        //damage
        damaged = new AnimatedSprite(spriteSheet, Frames(8, 1, 3), intiPos,AnimationSpeed, PlayerScale);
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

        if(direct == Direction.Up)
        {
            position.Y -= 1;
        }
        else if (direct == Direction.Down)
        {
            position.Y += 1;
        }
        else if (direct == Direction.Right)
        {
            position.X += 1;

            walkingSide.SetSpriteEffects(SpriteEffects.None);
            attackSide.SetSpriteEffects(SpriteEffects.None);
            idleSide.SetSpriteEffects(SpriteEffects.None);
        }
        else if (direct == Direction.Left)
        {
            position.X -= 1;

            walkingSide.SetSpriteEffects(SpriteEffects.FlipHorizontally);
            attackSide.SetSpriteEffects(SpriteEffects.FlipHorizontally);
            idleSide.SetSpriteEffects(SpriteEffects.FlipHorizontally);
        }
        UpdateSpritePositions();
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
                walkingUp.Update(gameTime);
            }
            else if (facingDirection == Direction.Down)
            {
                walkingDown.Update(gameTime);
            }
            else
            {
                walkingSide.Update(gameTime);
            }
        }
        else if (currentState == State.Attacking)
        {
            attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (facingDirection == Direction.Up)
            {
                attackUp.Update(gameTime);
            }
            else if (facingDirection == Direction.Down)
            {
                attackDown.Update(gameTime);
            }
            else
            {
                attackSide.Update(gameTime);
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
        idleUp.Position = position;
        idleDown.Position = position;
        idleSide.Position = position;

        walkingUp.Position = position;
        walkingDown.Position = position;
        walkingSide.Position = position;

        attackUp.Position = position;
        attackDown.Position = position;
        attackSide.Position = position;

        damaged.Position = position;
    }
    public void Draw(SpriteBatch spriteBatch){
        if (currentState == State.Idle)
        {
            if (facingDirection == Direction.Up)
            {
                idleUp.Draw(spriteBatch);
            }
            else if (facingDirection == Direction.Down)
            {
                idleDown.Draw(spriteBatch);
            }
            else
            {
                idleSide.Draw(spriteBatch);
            }
        }
        else if (currentState == State.Walking)
        {
            if (facingDirection == Direction.Up)
            {
                walkingUp.Draw(spriteBatch);
            }
            else if (facingDirection == Direction.Down)
            {
                walkingDown.Draw(spriteBatch);
            }
            else
            {
                walkingSide.Draw(spriteBatch);
            }
        }
        else if (currentState == State.Attacking)
        {
            if (facingDirection == Direction.Up)
            {
                attackUp.Draw(spriteBatch);
            }
            else if (facingDirection == Direction.Down)
            {
                attackDown.Draw(spriteBatch);
            }
            else
            {
                attackSide.Draw(spriteBatch);
            }
        }
        else if (currentState == State.Damaged)
        {
            damaged.Draw(spriteBatch);
        }
    }
}
