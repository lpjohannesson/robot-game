using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RobotGame.Systems;

namespace RobotGame
{
    public class Renderer
    {
        public int WindowScale = 3;

        public RenderTarget2D RenderTarget;
        public Rectangle RenderTargetRect;

        public GraphicsDeviceManager Graphics;
        public GraphicsDevice GraphicsDevice;

        public SpriteBatch SpriteBatch;

        public Texture2D
            BackgroundTexture,

            PlayerUpTexture,
            PlayerDownTexture,
            PlayerLeftTexture,
            PlayerRightTexture,

            PlayerBulletTexture,
            EnemyBulletTexture,

            AlienEnemyTexture,

            BatteryTexture,
            GearTexture,

            HealthBackTexture,
            HealthFrontTexture,

            GearIconTexture,
            
            FontTexture;

        public SpriteSystem SpriteSystem = new();
        public SpriteAnimatorSystem SpriteAnimatorSystem = new();
        public PhysicsBodyRendererSystem PhysicsBodyRendererSystem;
        public PhysicsAreaRendererSystem PhysicsAreaRendererSystem;

        public TextRenderer TextRenderer;

        public RobotGame Game;

        public Renderer(RobotGame game)
        {
            Game = game;
        }

        public void Initialize()
        {
            // Get graphics objects from game
            Graphics = Game.Graphics;
            GraphicsDevice = Game.GraphicsDevice;

            // Set window resolution
            Graphics.PreferredBackBufferWidth = 240 * WindowScale;
            Graphics.PreferredBackBufferHeight = 180 * WindowScale;

            Graphics.ApplyChanges();

            // Create viewport
            RenderTarget = new RenderTarget2D(GraphicsDevice, 240, 180);

            // Span viewport rect across entire screen
            RenderTargetRect = new Rectangle(0, 0,
                Graphics.PreferredBackBufferWidth,
                Graphics.PreferredBackBufferHeight);

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Add systems
            SpriteSystem = new SpriteSystem();
            SpriteAnimatorSystem = new SpriteAnimatorSystem();

            // Create debug systems
            PhysicsBodyRendererSystem = new PhysicsBodyRendererSystem(this);
            PhysicsAreaRendererSystem = new PhysicsAreaRendererSystem(this);
        }

        public void LoadContent()
        {
            ContentManager content = Game.Content;

            // Load textures
            BackgroundTexture = content.Load<Texture2D>("Textures/background");

            PlayerUpTexture = content.Load<Texture2D>("Textures/player_up");
            PlayerDownTexture = content.Load<Texture2D>("Textures/player_down");
            PlayerLeftTexture = content.Load<Texture2D>("Textures/player_left");
            PlayerRightTexture = content.Load<Texture2D>("Textures/player_right");

            PlayerBulletTexture = content.Load<Texture2D>("Textures/player_bullet");
            EnemyBulletTexture = content.Load<Texture2D>("Textures/enemy_bullet");

            AlienEnemyTexture = content.Load<Texture2D>("Textures/alien_enemy");

            BatteryTexture = content.Load<Texture2D>("Textures/battery");
            GearTexture = content.Load<Texture2D>("Textures/gear");

            HealthBackTexture = content.Load<Texture2D>("Textures/health_back");
            HealthFrontTexture = content.Load<Texture2D>("Textures/health_front");

            GearIconTexture = content.Load<Texture2D>("Textures/gear_icon");

            FontTexture = content.Load<Texture2D>("Textures/font_wide");

            // Create text renderer
            TextRenderer = new TextRenderer(FontTexture, new Point(12, 12));
        }

        public void Draw()
        {
            // Draw to viewport
            GraphicsDevice.SetRenderTarget(RenderTarget);
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);

            Game.CurrentScene.Draw();

            SpriteBatch.End();

            // Draw to main window
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            // Put viewport to screen
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            SpriteBatch.Draw(RenderTarget, RenderTargetRect, Color.White);
            SpriteBatch.End();
        }
    }
}
