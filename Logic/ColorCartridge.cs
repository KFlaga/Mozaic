using System.Drawing;

namespace MozaicLand
{
    public class ColorCartridge
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

    public class ColorCartridgeSlot
    {
        public PointF TopLeft { get; set; }
        public SizeF Size { get; set; }
        public ColorCartridge Cartridge { get; set; }
    }
}
