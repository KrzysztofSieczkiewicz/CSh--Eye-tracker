using Emgu.CV;
using EyeTracker.detection.utlis;

namespace EyeTracker.detection.nose
{
    internal class HaarNoseDetector: IFeatureDetector
    {
        private CascadeClassifier noseClassifier = new CascadeClassifier("./classifiers/haarcascade_mcs_nose.xml");
        private PrevDetections prevDetection = new PrevDetections(3);

        double scaleFactor = 1.1;
        int minNeighbours = 6;

        private Point _Position = new Point();
        public Point Position
        {
            get => _Position;
            set => _Position = value;
        }

        public Mat Detect(Mat frame)
        {
            int sectionHeight = frame.Rows * 2 / 3;
            int startY = frame.Rows * 1 / 3;

            Rectangle sectionRect = new Rectangle(0, startY, frame.Cols, sectionHeight);
            Mat croppedImg = new Mat(frame, sectionRect);

            var noses = noseClassifier.DetectMultiScale(croppedImg, scaleFactor, minNeighbours);
            if (noses.Length == 0)
            {
                Position = new Point(-1, -1);
                return frame;
            }

            var averagedDetection = RectanglesUtil.SpatialSmoothing(noses);
            var smoothedDetection = RectanglesUtil.TemporalSmoothing(prevDetection.Rects.ToArray(), averagedDetection);

            // OFFSET DETECTION COORDINATES
            var newPosition = new Point(smoothedDetection.X, smoothedDetection.Y + sectionHeight);

            Position = newPosition;
            prevDetection.AddResult(smoothedDetection);

            return new Mat(frame, smoothedDetection);
        }
    }
}
