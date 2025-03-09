using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RobotGame.Components;

namespace RobotGame.Systems
{
    public class BatterySystem : ISystem
    {
        public Vector2 AreaSize = new(16.0f, 16.0f);
        public GameRect[] AreaRects;

        public SpriteAnimation FlashAnimation;

        public QueryDescription Query;

        public RobotGame Game;
        public GameWorld World;

        public BatterySystem(RobotGame game, GameWorld world)
        {
            Game = game;
            World = world;

            Query = new QueryDescription().WithAll<
                BatteryComponent,
                PositionComponent,
                PhysicsAreaComponent,
                CollectibleComponent,
                SpriteComponent,
                SpriteAnimatorComponent>();

            // Create area rect
            AreaRects = new GameRect[]
            {
                new GameRect(Vector2.Zero, AreaSize)
            };
        }

        // Spawns a new battery
        public Entity CreateBattery(World entities, Vector2 position)
        {
            Entity entity = entities.Create(
                new BatteryComponent(),
                new PositionComponent { Position = position - AreaSize * 0.5f },
                new PhysicsAreaComponent { Rects = AreaRects },
                new CollectibleComponent(),
                new SpriteComponent { Texture = Game.Renderer.BatteryTexture },
                new SpriteAnimatorComponent());

            // Starting animation
            SpriteAnimatorSystem.PlayAnimation(ref entity.Get<SpriteAnimatorComponent>(), FlashAnimation);

            return entity;
        }

        public void Initialize()
        {
            // Create animation
            Texture2D texture = Game.Renderer.BatteryTexture;

            FlashAnimation = new SpriteAnimation(
                SpriteAnimatorSystem.GetFrames(texture, 2), 10.0f);
        }

        public void Update(World entities, float delta)
        {
            entities.Query(in Query, (
                Entity entity,
                ref BatteryComponent battery,
                ref PositionComponent position,
                ref PhysicsAreaComponent area,
                ref CollectibleComponent collectible,
                ref SpriteComponent sprite,
                ref SpriteAnimatorComponent spriteAnimator) =>
            {
                if (collectible.Collected)
                {
                    World.PlayerSystem.HealPlayer(World.Player, 1);

                    World.CollectibleSystem.CollectEntity(entity);

                    Game.Audio.BatterySound.Play();
                }
            });
        }
    }
}
