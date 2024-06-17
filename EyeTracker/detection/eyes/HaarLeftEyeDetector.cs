using Emgu.CV;
using EyeTracker.detection.utlis;

namespace EyeTracker.detection.eyes
{
    internal class HaarLeftEyeDetector
    {
        private CascadeClassifier leftEyeClassifier = new CascadeClassifier("./classifiers/haarcascade_lefteye_2splits.xml");
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
            int sectionWidth = frame.Cols / 2;
            Rectangle leftFaceSide = new Rectangle(sectionWidth, 0, sectionWidth, frame.Rows);
            Mat croppedImg = new Mat(frame, leftFaceSide);

            var leftEyes = leftEyeClassifier.DetectMultiScale(croppedImg, scaleFactor, minNeighbours);
            if (leftEyes.Length == 0)
            {
                Position = new Point(-1, -1);
                return frame;
            }

            var averagedDetection = RectanglesUtil.SpatialSmoothing(leftEyes);
            var smoothedDetection = RectanglesUtil.TemporalSmoothing(prevDetection.Rects.ToArray(), averagedDetection);

            // OFFSET DETECTION COORDINATES
            var newPosition = new Point(smoothedDetection.X + sectionWidth, smoothedDetection.Y);

            Position = newPosition;
            prevDetection.AddResult(smoothedDetection);

            return new Mat(frame, smoothedDetection);
        }
    }
}
