using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;
using System.Windows.Forms;
using System.Diagnostics;

namespace SharpSTG
{
    class Player : StgCharacter
    {

        public AutoFire AutoFire { get; set; }
        public Vector3 Position { get; set; }

        public float Speed { get; protected set; }
        public float SlowSpeed { get; protected set; }

        public Player()
        {
            Speed = 60;
            SlowSpeed = 20;
            AutoFire = new AutoFire(() =>
            {
                Fire();
            });

        }

       
        protected virtual void OnMove(Vector3 position, Vector3 velocity)
        {
            AnimationFrameUpdate((int)(velocity.X * 1000));
        }

        public virtual void Draw()
        {
            Vector3 direction = Vector3.Zero;
            if (Input.Global.GetKeyDown(Keys.Left))
                direction += Vector3.Left;
            if (Input.Global.GetKeyDown(Keys.Up))
                direction += Vector3.Up;
            if (Input.Global.GetKeyDown(Keys.Right))
                direction += Vector3.Right;
            if (Input.Global.GetKeyDown(Keys.Down))
                direction += Vector3.Down;

            direction.Normalize();
            float speed = Speed;
            if (Input.Global.GetKeyDown(Keys.ShiftKey))
                speed = SlowSpeed;
            OnMove(Position, direction * speed);
            Position += direction * Time.DeltaTimeSeconds * speed;
            Position = StgFrame.Global.InBox(Position, Width / 2, Height / 2);

            Matrix mat = Matrix.Translation(Position);
            Resource.device.SetTransform(TransformState.World, mat);
            DrawSprite();

            foreach (var b in bullets)
            {
                b.FrameUpdate();
                b.Draw();
            }

            if (AutoFire != null)
            {
                AutoFire.Enable = Input.Global.GetKeyDown(Keys.Z);
                AutoFire.FrameUpdate();
            }
        }

        protected List<PlayerBullet> bullets = new List<PlayerBullet>();
        public virtual void Fire()
        {
        }

    }

    class Reimu : Player
    {
        public override void Fire()
        {
            var b = new DirectionalBullet(Position + Vector3.Left * 6, 30);
            b.bulletRect = Resource.rectangle["reimu_bullet1"];
            bullets.Add(b);

            b = new DirectionalBullet(Position + Vector3.Right * 6, 0);
            b.bulletRect = Resource.rectangle["reimu_bullet1"];
            bullets.Add(b);

        }
        public Reimu()
        {
            texture = Resource.textures["reimu"];
            uvWidth = 0.125f;
            uvHeight = 0.1875f;
            zoomrate = 100;

            IdleUVs = new TexRect[] {
                new TexRect(0.125f*0,0,0.125f*1,0.1875f),
                new TexRect(0.125f*1,0,0.125f*2,0.1875f),
                new TexRect(0.125f*2,0,0.125f*3,0.1875f),
                new TexRect(0.125f*3,0,0.125f*4,0.1875f),
                new TexRect(0.125f*4,0,0.125f*5,0.1875f),
                new TexRect(0.125f*5,0,0.125f*6,0.1875f),
                new TexRect(0.125f*6,0,0.125f*7,0.1875f),
                new TexRect(0.125f*7,0,0.125f*8,0.1875f),
            };

            Idle2LeftUVs = new TexRect[] {
                new TexRect(0.125f*0,0.1875f,0.125f*1,0.375f),
                new TexRect(0.125f*1,0.1875f,0.125f*2,0.375f),
                new TexRect(0.125f*2,0.1875f,0.125f*3,0.375f),
                new TexRect(0.125f*3,0.1875f,0.125f*4,0.375f),
            };

            Idle2RightUVs = new TexRect[] {
                new TexRect(0.125f*0,0.375f,0.125f*1,0.5625f),
                new TexRect(0.125f*1,0.375f,0.125f*2,0.5625f),
                new TexRect(0.125f*2,0.375f,0.125f*3,0.5625f),
                new TexRect(0.125f*3,0.375f,0.125f*4,0.5625f),
            };

            LeftUVs = new TexRect[] {
                new TexRect(0.125f*4,0.1875f,0.125f*5,0.375f),
                new TexRect(0.125f*5,0.1875f,0.125f*6,0.375f),
                new TexRect(0.125f*6,0.1875f,0.125f*7,0.375f),
                new TexRect(0.125f*7,0.1875f,0.125f*8,0.375f),
            };

            RightUVs = new TexRect[] {
                new TexRect(0.125f*4,0.375f,0.125f*5,0.5625f),
                new TexRect(0.125f*5,0.375f,0.125f*6,0.5625f),
                new TexRect(0.125f*6,0.375f,0.125f*7,0.5625f),
                new TexRect(0.125f*7,0.375f,0.125f*8,0.5625f),
            };
          
        }
    }
}
