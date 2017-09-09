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

    class SCClear : ScriptCommand
    {
        public override bool Do()
        {
            STG.Stage.enemylist.Clear();
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
        long starttime = -1;
        public long waitingtime = 0;
        public override bool Do()
        {
            if (starttime < 0)
                starttime = STG.Stage.time.CurrentTime;
            if (starttime + waitingtime >= STG.Stage.time.CurrentTime)
            {
                //Console.WriteLine(starttime + waitingtime - script.stage.time.CurrentTime);
                return false;
            }
            else
            {
                starttime = -1;
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
                    return STG.Stage.enemylist.Count == 0;
            }
            return false;
        }

        int index = 0;
        public ScriptCommand[] command = null;
        
         
        public void back(string name)
        {
            for (int i = index - 1; i >= 0; i--)
                if (command[i].GetType()==typeof(SCTag))
                {
                    if ((command[i] as SCTag).name == name)
                    {
                        index = i;
                        return;
                    }
                }
        }

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
