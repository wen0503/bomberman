using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace bomberman
{
    internal class Explosion    //爆炸
    {
        public const int ExplosionHeight = 70, ExplosionWidth = 70;
        public PictureBox boxExplosion = new PictureBox();
        public const int duration = 1;    //爆炸持續時間

        public Explosion()
        {
            this.boxExplosion.Size = new Size(ExplosionHeight, ExplosionWidth); //設定爆炸大小
            this.boxExplosion.Image = Properties.Resources.explosion;           //設定爆炸圖案
            this.boxExplosion.Tag = "explosion";
        }

        public void Spawn(int posX, int posY)
        {
            this.boxExplosion.Location = new Point(posX, posY);
            //this.boxExplosion.BringToFront();
        }
    }
}
