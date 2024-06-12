using Emgu.CV;
using EyeTracker.detection.utlis;

namespace EyeTracker.detection.nose
{
    internal class HaarNoseDetector : IFeatureDetector
    {
        private CascadeClassifier noseClassifier = new CascadeClassifier("./classifiers/haarcascade_mcs_nose.xml");
        private PrevDetections prevDetection = new PrevDetections(3);

        double scaleFactor = 1.25;
        int minNeighbours = 10;

        public Rectangle Detect(Mat frame)
        {
            // TODO: LIMIT DETECTION TO CERTAIN PERCENTAGE OF PICTURE (PREFERABLY AROUND CENTER)
            // MAYBE MIDDLE OF THREE COLUMNS
            // MAYBE BOTTOM 2/3?
            var noseRect = noseClassifier.DetectMultiScale(frame, scaleFactor, minNeighbours);
            var averagedNose = RectanglesUtil.SpatialSmoothing(noseRect);

            prevDetection.AddResult(averagedNose);
            var prevNoses = prevDetection.Rects.ToArray();

            var nose = RectanglesUtil.TemporalSmoothing(prevNoses, averagedNose);

            return nose;
        }
    }
}
