using Microsoft.Xna.Framework;

namespace RobotGame.Scenes
{
    public class MainGameScene : IGameScene
    {
        public RobotGame Game;
        public GameWorld World;

        public MainGameScene(RobotGame game)
        {
            Game = game;
        }

        public void Initialize()
        {
            World = new GameWorld(Game);
            World.Initialize();
        }

        public void Update(float delta)
        {
            World.Update(delta);
        }

        public void Draw()
        {
            World.Draw(Game.Renderer);
        }
    }
}
