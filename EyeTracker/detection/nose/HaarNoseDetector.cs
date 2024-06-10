using Emgu.CV;
using EyeTracker.detection.utlis;

namespace EyeTracker.detection.nose
{
    internal class HaarNoseDetector : IFeatureDetector
    {
        private CascadeClassifier noseClassifier = new CascadeClassifier("./classifiers/haarcascade_mcs_nose.xml");
        private PrevDetections prevDetection = new PrevDetections(3);

        /* DEFAULT DETECTION SETTINGS */
        double scaleFactor = 1.1;
        int minNeighbours = 2;

        public Rectangle Detect(Mat frame)
        {
            var noseRect = noseClassifier.DetectMultiScale(frame, scaleFactor, minNeighbours);
            var averagedNose = RectanglesUtils.SpatialSmoothing(noseRect);

            prevDetection.AddResult(averagedNose);
            var prevNoses = prevDetection.Rects.ToArray();

            var nose = RectanglesUtils.TemporalSmoothing(prevNoses, averagedNose);

            return nose;
        }
    }
}
