using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSTG
{
    class Time
    {
        private static Stopwatch stopwatch { get; set; }
        private static long lasttime = 0;
        private static long delta = 0;

        public static float FPS { get; private set; }

        public static void Setup()
        {
            if (stopwatch == null)
                stopwatch = new Stopwatch();
            stopwatch.Reset();
            lasttime = 0;
            delta = 0;
            stopwatch.Start();
            FPS = 0;
        }

        public static void FrameUpdate()
        {
            var t = stopwatch.ElapsedMilliseconds;
            delta = t - lasttime;
            lasttime = t;
            if (delta > 0)
                FPS = 1000.0f / delta;
            
        }

        public static long DeltaTime { get { return delta; } }
        public static float DeltaTimeSeconds { get { return delta / 1000f; } }
        public static long TotalTime { get { return lasttime; } }
    }

    class Timeflow
    {
        public long CurrentTime { get;private set; }
        public float Flowrate { get; set; }
        public long DeltaTime { get; private set; }
        public bool Pause { get; set; }
        public void FrameUpdate()
        {
            if (Pause)
                return;
            long inc = (long)(Time.DeltaTime * Flowrate);
            DeltaTime = inc;
            CurrentTime += DeltaTime;
        }

        public Timeflow()
        {
            CurrentTime = 0;
            Flowrate = 1;
            DeltaTime = 0;
            Pause = true;
        }
    }
}
