using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSTG
{
    class Debug
    {
        //Device device;
        static Font font;
        public static RawColorBGRA DefaultFontColor { get; set; }
        static int height = 16;
        static int width = 8;
        public static void Init(Device device)
        {
            FontDescription desc;
            desc.CharacterSet = FontCharacterSet.GB2312;
            desc.FaceName = "";
            desc.Height = height;
            desc.Width = width;
            desc.Italic = false;
            desc.MipLevels = 1;
            desc.OutputPrecision = FontPrecision.Default;
            desc.PitchAndFamily = FontPitchAndFamily.Default;
            desc.Quality = FontQuality.Default;
            desc.Weight = FontWeight.Light;

            font = new Font(device, desc);
            DefaultFontColor = SharpDX.Color.White;
        }
        public static void DrawText(string text, int row, int column, RawColorBGRA color)
        {
            font.DrawText(null, text, column * width, row * height, color);
        }
        public static void DrawText(string text, int row, int column)
        {
            font.DrawText(null, text, column * width, row * height, DefaultFontColor);
        }
    }
}
