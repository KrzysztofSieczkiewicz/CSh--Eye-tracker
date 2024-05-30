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
                CvInvoke.GaussianBlur(frameGray, frameBlurred, new Size(3, 3), 0);
                CvInvoke.EqualizeHist(frameGray, frameEqualized);

                var faces = faceCascade.DetectMultiScale(frameBlurred, 1.2, 10);
                var leftEye = leftEyeCascade.DetectMultiScale(frameBlurred, 1.15, 8);
                var rightEye = rightEyeCascade.DetectMultiScale(frameBlurred, 1.15, 8);

                if (faces != null && faces.Length > 0)
                    CvInvoke.Rectangle(frame, faces[0], new MCvScalar(0, 255, 0), 2);

                if (leftEye != null && leftEye.Length > 0)
                    CvInvoke.Rectangle(frame, leftEye[0], new MCvScalar(255, 0, 0), 1);

                if (rightEye != null && rightEye.Length > 0)
                    CvInvoke.Rectangle(frame, rightEye[0], new MCvScalar(255, 0, 0), 1);


                CvInvoke.Imshow("faceDetection", frame);

                if (CvInvoke.WaitKey(1) == 27)
                    break;
            }
        }

    }
}