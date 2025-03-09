using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RobotGame.Components;
using System;
using System.Collections.Generic;

namespace RobotGame.Systems
{
    public ref struct PlayerData
    {
        public Entity Entity;
        public ref PlayerComponent Player;
        public ref PositionComponent Position;
        public ref VelocityComponent Velocity;
        public ref PhysicsBodyComponent Body;
        public ref PhysicsAreaComponent Area;
        public ref HealthComponent Health;
        public ref SpriteComponent Sprite;
        public ref SpriteAnimatorComponent SpriteAnimator;
    }

    public class PlayerSystem : ISystem
    {
        public const float Acceleration = 400.0f;
        public const float MaxSpeed = 60.0f;
        public const int MaxHealth = 4;

        public const float ShootTime = 0.2f;
        public const float BulletSpeed = 100.0f;

        public const float HurtTime = 1.0f;

        public const int CollectRectIndex = 0, HurtRectIndex = 1;

        public Vector2 BodySize = new(8.0f, 8.0f);
        public Vector2 SpriteOffset = new(-4.0f, -7.0f);

        public Vector2
            CollectAreaSize = new(12.0f, 12.0f),
            HurtAreaSize = new(6.0f, 6.0f);

        public GameRect[] AreaRects;

        public SpriteAnimation IdleAnimation, WalkAnimation;

        public Color HurtColor = Color.Red;

        public QueryDescription Query;

        public RobotGame Game;
        public GameWorld World;

        public PlayerSystem(RobotGame game, GameWorld world)
        {
            Game = game;
            World = world;

            Query = new QueryDescription().WithAll<
                PlayerComponent,
                PositionComponent,
                VelocityComponent,
                PhysicsBodyComponent,
                PhysicsAreaComponent,
                HealthComponent,
                SpriteComponent,
                SpriteAnimatorComponent>();

            // Create area rects
            AreaRects = new GameRect[]
            {
                new GameRect((BodySize - CollectAreaSize) * 0.5f, CollectAreaSize), // Collect rect
                new GameRect((BodySize - HurtAreaSize) * 0.5f, HurtAreaSize) // Hurt rect
            };
        }

        // Spawns a new player
        public Entity CreatePlayer(World entities, Vector2 position)
        {
            Entity entity = entities.Create(
                new PlayerComponent { FacingDirection = new Vector2(0.0f, 1.0f) },
                new PositionComponent { Position = position - BodySize * 0.5f },
                new VelocityComponent(),
                new PhysicsBodyComponent { Size = BodySize, MoverMask = 1, ColliderMask = 0 },
                new PhysicsAreaComponent { Rects = AreaRects },
                new HealthComponent() { Value = MaxHealth, MaxValue = MaxHealth },
                new SpriteComponent { Texture = Game.Renderer.PlayerDownTexture, Offset = SpriteOffset },
                new SpriteAnimatorComponent());

            // Starting animation
            SpriteAnimatorSystem.PlayAnimation(ref entity.Get<SpriteAnimatorComponent>(), IdleAnimation);

            return entity;
        }

        // Damages a player by an amount
        public void DamagePlayer(Entity entity, int amount)
        {
            ref PlayerComponent player = ref entity.Get<PlayerComponent>();

            // Skip if hurt timer still active
            if (player.HurtTimer > 0.0f)
            {
                return;
            }

            // Apply damage
            ref HealthComponent health = ref entity.Get<HealthComponent>();
            Health.ModifyHealth(ref health, health.Value - amount);

            // Update health bar
            World.HealthBar.UpdateDisplay();

            if (health.Value <= 0)
            {
                // Lose game
                World.LoseGame();
            }
            else
            {
                // Activate hurt timer
                player.HurtTimer = HurtTime;

                // Use hurt color
                ref SpriteComponent sprite = ref entity.Get<SpriteComponent>();
                sprite.Color = HurtColor;

                Game.Audio.HurtSound.Play();
            }
        }

        // Heals a player by an amount
        public void HealPlayer(Entity entity, int amount)
        {
            // Apply healing
            ref HealthComponent playerHealth = ref entity.Get<HealthComponent>();
            Health.ModifyHealth(ref playerHealth, playerHealth.Value + amount);

            // Update health bar
            World.HealthBar.UpdateDisplay();
        }

        // Returns a facing direction based on movement
        public static Vector2 GetFacingDirection(Vector2 direction)
        {
            if (direction.Y == 0.0f)
            {
                // Return left or right
                return new Vector2(direction.X, 0.0f);
            }

            // Return up or down
            return new Vector2(0.0f, direction.Y);
        }

        // Returns a texture based on a direction
        public Texture2D GetFacingTexture(Vector2 direction)
        {
            Renderer renderer = Game.Renderer;

            if (direction.Y == 0.0f)
            {
                // Return left or right
                if (direction.X < 0.0f)
                {
                    return renderer.PlayerLeftTexture;
                }

                return renderer.PlayerRightTexture;
            }

            // Return up or down
            if (direction.Y < 0.0f)
            {
                return renderer.PlayerUpTexture;
            }

            return renderer.PlayerDownTexture;
        }

