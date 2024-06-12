using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeTracker.detection.utils
{
    internal static class FaceOrientationUtil
    {
        public static double CalculateFaceYaw(Point leftEyeCenter, Point rightEyeCenter, Point noseCenter)
        {
            double yaw = Math.Atan2(noseCenter.Y - ((leftEyeCenter.Y + rightEyeCenter.Y) / 2), noseCenter.X - ((leftEyeCenter.X + rightEyeCenter.X) / 2));
            yaw = (yaw * 180 / Math.PI) - 90;

            return yaw;
        }

        public static double CalculateFacePitch(Point leftEyeCenter, Point rightEyeCenter, Point noseCenter)
        {
            double eyeHorizontalDistance = Math.Abs(leftEyeCenter.X - rightEyeCenter.X);
            double eyeNoseVerticalDistance = Math.Abs((leftEyeCenter.Y + rightEyeCenter.Y) / 2 - noseCenter.Y);
            double eyeVerticalDistance = Math.Abs(leftEyeCenter.Y - rightEyeCenter.Y);

            double pitch = Math.Acos(eyeNoseVerticalDistance / eyeHorizontalDistance);
            pitch = (pitch * 180 / Math.PI) - 45;

            return pitch;
        }

        public static double CalculateFaceRoll(Point leftEyeCenter, Point rightEyeCenter)
        {
            double roll = Math.Atan2(rightEyeCenter.Y - leftEyeCenter.Y, rightEyeCenter.X - leftEyeCenter.X);
            roll = roll * 180 / Math.PI;

            return roll;
        }
    }
}
