using Emgu.CV;

namespace EyeTracker.detection
{
    internal interface IFeatureDetector
    {
        Point Position { get; set; }

        Mat Detect(Mat frame);
    }
}
