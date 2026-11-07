using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Camera
{
    public Vector2 Position{get; private set;}
    public float Zoom{get; private set;}
    private Vector2 _targetPosition; 
    private float _targetZoom; 
    private float _duration; 
    private float _timePassed; 
    private bool _isTransition;
    public bool isTransition => _isTransition;
    public Camera(Vector2 pos, float _zoom = 1f)
    {
        Position = pos;
        Zoom = _zoom;

        _targetPosition = pos;
        _targetZoom = _zoom;
    }

    public void Transition(Vector2 targetPos, float targetZoom, float duratation)
    {
        _targetPosition= targetPos; 
        _targetZoom = targetZoom; 
        _duration = duratation; 
        _timePassed = 0f;
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

    private Vector2 _startPosition;
    private float _startZoom;

    public Matrix GetViewMatrix(Viewport viewport)
    {
        Vector2 screenCenter = new Vector2(
            viewport.Width / 2f,
            viewport.Height / 2f);

        return Matrix.CreateTranslation( -Position.X, -Position.Y, 0f) * Matrix.CreateScale(Zoom, Zoom, 1f) * Matrix.CreateTranslation(screenCenter.X, screenCenter.Y, 0f);
    }

}
