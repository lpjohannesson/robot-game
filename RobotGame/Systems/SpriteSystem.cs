using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RobotGame.Components;

namespace RobotGame.Systems
{
    public class SpriteSystem : ISystem
    {
        public QueryDescription Query;

        public SpriteSystem()
        {
            Query = new QueryDescription().WithAll<SpriteComponent, PositionComponent>();
        }

        public void Draw(Renderer renderer, World entities)
        {
            entities.Query(in Query, (
                Entity entity,
                ref SpriteComponent sprite,
                ref PositionComponent position) =>
            {
                SpriteEffects spriteEffects = sprite.FlipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                // Draw sprite
                renderer.SpriteBatch.Draw(
                    sprite.Texture,
                    position.Position + sprite.Offset,
                    sprite.Frame,
                    sprite.Color,
                    0.0f,
                    Vector2.Zero,
                    Vector2.One,
                    spriteEffects,
                    0.0f);
            });
        }

        public void Initialize()
        {

        }

        public void Update(World entities, float delta)
        {

        }
    }
}
