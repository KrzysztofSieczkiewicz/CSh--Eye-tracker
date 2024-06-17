using Emgu.CV;
using EyeTracker.detection.utlis;

namespace EyeTracker.detection.eyes
{
    internal class HaarRightEyeDetector
    {
        private CascadeClassifier rightEyeClassifier = new CascadeClassifier("./classifiers/haarcascade_righteye_2splits.xml");
        private PrevDetections prevDetection = new PrevDetections(3);

        double scaleFactor = 1.1;
        int minNeighbours = 2;

        private Point _position = new Point();
        public Point Position
        {
            get => _position;
            set => _position = value;
        }

        public Mat Detect(Mat frame)
        {
            Rectangle rightFaceSide = new Rectangle(0, 0, frame.Cols / 2, frame.Rows);
            Mat croppedImg = new Mat(frame, rightFaceSide);

            var rightEyes = rightEyeClassifier.DetectMultiScale(croppedImg, scaleFactor, minNeighbours);
            if (rightEyes.Length == 0)
            {
                Position = new Point(-1, -1);
                return frame;
            }

            var averagedDetection = RectanglesUtil.SpatialSmoothing(rightEyes);
            var smoothedDetection = RectanglesUtil.TemporalSmoothing(prevDetection.Rects.ToArray(), averagedDetection);

            Position = smoothedDetection.Location;
            prevDetection.AddResult(smoothedDetection);

            return new Mat(frame, smoothedDetection);
        }
    }
}
