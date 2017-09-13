using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSTG
{
    class Stage
    {
        public EnemyManager enemy = new EnemyManager();
        public Timeflow time = new Timeflow();
        public Script script = new Script();
        public BulletManager bullets = new BulletManager();

        public virtual void FrameUpdate()
        {
            foreach (var e in enemy)
            {

                if (e.OnStage)
                {
                    e.FrameUpdate();
                    e.Draw();
                }
            }

            foreach (var e in enemy)
                if (e.IsTimeout)
                {
                    enemy.Remove(e);
                    break;
                }

            bullets.FrameUpdate();
            time.FrameUpdate();
            script.FrameUpdate();

        }
        public void SpawnEnemy(Enemy enemy, long spawntime, Path path = null)
        {
            enemy.ShowTime = spawntime;
            enemy.time = time;
            if (path != null)
                enemy.Path = path;
            this.enemy.Add(enemy);
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
