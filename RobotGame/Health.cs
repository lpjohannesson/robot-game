using RobotGame.Components;
using System;

namespace RobotGame
{
    internal class Health
    {
        public static void ModifyHealth(ref HealthComponent health, int newHealth)
        {
            health.Value = Math.Clamp(newHealth, 0, health.MaxValue);
        }
    }
}
