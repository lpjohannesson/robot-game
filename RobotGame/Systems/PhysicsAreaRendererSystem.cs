using Arch.Core;
using Microsoft.Xna.Framework;
using RobotGame.Components;

namespace RobotGame.Systems
{
    public class PhysicsAreaRendererSystem : ISystem
    {
        public RectRenderer RectRenderer;

        public QueryDescription Query;

        public Renderer Renderer;

        public PhysicsAreaRendererSystem(Renderer renderer)
        {
            Renderer = renderer;

            Query = new QueryDescription().WithAll<PhysicsAreaComponent, PositionComponent>();
        }

        public void Draw(Renderer renderer, World entities)
        {
            entities.Query(in Query, (
                Entity entity,
                ref PhysicsAreaComponent area,
                ref PositionComponent position) =>
            {
                // Draw every bounding box of area
                for (int i = 0; i < area.Rects.Length; i++)
                {
                    RectRenderer.DrawRect(renderer, PhysicsAreaSystem.GetRect(area, position, i));
                }
            });
        }

        public void Initialize()
        {
            RectRenderer = new RectRenderer(Renderer, Color.Green);
        }

        public void Update(World entities, float delta)
        {
        }
    }
}
