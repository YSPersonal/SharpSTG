using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;

namespace SharpSTG
{
    
    class Rectangle
    {
        TexRect texRect;
        Texture texture;
        float width;
        float height;
        public float angleOffset = 0;
        VertexBuffer vbuffer = null;
        
        public Rectangle(TexRect texRect, Texture texture, float width, float height, float angleOffset=0)
        {
            this.texRect = texRect;
            this.texture = texture;
            this.width = width;
            this.height = height;
            this.angleOffset = angleOffset;
        }

        public void Draw(Vector3 position, float angle)
        {
            if (vbuffer == null)
                vbuffer = new VertexBuffer(Resource.device, sizeof(float) * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            float hw = width / 2;
            float hh = height / 2;

            TexRect rc = texRect;


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

            
            Matrix matWorld = Matrix.Translation(position);
            Matrix matRotation = Matrix.RotationZ((float)(angle+angleOffset * Math.PI / 180));

            device.SetTransform(TransformState.World, matRotation * matWorld);

            device.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
            device.SetTexture(0, null);
        }
    }

   
    abstract class Bullet
    {
        public Rectangle bulletRect;

        public long FlyingTime { get { return Time.TotalTime - StartTime; } }
        public bool TimeOut { get { return FlyingTime > LifeTime; } }
        public long LifeTime { get; set; }
        public long StartTime { get; private set; }

        public Vector3 Position { get; protected set; }
        public float Rotation { get; protected set; }
        public Bullet()
        {
            StartTime = Time.TotalTime;
            LifeTime = 2000;
            Position = Vector3.Zero;
            Rotation = 0;
        }
        public abstract void FrameUpdate();
        public void Draw()
        {
            bulletRect.Draw(Position, Rotation);
        }
    }

    class BulletManager
    {
        List<Bullet> bullets = new List<Bullet>();

        public void Draw()
        {
            foreach (var i in bullets)
            {
                if (i.TimeOut)
                    bullets.Remove(i);
                else
                {
                    i.FrameUpdate();
                    i.Draw();
                }
            }
        }
    }



    class DirectBullet : Bullet
    {
        Vector3 startPosition;
        Vector3 direction;
        public float Speed { get; set; } = 500;
        public DirectBullet(Vector3 start, float angle, float speed)
        {
            startPosition = start;
            var rad = angle * Math.PI / 180;
            Rotation = (float)rad;
            direction = new Vector3(-(float)Math.Sin(rad), (float)Math.Cos(rad), 0);
            Speed = speed;
        }
        public DirectBullet(Vector3 start, Vector3 target)
        {
            start.Z = 0;
            this.startPosition = start;
            this.direction = target - start;
            direction.Normalize();
            this.Rotation = (float)Math.Asin(Vector3.Cross(direction, Vector3.Up).Length());
            if (Vector3.Dot(direction, Vector3.Up) < 0)
                Rotation = (float)Math.PI - Rotation;

        }

        public override void FrameUpdate()
        {
            Position = FlyingTime * 0.001f * Speed * direction + startPosition;
        }
    }

    delegate void StgTimerAction();

    class StgTimer
    {
        public StgTimerAction Action { get; set; }
        public long Interval { get; set; }

        public bool enable = false;
        public bool Enable
        {
            get { return enable; }
            set
            {
                if (enable != value)
                {
                    enable = value;
                    waitingtime = Interval;
                }
            }
        }

        long waitingtime = 0;

        public StgTimer(StgTimerAction action)
        {
            Action = action;
            Interval = 100;
            waitingtime = 0;
            Enable = false;
        }
        public void FrameUpdate()
        {
            if (Enable)
            {
                waitingtime += Time.DeltaTime;
                if (waitingtime >= Interval)
                {
                    waitingtime -= Interval;
                    Action();
                }
            }
        }
    }
}
