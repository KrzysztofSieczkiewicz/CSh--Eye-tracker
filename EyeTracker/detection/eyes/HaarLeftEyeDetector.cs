using Emgu.CV;
using EyeTracker.detection.utlis;

namespace EyeTracker.detection.eyes
{
    // TODO: FIX THIS MESS PLS
    internal class HaarLeftEyeDetector : IFeatureDetector
    {
        private CascadeClassifier leftEyeClassifier = new CascadeClassifier("./classifiers/haarcascade_lefteye_2splits.xml");
        private PrevDetections prevDetection = new PrevDetections(3);

        /* DEFAULT DETECTION SETTINGS */
        double scaleFactor = 1.1;
        int minNeighbours = 2;

        public Rectangle Detect(Mat frame)
        {
            var leftEyeRectangles = leftEyeClassifier.DetectMultiScale(frame, scaleFactor, minNeighbours);
            var averagedLeftEye = RectanglesUtils.SpatialSmoothing(leftEyeRectangles);

            prevDetection.AddResult(averagedLeftEye);
            var prevLeftEyes = prevDetection.Rects.ToArray();

            var leftEye = RectanglesUtils.TemporalSmoothing(prevLeftEyes, averagedLeftEye);

            return leftEye;
        }
    }
}
