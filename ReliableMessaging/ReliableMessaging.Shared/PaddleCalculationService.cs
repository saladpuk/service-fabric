using System;
using System.Collections.Generic;
using System.Text;

namespace ReliableMessaging.Shared
{
    public class PaddleCalculationService
    {
        public CalculationResponse CalculatePosition(CalculationRequest req)
        {
            var random = new Random();
            var movement = (req.BallPositionY - (req.PlayerPositionY + req.PlayerHeight / 2)) * 0.1;
            movement += random.NextDouble() * 3;
            return new CalculationResponse
            {
                MovementPoistionY = movement
            };
        }
    }
}
