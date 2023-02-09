using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace bomberman
{
    internal class Block    //場景方塊
    {
        public const int BlockHeight = 70, BlockWidth = 70;
        public PictureBox boxBlock = new PictureBox();

        public Block()
        {
            this.boxBlock.Size = new Size(BlockHeight, BlockWidth); //設定場景方塊大小
            this.boxBlock.SendToBack();     //將場景方塊移至圖層最下方
        }

        //生成
        public void Spawn(string type, int posX, int posY)
        {
            switch (type)  //從level1文字檔中讀取的字來判斷牆(W)或路(N)
            {
                case "W":   //將方塊的圖片依上述判斷做改變
                    this.boxBlock.Image = Properties.Resources.wall;
                    this.boxBlock.Tag = "wall";
                    break;

                case "N":
                    this.boxBlock.Image = Properties.Resources.grass;
                    this.boxBlock.Tag = "grass";
                    break;

                case "D":
                    this.boxBlock.Image = Properties.Resources.dirt;
                    this.boxBlock.Tag = "dirt";
                    break;

                default:
                    Console.Error.WriteLine("Level Text File Error!");
                    break;
            }
            this.boxBlock.Location = new Point(posX, posY);  //移動方塊到生成位置
        }
    }
}
