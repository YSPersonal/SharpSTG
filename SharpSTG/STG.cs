using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using Color = SharpDX.Color;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSTG
{
    class STG
    {
        public static Stage Stage { get; set; }
        
        //static Direct3D D3D;
        //static Device device;
        //static RenderForm form;
        public static Player player;
        public static Math Math { get; private set; }
        //public static float FPS { get; private set; } = 0;
        public static FPS fps;
        public static void LoadResource()
        {
            Resource.form = new RenderForm("SharpSTG");
            Resource.form.Width = 480;
            Resource.form.Height = 640;

            Resource.D3D = new Direct3D();
            Resource.device = new Device(Resource.D3D, 0, DeviceType.Hardware, Resource.form.Handle, CreateFlags.HardwareVertexProcessing, 
                new PresentParameters(Resource.form.ClientSize.Width, Resource.form.ClientSize.Height));
            Resource.Load(Resource.device);

            Time.Setup();
            Input.Init(Resource.form);
            Debug.Init(Resource.device);

            //new StgMath(form).SetAsGlobal();
            //StgMath.Global.SetMatrix(device);
            fps = new FPS();
        }
        
        public static void Run()
        {
            player = new Reimu();
            Stage.time = new Timeflow();
            Stage.time.Pause = false;

            Math = new Math();
            Math.SetCoordinate();

            //int frameCount = 0;
            RenderLoop.Run(Resource.form, () =>
            {
                //frameCount++;

                Time.FrameUpdate();
                fps.FrameUpdate();
                Resource.device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);
                Resource.device.BeginScene();

                Resource.device.SetRenderState(RenderState.ZEnable, true);
                Resource.device.SetRenderState(RenderState.Lighting, false);
                Resource.device.SetRenderState(RenderState.AlphaBlendEnable, true);
                Resource.device.SetRenderState(RenderState.SourceBlend, 5);

                player.Draw();
                Stage.FrameUpdate();

                Debug.DrawText(string.Format("{0} FPS", fps.Get), 0, 0);

                Resource.device.EndScene();
                Resource.device.Present();
                Input.Global.ClearPressedEvent();
                
            });

            Resource.device.Dispose();
            Resource.D3D.Dispose();
        }
    }

    class FPS
    {
        long time=0;
        long frameCount=0;
        float value = 0;

        int size = 10;
        float rate = 0.02f;
        public void FrameUpdate() {
            time += Time.DeltaTime;
            frameCount++;

            if (frameCount >= size)
            {
                float v = frameCount*1000.0f / time;
                value = value *(1-rate) + v * rate;
                time = 0;
                frameCount = 0;
            }
        }
        public float Get { get { return value; } }
    }
}
