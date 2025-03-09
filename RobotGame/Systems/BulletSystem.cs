using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RobotGame.Components;
using System;
using System.Collections.Generic;

namespace RobotGame.Systems
{
    public class BulletSystem : ISystem
    {
        public Vector2 BodySize = new(4.0f, 4.0f);

        public Vector2 AreaSize = new(6.0f, 6.0f);
        public GameRect[] AreaRects;

        public Vector2 SpriteOffset = new(-2.0f, -2.0f);

        public SpriteAnimation FlashAnimation, EndAnimation;

        public QueryDescription Query;

        public RobotGame Game;
        public GameWorld World;

        public BulletSystem(RobotGame game, GameWorld world)
        {
            Game = game;
            World = world;

            Query = new QueryDescription().WithAll<
                BulletComponent,
                PositionComponent,
                VelocityComponent,
                PhysicsBodyComponent,
                PhysicsAreaComponent,
                SpriteComponent,
                SpriteAnimatorComponent>();

            // Create area rect
            AreaRects = new GameRect[]
            {
                new GameRect((BodySize - AreaSize) * 0.5f, AreaSize)
            };
        }

        // Spawns a new bullet
        public Entity CreateBullet(World entities, Vector2 position, Vector2 direction, float speed, BulletType type)
        {
            Texture2D texture = type switch
            {
                BulletType.Player => Game.Renderer.PlayerBulletTexture,
                BulletType.Enemy => Game.Renderer.EnemyBulletTexture,
                _ => Game.Renderer.PlayerBulletTexture
            };

            Entity entity = entities.Create(
                new BulletComponent { Type = type },
                new PositionComponent { Position = position - BodySize * 0.5f },
                new VelocityComponent { Velocity = direction * speed },
                new PhysicsBodyComponent { Size = BodySize, MoverMask = 1, ColliderMask = 0 },
                new PhysicsAreaComponent { Rects = AreaRects },
                new SpriteComponent { Texture = texture, Offset = SpriteOffset },
                new SpriteAnimatorComponent());

            // Starting animation
            SpriteAnimatorSystem.PlayAnimation(ref entity.Get<SpriteAnimatorComponent>(), FlashAnimation);

            return entity;
        }

        // Destroys a bullet
        public void DestroyBullet(Entity entity)
        {
            ref BulletComponent bullet = ref entity.Get<BulletComponent>();

            // Skip if already ended
            if (bullet.Ended)
            {
                return;
            }

            bullet.Ended = true;

            SpriteAnimatorSystem.PlayAnimation(ref entity.Get<SpriteAnimatorComponent>(), EndAnimation);

            // Stop moving
            entity.Get<VelocityComponent>().Velocity = Vector2.Zero;
        }

        public void Initialize()
        {
            // Create animations
            Texture2D texture = Game.Renderer.PlayerBulletTexture;

            List<Rectangle>
                spriteFrames = SpriteAnimatorSystem.GetFrames(texture, 5),
                flashFrames = new() { spriteFrames[0], spriteFrames[1] },
                endFrames = new() { spriteFrames[2], spriteFrames[3], spriteFrames[4] };

            FlashAnimation = new SpriteAnimation(flashFrames, 10.0f);
            EndAnimation = new SpriteAnimation(endFrames, 20.0f, loops: false);
        }

        public void Update(World entities, float delta)
        {
            entities.Query(in Query, (
                Entity entity,
                ref BulletComponent bullet,
                ref PositionComponent position,
                ref VelocityComponent velocity,
                ref PhysicsBodyComponent body,
                ref PhysicsAreaComponent area,
                ref SpriteComponent sprite,
                ref SpriteAnimatorComponent spriteAnimator) =>
            {
                // Destroy bullet if animation finished
                if (bullet.Ended)
                {
                    if (spriteAnimator.Finished)
                    {
                        World.QueueDestroyEntity(entity);
                    }

                    return;
                }

                // Destroy if hit something
                if (body.Collisions.Count > 0)
                {
                    DestroyBullet(entity);
                }

                switch (bullet.Type)
                {
                    case BulletType.Player:
                        // Check for enemy collisions
                        foreach (PhysicsAreaCollision collision in area.Collisions)
                        {
                            if (!collision.Entity.Has<EnemyComponent>())
                            {
                                continue;
                            }

                            World.EnemySystem.DestroyEnemy(collision.Entity);
                            DestroyBullet(entity);
                        }

                        break;

                    case BulletType.Enemy:
                        // Check for player collisions
                        foreach (PhysicsAreaCollision collision in area.Collisions)
                        {
                            if (!collision.Entity.Has<PlayerComponent>())
                            {
                                continue;
                            }

                            if (collision.EntityRectIndex != PlayerSystem.HurtRectIndex)
                            {
                                continue;
                            }

                            World.PlayerSystem.DamagePlayer(collision.Entity, 1);
                            DestroyBullet(entity);
                        }

                        break;

                    default:
                        break;
                }
            });
        }
    }
}
