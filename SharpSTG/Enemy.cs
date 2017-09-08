using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSTG
{
    //enum EnemyState
    //{
    //    BeforeSpawn,
    //    Alive,
    //    Timeout,
    //    Killed
    //}

    class Enemy : StgCharacter
    {
        //EnemyState state = EnemyState.BeforeSpawn;
        //public SpriteQuad SpriteQuad { get; set; }
        public Vector3 Position { get; private set; }
        public virtual void Draw()
        {
            if (OnStage)
            {
                Resource.device.SetTransform(TransformState.World, Matrix.Translation(Position));
                DrawSprite();
            }

        }
        public Timeflow time { private get; set; }
        public Path Path { get; set; }
        public long ShowTime { get; set; }
        public float Speed = 100f;
        bool timeout = false;
        public bool IsTimeout { get { return timeout; } }
        public void TimeOut()
        {
            timeout = true;
        }
        public bool OnStage { get { return time.CurrentTime >= ShowTime; } }
        public void FrameUpdate()
        {
            float distance = (time.CurrentTime - ShowTime) * Speed / 1000;
            if (distance > Path.length)
                TimeOut();
            if (Path != null)
                Position = Path.GetPosition(distance);
        }
        public virtual void OnUpdate()
        {

        }

    }

    class StaticEnemy : Enemy
    {
        public StaticEnemy(string textureName)
        {
            texture = Resource.textures[textureName];
            
        }
    }

    class DemoEnemy : Enemy
    {
        public DemoEnemy()
        {
            Speed = 50;
            uvWidth = 0.09375f;
            uvHeight = 0.0625f;
            zoomrate = 200;
            texture = Resource.textures["enemy1"];
            IdleUVs = new QuadTexRect[] {
                new QuadTexRect(0.09375f*0,0,0.09375f*1,0.0625f),
                new QuadTexRect(0.09375f*1,0,0.09375f*2,0.0625f),
                new QuadTexRect(0.09375f*2,0,0.09375f*3,0.0625f),
                new QuadTexRect(0.09375f*3,0,0.09375f*4,0.0625f),
            };
        }
    }


    class WayPoint
    {
        public Vector3 Position { get; set; }
        public float Milestone { get; set; }
    }

    class Path
    {
        public virtual float length { get;}
        public virtual Vector3 GetPosition(float distance)
        {
            return Vector3.Zero;
        }
        public virtual void Draw(Device device) { }
    }

    class WayPointPath : Path
    {
        public static Vector3[] Smooth(Vector3[] points, int level = 1)
        {
            if (level <= 0)
                return points;

            if (level == 1)
            {
                Vector3[] v = new Vector3[points.Length + 1];
                v[0] = points[0];
                v[v.Length - 1] = points[points.Length - 1];
                for (int i = 1; i < v.Length - 1; i++)
                {
                    v[i] = (points[i - 1] + points[i]) * 0.5f;
                }
                return v;
            }
            return Smooth(Smooth(points), level - 1);
            //S_n(v) = S_n-1(S(v))
            //       = S_n-2(S(S(v)))
        }

        public static Vector3[] Smooth2(Vector3[] points, int level = 1)
        {
            if (level == 0 || points.Length <= 2)
                return points;
            if (level == 1)
            {
                int size = (points.Length - 2) * 2 + 2;
                Vector3[] v = new Vector3[size];
                v[0] = points[0];
                v[size - 1] = points[points.Length - 1];

                int i = 1;
                int j = 1;
                while (i < size - 1)
                {
                    v[i] = points[j - 1] * 0.25f + points[j] * 0.75f;
                    v[i + 1] = points[j + 1] * 0.25f + points[j] * 0.75f;
                    i += 2;
                    j += 1;
                }
                return v;
            }
            return Smooth2(Smooth2(points), level - 1);
        }

        public WayPoint[] WayPoints { get; set; }
        public override float length
        {
            get
            {
                return WayPoints[WayPoints.Length-1].Milestone;
            }
        }
        public void BuildPath(Vector3[] points)
        {
            WayPoints = new WayPoint[points.Length];
            float lengthsum = 0;
            WayPoints[0] = new WayPoint();
            WayPoints[0].Position = points[0];
            WayPoints[0].Milestone = 0;
            for (int i = 1; i < WayPoints.Length; i++)
            {
                float D = (points[i] - points[i - 1]).Length();
                WayPoints[i] = new WayPoint();
                WayPoints[i].Position = points[i];
                lengthsum += D;
                WayPoints[i].Milestone = lengthsum;
            }
        }


        public WayPointPath(Vector3[] points)
        {
            BuildPath(points);
        }

        public WayPointPath(WayPointPath p1, WayPointPath p2)
        {
            WayPoints = new WayPoint[p1.WayPoints.Length + p2.WayPoints.Length];
            int i = 0;
            int j = 0;
            float D1 = p1.WayPoints[p1.WayPoints.Length - 1].Milestone;
            float D2 = (p2.WayPoints[0].Position - p1.WayPoints[p1.WayPoints.Length - 1].Position).Length();
            float D = D1 + D2;

            for (; i < p1.WayPoints.Length; i++)
            {
                WayPoints[i] = new WayPoint();
                WayPoints[i].Position = p1.WayPoints[i].Position;
                WayPoints[i].Milestone = p1.WayPoints[i].Milestone;
            }
            for (; i < WayPoints.Length; i++, j++)
            {
                WayPoints[i] = new WayPoint();
                WayPoints[i].Position = p1.WayPoints[j].Position;
                WayPoints[i].Milestone = p1.WayPoints[j].Milestone + D;
            }
        }

        public override Vector3 GetPosition(float distance)
        {
            if (distance <= 0)
                return WayPoints[0].Position;

            if (distance >= WayPoints[WayPoints.Length - 1].Milestone)
                return WayPoints[WayPoints.Length - 1].Position;

            int left = 0;
            int right = WayPoints.Length - 1;

            while (right - left > 1)
            {
                int middle = (left + right) / 2;
                if (WayPoints[middle].Milestone > distance)
                    right = middle;
                else
                    left = middle;
            }

            float D = WayPoints[right].Milestone - WayPoints[left].Milestone;
            float tl = distance - WayPoints[left].Milestone;
            float tr = WayPoints[right].Milestone - distance;

            Vector3 p = (WayPoints[left].Position * tr + WayPoints[right].Position * tl) / D;

            return p;
        }

        public override void Draw(Device device)
        {
            Vector3[] v = new Vector3[WayPoints.Length];
            for (int i = 0; i < WayPoints.Length; i++)
            {
                v[i] = WayPoints[i].Position;
            }

            device.SetTransform(TransformState.World, Matrix.Identity);
            device.SetRenderState(RenderState.Lighting, false);
            device.SetRenderState(RenderState.AlphaBlendEnable, false);
            //device.SetRenderState(RenderState.Lighting, false);
            device.VertexFormat = VertexFormat.Position;
            device.DrawUserPrimitives<Vector3>(PrimitiveType.LineStrip, v.Length - 1, v);

        }
    }
}
