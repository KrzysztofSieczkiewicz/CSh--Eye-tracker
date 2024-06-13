namespace EyeTracker.detection.utlis
{
    internal static class RectanglesUtil
    {
        /* 
         * Averages provided rectangles
        */
        public static Rectangle SpatialSmoothing(Rectangle[] rectangles)
        {
            if (rectangles.Length == 1) return rectangles[0];

            int sumX=0;
            int sumY=0;
            int sumWidth=0;
            int sumHeight=0;

            foreach (var rectangle in rectangles)
            {
                sumX += rectangle.Location.X;
                sumY += rectangle.Location.Y;
                sumWidth += rectangle.Width;
                sumHeight += rectangle.Height;
            }

            int count = rectangles.Length;

            int avgX = sumX/ count;
            int avgY = sumY/ count;
            int avgWidth = sumWidth/ count;
            int avgHeight = sumHeight/ count;

            return new Rectangle(avgX, avgY, avgWidth, avgHeight);
        }

        /* 
         * Averages provided collection and averages it again with with current
        */
        public static Rectangle TemporalSmoothing(Rectangle[] prevFramesRect, Rectangle currFrameRect)
        {
            if (prevFramesRect.Length == 0) return currFrameRect;

            Rectangle prevFramesAveraged = SpatialSmoothing(prevFramesRect);

            Rectangle averagedPosition = new Rectangle();

            averagedPosition.X = (prevFramesAveraged.X + currFrameRect.X) / 2;
            averagedPosition.Y = (prevFramesAveraged.Y + currFrameRect.Y) / 2;
            averagedPosition.Width = (prevFramesAveraged.Width + currFrameRect.Width) / 2;
            averagedPosition.Height = (prevFramesAveraged.Height + currFrameRect.Height) / 2;

            return averagedPosition;
        }
    }
}
