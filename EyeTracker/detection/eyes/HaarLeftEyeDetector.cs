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
            int sectionWidth = (int)(frame.Cols / 2);
            Rectangle leftFaceSide = new Rectangle(sectionWidth, 0, sectionWidth, (int)frame.Rows);
            Mat croppedImg = new Mat(frame, leftFaceSide);

            var leftEyes = leftEyeClassifier.DetectMultiScale(croppedImg, scaleFactor, minNeighbours);
            if (leftEyes.Length == 0) return new Rectangle();

            var averagedLeftEye = RectanglesUtil.SpatialSmoothing(leftEyes);

            prevDetection.AddResult(averagedLeftEye);
            var prevLeftEyes = prevDetection.Rects.ToArray();

            var leftEye = RectanglesUtil.TemporalSmoothing(prevLeftEyes, averagedLeftEye);

            // Move rectangle to match original image coordinates
            leftEye.X += sectionWidth;

            return leftEye;
        }
    }
}
