using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using RobotGame.Components;
using System;
using System.Collections.Generic;

namespace RobotGame.Systems
{
    public class PhysicsBodySystem : ISystem
    {
        public QueryDescription MoverQuery, ColliderQuery;

        public PhysicsBodySystem()
        {
            MoverQuery = new QueryDescription().WithAll<PhysicsBodyComponent, PositionComponent, VelocityComponent>();
            ColliderQuery = new QueryDescription().WithAll<PhysicsBodyComponent, PositionComponent>();
        }

        // Gets the rect of a body 
        public static GameRect GetRect(PhysicsBodyComponent body, PositionComponent position)
        {
            return new GameRect(position.Position, body.Size);
        }

        // Returns whether or not two entities have matching layers
        public static bool BodyLayersMatch(
            ref PhysicsBodyComponent moverBody,
            ref PhysicsBodyComponent collderBody)
        {
            return (moverBody.MoverMask & collderBody.ColliderMask) != 0;
        }

        // Gets a collision along the X axis
        public static PhysicsBodyCollision? GetCollisionX(
            ref PhysicsBodyComponent moverBody,
            ref PositionComponent moverPosition,
            ref VelocityComponent moverVelocity,
            Entity collider,
            float endPosition)
        {
            ref PhysicsBodyComponent colliderBody = ref collider.Get<PhysicsBodyComponent>();

            if (!BodyLayersMatch(ref moverBody, ref colliderBody))
            {
                return null;
            }

            ref PositionComponent colliderPosition = ref collider.Get<PositionComponent>();

            // Check vertical alignment
            if (moverPosition.Position.Y + moverBody.Size.Y <= colliderPosition.Position.Y ||
                moverPosition.Position.Y >= colliderPosition.Position.Y + colliderBody.Size.Y)
            {
                return null;
            }

            if (moverVelocity.Velocity.X < 0.0f)
            {
                // Left
                if (moverPosition.Position.X + moverBody.Size.X <= colliderPosition.Position.X)
                {
                    return null;
                }

                float wall = colliderPosition.Position.X + colliderBody.Size.X;

                if (endPosition < wall)
                {
                    return new PhysicsBodyCollision
                    {
                        Position = new Vector2(wall, moverPosition.Position.Y),
                        Normal = new Vector2(1.0f, 0.0f)
                    };
                }
            }
            else
            {
                // Right
                if (moverPosition.Position.X >= colliderPosition.Position.X + colliderBody.Size.X)
                {
                    return null;
                }

                float wall = colliderPosition.Position.X - moverBody.Size.X;

                if (endPosition > wall)
                {
                    return new PhysicsBodyCollision
                    {
                        Position = new Vector2(wall, moverPosition.Position.Y),
                        Normal = new Vector2(-1.0f, 0.0f)
                    };
                }
            }

            // No collision
            return null;
        }

        // Gets a collision along the Y axis
        public static PhysicsBodyCollision? GetCollisionY(
            ref PhysicsBodyComponent moverBody,
            ref PositionComponent moverPosition,
            ref VelocityComponent moverVelocity,
            Entity collider,
            float endPosition)
        {
            ref PhysicsBodyComponent colliderBody = ref collider.Get<PhysicsBodyComponent>();

            if (!BodyLayersMatch(ref moverBody, ref colliderBody))
            {
                return null;
            }

            ref PositionComponent colliderPosition = ref collider.Get<PositionComponent>();

            // Check horizontal alignment
            if (moverPosition.Position.X + moverBody.Size.X <= colliderPosition.Position.X ||
                moverPosition.Position.X >= colliderPosition.Position.X + colliderBody.Size.X)
            {
                return null;
            }

            if (moverVelocity.Velocity.Y < 0.0f)
            {
                // Up
                if (moverPosition.Position.Y + moverBody.Size.Y <= colliderPosition.Position.Y)
                {
                    return null;
                }

                float wall = colliderPosition.Position.Y + colliderBody.Size.Y;

                if (endPosition < wall)
                {
                    return new PhysicsBodyCollision
                    {
                        Position = new Vector2(moverPosition.Position.X, wall),
                        Normal = new Vector2(0.0f, 1.0f)
                    };
                }
            }
            else
            {
                // Down
                if (moverPosition.Position.Y >= colliderPosition.Position.Y + colliderBody.Size.Y)
                {
                    return null;
                }

                float wall = colliderPosition.Position.Y - moverBody.Size.Y;

                if (endPosition > wall)
                {
                    return new PhysicsBodyCollision
                    {
                        Position = new Vector2(moverPosition.Position.X, wall),
                        Normal = new Vector2(0.0f, -1.0f)
                    };
                }
            }

            // No collision
            return null;
        }

        public void Initialize()
        {

        }

        public void Update(World entities, float delta)
        {
            // Put matching entities into list
            List<Entity> movers = new(), colliders = new();

            entities.GetEntities(MoverQuery, movers);
            entities.GetEntities(ColliderQuery, colliders);

            foreach (Entity mover in movers)
            {
                ref PhysicsBodyComponent moverBody = ref mover.Get<PhysicsBodyComponent>();
                ref PositionComponent moverPosition = ref mover.Get<PositionComponent>();
                ref VelocityComponent moverVelocity = ref mover.Get<VelocityComponent>();

                // Clear collisions
                moverBody.Collisions.Clear();

                // Collide X
                if (moverVelocity.Velocity.X != 0.0f)
                {
                    float endPosition = moverPosition.Position.X + moverVelocity.Velocity.X * delta;
                    PhysicsBodyCollision? collision = null;

                    foreach (Entity collider in colliders)
                    {
                        // Skip matches
                        if (mover == collider)
                        {
                            continue;
                        }

                        // Check new collision
                        PhysicsBodyCollision? newCollision = GetCollisionX(
                            ref moverBody, ref moverPosition, ref moverVelocity,
                            collider, endPosition);

                        if (newCollision != null)
                        {
                            collision = newCollision;
                            endPosition = collision.Value.Position.X;
                        }
                    }

                    // Apply movement
                    moverPosition.Position.X = endPosition;
                    
                    if (collision != null)
                    {
                        moverBody.Collisions.Add(collision.Value);

                        moverVelocity.Velocity.X *= moverBody.Bounce.X;
                    }
                }

                // Collide Y
                if (moverVelocity.Velocity.Y != 0.0f)
                {
                    float endPosition = moverPosition.Position.Y + moverVelocity.Velocity.Y * delta;
                    PhysicsBodyCollision? collision = null;

                    foreach (Entity collider in colliders)
                    {
                        // Skip matches
                        if (mover == collider)
                        {
                            continue;
                        }

                        // Check new collision
                        PhysicsBodyCollision? newCollision = GetCollisionY(
                            ref moverBody, ref moverPosition, ref moverVelocity,
                            collider, endPosition);

                        if (newCollision != null)
                        {
                            collision = newCollision;
                            endPosition = collision.Value.Position.Y;
                        }
                    }

                    // Apply movement
                    moverPosition.Position.Y = endPosition;

                    if (collision != null)
                    {
                        moverBody.Collisions.Add(collision.Value);

                        moverVelocity.Velocity.Y *= moverBody.Bounce.Y;
                    }
                }
            }
        }
    }
}
