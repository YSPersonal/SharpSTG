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
        }
        
        public static void Run()
        {
            player = new Reimu();
            Stage.time = new Timeflow();
            Stage.time.Pause = false;

            Math = new Math();
            Math.SetCoordinate();
            RenderLoop.Run(Resource.form, () =>
            {              

                Time.FrameUpdate();
                Resource.device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);
                Resource.device.BeginScene();

                Resource.device.SetRenderState(RenderState.ZEnable, true);
                Resource.device.SetRenderState(RenderState.Lighting, false);
                Resource.device.SetRenderState(RenderState.AlphaBlendEnable, true);
                Resource.device.SetRenderState(RenderState.SourceBlend, 5);

                player.Draw();
                Stage.FrameUpdate();

                Resource.device.EndScene();
                Resource.device.Present();
                Input.Global.ClearPressedEvent();

            });

            Resource.device.Dispose();
            Resource.D3D.Dispose();
        }
    }
}
