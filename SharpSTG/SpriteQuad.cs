using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSTG
{
    class SpriteQuad
    {
        public static SpriteQuad Create(Device device, float width = 100, float height = 100)
        {
            SpriteQuad quad = new SpriteQuad();
            quad.vbuffer = new VertexBuffer(device, sizeof(float) * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            quad.width = width;
            quad.height = height;

            return quad;
        }

        public static SpriteQuad Create(Texture texture, float width = 100, float height = 100)
        {
            SpriteQuad q= Create(texture.Device, width, height);
            q.texture = texture;
            return q;
        }


        public Texture texture { get; set; }
        public VertexBuffer vbuffer { get; set; }

        public float width { get; set; }
        public float height { get; set; }
        public float hstep { get; set; }
        public float vstep { get; set; }

        public void UseArea(int row, int column)
        {
            float hw = width / 2;
            float hh = height / 2;

            vbuffer.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                -hw,  -hh,  0.0f,   hstep*(column+0), vstep*(row+1),//0.0f, 1.0f,
                -hw,  hh,   0.0f,   hstep*(column+0), vstep*(row+0),//0.0f, 0.0f,
                hw,   hh,   0.0f,   hstep*(column+1), vstep*(row+0),//1.0f, 0.0f,
                hw,   -hh,  0.0f,   hstep*(column+1), vstep*(row+1),//1.0f, 1.0f,
            });
            vbuffer.Unlock();
        }
        public void UseArea(float x, float y, float width, float height)
        {
            float hw = this.width / 2;
            float hh = this.height / 2;
            vbuffer.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                -hw,  -hh,  0.0f,   x, y+height,//0.0f, 1.0f,
                -hw,  hh,   0.0f,   x, y,//0.0f, 0.0f,
                hw,   hh,   0.0f,   x+width, y,//1.0f, 0.0f,
                hw,   -hh,  0.0f,   x+width, y+height,//1.0f, 1.0f,
            });
            vbuffer.Unlock();
            //Console.WriteLine("{0} {1}", hw, hh);
        }
        public void Draw()
        {
            Device device = vbuffer.Device;
            device.SetTexture(0, texture);
            device.VertexFormat = VertexFormat.Position | VertexFormat.Texture1;
            device.SetStreamSource(0, vbuffer, 0, sizeof(float) * 5);
            device.SetRenderState(RenderState.AlphaBlendEnable, true);
            device.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
            device.SetTexture(0, null);
        }
        public void Draw(Vector3 position, float rotation)
        {
            Device device = vbuffer.Device;
            Matrix matWorld = Matrix.Translation(position);
            Matrix matRotation = Matrix.RotationZ(rotation);
            
            device.SetTransform(TransformState.World, matRotation * matWorld);
            Draw();
            device.SetTransform(TransformState.World, Matrix.Identity);

        }
    }
}

