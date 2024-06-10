using Emgu.CV;

namespace EyeTracker.detection
{
    internal interface IFeatureDetector
    {
        Rectangle Detect(Mat frame);
    }
}
