using System.Drawing;

namespace MozaicLand
{
    public class ColorCartridge
    {
        public int ColorIndex { get; private set; } = ColorTable.NoColor;
        public int InitialCount { get; private set; } = 0;
        public int CurrentCount { get; set; } = 0;
        
        public ColorCartridge(int colorIndex, int count)
        {
            ColorIndex = colorIndex;
            InitialCount = count;
            CurrentCount = count;
        }
    }
}
