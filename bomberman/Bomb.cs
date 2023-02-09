using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman
{
    internal class Bomb //炸彈
    {
        private const int BombHeight = 70, BombWidth = 70;
        public PictureBox boxBomb = new PictureBox();
        public const int fuze = 3;      //炸彈引信
        public int bombX, bombY;        //炸彈座標

        public Bomb() 
        {
            this.boxBomb.Size = new Size(BombHeight, BombWidth);    //設定炸彈大小
            this.boxBomb.Image = Properties.Resources.bomb;         //設定炸彈圖案
            this.boxBomb.Tag = "bomb";
        }

        //生成
        public void Spawn(int posX, int posY)
        {
            this.boxBomb.Visible = true;    //顯示炸彈

            //校正炸彈位置(使其維持在格子內)
            this.bombX = posX - (posX % Block.BlockWidth);  
            this.bombY = posY - (posY % Block.BlockHeight);

            this.boxBomb.Location = new Point(bombX, bombY);//移動炸彈到生成位置
        }
    }
}
