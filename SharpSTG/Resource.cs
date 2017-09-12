using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSTG
{
    class Resource
    {
        public static Device device;
        public static Dictionary<string, Texture> textures=new Dictionary<string, Texture>();
        public static Dictionary<string, VertexBuffer> vbuffers=new Dictionary<string, VertexBuffer>();

        public static Dictionary<string, Enemy> enemy=new Dictionary<string, Enemy>();
        public static Dictionary<string, Path> path=new Dictionary<string, Path>();

        public static Dictionary<string, Rectangle> rectangle = new Dictionary<string, Rectangle>();
        public static void Load(Device device) {
            Resource.device = device;
            textures["reimu"] = Texture.FromFile(device, "reimu.png");
            textures["enemy1"] = Texture.FromFile(device, "enemy1.png");

            rectangle["reimu_bullet1"] = new Rectangle(new TexRect(0, 0.570313f, 0.054688f, 0.617188f), textures["reimu"], 7, 6,90);
                        
            path["path0"]= new WayPointPath(WayPointPath.Smooth2(new Vector3[] {
                new Vector3(-40,50,0),
                new Vector3(-40,-50,0),
                new Vector3(40,50,0),
                new Vector3(40,-50,0)
                }, 5));
        }


    }
}
