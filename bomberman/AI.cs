using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bomberman
{
    internal class AI : Creature    //AI
    {
        private List<int> lstWay = new List<int>();

        private int[,] visited = new int[Form1.MAP_SIZE, Form1.MAP_SIZE];   //記錄走過的路線

        private const int INF = 2147483647;
        private int min = INF;
        private int way = 4;
        public int step;        //AI走的步數
        public int place = 0;   //AI是否放炸彈

        private bool HaveWay = false;

        public AI() : base(Properties.Resources.AI, 10)
        {
            this.boxCreature.Tag = "AI";
        }

        public void Move()
        {
            this.hitbox.Left = this.boxCreature.Left;  //先將hitbox移至AI位置
            this.hitbox.Top = this.boxCreature.Top;

            switch (this.way)//0上 1下 2左 3右
            {                
                case 0:
                    this.hitbox.Top -= this.Speed; //先讓hitbox移動
                    break;

                case 1:
                    this.hitbox.Top += this.Speed;
                    break;

                case 2:
                    this.hitbox.Left -= this.Speed;
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

        public void ChooseWay(int pX, int pY, int[,] obstacal)
        {
            int aiX = this.boxCreature.Left / Block.BlockWidth;
            int aiY = this.boxCreature.Top / Block.BlockHeight;

            pX /= Block.BlockWidth;
            pY /= Block.BlockHeight;

            this.way = 4;
            if (aiX != pX || aiY != pY)
            {
                for (int i = 0; i < Form1.MAP_SIZE; i++)
                {
                    for (int j = 0; j < Form1.MAP_SIZE; j++)
                    {
                        this.visited[i, j] = 0;
                    }
                }

                this.min = INF;
                this.visited[aiY, aiX] = 1;
                dfs(obstacal, this.visited, aiX, aiY, pX, pY, 0);
                if (!this.HaveWay)
                {
                    this.way = 4;
                }
                this.HaveWay = false;
            }
        }

        private void dfs(int[,] ob, int[,] vi, int curX, int curY, int destX, int destY, int step)
        {
            int[][] next = new int[][]
            {
                new int[] {0, -1},  //上
                new int[] {0, 1},   //下
                new int[] {-1, 0},  //左
                new int[] {1, 0}    //右
            };

            int tempX, tempY;

            //到達目的地
            if (curX == destX && curY == destY)
            {
                this.HaveWay = true;

                if (step < min)
                {
                    this.min = step;
                    this.way = this.lstWay.ElementAt(0);
                    //System.Diagnostics.Debug.WriteLine("best step = " + step);
                }
                return;
            }

            for (int i = 0; i < next.Length; i++) //0上 1下 2左 3右
            {
                tempX = curX + next[i][0];
                tempY = curY + next[i][1];

                //當沒走過且沒障礙
                if (ob[tempY, tempX] == 0 && vi[tempY, tempX] == 0)
                {
                    //標記該點為"走過"
                    vi[tempY, tempX] = 1;
                    this.lstWay.Add(i);
                    var index = this.lstWay.Count - 1;

                    dfs(ob, vi, tempX, tempY, destX, destY, step + 1);

                    //如果執行到這裡，表示"已達目的地"或"走到無路可走"，須將先前的路取消標記
                    this.lstWay.RemoveAt(index);
                    vi[tempY, tempX] = 0;
                }
            }
            return;
        }
    }
}
