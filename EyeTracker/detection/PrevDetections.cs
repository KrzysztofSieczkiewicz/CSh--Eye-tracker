namespace EyeTracker.detection
{
    internal class PrevDetections
    {
        private int limit = 3;
        public List<Rectangle> Rects { get; private set; } = new List<Rectangle>();

        public PrevDetections(int limit)
        {
            this.limit = limit;
        }

        public void AddResult(Rectangle newValue)
        {
            if (Rects.Count >= limit)
                Rects.RemoveAt(0);

            Rects.Add(newValue);
        }
    }
}
