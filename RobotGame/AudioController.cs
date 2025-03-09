using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace RobotGame
{
    public class AudioController
    {
        public Song Music;
        public SoundEffect HurtSound, EnemyHurtSound, BatterySound, GearSound;

        public RobotGame Game;

        public AudioController(RobotGame game)
        {
            Game = game;
        }

        public void Initialize()
        {
            // Enable looping
            MediaPlayer.IsRepeating = true;

            ContentManager content = Game.Content;

            Music = content.Load<Song>("Sounds/music");

            HurtSound = content.Load<SoundEffect>("Sounds/hurt");
            EnemyHurtSound = content.Load<SoundEffect>("Sounds/enemy_hurt");
            BatterySound = content.Load<SoundEffect>("Sounds/battery");
            GearSound = content.Load<SoundEffect>("Sounds/gear");
        }
    }
}
