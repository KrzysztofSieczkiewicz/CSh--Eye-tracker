using Emgu.CV;
using EyeTracker.detection.utlis;

namespace EyeTracker.detection.face
{
    internal class HaarFaceDetector : IFaceDetector
    {
        private CascadeClassifier faceClassifier = new CascadeClassifier("./classifiers/haarcascade_frontalface_default.xml");

        /* DEFAULT DETECTION SETTINGS */
        private double scaleFactor = 1.2;
        private int minNeighbours = 4;
        private Size minSize = new Size(110, 110);
        private Size maxSize = new Size(275, 275);

        /* */
        private PrevDetections prevFrameDetection = new PrevDetections(3);

        public Rectangle DetectFace(Mat frame)
        {
            var faceClassifier = new CascadeClassifier("./classifiers/haarcascade_frontalface_default.xml");
            double scaleFactor = 1.2;
            int minNeighbours = 4;
            Size minSize = new Size(110, 110);
            Size maxSize = new Size(275, 275);

            var faces = faceClassifier.DetectMultiScale(frame, scaleFactor, minNeighbours, minSize, maxSize);
            var averagedRect = DetectionUtils.SpatialSmoothing(faces);

            prevFrameDetection.AddResult(averagedRect);
            var prevFramesRect = prevFrameDetection.Rects.ToArray();

            var smoothedRect = DetectionUtils.TemporalSmoothing(prevFramesRect, averagedRect);

            return smoothedRect;
        }

    }
}
