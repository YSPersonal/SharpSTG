using SharpSTG.Script;
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
        public ScriptState script = new ScriptState();
        public BulletManager bullets = new BulletManager();

        public virtual void FrameUpdate()
        {
            enemy.FrameUpdate();

            foreach (var e in enemy)
                foreach (var b in STG.player.bullets)
                    if (e.HitDetect(b))
                    {
                        e.OnBulletHit(b);
                        b.HitTarget = e;
                    }
            
            foreach(var b in bullets)
                if (Math.HitDetect(b.Position, b.HitSize, STG.player.Position, 1))
                {
                    b.HitTarget = STG.player;
                }

            bullets.FrameUpdate();
            time.FrameUpdate();
            script.FrameUpdate();
        }

        public void SpawnEnemy(Enemy enemy, long spawntime, Path path = null)
        {
            enemy.ShowTime = spawntime;
            enemy.Time = time;
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
