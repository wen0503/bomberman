using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace bomberman
{
    internal class AI : Creature    //AI
    {

        public AI() : base(Properties.Resources.AI, 10)
        {
            this.boxCreature.Tag = "AI";
        }

        public void Movement(int way)
        {
            this.hitbox.Left = this.boxCreature.Left;  //先將hitbox移至AI位置
            this.hitbox.Top = this.boxCreature.Top;

            switch (way)//0上 1下 2左 3右
            {                
                case 0:
                    this.hitbox.Top -= this.Speed; //先讓hitbox移動
                    break;

                case 2:
                    this.hitbox.Left -= this.Speed;
                    break;

                case 1:
                    this.hitbox.Top += this.Speed;
                    break;

                case 3:
                    this.hitbox.Left += this.Speed;
                    break;

                default:
                    break;
            }
        }

        //選擇重生地
        public void ChoosePlace()
        {
            Random rand = new Random();

            int posX = rand.Next(Block.BlockWidth * 11 + 1);    //在地圖範圍內選擇任意座標
            int posY = rand.Next(Block.BlockHeight * 11 + 1);

            posX -= posX % Block.BlockWidth;    //校正重生位置(使其維持在格子內)
            posY -= posY % Block.BlockHeight;

            this.hitbox.Location = new Point(posX, posY);   //移動hitbox至該預選地(方便之後是否重生的判斷)
        }
    }
}
