using Emgu.CV;
using EyeTracker.detection.utlis;

namespace EyeTracker.detection.eyes
{
    // TODO: FIX THIS MESS PLS
    internal class HaarEyesDetector
    {
        private CascadeClassifier leftEyeClassifier = new CascadeClassifier("./classifiers/haarcascade_lefteye_2splits.xml");
        private CascadeClassifier rightEyeClassifier = new CascadeClassifier("./classifiers/haarcascade_righteye_2splits.xml");

        private PrevDetections prevLeftEyesDetection = new PrevDetections(3);
        private PrevDetections prevRightEyesDetection = new PrevDetections(3);

        /* DEFAULT DETECTION SETTINGS */
        double scaleFactor = 1.1;
        int minNeighbours = 2;

        public Rectangle[] DetectEyes(Mat frame)
        {
            var leftEyeRectangles = leftEyeClassifier.DetectMultiScale(frame, scaleFactor, minNeighbours);
            var rightEyeRectangles = rightEyeClassifier.DetectMultiScale(frame, scaleFactor, minNeighbours);

            var averagedLeftEye = DetectionUtils.SpatialSmoothing(leftEyeRectangles);
            var averagedRightEye = DetectionUtils.SpatialSmoothing(rightEyeRectangles);

            prevLeftEyesDetection.AddResult(averagedLeftEye);
            prevRightEyesDetection.AddResult(averagedRightEye);

            var prevLeftEyes = prevLeftEyesDetection.Rects.ToArray();
            var prevRightEyes = prevRightEyesDetection.Rects.ToArray();

            var leftEye = DetectionUtils.TemporalSmoothing(prevLeftEyes, averagedLeftEye);
            var rightEye = DetectionUtils.TemporalSmoothing(prevRightEyes, averagedRightEye);

            Rectangle[] eyes = new Rectangle[2];
            eyes[0] = leftEye;
            eyes[1] = rightEye;

            return eyes;
        }
    }
}
