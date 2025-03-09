using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RobotGame.Components;
using System;

namespace RobotGame.Systems
{
    public class AlienEnemySystem : ISystem
    {
        public const float Speed = 20.0f;
        public const float MoveTime = 1.0f;
        public const float ShootTime = 1.0f;
        public const float BulletSpeed = 50.0f;

        public Vector2 BodySize = new(8.0f, 8.0f);
        public Vector2 SpriteOffset = new(-4.0f, -7.0f);

        public Vector2 AreaSize = new(12.0f, 12.0f);
        public GameRect[] AreaRects;

        public SpriteAnimation WalkAnimation;

        public QueryDescription Query;

        public RobotGame Game;
        public GameWorld World;

        public AlienEnemySystem(RobotGame game, GameWorld world)
        {
            Game = game;
            World = world;

            Query = new QueryDescription().WithAll<
                AlienEnemyComponent,
                EnemyComponent,
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

        // Spawns a new enemy
        public Entity CreateAlienEnemy(World entities, Vector2 position)
        {
            Entity entity = entities.Create(
                new AlienEnemyComponent(),
                new EnemyComponent(),
                new PositionComponent { Position = position - BodySize * 0.5f },
                new VelocityComponent(),
                new PhysicsBodyComponent { Size = BodySize, Bounce = -Vector2.One, MoverMask = 1, ColliderMask = 0 },
                new PhysicsAreaComponent { Rects = AreaRects },
                new SpriteComponent { Texture = Game.Renderer.AlienEnemyTexture, Offset = SpriteOffset },
                new SpriteAnimatorComponent());

            // Starting animation
            SpriteAnimatorSystem.PlayAnimation(ref entity.Get<SpriteAnimatorComponent>(), WalkAnimation);

            return entity;
        }

        public void RandomizeMovement(ref VelocityComponent velocity)
        {
            float angle = World.Random.Next() * MathF.PI * 2.0f;
            velocity.Velocity = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * Speed;
        }

        public void Initialize()
        {
            // Create animation
            Texture2D texture = Game.Renderer.AlienEnemyTexture;

            WalkAnimation = new SpriteAnimation(
                SpriteAnimatorSystem.GetFrames(texture, 2), 5.0f);
        }

        public void Update(World entities, float delta)
        {
            entities.Query(in Query, (
                Entity entity,
                ref AlienEnemyComponent alienEnemy,
                ref EnemyComponent enemy,
                ref PositionComponent position,
                ref VelocityComponent velocity,
                ref PhysicsBodyComponent body,
                ref PhysicsAreaComponent area,
                ref SpriteComponent sprite,
                ref SpriteAnimatorComponent spriteAnimator) =>
            {
                // Flip if moving left
                sprite.FlipX = velocity.Velocity.X < 0.0f;

                // Randomize movement direction on a time step
                if (alienEnemy.MoveTimer <= 0.0f)
                {
                    alienEnemy.MoveTimer = MoveTime;

                    RandomizeMovement(ref velocity);
                }
                else
                {
                    alienEnemy.MoveTimer -= delta;
                }

                // Shoot bullets on a time step
                if (alienEnemy.ShootTimer <= 0.0f)
                {
                    alienEnemy.ShootTimer = ShootTime;

                    // Aim orthogonally to player
                    Vector2 bulletDirection;

                    Vector2 playerPos = World.Player.Get<PositionComponent>().Position;
                    Vector2 toPlayer = playerPos - position.Position;

                    if (MathF.Abs(toPlayer.X) < MathF.Abs(toPlayer.Y))
                    {
                        bulletDirection = new Vector2(0.0f, MathF.Sign(toPlayer.Y));
                    }
                    else
                    {
                        bulletDirection = new Vector2(MathF.Sign(toPlayer.X), 0.0f);
                    }

                    World.BulletSystem.CreateBullet(
                        entities, position.Position + body.Size * 0.5f, bulletDirection, BulletSpeed, BulletType.Enemy);
                }
                else
                {
                    alienEnemy.ShootTimer -= delta;
                }
            });
        }
    }
}
