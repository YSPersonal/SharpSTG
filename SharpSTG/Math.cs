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
    class Math
    {        
        public float HorizontalRange { get; private set; }
        public float VerticalRange { get; private set; }
        public Vector3 OriginOffset { get; private set; }

        public Math(float range = 100.0f)
        {
            if (Resource.form.Width < Resource.form.Height)
            {
                HorizontalRange = range;
                VerticalRange = Resource.form.Height * range / Resource.form.Width;
            }
            else
            {
                VerticalRange = range;
                HorizontalRange = Resource.form.Width * range / Resource.form.Height;
            }
        }

        public void SetCoordinate()
        {
            Resource.device.SetTransform(TransformState.Projection, Matrix.OrthoLH(HorizontalRange, VerticalRange, 0, 1));
        }

        public Vector3 Box(Vector3 input, float marginX = 0, float marginY=0)
        {
            return new Vector3(
            System.Math.Min(System.Math.Max(input.X, -HorizontalRange / 2+ marginX), HorizontalRange / 2- marginX),
            System.Math.Min(System.Math.Max(input.Y, -VerticalRange / 2+ marginY), VerticalRange / 2- marginY),
            input.Z
            );
        }

        public bool IsInBox(Vector3 input, float marginX = 0, float marginY = 0)
        {
            return input.X > -HorizontalRange / 2 + marginX &&
                input.X < HorizontalRange / 2 - marginX &&
                input.Y > -VerticalRange / 2 + marginY &&
                input.Y < VerticalRange / 2 - marginY;
        }

        public static bool HitDetect(Vector3 bullet, float bulletRadius, Vector3 target, float targetRadius)
        {
            return (target - bullet).LengthSquared() < System.Math.Pow(bulletRadius + targetRadius, 2);
        }        
    }
}
