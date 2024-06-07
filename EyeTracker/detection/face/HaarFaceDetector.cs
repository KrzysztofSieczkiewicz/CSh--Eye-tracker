using Emgu.CV;
using EyeTracker.detection.utlis;

namespace EyeTracker.detection.face
{
    internal class HaarFaceDetector : IFaceDetector
    {
        private CascadeClassifier faceClassifier = new CascadeClassifier("./classifiers/haarcascade_frontalface_default.xml");

        private PrevDetections prevFrameDetection = new PrevDetections(3);

        /* DEFAULT DETECTION SETTINGS */
        private double scaleFactor = 1.2;
        private int minNeighbours = 4;
        private Size minSize = new Size(110, 110);
        private Size maxSize = new Size(275, 275);


        public Rectangle DetectFace(Mat frame)
        {
            var faces = faceClassifier.DetectMultiScale(frame, scaleFactor, minNeighbours, minSize, maxSize);
            var averagedFace = DetectionUtils.SpatialSmoothing(faces);

            prevFrameDetection.AddResult(averagedFace);
            var prevFramesRect = prevFrameDetection.Rects.ToArray();

            var face = DetectionUtils.TemporalSmoothing(prevFramesRect, averagedFace);

            return face;
        }

    }
}
