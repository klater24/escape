using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using escape.Interfaces;
using escape.Sprites;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Input;
public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class Player
{ 

  
    private Direction facingDirection;
    private Vector2 position;
    private AnimatedSprite walking;
    private AnimatedSprite attack;
    private AnimatedSprite idle;
    private SpriteBatch _sprite;
     private Vector2 initialPosition;
    
        public Player(Vector2 intiPos, Texture2D idText, int idFr, Texture2D walk, int walkFr, Texture2D attack_, int attackFr, SpriteBatch _spr){
        facingDirection = Direction.Down;
        position = intiPos; 
        initialPosition = intiPos;
        walking = new AnimatedSprite(walk, new Rectangle[walkFr], intiPos);
        attack = new AnimatedSprite(attack_, new Rectangle[attackFr], intiPos);
        idle = new AnimatedSprite(idText, new Rectangle[idFr], intiPos);
        _sprite = _spr;
    }
    
    public Vector2 getPosit()
    {
        return position;
    }
    public void Move(Direction direct)
    {
        facingDirection = direct;
        if(direct == Direction.Up)
        {
            position.Y += 1;
        }
        else if (direct == Direction.Down)
        {
            position.Y -= 1;
        }
        else if (direct == Direction.Right)
        {
            position.X += 1;
        }
        else if (direct == Direction.Left)
        {
            position.X -= 1;
        }
        
    }
    public Direction GetFacingDirection()
    {
        return facingDirection;
    }
    public void Attack()
    {
        attack.Draw(_sprite);
    }
    public void Walk()
    {
        walking.Draw(_sprite);
    }
    public void Idle()
    {
        idle.Draw(_sprite);
    }
    public void Reset()
    {
        position = initialPosition;
        facingDirection = Direction.Down;
    }
}