using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSTG
{
    class Stage
    {
        public List<Enemy> enemylist = new List<Enemy>();
        public Timeflow time = new Timeflow();
        public Script script = new Script();
        public List<Bullet> bullets = new List<Bullet>();
        public virtual void FrameUpdate()
        {
            foreach (var e in enemylist)
            {

                if (e.OnStage)
                {
                    e.FrameUpdate();
                    e.Draw();
                }
            }

            foreach (var e in enemylist)
                if (e.IsTimeout)
                {
                    enemylist.Remove(e);
                    break;
                }
            
            time.FrameUpdate();
            script.FrameUpdate();

        }
        public void SpawnEnemy(Enemy enemy, long spawntime, Path path = null)
        {
            enemy.ShowTime = spawntime;
            enemy.time = time;
            if (path != null)
                enemy.Path = path;
            enemylist.Add(enemy);
        }
        public void SpawnEnemy(Enemy enemy, Path path = null)
        {
            SpawnEnemy(enemy, time.CurrentTime, path);
        }

        public void SpawnEnemyLater(Enemy enemy, long delay, Path path = null)
        {
            SpawnEnemy(enemy, time.CurrentTime + delay, path);
        }

    }

}
