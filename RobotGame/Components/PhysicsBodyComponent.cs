using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RobotGame.Components
{
    public struct PhysicsBodyComponent
    {
        public Vector2 Size;
        public Vector2 Bounce;
        public int MoverMask, ColliderMask;
        public List<PhysicsBodyCollision> Collisions;

        public PhysicsBodyComponent()
        {
            MoverMask = 1;
            ColliderMask = 1;

            Collisions = new List<PhysicsBodyCollision>();
        }
    }
}
