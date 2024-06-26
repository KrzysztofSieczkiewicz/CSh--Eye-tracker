using Emgu.CV;
using Emgu.CV.Structure;
using EyeTracker.detection.eyes;
using EyeTracker.detection.face;
using EyeTracker.detection.nose;
using EyeTracker.detection.utils;

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

            HaarFaceDetector faceDetector = new HaarFaceDetector();
            HaarNoseDetector noseDetector = new HaarNoseDetector();
            HaarLeftEyeDetector lEyeDetector = new HaarLeftEyeDetector();
            HaarRightEyeDetector rEyeDetector = new HaarRightEyeDetector();

            while (true)
            {
                // CAPTURE AND PROCESS THE IMAGE
                videoCapture.Read(frame);
                CvInvoke.CvtColor(frame, frameGray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                CvInvoke.GaussianBlur(frameGray, frameBlurred, new Size(5, 5), 0);
                CvInvoke.EqualizeHist(frameGray, frameEqualized);

                // DETECT FACE AND CROP IMAGE
                Mat faceImg = faceDetector.Detect(frameEqualized);
                if (faceDetector.Position == new Point(-1, -1)) continue;

                // DETECT EYES AND NOSE
                var noseImg = noseDetector.Detect(faceImg);
                var leftEyeImg = lEyeDetector.Detect(faceImg);
                var rightEyeImg = rEyeDetector.Detect(faceImg);

                if (noseDetector.Position == new Point(-1, -1) || 
                    lEyeDetector.Position == new Point(-1, -1) || 
                    rEyeDetector.Position == new Point(-1, -1)) continue;

                // CALCULATE FACE ORIENTATION
                double yaw = FaceOrientationUtil.CalculateFaceYaw(lEyeDetector.Position, rEyeDetector.Position, noseDetector.Position);
                double pitch = FaceOrientationUtil.CalculateFacePitch(lEyeDetector.Position, rEyeDetector.Position, noseDetector.Position);
                double roll = FaceOrientationUtil.CalculateFaceRoll(lEyeDetector.Position, rEyeDetector.Position);

                // PUT RECTANGLES ON IMAGE
                //CvInvoke.Rectangle(frameFaceCropped, nose, new MCvScalar(255, 0, 0), 1);
                //CvInvoke.Rectangle(frameFaceCropped, leftEye, new MCvScalar(255, 0, 0), 1);
                //CvInvoke.Rectangle(frameFaceCropped, rightEye, new MCvScalar(255, 0, 0), 1);

                // PUT TEXT ON IMAGE
                CvInvoke.PutText(faceImg, "YAW: " + yaw.ToString(), new Point(15, 15), Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.75, new MCvScalar(255, 0, 0));
                //CvInvoke.PutText(frameEqualized, "ROLL: " + roll.ToString(), new Point(15, 15), Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.75, new MCvScalar(255, 0, 0));
                //CvInvoke.PutText(frameEqualized, "PITCH: " + pitch.ToString(), new Point(15, 15), Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.75, new MCvScalar(255, 0, 0));

                // SHOW IMAGE
                CvInvoke.Imshow("faceDetection", faceImg);

                // BREAK CONDITION
                if (CvInvoke.WaitKey(1) == 27)
                    break;
            }
        }
    }
}