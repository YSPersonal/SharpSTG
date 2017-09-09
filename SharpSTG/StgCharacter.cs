using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSTG
{
    /// <summary>
    /// 定义贴图的矩形区域，用于描述2D角色的UV贴图坐标
    /// </summary>
    struct QuadTexRect
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;
        public QuadTexRect(float left, float top, float right, float bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
    }
    class StgCharacter
    {

        protected Texture texture = null;
        protected float zoomrate = 10;
        protected float uvWidth = 1;
        protected float uvHeight = 1;
        
        protected QuadTexRect[] IdleUVs=new QuadTexRect[] {new QuadTexRect(0,0,1,1) };
        protected QuadTexRect[] Idle2LeftUVs = new QuadTexRect[] { new QuadTexRect(0, 0, 1, 1) };
        protected QuadTexRect[] LeftUVs = new QuadTexRect[] { new QuadTexRect(0, 0, 1, 1) };
        protected QuadTexRect[] Idle2RightUVs = new QuadTexRect[] { new QuadTexRect(0, 0, 1, 1) };
        protected QuadTexRect[] RightUVs = new QuadTexRect[] { new QuadTexRect(0, 0, 1, 1) };


        VertexBuffer vbuffer;
        long lefttime = 0;
        long righttime = 0;

        public float Width { get { return zoomrate * uvWidth; } }
        public float Height { get { return zoomrate * uvHeight; } }
        public void AnimationFrameUpdate(int direction)
        {

            if (direction > 0)
            {
                righttime += Time.DeltaTime;
                lefttime = 0;
            }
            else if (direction < 0)
            {
                lefttime += Time.DeltaTime;
                righttime = 0;
            }
            else
            {
                lefttime = Math.Min(Idle2LeftUVs.Length * 50, lefttime);
                righttime = Math.Min(Idle2RightUVs.Length * 50, righttime);
                lefttime -= Time.DeltaTime;
                lefttime = Math.Max(0, lefttime);
                righttime -= Time.DeltaTime;
                righttime = Math.Max(0, righttime);
            }
        }
        
        int interval = 100;

        QuadTexRect getRect()
        {
            if (lefttime > 0)
            {
                long i = lefttime / interval;
                if (i < Idle2LeftUVs.Length)
                    return Idle2LeftUVs[i];
                return LeftUVs[i % LeftUVs.Length];
            }
            else if (righttime > 0)
            {
                long i = righttime / interval;
                if (i < Idle2RightUVs.Length)
                    return Idle2RightUVs[i];
                return RightUVs[i % RightUVs.Length];
            }
            else return IdleUVs[(Time.TotalTime / interval) % IdleUVs.Length];
        }

        public void DrawSprite()
        {

            if (vbuffer == null)
                vbuffer = new VertexBuffer(Resource.device, sizeof(float) * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            float hw = Width / 2;
            float hh = Height / 2;
           
            QuadTexRect rc = getRect();
                      

            vbuffer.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                -hw,  -hh,  0.0f,   rc.Left, rc.Bottom,//0.0f, 1.0f,
                -hw,  hh,   0.0f,   rc.Left, rc.Top,//0.0f, 0.0f,
                hw,   hh,   0.0f,   rc.Right, rc.Top,//1.0f, 0.0f,
                hw,   -hh,  0.0f,   rc.Right, rc.Bottom,//1.0f, 1.0f,
            });
            vbuffer.Unlock();

            Device device = Resource.device;

            device.SetRenderState(RenderState.Lighting, false);
            device.SetRenderState(RenderState.AlphaBlendEnable, true);
            device.SetRenderState(RenderState.SourceBlend, 5);
            device.SetRenderState(RenderState.DestinationBlend, 6);

            device.SetTexture(0, texture);
            device.VertexFormat = VertexFormat.Position | VertexFormat.Texture1;
            device.SetStreamSource(0, vbuffer, 0, sizeof(float) * 5);
            //device.SetRenderState(RenderState.AlphaBlendEnable, true);
            device.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
            device.SetTexture(0, null);
        }
    }
}
