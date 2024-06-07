using Emgu.CV;
using Emgu.CV.Structure;

namespace EyeTracker
{
    internal static class Program
    {

        [STAThread]
        static void Main()
        {
            Detection();
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

        public static void Detection()
        {
            var videoCapture = new VideoCapture(0, VideoCapture.API.DShow);

            Mat frame = new();
            Mat frameGray = new();
            Mat frameBlurred = new();
            Mat frameEqualized = new();

            Rectangle[] prevFrameFaces = new Rectangle[0];
            Rectangle[] prevFrameNoses = new Rectangle[0];
            Rectangle[] prevFrameLeftEyes = new Rectangle[0];
            Rectangle[] prevFrameRightEyes = new Rectangle[0];

            while (true)
            {
                // CAPTURE AND PROCESS THE IMAGE
                videoCapture.Read(frame);
                CvInvoke.CvtColor(frame, frameGray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                CvInvoke.GaussianBlur(frameGray, frameBlurred, new Size(5, 5), 0);
                CvInvoke.EqualizeHist(frameGray, frameEqualized);
                

                // DETECT FACE
                var faces = DetectFace(frameEqualized);
                var face = TemporalSmoothing(prevFrameFaces, faces); // TODO: MOVE THIS INTO DETECTION METHOD?

                Mat frameFaceCropped = new Mat(frameEqualized, face);

                // DETECT FEATURES
                var noses = DetectNose(frameFaceCropped);
                var nose = TemporalSmoothing(prevFrameNoses, noses);
                var leftEyes = DetectLeftEye(frameFaceCropped);
                var leftEye = TemporalSmoothing(prevFrameLeftEyes, leftEyes);
                var rightEyes = DetectRightEye(frameFaceCropped);
                var rightEye = TemporalSmoothing(prevFrameRightEyes, rightEyes);

                // OVERWRITE PREVIOUS FRAME
                prevFrameFaces = faces;
                prevFrameNoses = noses;
                prevFrameLeftEyes = leftEyes;
                prevFrameRightEyes = rightEyes;

                if (noses != null && noses.Length > 0
                    && leftEyes != null && leftEyes.Length > 0
                    && rightEyes != null && rightEyes.Length > 0)
                {
                    // TAKE PICTURE OF EACH EYE
                    Mat frameLeftEyeCropped = new Mat(frameFaceCropped, leftEyes[0]);
                    Mat frameRightEyeCropped = new Mat(frameFaceCropped, leftEyes[0]);

                    // TODO: CALCULATE VARIATION OF FACE POSITION -> IF TOO LARGE - skip frame (also can be used for creating gaze radius)
                    // TODO: BASED ON EYES RECTANGLES POSITION -> CALCULATE FACE ORIENTATION IN RELATION TO THE SCREEN NORMAL AXIS
                    // TODO: RECOGNIZE EYE IRIS FOR EACH EYE


                    if (faces != null && faces.Length > 0)
                        CvInvoke.Rectangle(frameFaceCropped, face, new MCvScalar(0, 255, 0), 2);
                    if (noses != null && noses.Length > 0)
                        foreach(var noser in noses)
                        CvInvoke.Rectangle(frameFaceCropped, noser, new MCvScalar(255, 0, 0), 1);
                    if (leftEyes != null && leftEyes.Length > 0)
                        CvInvoke.Rectangle(frameFaceCropped, leftEye, new MCvScalar(255, 0, 0), 1);
                    if (rightEyes != null && rightEyes.Length > 0)
                        CvInvoke.Rectangle(frameFaceCropped, rightEye, new MCvScalar(255, 0, 0), 1);

                    var leftEyeCenter = new Point(leftEyes[0].Location.X + leftEyes[0].Width, leftEyes[0].Location.Y + leftEyes[0].Height);
                    var rightEyeCenter = new Point(rightEyes[0].Location.X + rightEyes[0].Width, rightEyes[0].Location.Y + rightEyes[0].Height);
                    var noseCenter = new Point(noses[0].Location.X + noses[0].Width, noses[0].Location.Y + noses[0].Height);

                    var yaw = CalculateFaceYaw(leftEyeCenter, rightEyeCenter, noseCenter);
                    var roll = CalculateFaceRoll(leftEyeCenter, rightEyeCenter);
                    var pitch = CalculateFacePitch(leftEyeCenter, rightEyeCenter, noseCenter);

                    CvInvoke.PutText(frameFaceCropped, "YAW: " + yaw.ToString(), new Point(15, 15), Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.75, new MCvScalar(255, 0, 0));
                    //CvInvoke.PutText(frameFaceCropped, "ROLL: " + roll.ToString(), new Point(15, 15), Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.75, new MCvScalar(255, 0, 0));
                    //CvInvoke.PutText(frameFaceCropped, "PITCH: " + pitch.ToString(), new Point(15, 15), Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.75, new MCvScalar(255, 0, 0));

                    CvInvoke.Imshow("faceDetection", frameFaceCropped);
                }
                

                if (CvInvoke.WaitKey(1) == 27)
                    break;
            }
        }

        private static Rectangle[] DetectFace(Mat image)
        {
            var faceClassifier = new CascadeClassifier("./classifiers/haarcascade_frontalface_default.xml");
            double scaleFactor = 1.2;
            int minNeighbours = 4;
            Size minSize = new Size(110, 110);
            Size maxSize = new Size(275, 275);

            var faceRectangles = faceClassifier.DetectMultiScale(image, scaleFactor, minNeighbours, minSize, maxSize);

            return faceRectangles;
        }

        private static Rectangle[] DetectLeftEye(Mat image)
        {
            var eyeClassifier = new CascadeClassifier("./classifiers/haarcascade_lefteye_2splits.xml");
            double scaleFactor = 1.1;
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

        private static Rectangle[] DetectRightEye(Mat image)
        {
            var eyeClassifier = new CascadeClassifier("./classifiers/haarcascade_righteye_2splits.xml");
            double scaleFactor = 1.1;
            int minNeighbours = 3;

            // Crop the image
            int centerX = (int)(image.Cols / 2);
            Rectangle cropArea = new Rectangle(0, 0, centerX, (int)image.Rows);
            Mat croppedImage = new Mat(image, cropArea);

            var rightEyeRectangles = eyeClassifier.DetectMultiScale(croppedImage, scaleFactor, minNeighbours);

            return rightEyeRectangles;
        }

        private static Rectangle[] DetectNose(Mat image)
        {
            var noseClassifier = new CascadeClassifier("./classifiers/haarcascade_mcs_nose.xml");
            double scaleFactor = 1.1;
            int minNeighbours = 8;

            var rightEyeRectangles = noseClassifier.DetectMultiScale(image, scaleFactor, minNeighbours);

            return rightEyeRectangles;
        }

        private static Rectangle TemporalSmoothing(Rectangle[] oldPositions, Rectangle[] newPositions)
        {
            if (newPositions.Length == 0) return default;

            Rectangle averagedPosition = newPositions[0];

            for (int i = 1; i < newPositions.Length; i++)
            {
                averagedPosition.X = (averagedPosition.X + newPositions[i].X) / 2;
                averagedPosition.Y = (averagedPosition.Y + newPositions[i].Y) / 2;
                averagedPosition.Width = (averagedPosition.Width + newPositions[i].Width) / 2;
                averagedPosition.Height = (averagedPosition.Height + newPositions[i].Height) / 2;
            }

            return averagedPosition;
        }


        private static double CalculateFaceYaw(Point leftEyeCenter, Point rightEyeCenter, Point noseCenter)
        {
            double yaw = Math.Atan2(noseCenter.Y - ((leftEyeCenter.Y + rightEyeCenter.Y) / 2), noseCenter.X - ((leftEyeCenter.X + rightEyeCenter.X) / 2));

            yaw = (yaw * 180 / Math.PI) - 90;

            return yaw;
        }

        private static double CalculateFacePitch(Point leftEyeCenter, Point rightEyeCenter, Point noseCenter)
        {
            double eyeHorizontalDistance = Math.Abs(leftEyeCenter.X - rightEyeCenter.X);
            double eyeNoseVerticalDistance = Math.Abs((leftEyeCenter.Y + rightEyeCenter.Y) / 2 - noseCenter.Y);
            double eyeVerticalDistance = Math.Abs(leftEyeCenter.Y - rightEyeCenter.Y);

            double pitch = Math.Acos(eyeNoseVerticalDistance / eyeHorizontalDistance);

            pitch = (pitch * 180 / Math.PI) - 45;

            return pitch;
        }

        private static double CalculateFaceRoll(Point leftEyeCenter, Point rightEyeCenter)
        {
            double roll = Math.Atan2(rightEyeCenter.Y - leftEyeCenter.Y, rightEyeCenter.X - leftEyeCenter.X);

            roll = roll * 180 / Math.PI;

            return roll;
        }
    }
}