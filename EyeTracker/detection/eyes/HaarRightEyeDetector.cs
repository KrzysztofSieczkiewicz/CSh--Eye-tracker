using Emgu.CV;
using EyeTracker.detection.utlis;

namespace EyeTracker.detection.eyes
{
    internal class HaarEyesDetector : IFeatureDetector
    {
        private CascadeClassifier rightEyeClassifier = new CascadeClassifier("./classifiers/haarcascade_righteye_2splits.xml");
        private PrevDetections prevDetection = new PrevDetections(3);

        /* DEFAULT DETECTION SETTINGS */
        double scaleFactor = 1.1;
        int minNeighbours = 2;

        public Rectangle Detect(Mat frame)
        {
            var rightEyeRectangles = rightEyeClassifier.DetectMultiScale(frame, scaleFactor, minNeighbours);
            var averagedRightEye = RectanglesUtils.SpatialSmoothing(rightEyeRectangles);

            prevDetection.AddResult(averagedRightEye);
            var prevRightEyes = prevDetection.Rects.ToArray();

            var rightEye = RectanglesUtils.TemporalSmoothing(prevRightEyes, averagedRightEye);

            return rightEye;
        }
    }
}
