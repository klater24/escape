using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace escape.View;

public class Camera
{
    public Vector2 Position{get; private set;}
    public float Zoom{get; private set;}
    private Vector2 _startPosition;
    private Vector2 _targetPosition; 
    private float _startZoom;
    private float _targetZoom; 
    private float _duration; 
    private float _timePassed; 
    public bool _isTransition {get; private set;}
    public bool IsTransitioning => _isTransition;
    public bool isTransition => _isTransition;
    public Camera(Vector2 pos, float _zoom = 1f)
    {
        Position = pos;
        Zoom = MathHelper.Clamp(_zoom, 0.25f, 4f);

        _targetPosition = pos;
        _targetZoom = MathHelper.Clamp(_zoom, 0.25f, 4f);
    }

    public void Transition(Vector2 targetPos, float targetZoom, float duratation)
    {
        _startPosition = Position; 
        _startZoom = Zoom;
        _targetPosition= targetPos; 
        _targetZoom = MathHelper.Clamp(targetZoom, 0.25f, 4f); 
        _duration = Math.Max(0f, duratation); 
        _timePassed = 0f;
        if (_duration == 0f)
        {
            Position = targetPos;
            Zoom = _targetZoom;
            _isTransition = false;
            return;
        }
        _isTransition=true; 
    }
    public void Update(GameTime gameTime)
    {
        if (!_isTransition)
        {
            return;
        }
        float time = (float)gameTime.ElapsedGameTime.TotalSeconds; 

        _timePassed+= time; 
        float progress = MathHelper.Clamp(_timePassed/_duration, 0f, 1f);
        float smooth = MathHelper.SmoothStep(0f, 1f, progress);
         Position = Vector2.Lerp(_startPosition, _targetPosition, smooth);

            Zoom = MathHelper.Lerp(_startZoom, _targetZoom, smooth);

        if (progress >= 1f)
        {
            Position = _targetPosition;
            Zoom = _targetZoom;
            _isTransition = false;
        }
    }
        public void Follow(Vector2 target, GameTime gameTime)
    {
        if (_isTransition)
            return;

        float dt =(float)gameTime.ElapsedGameTime.TotalSeconds;

        float amount = 1f - MathF.Exp(-8f * dt);

        Position = Vector2.Lerp(Position,target,amount);
    }

    public Matrix GetViewMatrix(Viewport viewport)
    {
        Vector2 screenCenter = new Vector2(
            viewport.Width / 2f,
            viewport.Height / 2f);

        return Matrix.CreateTranslation( -Position.X, -Position.Y, 0f) * Matrix.CreateScale(Zoom, Zoom, 1f) * Matrix.CreateTranslation(screenCenter.X, screenCenter.Y, 0f);
    }

}
