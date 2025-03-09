using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using RobotGame.Components;

namespace RobotGame
{
    public class GearDisplay
    {
        public Vector2
            IconPosition = new(20.0f, 2.0f),
            TextPosition = new(36.0f, 2.0f);

        public RobotGame Game;
        public GameWorld World;

        public GearDisplay(RobotGame game, GameWorld world)
        {
            Game = game;
            World = world;
        }

        public void Draw(Renderer renderer)
        {
            // Get player's gear count
            int gearCount = World.Player.Get<PlayerComponent>().GearCount;

            // Draw icon and count text
            renderer.SpriteBatch.Draw(renderer.GearIconTexture, IconPosition, Color.White);
            renderer.TextRenderer.DrawText(renderer, "x" + gearCount, TextPosition, Color.White);
        }
    }
}
