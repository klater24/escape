// A TEST FILE FOR COLLISION DETECTION. NOT PART OF THE GAME.

// using System;
// using escape.Interfaces;
// using Microsoft.Xna.Framework;

// namespace escape.Collisions;

// public static class TestCollision
// {
//     public static void RunAll()
//     {
//         Check("Partial overlap",
//             new Rectangle(0, 0, 48, 48),
//             new Rectangle(40, 0, 48, 48), true);

//         Check("Containment",
//             new Rectangle(0, 0, 48, 48),
//             new Rectangle(10, 10, 8, 8), true);

//         Check("Horizontal separation",
//             new Rectangle(0, 0, 48, 48),
//             new Rectangle(60, 0, 48, 48), false);

//         Check("Vertical separation",
//             new Rectangle(0, 0, 48, 48),
//             new Rectangle(0, 60, 48, 48), false);

//         Check("Touching edges",
//             new Rectangle(0, 0, 48, 48),
//             new Rectangle(48, 0, 48, 48), false);

//         Check("Touching corners",
//             new Rectangle(0, 0, 48, 48),
//             new Rectangle(48, 48, 48, 48), false);

//         Check("Empty bounds",
//             new Rectangle(10, 10, 0, 48),
//             new Rectangle(0, 0, 48, 48), false);

//         var first = new TestCollider
//         {
//             Bounds = new Rectangle(0, 0, 48, 48)
//         };

//         var second = new TestCollider
//         {
//             Bounds = first.Bounds,
//             IsCollisionEnabled = false
//         };

//         Expect("Disabled second object",
//             AabbCollisionDetector.IsColliding(first, second), false);

//         Expect("Disabled first object",
//             AabbCollisionDetector.IsColliding(second, first), false);

//         Expect("Self-collision",
//             AabbCollisionDetector.IsColliding(first, first), false);
//     }

//     private static void Check(
//         string name, Rectangle a, Rectangle b, bool expected)
//     {
//         var first = new TestCollider { Bounds = a };
//         var second = new TestCollider { Bounds = b };

//         Expect(name,
//             AabbCollisionDetector.IsColliding(first, second),
//             expected);

//         Expect(name + " (reversed)",
//             AabbCollisionDetector.IsColliding(second, first),
//             expected);
//     }

//     private static void Expect(string name, bool actual, bool expected)
//     {
//         if (actual != expected)
//             throw new InvalidOperationException(
//                 $"{name}: expected {expected}, got {actual}.");
//     }

//     private sealed class TestCollider : ICollidable
//     {
//         public Rectangle Bounds { get; set; }
//         public Collision Category => Collision.Environment;
//         public bool IsCollisionEnabled { get; set; } = true;
//     }
// }