        // Applies acceleration on an axis
        public static float ApplyMovement(float velocity, float moveDirection, float delta)
        {
            if (moveDirection == 0.0f)
            {
                // Decelerate by slowing towards zero
                float velocityChange = Acceleration * delta;

                if (velocity < 0.0f)
                {
                    return MathF.Min(velocity + velocityChange, 0.0f);
                }

                return MathF.Max(velocity - velocityChange, 0.0f);
            }
            else
            {
                // Accelerate with maximum speed
                float newVelocity = velocity + Acceleration * moveDirection * delta;
                return Math.Clamp(newVelocity, -MaxSpeed, MaxSpeed);
            }
        }

        // Moves across both axes
        public void Move(ref PlayerData playerData, Vector2 moveDirection, float delta)
        {
            playerData.Velocity.Velocity.X =
                ApplyMovement(playerData.Velocity.Velocity.X, moveDirection.X, delta);

            playerData.Velocity.Velocity.Y =
                ApplyMovement(playerData.Velocity.Velocity.Y, moveDirection.Y, delta);
        }

        // Fires projectiles
        public void Shoot(
            ref PlayerData playerData,
            World entities,
            Vector2 shootDirection,
            float delta)
        {
            // Check shoot timer is finished
            if (playerData.Player.ShootTimer <= 0.0f)
            {
                // Check if shooting in a direction
                if (shootDirection != Vector2.Zero)
                {
                    // Summon bullet centered on the player's hitbox
                    BulletSystem bulletSystem = World.BulletSystem;

                    Vector2 bulletPosition =
                        playerData.Position.Position + playerData.Body.Size * 0.5f;

                    bulletSystem.CreateBullet(
                        entities, bulletPosition, shootDirection, BulletSpeed, BulletType.Player);

                    // Reset shoot timer
                    playerData.Player.ShootTimer = ShootTime;
                }
            }
            else
            {
                // Advance shoot timer
                playerData.Player.ShootTimer -= delta;
            }
        }

        // Controls animations
        public void Animate(ref PlayerData playerData, Vector2 moveDirection)
        {
            if (moveDirection == Vector2.Zero)
            {
                // Show idle animation
                SpriteAnimatorSystem.PlayAnimation(ref playerData.SpriteAnimator, IdleAnimation);
            }
            else
            {
                // Check if movement is facing away from current direction
                if (Vector2.Dot(playerData.Player.FacingDirection, moveDirection) <= 0.0)
                {
                    // Change sprite direction
                    playerData.Player.FacingDirection = GetFacingDirection(moveDirection);
                    playerData.Sprite.Texture = GetFacingTexture(playerData.Player.FacingDirection);
                }

                // Play walking animation
                SpriteAnimatorSystem.PlayAnimation(ref playerData.SpriteAnimator, WalkAnimation);
            }
        }

        public void Initialize()
        {
            // Create animations
            Texture2D texture = Game.Renderer.PlayerDownTexture;

            List<Rectangle>
                spriteFrames = SpriteAnimatorSystem.GetFrames(texture, 3),
                idleFrames = new() { spriteFrames[0] },
                walkFrames = new() { spriteFrames[0], spriteFrames[1], spriteFrames[2] };

            IdleAnimation = new SpriteAnimation(idleFrames, 0.0f);
            WalkAnimation = new SpriteAnimation(walkFrames, 10.0f);
        }

        public void Update(World entities, float delta)
        {
            entities.Query(in Query, (
                Entity entity,
                ref PlayerComponent player,
                ref PositionComponent position,
                ref VelocityComponent velocity,
                ref PhysicsBodyComponent body,
                ref PhysicsAreaComponent area,
                ref HealthComponent health,
                ref SpriteComponent sprite,
                ref SpriteAnimatorComponent spriteAnimator) =>
            {
                // Pack components into struct
                PlayerData playerData = new()
                {
                    Entity = entity,
                    Player = ref player,
                    Position = ref position,
                    Velocity = ref velocity,
                    Body = ref body,
                    Area = ref area,
                    Health = ref health,
                    Sprite = ref sprite,
                    SpriteAnimator = ref spriteAnimator
                };

                // Obtain input variables
                Input input = Game.Input;

                Vector2 moveDirection = input.GetVector(Keys.W, Keys.S, Keys.A, Keys.D);
                Vector2 shootDirection = input.GetVector(Keys.Up, Keys.Down, Keys.Left, Keys.Right);

                // Perform actions
                Move(ref playerData, moveDirection, delta);
                Shoot(ref playerData, entities, shootDirection, delta);
                Animate(ref playerData, moveDirection);

                // Update hurt timer
                if (player.HurtTimer > 0.0f)
                {
                    player.HurtTimer -= delta;

                    // Reset sprite color if finished
                    if (player.HurtTimer <= 0.0f)
                    {
                        sprite.Color = Color.White;
                    }
                }
            });
        }
    }
}
