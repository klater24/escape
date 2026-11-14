using escape.Collisions;
using escape.Interfaces;
using Microsoft.Xna.Framework;

namespace escape;

public sealed class SideScrollingMovement
{
    private const float WalkSpeed = 150f;
    private const float JumpSpeed = 420f;
    private const float Gravity = 1000f;
    private const float MaximumFallSpeed = 500f;
    private const double MaximumStepSeconds = 1.0 / 120.0;
    private float _horizontalInput;
    private float _verticalSpeed;

    public void SetHorizontalInput(float direction) => _horizontalInput = Math.Clamp(direction, -1f, 1f);

    private static bool IsSupported(IMoveCollider body, IReadOnlyList<ISolidCollider> obstacles)
    {
        Rectangle feet = new(body.Bounds.Left, body.Bounds.Bottom, body.Bounds.Width, 1);
        return obstacles.Any(wall => wall.IsSolid && wall.IsCollisionEnabled && feet.Intersects(wall.Bounds));
    }

    public bool TryJump(IMoveCollider body, IReadOnlyList<ISolidCollider> obstacles)
    {
        // Check the floor here so walking off a ledge never leaves a spare jump in the air
        if (_verticalSpeed < 0f || !IsSupported(body, obstacles)) return false;
        _verticalSpeed = -JumpSpeed;
        return true;
    }

    public void Update(IMoveCollider body, IReadOnlyList<ISolidCollider> obstacles, GameTime gameTime)
    {
        double remaining = gameTime.ElapsedGameTime.TotalSeconds;
        while (remaining > 0)
        {
            float step = (float)Math.Min(remaining, MaximumStepSeconds);
            EnvironmentCollisionResolver.Move(body, new Vector2(_horizontalInput * WalkSpeed * step, 0), obstacles);
            _verticalSpeed = Math.Min(_verticalSpeed + Gravity * step, MaximumFallSpeed);
            float expectedY = body.Position.Y + _verticalSpeed * step;
            EnvironmentCollisionResolver.Move(body, new Vector2(0, _verticalSpeed * step), obstacles);

            // A floor or ceiling stops vertical speed so the next frame starts cleanly
            if (Math.Abs(body.Position.Y - expectedY) > 0.001f) _verticalSpeed = 0;
            remaining -= step;
        }

        // Input is supplied again each frame so releasing a key stops horizontal movement
        _horizontalInput = 0;
    }

    public void Reset()
    {
        _horizontalInput = 0;
        _verticalSpeed = 0;
    }
}
