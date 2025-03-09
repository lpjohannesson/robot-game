using Arch.Core;
using Arch.Core.Extensions;
using RobotGame.Components;

namespace RobotGame.Systems
{
    public class CollectibleSystem : ISystem
    {
        public QueryDescription Query;

        public RobotGame Game;
        public GameWorld World;

        public CollectibleSystem(RobotGame game, GameWorld world)
        {
            Game = game;
            World = world;

            Query = new QueryDescription().WithAll<
                CollectibleComponent, 
                PhysicsAreaComponent>();
        }

        public void CollectEntity(Entity entity)
        {
            World.QueueDestroyEntity(entity);
        }

        public void Initialize()
        {

        }

        public void Update(World entities, float delta)
        {
            entities.Query(in Query, (
                Entity entity,
                ref CollectibleComponent collectible,
                ref PhysicsAreaComponent area) =>
            {
                // Update despawn timer
                if (collectible.DespawnTimer <= 0.0f)
                {
                    World.QueueDestroyEntity(entity);
                }
                else
                {
                    collectible.DespawnTimer -= delta;
                }

                // Check if collected by player
                foreach (PhysicsAreaCollision collision in area.Collisions)
                {
                    if (!collision.Entity.Has<PlayerComponent>())
                    {
                        continue;
                    }

                    if (collision.RectIndex != PlayerSystem.CollectRectIndex)
                    {
                        continue;
                    }

                    collectible.Collected = true;
                }
            });
        }
    }
}
