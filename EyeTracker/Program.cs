using Emgu.CV;
using Emgu.CV.Structure;
using System.Diagnostics;

namespace EyeTracker
{
    internal static class Program
    {

        [STAThread]
        static void Main()
        {

            FaceDetection();
            /*
            Mat picture = new Mat();

            string win1 = "Test Window (Press any key to close)";

            CvInvoke.NamedWindow(win1);
            using (Mat frame = new Mat())
            using (VideoCapture capture = new VideoCapture())
                while(CvInvoke.WaitKey(1) == -1)
                {
                    capture.Read(frame);
                    CvInvoke.Imshow(win1, frame);
                }
            */

            /*
            picture = CvInvoke.Imread("./img/test0.png");
            CvInvoke.Imshow("Screenshot", picture);
            CvInvoke.WaitKey();
            */

            //ApplicationConfiguration.Initialize();
            //Application.Run(new Form1());
        }

        public static void FaceDetection()
        {
            var faceCascade = new CascadeClassifier("./detection/haarcascade_frontalface_default.xml");
            var leftEyeCascade = new CascadeClassifier("./detection/haarcascade_lefteye_2splits.xml");
            var rightEyeCascade = new CascadeClassifier("./detection/haarcascade_righteye_2splits.xml");

            var videoCapture = new VideoCapture(0, Emgu.CV.VideoCapture.API.DShow);

            Mat frame = new();
            Mat frameGray = new();
            Mat frameBlurred = new();
            Mat frameEqualized = new();

            while (true)
            {
                videoCapture.Read(frame);

                CvInvoke.CvtColor(frame, frameGray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                CvInvoke.GaussianBlur(frameGray, frameBlurred, new Size(5, 5), 0);
                CvInvoke.EqualizeHist(frameGray, frameEqualized);

                var faces = faceCascade.DetectMultiScale(frameEqualized, 1.2, 10);
                var leftEye = leftEyeCascade.DetectMultiScale(frameEqualized, 1.15, 12);
                var rightEye = rightEyeCascade.DetectMultiScale(frameEqualized, 1.15, 12);

                if (faces != null && faces.Length > 0)
                    CvInvoke.Rectangle(frameEqualized, faces[0], new MCvScalar(0, 255, 0), 2);

                if (leftEye != null && leftEye.Length > 0)
                    CvInvoke.Rectangle(frameEqualized, leftEye[0], new MCvScalar(255, 0, 0), 1);

                if (rightEye != null && rightEye.Length > 0)
                    CvInvoke.Rectangle(frameEqualized, rightEye[0], new MCvScalar(255, 0, 0), 1);

                CvInvoke.Rectangle(frameEqualized, new Rectangle(new Point(10, 10), new Size(50, 50)), new MCvScalar(255, 0, 0), 1);


                CvInvoke.Imshow("faceDetection", frameEqualized);

                if (CvInvoke.WaitKey(1) == 27)
                    break;
            }
        }

        public static Rectangle[] DetectFace(Mat image)
        {
            var faceClassifier = new CascadeClassifier("./detection/haarcascade_frontalface_default.xml");
            double scaleFactor = 1.2;
            int minNeighbours = 10;
            Size minSize = new Size(110, 110);
            Size maxSize = new Size(275, 275);

            var faceRectangles = faceClassifier.DetectMultiScale(image, scaleFactor, minNeighbours, minSize, maxSize);

            return faceRectangles;
        }

        public static Rectangle[] DetectLeftEye(Mat image)
        {
            var eyeClassifier = new CascadeClassifier("./detection/haarcascade_lefteye_2splits.xml");
            double scaleFactor = 1.15;
            int minNeighbours = 12;

            var leftEyeRectangles = eyeClassifier.DetectMultiScale(image, scaleFactor, minNeighbours);

            return leftEyeRectangles;
        }

    }
}