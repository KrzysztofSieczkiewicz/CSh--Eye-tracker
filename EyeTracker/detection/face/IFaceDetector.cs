using Emgu.CV;

namespace EyeTracker.detection.face
{
    internal interface IFaceDetector
    {
        Rectangle DetectFace(Mat frame);
    }
}
