using Emgu.CV;

namespace EyeTracker.faceDetection
{
    internal class HaarFaceDetector : IFaceDetector
    {
        private CascadeClassifier faceClassifier;
        private Mat image;

        /* DEFAULT DETECTION SETTINGS */
        private double scaleFactor = 1.2;
        private int minNeighbours = 4;
        private Size minSize = new Size(110, 110);
        private Size maxSize = new Size(275, 275);


        public HaarFaceDetector() 
        {
            faceClassifier = new CascadeClassifier("./detection/haarcascade_frontalface_default.xml");

        }

        public DetectedFace DetectFace()
        {
            return new DetectedFace();
        }

    }
}
