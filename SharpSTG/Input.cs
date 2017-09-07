using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpSTG
{
    class Input
    {
        public static Input Global { get; private set; }
        public static void Init(Form form) {
            Global = new Input();
            Global.SetEventHandler(form);
        }

        private Dictionary<Keys, bool> keydown=new Dictionary<Keys, bool>();
        private List<Char> KeyPressed=new List<char>();

        public bool GetKeyDown(Keys key)
        {
            return keydown.Keys.Contains(key) && keydown[key];
        }

        public bool GetKeyPressed(Char key)
        {
            return KeyPressed.Contains(key);
        }

        public void SetEventHandler(Form form)
        {
            form.KeyDown += this.OnKeyDown;
            form.KeyUp += this.OnKeyUp;
            form.LostFocus += this.OnLostFocus;
            form.KeyPress += OnKeyPress;
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            //Console.WriteLine(e.KeyChar);
            this.KeyPressed.Add(e.KeyChar);
        }

        public void ClearPressedEvent()
        {
            KeyPressed.Clear();
        }

        private void OnKeyDown(Object sender, KeyEventArgs args)
        {
            keydown[args.KeyCode] = true;
            //Console.WriteLine((int)args.KeyCode+" "+args.KeyValue);
            //if (args.Shift)
            //    Console.WriteLine("With Shift");
        }
        private void OnKeyUp(Object sender, KeyEventArgs args)
        {
            keydown[args.KeyCode] = false;
        }
        private void OnLostFocus(Object sender, EventArgs args)
        {
            keydown.Clear();
        }
    }
}
