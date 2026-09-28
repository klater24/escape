
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
    Attacking
}

public class Player
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
    private Vector2 initialPosition;
    private State currentState;
    
        public Player(Vector2 intiPos, Texture2D spriteSheet)
        {
        facingDirection = Direction.Down;
        position = intiPos; 
        currentState = State.Idle;
        initialPosition = intiPos;

        //idle
        Rectangle idleDownFrame = new Rectangle(0, 11, 16, 16);  // frame 1
        Rectangle idleSideFrame = new Rectangle(32, 11, 16, 16); // frame 3
        Rectangle idleUpFrame = new Rectangle(64, 11, 16, 16);   

        //walk
        Rectangle[] walkUpFrame =
        {
            new Rectangle(64, 11, 16, 16),  // 5
            new Rectangle(80, 11, 16, 16)   // 6
        };
        Rectangle[] walkDownFrame =
        {
            new Rectangle(0, 11, 16, 16),   
            new Rectangle(16, 11, 16, 16)   
        };
        Rectangle[] walkSideFrame =
        {
            new Rectangle(32, 11, 16, 16),  // 3
            new Rectangle(48, 11, 16, 16)   // 4
        };
        //attack
        Rectangle[] attackUpFrame =
        {
            new Rectangle(0, 59, 16, 16),   
            new Rectangle(16, 59, 16, 16),  
            new Rectangle(32, 59, 16, 16),  
            new Rectangle(48, 59, 16, 16)   
        };
        Rectangle[] attackDownFrame =
        {
            new Rectangle(0, 27, 16, 16),   
            new Rectangle(16, 27, 16, 16),  
            new Rectangle(32, 27, 16, 16),  
            new Rectangle(48, 27, 16, 16)   
            
        };
        Rectangle[] attackSideFrame =
        {
            new Rectangle(0, 43, 16, 16),   
            new Rectangle(16, 43, 16, 16),  
            new Rectangle(32, 43, 16, 16),  
            new Rectangle(48, 43, 16, 16)   
        };

        //walk
        walkingUp = new AnimatedSprite(spriteSheet, walkUpFrame, intiPos);
        walkingDown = new AnimatedSprite(spriteSheet, walkDownFrame, intiPos);
        walkingSide = new AnimatedSprite(spriteSheet, walkSideFrame, intiPos);
        //attack
        attackUp = new AnimatedSprite(spriteSheet, attackUpFrame, intiPos);
        attackDown = new AnimatedSprite(spriteSheet, attackDownFrame, intiPos);
        attackSide = new AnimatedSprite(spriteSheet, attackSideFrame, intiPos);
        //idle
        idleUp = new StaticSprite(spriteSheet, idleUpFrame, intiPos);
        idleDown = new StaticSprite(spriteSheet, idleDownFrame, intiPos);
        idleSide = new StaticSprite(spriteSheet, idleSideFrame, intiPos);
    }
    
    public Vector2 getPosit()
    {
        return position;
    }
    public void Move(Direction direct)
    {
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
        }
        else if (direct == Direction.Left)
        {
            position.X -= 1;
        }
        UpdateSpritePositions();
    }
    public Direction GetFacingDirection()
    {
        return facingDirection;
    }
    public void Attack()
    {
        currentState = State.Attacking; 
    }
    public void Walk()
    {
        currentState = State.Walking; 
    }
    public void Idle()
    {
        currentState = State.Idle;
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
    }
    public void Draw(SpriteBatch spriteBatch){
       if(currentState == State.Idle)
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
       else if(currentState == State.Walking)
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
       else if(currentState == State.Attacking)
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
    }
}
