using Emgu.CV;
using Emgu.CV.Structure;
using EyeTracker.detection;
using EyeTracker.detection.eyes;
using EyeTracker.detection.face;
using EyeTracker.detection.nose;
using EyeTracker.detection.utils;
using EyeTracker.detection.utlis;

namespace EyeTracker
{
    internal static class Program
    {

        [STAThread]
        static void Main()
        {
            newDetection();
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

        // TODO: INTRODUCE ERROR HANDLING

        public static void newDetection()
        {
            var videoCapture = new VideoCapture(0, VideoCapture.API.DShow);

            Mat frame = new();
            Mat frameGray = new();
            Mat frameBlurred = new();
            Mat frameEqualized = new();

            IFeatureDetector faceDetector = new HaarFaceDetector();
            IFeatureDetector noseDetector = new HaarNoseDetector();
            IFeatureDetector lEyeDetector = new HaarLeftEyeDetector();
            IFeatureDetector rEyeDetector = new HaarRightEyeDetector();

            while (true)
            {
                // CAPTURE AND PROCESS THE IMAGE
                videoCapture.Read(frame);
                CvInvoke.CvtColor(frame, frameGray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                CvInvoke.GaussianBlur(frameGray, frameBlurred, new Size(5, 5), 0);
                CvInvoke.EqualizeHist(frameGray, frameEqualized);

                // DETECT FACE AND CROP IMAGE
                var face = faceDetector.Detect(frameEqualized);
                Mat frameFaceCropped = new Mat(frameEqualized, face);

                // DETECT EYES AND NOSE
                var nose = noseDetector.Detect(frameFaceCropped);
                var leftEye = lEyeDetector.Detect(frameFaceCropped);
                var rightEye = rEyeDetector.Detect(frameFaceCropped);

                // GET CENTERS FOR DETECTED RECTANGLES
                var noseCenter = RectanglesUtil.GetCenter(nose);
                var leftEyeCenter = RectanglesUtil.GetCenter(leftEye);
                var rightEyeCenter = RectanglesUtil.GetCenter(rightEye);

                // CALCULATE FACE ORIENTATION
                double yaw = FaceOrientationUtil.CalculateFaceYaw(leftEyeCenter, rightEyeCenter, noseCenter);
                double pitch = FaceOrientationUtil.CalculateFacePitch(leftEyeCenter, rightEyeCenter, noseCenter);
                double roll = FaceOrientationUtil.CalculateFaceRoll(leftEyeCenter, rightEyeCenter);

                // PUT RECTANGLES ON IMAGE
                CvInvoke.Rectangle(frameFaceCropped, face, new MCvScalar(0, 255, 0), 2);
                CvInvoke.Rectangle(frameFaceCropped, nose, new MCvScalar(255, 0, 0), 1);
                CvInvoke.Rectangle(frameFaceCropped, leftEye, new MCvScalar(255, 0, 0), 1);
                CvInvoke.Rectangle(frameFaceCropped, rightEye, new MCvScalar(255, 0, 0), 1);

                // PUT TEXT ON IMAGE
                CvInvoke.PutText(frameFaceCropped, "YAW: " + yaw.ToString(), new Point(15, 15), Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.75, new MCvScalar(255, 0, 0));
                //CvInvoke.PutText(frameFaceCropped, "ROLL: " + roll.ToString(), new Point(15, 15), Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.75, new MCvScalar(255, 0, 0));
                //CvInvoke.PutText(frameFaceCropped, "PITCH: " + pitch.ToString(), new Point(15, 15), Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.75, new MCvScalar(255, 0, 0));

                // SHOW IMAGE
                CvInvoke.Imshow("faceDetection", frameFaceCropped);

                // BREAK CONDITION
                if (CvInvoke.WaitKey(1) == 27)
                    break;
            }
        }
    }
}