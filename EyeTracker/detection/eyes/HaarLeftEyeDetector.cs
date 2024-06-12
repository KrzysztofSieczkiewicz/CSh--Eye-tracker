using Emgu.CV;
using EyeTracker.detection.utlis;

namespace EyeTracker.detection.eyes
{
    internal class HaarLeftEyeDetector : IFeatureDetector
    {
        private CascadeClassifier leftEyeClassifier = new CascadeClassifier("./classifiers/haarcascade_lefteye_2splits.xml");
        private PrevDetections prevDetection = new PrevDetections(3);

        double scaleFactor = 1.1;
        int minNeighbours = 2;

        public Rectangle Detect(Mat frame)
        {
            int centerX = (int)(frame.Cols / 2);
            Rectangle leftFaceSide = new Rectangle(centerX, 0, centerX, (int)frame.Rows);
            Mat croppedImg = new Mat(frame, leftFaceSide);

            var leftEyeRectangles = leftEyeClassifier.DetectMultiScale(croppedImg, scaleFactor, minNeighbours);
            var averagedLeftEye = RectanglesUtil.SpatialSmoothing(leftEyeRectangles);

            prevDetection.AddResult(averagedLeftEye);
            var prevLeftEyes = prevDetection.Rects.ToArray();

            var leftEye = RectanglesUtil.TemporalSmoothing(prevLeftEyes, averagedLeftEye);

            // Move rectangle to match original image coordinates
            leftEye.X += centerX;

            return leftEye;
        }
    }
}
