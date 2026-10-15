using System;
using System.Collections.Generic;
using escape.Interfaces;
using Microsoft.Xna.Framework;

namespace escape.Collisions;

// Moves a body with a bounding box (AABB) while preventing overlap with solid objects
public static class EnvironmentCollisionResolver
{
    public static void Move(
        IMoveCollider body,
        Vector2 movement,
        IReadOnlyList<ISolidCollider> obstacles
    )
    {
        ArgumentNullException.ThrowIfNull(body);
        ArgumentNullException.ThrowIfNull(obstacles);

        if (!float.IsFinite(movement.X) || !float.IsFinite(movement.Y))
            throw new ArgumentException("Movement vector must be finite", nameof(movement));

        if (movement == Vector2.Zero)
            return;

        // Limit step to one pixel per axis to avoid skipping walls
        float pixelMovement = Math.Max(Math.Abs(movement.X), Math.Abs(movement.Y));
        // Calculate the number of steps needed to move the body without skipping any pixels
        int steps = checked((int)Math.Ceiling(pixelMovement));
        // Calculate the movement per step
        Vector2 step = movement / steps;

        // Move the body in small steps, checking for collisions at each step
        for (int i = 0; i < steps; i++)
        {
            MoveAlongAxis(body, step.X, true, obstacles);
            MoveAlongAxis(body, step.Y, false, obstacles);
        }
    }

    private static void MoveAlongAxis(
        IMoveCollider body,
        float movement,
        bool horizontal,
        IReadOnlyList<ISolidCollider> obstacles
    )
    {
        // If there is no movement along this axis, return early
        if (movement == 0)
            return;
        
        // Calculate the next position of the body after applying the movement
        Vector2 nextPosition = body.Position;

        // Move the body along the specified axis by the movement amount
        if (horizontal)
            nextPosition.X += movement;
        else
            nextPosition.Y += movement;

        // Update the body's position to the next position before checking for collisions
        body.Position = nextPosition;

        // Keep the attempted bounds fixed while examining every obstacle
        Rectangle attemptedBounds = body.Bounds;
        float correction = 0f;

        // Check for collisions with each obstacle and calculate the required correction
        foreach (ISolidCollider obstacle in obstacles)
        {
            // Skip non-solid obstacles and those that do not collide with the body
            if (!obstacle.IsSolid ||
                !AABBCollisionDetector.IsColliding(body, obstacle))
            {
                continue;
            }

            Rectangle wall = obstacle.Bounds;
            float requiredCorrection;

            // Determine the required correction to resolve the collision based on the movement direction and axis
            if (horizontal)
            {
                requiredCorrection = movement > 0f
                    ? wall.Left - attemptedBounds.Right
                    : wall.Right - attemptedBounds.Left;
            }
            else
            {
                requiredCorrection = movement > 0f
                    ? wall.Top - attemptedBounds.Bottom
                    : wall.Bottom - attemptedBounds.Top;
            }

            // Choose the strongest blocking correction, regardless of the order in which obstacles appear in the collection
            correction = movement > 0f
                ? MathF.Min(correction, requiredCorrection)
                : MathF.Max(correction, requiredCorrection);
        }

        // If no correction is needed, return early
        if (correction == 0f)
            return;

        // Translate the position by the overlap amount
        // Also supports body hitboxes that are offset from the object's position.
        if (horizontal)
            nextPosition.X += correction;
        else
            nextPosition.Y += correction;

        body.Position = nextPosition;
    }
}