using SharpDX;
using System;
using System.Collections.Generic;

namespace SharpSTG
{
    class BulletSprite
    {
        public SpriteQuad quad = null;
        public float angleOffset = 0;

        public void Draw(Vector3 position, float angle)
        {
            quad.Draw(position, angle + angleOffset);
        }
    }

    abstract class Bullet
    {
        public BulletSprite Sprite { get; set; }
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
            //Console.WriteLine(Position);
            Sprite.Draw(Position, Rotation);
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

    class DirectionalBullet : Bullet
    {
        Vector3 startPosition;
        Vector3 direction;
        public float Speed { get; set; } = 500;
        public DirectionalBullet(Vector3 start, float angle)
        {
            startPosition = start;
            var rad = angle * Math.PI / 180;
            Rotation = (float)rad;
            direction = new Vector3(-(float)Math.Sin(rad), (float)Math.Cos(rad), 0);
        }
        public DirectionalBullet(Vector3 start, Vector3 target)
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

    delegate void FireAction();

    class AutoFire
    {
        public FireAction Action { get; set; }
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

        public AutoFire(FireAction action)
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
