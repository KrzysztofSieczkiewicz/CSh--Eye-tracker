using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeTracker.faceDetection
{
    internal interface IFaceDetector
    {
        DetectedFace DetectFace();
    }
}
