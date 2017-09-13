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
        static void Main(string[] args)
        {
            STG.LoadResource();
            STG.Stage = new Stage(); 
            STG.Stage.script.command = new ScriptCommand[] {
                Script.CreateCommand("SharpSTG.SCTag begin"),
                Script.CreateCommand("SharpSTG.SCSpawn SharpSTG.DemoEnemy 1000 path0"),
                Script.CreateCommand("SharpSTG.SCSpawn SharpSTG.DemoEnemy 1200 path0"),
                Script.CreateCommand("SharpSTG.SCSpawn SharpSTG.DemoEnemy 1400 path0"),
                Script.CreateCommand("SharpSTG.SCSpawn SharpSTG.DemoEnemy 1600 path0"),
                Script.CreateCommand("SharpSTG.SCWait 2000"),
                Script.CreateCommand("SharpSTG.SCFire 0,50 150 50 bullet_1_1"),
                Script.CreateCommand("SharpSTG.SCFire 0,50 160 50 bullet_1_1"),
                Script.CreateCommand("SharpSTG.SCFire 0,50 170 50 bullet_1_1"),
                Script.CreateCommand("SharpSTG.SCFire 0,50 180 50 bullet_1_1"),
                Script.CreateCommand("SharpSTG.SCRepeat begin")

            };

            STG.Run();
            
        }
    }
}
