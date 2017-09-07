using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSTG
{
   
    
    class StgCharacter
    {
        
        protected Texture texture=null;
        protected float zoomrate =10;
        protected float textureWidth =1;
        protected float textureHeight =1;
        protected Vector2[] IdleTexturePos=null;
        protected Vector2[] LeftTexturePos=null;
        protected Vector2[] RightTexturePos=null;

        protected RectangleF[] IdleUVs;
        protected RectangleF[] Idle2LeftUVs;
        protected RectangleF[] LeftUVs;
        protected RectangleF[] Idle2RightUVs;
        protected RectangleF[] RightUVs;


        VertexBuffer vbuffer;
        long lefttime = 0;
        long righttime = 0;
        long LeftDuration { get { return (LeftTexturePos.Length - 1) * 50; } }
        long RightDuration { get { return (RightTexturePos.Length - 1) * 50; } }

        public float Width { get { return zoomrate * textureWidth; } }
        public float Height { get { return zoomrate * textureHeight; } }
        public void AnimationFrameUpdate(int direction)
        {
            
            if (direction > 0)
            {
                righttime += Time.DeltaTime;
                righttime = Math.Min(RightDuration, righttime);
                lefttime = 0;
            }
            else if (direction < 0)
            {
                lefttime += Time.DeltaTime;
                lefttime = Math.Min(LeftDuration, lefttime);
                righttime = 0;
            }
            else
            {
                lefttime -= Time.DeltaTime;
                lefttime = Math.Max(0, lefttime);
                righttime -= Time.DeltaTime;
                righttime = Math.Max(0, righttime);
            }
        }
        Vector2 TexturePosition
        {
            get
            {
                if (lefttime > 0)
                    return LeftTexturePos[lefttime / 50];
                else if (righttime > 0)
                    return RightTexturePos[righttime / 50];
                return IdleTexturePos[(Time.TotalTime / 100) % IdleTexturePos.Length];

            }
        }

       
        RectangleF getRect()
        {
            if (lefttime > 0)
            {
                long i = lefttime / 50;
                if (i < Idle2LeftUVs.Length)
                    return Idle2LeftUVs[i];
                return LeftUVs[i % LeftUVs.Length];
            }
            else if (righttime > 0)
            {
                long i = righttime / 50;
                if (i < Idle2RightUVs.Length)
                    return Idle2RightUVs[i];
                return RightUVs[i % RightUVs.Length];
            }
            else return IdleUVs[(Time.TotalTime / 50) % IdleUVs.Length];
        }
                
        public void DrawSprite()
        {

            if (vbuffer == null)
                vbuffer = new VertexBuffer(Resource.device, sizeof(float) * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            float hw = Width/ 2;
            float hh = Height / 2;
            Vector2 xy = TexturePosition;
            float x = xy.X;
            float y = xy.Y;
                 


            vbuffer.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                -hw,  -hh,  0.0f,   x, y+textureHeight,//0.0f, 1.0f,
                -hw,  hh,   0.0f,   x, y,//0.0f, 0.0f,
                hw,   hh,   0.0f,   x+textureWidth, y,//1.0f, 0.0f,
                hw,   -hh,  0.0f,   x+textureWidth, y+textureHeight,//1.0f, 1.0f,
            });
            vbuffer.Unlock();

            Device device = Resource.device;
            device.SetTexture(0, texture);
            device.VertexFormat = VertexFormat.Position | VertexFormat.Texture1;
            device.SetStreamSource(0, vbuffer, 0, sizeof(float) * 5);
            device.SetRenderState(RenderState.AlphaBlendEnable, true);
            device.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
            //device.DrawUserPrimitives<PTVertex>(PrimitiveType.TriangleFan, 2, buffer);
            device.SetTexture(0, null);
        }
    }
}
