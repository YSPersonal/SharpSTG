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
using SharpSTG.Script;

namespace SharpSTG
{
    class Program
    {  
        static void Main(string[] args)
        {
            STG.LoadResource();
            STG.Stage = new Stage(); 
            STG.Stage.script.command = new Command[] {
                ScriptState.CreateCommand("SharpSTG.Script.SCTag begin"),
                ScriptState.CreateCommand("SharpSTG.Script.SCSpawn SharpSTG.DemoEnemy 1000 path0"),
                ScriptState.CreateCommand("SharpSTG.Script.SCSpawn SharpSTG.DemoEnemy 1200 path0"),
                ScriptState.CreateCommand("SharpSTG.Script.SCSpawn SharpSTG.DemoEnemy 1400 path0"),
                ScriptState.CreateCommand("SharpSTG.Script.SCSpawn SharpSTG.DemoEnemy 1600 path0"),
                ScriptState.CreateCommand("SharpSTG.Script.SCWait 2000"),
                ScriptState.CreateCommand("SharpSTG.Script.SCFire 0,50 150 50 bullet_1_1"),
                ScriptState.CreateCommand("SharpSTG.Script.SCFire 0,50 160 50 bullet_1_1"),
                ScriptState.CreateCommand("SharpSTG.Script.SCFire 0,50 170 50 bullet_1_1"),
                ScriptState.CreateCommand("SharpSTG.Script.SCFire 0,50 180 50 bullet_1_1"),
                ScriptState.CreateCommand("SharpSTG.Script.SCRepeat begin")

            };

            STG.Run();
            
        }
    }
}
