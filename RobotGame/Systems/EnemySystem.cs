using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using RobotGame.Components;

namespace RobotGame.Systems
{
    public class EnemySystem : ISystem
    {
        public RobotGame Game;
        public GameWorld World;

        public EnemySystem(RobotGame game, GameWorld world)
        {
            Game = game;
            World = world;
        }

        // Destroys an enemy
        public void DestroyEnemy(Entity entity)
        {
            World entities = World.Entities;

            // Create a random drop
            Vector2 dropPosition = entity.Get<PositionComponent>().Position + entity.Get<PhysicsBodyComponent>().Size * 0.5f;

            if (World.Random.Next() % 4 == 0)
            {
                World.BatterySystem.CreateBattery(entities, dropPosition);
            }
            else
            {
                World.GearSystem.CreateGear(entities, dropPosition);
            }

            World.QueueDestroyEntity(entity);

            Game.Audio.EnemyHurtSound.Play();
        }

        public void Initialize()
        {
            
        }

        public void Update(World entities, float delta)
        {
            
        }
    }
}
