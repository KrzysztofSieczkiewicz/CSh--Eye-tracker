using Emgu.CV;
using EyeTracker.detection.utlis;

namespace EyeTracker.detection.face
{
    internal class HaarFaceDetector : IFeatureDetector
    {
        private CascadeClassifier faceClassifier = new CascadeClassifier("./classifiers/haarcascade_frontalface_default.xml");
        private PrevDetections prevFrameDetection = new PrevDetections(3);

        private double scaleFactor = 1.2;
        private int minNeighbours = 4;
        private Size minSize = new Size(110, 110);
        private Size maxSize = new Size(275, 275);


        public Rectangle Detect(Mat frame)
        {
            var faces = faceClassifier.DetectMultiScale(frame, scaleFactor, minNeighbours, minSize, maxSize);
            if (faces.Length == 0) return new Rectangle();

            var averagedFace = RectanglesUtil.SpatialSmoothing(faces);
            prevFrameDetection.AddResult(averagedFace);

            var prevFramesRect = prevFrameDetection.Rects.ToArray();
            var face = RectanglesUtil.TemporalSmoothing(prevFramesRect, averagedFace);

            return face;
        }

    }
}
