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

            var videoCapture = new VideoCapture(0, VideoCapture.API.DShow);

            Mat frame = new();
            Mat frameGray = new();
            Mat frameBlurred = new();
            Mat frameEqualized = new();

            Mat frameCropped = new();

            while (true)
            {
                videoCapture.Read(frame);

                CvInvoke.CvtColor(frame, frameGray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                CvInvoke.GaussianBlur(frameGray, frameBlurred, new Size(5, 5), 0);
                CvInvoke.EqualizeHist(frameGray, frameEqualized);

                var faces = DetectFace(frameEqualized);

                Rectangle[] leftEyes = new Rectangle[0];
                Rectangle[] rightEyes = new Rectangle[0];
                if (faces != null && faces.Length > 0)
                {
                    frameCropped = new Mat(frameEqualized, faces[0]);

                    leftEyes = DetectLeftEye(frameCropped);
                    rightEyes = DetectRightEye(frameCropped);
                }

                if (faces != null && faces.Length > 0)
                    foreach (var face in faces)
                        CvInvoke.Rectangle(frame, face, new MCvScalar(0, 255, 0), 2);
                if (faces != null && faces.Length == 0)
                    CvInvoke.Rectangle(frameCropped, new Rectangle(new Point(50, 50), new Size(100, 100)) , new MCvScalar(0, 255, 0), 2);

                if (leftEyes != null && leftEyes.Length > 0)
                    foreach (var leftEye in leftEyes)
                        CvInvoke.Rectangle(frameCropped, leftEye, new MCvScalar(255, 0, 0), 1);

                if (rightEyes != null && rightEyes.Length > 0)
                    foreach (var rightEye in rightEyes)
                        CvInvoke.Rectangle(frameCropped, rightEye, new MCvScalar(255, 0, 0), 1);


                CvInvoke.Imshow("faceDetection", frameCropped);

                if (CvInvoke.WaitKey(1) == 27)
                    break;
            }
        }

        public static Rectangle[] DetectFace(Mat image)
        {
            var faceClassifier = new CascadeClassifier("./detection/haarcascade_frontalface_default.xml");
            double scaleFactor = 1.2;
            int minNeighbours = 4;
            Size minSize = new Size(110, 110);
            Size maxSize = new Size(275, 275);

            var faceRectangles = faceClassifier.DetectMultiScale(image, scaleFactor, minNeighbours, minSize, maxSize);

            return faceRectangles;
        }

        public static Rectangle[] DetectLeftEye(Mat image)
        {
            var eyeClassifier = new CascadeClassifier("./detection/haarcascade_lefteye_2splits.xml");
            double scaleFactor = 1.15;
            int minNeighbours = 3;

            // Crop the image
            int centerX = (int)(image.Cols / 2);
            Rectangle cropArea = new Rectangle(centerX, 0, centerX, (int)image.Rows);
            Mat croppedImage = new Mat(image, cropArea);

            var leftEyeRectangles = eyeClassifier.DetectMultiScale(croppedImage, scaleFactor, minNeighbours);

            // Move rectangles to match original image coordinates
            for (int i = 0; i < leftEyeRectangles.Length; i++)
                leftEyeRectangles[i].X += centerX;

            return leftEyeRectangles;
        }

        public static Rectangle[] DetectRightEye(Mat image)
        {
            var eyeClassifier = new CascadeClassifier("./detection/haarcascade_righteye_2splits.xml");
            double scaleFactor = 1.15;
            int minNeighbours = 3;

            // Crop the image
            int centerX = (int)(image.Cols / 2);
            Rectangle cropArea = new Rectangle(0, 0, centerX, (int)image.Rows);
            Mat croppedImage = new Mat(image, cropArea);

            var rightEyeRectangles = eyeClassifier.DetectMultiScale(croppedImage, scaleFactor, minNeighbours);

            return rightEyeRectangles;
        }
    }
}