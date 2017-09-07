using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpSTG
{
    class StgFrame
    {
        public static StgFrame Global { get; private set; }
        public void SetAsGlobal() { Global = this; }


        public float HorizontalRange { get; private set; }
        public float VerticalRange { get; private set; }
        public Vector3 OriginOffset { get; private set; }

        public StgFrame(Control control, float range = 100.0f)
        {
            if (control.Width < control.Height)
            {
                HorizontalRange = range;
                VerticalRange = control.Height * range / control.Width;
            }
            else
            {
                VerticalRange = range;
                HorizontalRange = control.Width * range / control.Height;
            }
        }

        public void SetMatrix(Device device)
        {
            device.SetTransform(TransformState.Projection, Matrix.OrthoLH(HorizontalRange, VerticalRange, 0, 1));
        }

        public Vector3 InBox(Vector3 input, float marginX = 0, float marginY=0)
        {
            return new Vector3(
            Math.Min(Math.Max(input.X, -HorizontalRange / 2+marginX), HorizontalRange / 2-marginX),
            Math.Min(Math.Max(input.Y, -VerticalRange / 2+marginY), VerticalRange / 2-marginY),
            input.Z
            );
        }
    }
}
