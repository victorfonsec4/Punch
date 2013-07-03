using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Punch
{
    class Player
    {
        int health;
        Vector3 rightHandPos, leftHandPos, lastRightHandPos, lastLeftHandPos;

        public Player(int health)
        {
            this.health = health;
            rightHandPos = Vector3.Zero;
            leftHandPos = Vector3.Zero;
            lastLeftHandPos = Vector3.Zero;
            lastRightHandPos = Vector3.Zero;
        }

        public Vector2 Update(Vector3 rightHandPos, Vector3 leftHandPos, GameTime gameTime)
        {
            this.rightHandPos = rightHandPos;
            this.leftHandPos = leftHandPos;
            if (lastLeftHandPos.Z - leftHandPos.Z > 0.1)
                return new Vector2(leftHandPos.X, leftHandPos.Y);
            else if (lastRightHandPos.Z - rightHandPos.Z > 0.1)
                return new Vector2(rightHandPos.X, rightHandPos.Y);
            Debug.WriteLine("RightHandPos.Z:" + rightHandPos.Z);
            Debug.WriteLine("LastRightHandPos.Z:" + lastRightHandPos.Z);
            lastRightHandPos = rightHandPos;
            lastLeftHandPos = leftHandPos;
            return Vector2.Zero;
        }
        
    }
}
