namespace MozaicLand
{
    public struct ColorCartridge
    {
        public int ColorIndex { get; private set; }
        public int InitialCount { get; private set; }
        public int CurrentCount { get; set; }
        
        public ColorCartridge(int colorIndex, int count)
        {
            ColorIndex = colorIndex;
            InitialCount = count;
            CurrentCount = count;
        }
    }
}
