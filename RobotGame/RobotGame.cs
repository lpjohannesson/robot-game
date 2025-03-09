using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RobotGame.Scenes;

namespace RobotGame
{
    public class RobotGame : Game
    {
        public GraphicsDeviceManager Graphics;

        public Renderer Renderer;
        public AudioController Audio;

        public Input Input = new();

        public IGameScene CurrentScene;

        public RobotGame()
        {
            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            Renderer = new Renderer(this);
            Audio = new AudioController(this);
        }

        // Changes to a new scene
        public void ChangeScene(IGameScene scene)
        {
            CurrentScene = scene;
            scene.Initialize();
        }

        protected override void Initialize()
        {
            Renderer.Initialize();
            Audio.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Renderer.LoadContent();

            ChangeScene(new MenuScene(this));

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            CurrentScene.Update(delta);

            Input.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Renderer.Draw();

            base.Draw(gameTime);
        }
    }
}