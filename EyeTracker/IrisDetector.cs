using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace EyeTracker
{
    internal class IrisDetector
    {
        int thresholdLower = 50;
        int thresholdUpper = 150;
        public void Detect(Mat img) {

            CircleF[] circles = CvInvoke.HoughCircles(
                img,
                HoughModes.Gradient,
                dp: 1,  // The inverse ratio of the accumulator resolution to the image resolution
                minDist: img.Rows / 8,  // Minimum distance between the centers of the detected circles
                param1: 100,  // The higher threshold of the two passed to the Canny edge detector
                param2: 30,  // The accumulator threshold for the circle centers at the detection stage
                minRadius: 0,  // Minimum radius of the circles to be detected
                maxRadius: 0);  // Maximum radius of the circles to be detected

            foreach (CircleF circle in circles)
            {
                CvInvoke.Circle(img, Point.Round(circle.Center), (int)circle.Radius, new Bgr(Color.Red).MCvScalar, 2);
            }
        }
    }
}
