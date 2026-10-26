using Microsoft.Xna.Framework;

namespace escape.Enemies;

public sealed class EnemyCombatState
{
    private const float FlashDuration = 0.4f;
    private const float FlashInterval = 0.08f;
    private readonly int _startingHealth;
    private float _flashTimer;

    public int Health { get; private set; }
    public bool IsDefeated => Health == 0;
    public bool IsVisible => !IsDefeated &&
        (_flashTimer <= 0f || (int)(_flashTimer / FlashInterval) % 2 != 0);

    public EnemyCombatState(int startingHealth)
    {
        if (startingHealth <= 0)
            throw new ArgumentOutOfRangeException(nameof(startingHealth));
        _startingHealth = startingHealth;
        Reset();
    }

    public void TakeDamage()
    {
        if (IsDefeated) return;
        Health--;
        _flashTimer = FlashDuration;
    }

    public void Update(GameTime gameTime) => _flashTimer = MathF.Max(
        0f, _flashTimer - (float)gameTime.ElapsedGameTime.TotalSeconds);

    public void Reset()
    {
        Health = _startingHealth;
        _flashTimer = 0f;
    }
}
