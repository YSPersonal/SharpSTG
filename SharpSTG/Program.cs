using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using SharpDX.Direct3D;
using Color = SharpDX.Color;
namespace SharpSTG
{
    class Program
    {
        struct Vertex
        {
            public Vector4 Position;
            public SharpDX.ColorBGRA Color;
        }

        static void Main(string[] args)
        {
            Stage stage=new Stage();
            var form = new RenderForm("SharpSTG");
            form.Width = 480;
            form.Height = 640;
            // Creates the Device
            var direct3D = new Direct3D();

            var device = new Device(direct3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(form.ClientSize.Width, form.ClientSize.Height));
            Resource.Load(device);

            Player c = new Reimu();

            //var vertices = new VertexBuffer(device, 3 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            //vertices.Lock(0, 0, LockFlags.None).WriteRange(new[] {
            //    new Vertex() { Color = Color.Red, Position = new Vector4(400.0f, 100.0f, 0.5f, 1.0f) },
            //    new Vertex() { Color = Color.Blue, Position = new Vector4(650.0f, 500.0f, 0.5f, 1.0f) },
            //    new Vertex() { Color = Color.Green, Position = new Vector4(150.0f, 500.0f, 0.5f, 1.0f) }
            //});
            //vertices.Unlock();

            //var vertexElems = new[] {
            //    new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
            //    new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
            //    VertexElement.VertexDeclarationEnd
            //};

            //var vertexDecl = new VertexDeclaration(device, vertexElems);
            //form.KeyDown += c.OnKeyDown;
            //c.SetEventHandler(form);

            Time.Setup();
            Input.Init(form);
            Debug.Init(device);

            new StgFrame(form).SetAsGlobal();
            StgFrame.Global.SetMatrix(device);


            Path p = new WayPointPath(WayPointPath.Smooth2(new Vector3[] {
                new Vector3(-40,50,0),
                new Vector3(-40,-50,0),
                new Vector3(40,50,0),
                new Vector3(40,-50,0)
                }, 5));

            stage.script.command = new ScriptCommand[] {
                Script.CreateCommand("SharpSTG.SCTag begin"),
                Script.CreateCommand("SharpSTG.SCSpawn SharpSTG.DemoEnemy 1000 path0"),
                Script.CreateCommand("SharpSTG.SCSpawn SharpSTG.DemoEnemy 1200 path0"),
                Script.CreateCommand("SharpSTG.SCSpawn SharpSTG.DemoEnemy 1400 path0"),
                Script.CreateCommand("SharpSTG.SCSpawn SharpSTG.DemoEnemy 1600 path0"),
                Script.CreateCommand("SharpSTG.SCWait 2000"),
                Script.CreateCommand("SharpSTG.SCRepeat begin")
                
            };
            stage.Start();
            //Enemy e = new DemoEnemy(p, 500);
            RenderLoop.Run(form, () =>
            {
                Time.FrameUpdate();
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);
                device.BeginScene();

                device.SetRenderState(RenderState.ZEnable, true);
                device.SetRenderState(RenderState.Lighting, true);
                device.SetRenderState(RenderState.AlphaBlendEnable, false);
                device.SetRenderState(RenderState.SourceBlend, 5);

                //device.SetStreamSource(0, vertices, 0, 20);
                //device.VertexDeclaration = vertexDecl;
                //device.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
                //Debug.DrawText(string.Format("{0} fps", Time.DeltaTime), 0, 0);
                c.Draw();
                stage.FrameUpdate();

                p.Draw(device);
                //e.FrameUpdate();
                //e.Draw();
                device.EndScene();
                device.Present();
                Input.Global.ClearPressedEvent();
            });

            //vertices.Dispose();
            device.Dispose();
            direct3D.Dispose();
        }
    }
}
