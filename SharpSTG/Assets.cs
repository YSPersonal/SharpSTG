using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;
namespace SharpSTG
{
    class Assets
    {
        Device device;
        Dictionary<String, Texture> textures;

        public Assets(Device device)
        {
            this.device = device;
            this.textures = new Dictionary<string, Texture>();
        }

        public void load()
        {
            textures.Add("demo", Texture.FromFile(device, "./1.png"));
        }

        public Character CreateCharacter()
        {
            Character c = new Character();
            c.vbuffer = new VertexBuffer(device, Utilities.SizeOf<float>() * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            c.Texture = textures["demo"];
            c.vbuffer.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                -0.5f,  -0.5f,  0.0f,   0.0f, 1.0f,
                -0.5f,  0.5f,   0.0f,   0.0f, 0.0f,
                0.5f,   0.5f,   0.0f,   1.0f, 0.0f,
                0.5f,   -0.5f,  0.0f,   1.0f, 1.0f,
            });
            c.vbuffer.Unlock();
            return c;
        }
    }
}
