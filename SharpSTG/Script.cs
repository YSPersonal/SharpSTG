using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSTG
{
    class ScriptCommand
    {
        public virtual bool Do() { return true; }
        public virtual void OnCreate(string parameter) { }
    }

    class SCSpawn : ScriptCommand
    {
        public long delay;
        public Path path;
        public Type type;
        public object[] parameter;

        public override bool Do()
        {
            Enemy e = (Enemy)System.Activator.CreateInstance(type, parameter);
            //throw new NotImplementedException();
            STG.Stage.SpawnEnemyLater(e, delay, path);
            return true;
        }

        public override void OnCreate(string parameter)
        {
            var s = parameter.Split(' ');
            type = Type.GetType(s[0]);
            delay = long.Parse(s[1]);
            path = Resource.path[s[2]];
            //Enemy e = null;
            if (s.Length > 3)
                this.parameter = new object[] { s[3] };
            else
                this.parameter = null;

        }
    }

    class SCFire : ScriptCommand
    {
        Vector3 getVector(string str)
        {
            if (str == "player")
                return STG.player.Position;
            if (str == "enemy")
                return Enemy.currentUpdate.Position;

            else
            {
                var s = str.Split('+');
                if (s.Length == 1)
                {
                    var ss = s[0].Split(',');
                    return new Vector3(float.Parse(ss[0]), float.Parse(ss[1]), 0);
                }
                var sum = Vector3.Zero;
                foreach (var i in s)
                    sum += getVector(i);
                return sum;
            }
        }


        public override bool Do()
        {
            DirectBullet b = null;
            if (target != start)
                b = new DirectBullet(start, target, speed);
            else
                b = new DirectBullet(start, angle, speed);
            b.bulletRect = bulletimage;
            STG.Stage.bullets.Add(b);
            return true;
        }

        Vector3 start;
        Vector3 target;
        float speed;
        float angle;
        Rectangle bulletimage;
        public override void OnCreate(string parameter)
        {
            var s = parameter.Split(' ');
           
            start = getVector(s[0]);
            target = start;
            angle = 0;

            if(!float.TryParse(s[1],out angle))
            {
                angle = 0;
                target = getVector(s[1]);
            }
            
            speed = float.Parse(s[2]);
            bulletimage = Resource.rectangle[s[3]];
        }
    }

    class SCClear : ScriptCommand
    {
        public override bool Do()
        {
            STG.Stage.enemy.Clear();
            return true;
        }
    }

    class SCRepeat : ScriptCommand
    {
        public string repeatcondition;
        public string jumptag;

        public override bool Do()
        {
            if (Script.check(STG.Stage.script, repeatcondition))
                STG.Stage.script.back(jumptag);
            return true;
        }
        public override void OnCreate(string parameter)
        {
            var s = parameter.Split(' ');
            jumptag = s[0];
            repeatcondition = "true";
            if (s.Length >= 2)
                repeatcondition = s[2];

        }
    }

    class SCTag : ScriptCommand
    {
        public string name;
        public override void OnCreate(string parameter)
        {
            name = parameter;
        }
    }

    class SCWait : ScriptCommand
    {
        public long waitingtime = 0;
        public override bool Do()
        {
            if (STG.Stage.script.waitstartpoint < 0)
                STG.Stage.script.waitstartpoint = STG.Stage.time.CurrentTime;
            if (STG.Stage.script.waitstartpoint + waitingtime >= STG.Stage.time.CurrentTime)
            {
                //Console.WriteLine(starttime + waitingtime - script.stage.time.CurrentTime);
                return false;
            }
            else
            {
                STG.Stage.script.waitstartpoint = -1;
                return true;
            }
        }
        public override void OnCreate(string parameter)
        {
            if (parameter != null)
                waitingtime = long.Parse(parameter);
        }
    }


    class Script
    {
        public static ScriptCommand CreateCommand(string cmd)
        {
            int split = cmd.IndexOf(' ');
            string type = cmd;
            string parameter = null;
            if (split > 0)
            {
                type = cmd.Substring(0, split);
                parameter = cmd.Substring(split + 1);
            }


            var cmdtype = Type.GetType(type);
            if (cmdtype == null)
                return null;
            var cmdobj = (ScriptCommand)System.Activator.CreateInstance(cmdtype);
            //cmdobj.rawParameter = parameter;
            cmdobj.OnCreate(parameter);
            return cmdobj;

        }

        public static bool check(Script script, string condition)
        {
            switch (condition)
            {
                case "true":
                    return true;

                case "enemy_clear":
                    return STG.Stage.enemy.Count == 0;
            }
            return false;
        }

        int index = 0;
        public ScriptCommand[] command = null;


        public void back(string name)
        {
            for (int i = index - 1; i >= 0; i--)
                if (command[i].GetType() == typeof(SCTag))
                {
                    if ((command[i] as SCTag).name == name)
                    {
                        index = i;
                        return;
                    }
                }
        }
        public long waitstartpoint = -1;
        bool commandbreak = false;
        public void BreakCommand() { commandbreak = true; }
        public void FrameUpdate()
        {
            if (command == null)
                return;
            commandbreak = false;
            while (index >= 0 && index < command.Length)
            {
                //bool isbreak = command[index].scriptbreak;
                if (command[index].Do())
                    index++;
                else
                    break;
                //if (isbreak)
                //    break;
                if (commandbreak)
                    break;
            }
        }
    }
}
