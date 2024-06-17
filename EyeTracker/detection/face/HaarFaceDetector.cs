using Emgu.CV;
using EyeTracker.detection.utlis;

namespace EyeTracker.detection.face
{
    internal class HaarFaceDetector
    {
        private CascadeClassifier faceClassifier = new CascadeClassifier("./classifiers/haarcascade_frontalface_default.xml");
        private PrevDetections prevFrameDetection = new PrevDetections(3);

        private double scaleFactor = 1.2;
        private int minNeighbours = 4;
        private Size minSize = new Size(110, 110);
        private Size maxSize = new Size(275, 275);

        private Point _position = new Point();
        public Point Position
        {
            get => _position;
            set => _position = value;
        }

        public Mat Detect(Mat frame)
        {
            var faces = faceClassifier.DetectMultiScale(frame, scaleFactor, minNeighbours, minSize, maxSize);
            if (faces.Length == 0)
            {
                Position = new Point(-1, -1);
                return frame;
            }

            Rectangle averagedDetection = RectanglesUtil.SpatialSmoothing(faces);
            Rectangle smoothedDetection = RectanglesUtil.TemporalSmoothing(prevFrameDetection.Rects.ToArray(), averagedDetection);

            Position = smoothedDetection.Location;
            prevFrameDetection.AddResult(averagedDetection);

            return new Mat(frame, smoothedDetection);
        }

    }
}
