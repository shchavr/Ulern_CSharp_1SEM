using System;

namespace func_rocket
{
    public class ControlTask
    {
        public static Turn ControlRocket(Rocket rocket, Vector target)
        {
            var distanceTarget = target - rocket.Location;
            var currentRocketDirection = rocket.Velocity.Angle * 2 / 3 + rocket.Direction / 3;
            var angleDifference = distanceTarget.Angle - currentRocketDirection;
            return angleDifference > 0 ? Turn.Right : Turn.Left;
        }
    }
}