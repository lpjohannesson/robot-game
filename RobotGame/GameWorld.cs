using Arch.Core;
using RobotGame.Systems;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using RobotGame.Components;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework.Media;
using RobotGame.Scenes;

namespace RobotGame
{
    public class GameWorld
    {
        public const float WallThickness = 8.0f;

        public GameRect WallsRect = new(new Vector2(30.0f, 18.0f), new Vector2(180.0f, 144.0f));

        public World Entities;

        public Entity Player;

        public PhysicsBodySystem PhysicsBodySystem;
        public PhysicsAreaSystem PhysicsAreaSystem;

        public PlayerSystem PlayerSystem;
        public BulletSystem BulletSystem;

        public EnemySystem EnemySystem;
        public AlienEnemySystem AlienEnemySystem;

        public CollectibleSystem CollectibleSystem;
        public GearSystem GearSystem;
        public BatterySystem BatterySystem;

        public EnemyFactory EnemyFactory;

        public HealthBar HealthBar;
        public GearDisplay GearDisplay;

        public Random Random = new();

        public List<Entity> EntityDestructionQueue = new();

        public List<ISystem> Systems = new();

        public RobotGame Game;

        public GameWorld(RobotGame game)
        {
            Game = game;

            // Generic systems
            PhysicsBodySystem = new PhysicsBodySystem();
            Systems.Add(PhysicsBodySystem);

            PhysicsAreaSystem = new PhysicsAreaSystem();
            Systems.Add(PhysicsAreaSystem);

            // Main entities
            PlayerSystem = new PlayerSystem(Game, this);
            Systems.Add(PlayerSystem);

            BulletSystem = new BulletSystem(Game, this);
            Systems.Add(BulletSystem);

            EnemySystem = new EnemySystem(Game, this);
            Systems.Add(EnemySystem);

            AlienEnemySystem = new AlienEnemySystem(Game, this);
            Systems.Add(AlienEnemySystem);

            // Collectibles
            CollectibleSystem = new CollectibleSystem(Game, this);
            Systems.Add(CollectibleSystem);

            GearSystem = new GearSystem(Game, this);
            Systems.Add(GearSystem);

            BatterySystem = new BatterySystem(Game, this);
            Systems.Add(BatterySystem);

            // Create stat displays
            HealthBar = new HealthBar(Game, this);
            GearDisplay = new GearDisplay(Game, this);
        }

        // Loses the game
        public void LoseGame()
        {
            // Go to lose screen
            Game.ChangeScene(new LoseScene(Game));
        }

        // Creates a wall
        public void CreateWall(GameRect rect)
        {
            Entities.Create(
                new PositionComponent { Position = rect.Position },
                new PhysicsBodyComponent { Size = rect.Size });
        }

        // Creates the game's walls
        public void CreateWalls()
        {
            // Get rect sizes
            Vector2 wideSize = new(WallsRect.Size.X, WallThickness), tallSize = new(WallThickness, WallsRect.Size.Y);

            // Left
            CreateWall(new GameRect(
                new Vector2(WallsRect.Position.X - WallThickness, WallsRect.Position.Y), tallSize));

            // Right
            CreateWall(new GameRect(
                new Vector2(WallsRect.Position.X + WallsRect.Size.X, WallsRect.Position.Y), tallSize));

            // Top
            CreateWall(new GameRect(
                new Vector2(WallsRect.Position.X, WallsRect.Position.Y - WallThickness), wideSize));

            // Bottom
            CreateWall(new GameRect(
                new Vector2(WallsRect.Position.X, WallsRect.Position.Y + WallsRect.Size.Y), wideSize));
        }

        // Queues an entity to be destroyed
        public void QueueDestroyEntity(Entity entity)
        {
            EntityDestructionQueue.Add(entity);
        }

        public void Initialize()
        {
            Entities = World.Create();

            // Start each system by interface
            foreach (ISystem system in Systems)
            {
                system.Initialize();
            }

            // Add renderer systems
            Renderer renderer = Game.Renderer;

            Systems.Add(renderer.SpriteAnimatorSystem);
            Systems.Add(renderer.PhysicsBodyRendererSystem);
            Systems.Add(renderer.PhysicsAreaRendererSystem);

            // Create objects
            CreateWalls();

            Player = PlayerSystem.CreatePlayer(Entities, new Vector2(120.0f, 90.0f));

            EnemyFactory = new EnemyFactory(Game, this);

            HealthBar.UpdateDisplay();

            // Play music
            MediaPlayer.Play(Game.Audio.Music);
        }

        public void Update(float delta)
        {
            EnemyFactory.Update(delta);

            // Update each system by interface
            foreach (ISystem system in Systems)
            {
                system.Update(Entities, delta);
            }

            // Delete queued entities
            foreach (Entity entity in EntityDestructionQueue)
            {
                // Skip dead entities
                if (!entity.IsAlive())
                {
                    continue;
                }

                Entities.Destroy(entity);
            }

            EntityDestructionQueue.Clear();
        }

        // Draws debug info
        public void DrawDebug(Renderer renderer)
        {
            renderer.PhysicsBodyRendererSystem.Draw(renderer, Entities);
            renderer.PhysicsAreaRendererSystem.Draw(renderer, Entities);
        }

        // Draws the game world
        public void Draw(Renderer renderer)
        {
            renderer.SpriteBatch.Draw(renderer.BackgroundTexture, Vector2.Zero, Color.White);

            renderer.SpriteSystem.Draw(renderer, Entities);

            // Draw stat displays
            HealthBar.Draw(renderer);
            GearDisplay.Draw(renderer);

            //DrawDebug();
        }
    }
}
