using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bomberman
{
    internal class Creature //生物
    {
        public const int CreatureHeight = 70, CreatureWidth = 70;
        public const int DefaultDuration = 1;   //初始爆炸持續時間
        public const int DefaultFuze = 3;       //初始炸彈引信

        public int Speed;                       //生物移動速度
        public int duration = DefaultDuration;  //爆炸持續時間
        public int fuze = DefaultFuze;          //炸彈引信

        public bool OnBomb = false;             //此生物使否在炸彈上
        public bool BombPlaced = false;         //是否已放下炸彈(場上是否已存在此生物放下的炸彈)
        public bool Explode = false;            //放下的炸彈是否已爆炸

        public PictureBox boxCreature = new PictureBox();
        public PictureBox hitbox = new PictureBox();

        public Bomb bomb = new Bomb();

        public List<Explosion> explosions = new List<Explosion>();      //爆炸的陣列(存放所有生物的Explosion)

        public Creature(int speed) 
        {           
            this.boxCreature.Size = new Size(CreatureHeight, CreatureWidth);//設定生物大小
            this.hitbox.Size = new Size(CreatureHeight, CreatureWidth);     //設定生物hitbox大小
            this.boxCreature.BringToFront();                                //將生物圖層移至最上面

            this.Speed = speed;             //設定生物速度
            for (int i = 0; i < 5; i++)
            {
                this.explosions.Add(new Explosion());   //將生物產生的爆炸放入陣列               
            }
        }

        //生成
        public void Spawn(int spawnX, int spawnY)
        {
            this.boxCreature.Visible = true;                        //顯示生物
            this.boxCreature.Location = new Point(spawnX, spawnY);  //移動生物到生成位置
            this.hitbox.Location = new Point(spawnX, spawnY);       //移動hitbox到生成位置
        }

        //死亡
        public void Die()
        {
            this.boxCreature.Visible = false;               //隱藏生物
            this.boxCreature.Location = new Point(0, 0);    //將生物移至角落(避免影響遊戲)
            this.hitbox.Location = new Point(0, 0);         //將hitbox移至角落(避免影響遊戲)
        }
    }
}