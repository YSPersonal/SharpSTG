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
        
        static Direct3D D3D;
        static Device device;
        static RenderForm form;

        public static void LoadResource()
        {
            form = new RenderForm("SharpSTG");
            form.Width = 480;
            form.Height = 640;

            D3D = new Direct3D();
            device = new Device(D3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(form.ClientSize.Width, form.ClientSize.Height));
            Resource.Load(device);

            Time.Setup();
            Input.Init(form);
            Debug.Init(device);

            new StgFrame(form).SetAsGlobal();
            StgFrame.Global.SetMatrix(device);
        }
        
        public static void Run()
        {
            Player player = new Reimu();
            Stage.Start();
            RenderLoop.Run(form, () =>
            {
               

                Time.FrameUpdate();
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);
                device.BeginScene();

                device.SetRenderState(RenderState.ZEnable, true);
                device.SetRenderState(RenderState.Lighting, false);
                device.SetRenderState(RenderState.AlphaBlendEnable, true);
                device.SetRenderState(RenderState.SourceBlend, 5);

                player.Draw();
                Stage.FrameUpdate();

                device.EndScene();
                device.Present();
                Input.Global.ClearPressedEvent();

            });

            device.Dispose();
            D3D.Dispose();
        }
    }
}
