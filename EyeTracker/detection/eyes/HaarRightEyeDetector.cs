using Emgu.CV;
using EyeTracker.detection.utlis;

namespace EyeTracker.detection.eyes
{
    internal class HaarRightEyeDetector : IFeatureDetector
    {
        private CascadeClassifier rightEyeClassifier = new CascadeClassifier("./classifiers/haarcascade_righteye_2splits.xml");
        private PrevDetections prevDetection = new PrevDetections(3);

        double scaleFactor = 1.1;
        int minNeighbours = 2;

        public Rectangle Detect(Mat frame)
        {
            Rectangle rightFaceSide = new Rectangle(0, 0, (frame.Cols / 2), (int)frame.Rows);
            Mat croppedImg = new Mat(frame, rightFaceSide);

            var rightEyes = rightEyeClassifier.DetectMultiScale(croppedImg, scaleFactor, minNeighbours);
            if (rightEyes.Length == 0) return new Rectangle();

            var averagedRightEye = RectanglesUtil.SpatialSmoothing(rightEyes);

            prevDetection.AddResult(averagedRightEye);
            var prevRightEyes = prevDetection.Rects.ToArray();

            var rightEye = RectanglesUtil.TemporalSmoothing(prevRightEyes, averagedRightEye);

            return rightEye;
        }
    }
}
