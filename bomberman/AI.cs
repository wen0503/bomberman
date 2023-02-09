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
        private List<int> lstWay = new List<int>(); //儲存AI走最短路徑時會走的所有方向

        private int[,] visited = new int[Form1.MAP_SIZE, Form1.MAP_SIZE];   //記錄走過的方塊

        private const int INF = 2147483647; //定義無限大的值
        private int min = INF;  //AI的最少步數
        private int way = 4;    //AI走的方向
        public int step;        //AI走的步數
        public int place = 0;   //AI是否放炸彈

        private bool HaveWay = false;   //AI是否可以抵達目的地

        public AI() : base(Properties.Resources.AI, 10)
        {
            this.boxCreature.Tag = "AI";
        }

        //移動
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

            //在地圖範圍內選擇任意座標
            int posX = rand.Next(Block.BlockWidth * 11 + 1);    
            int posY = rand.Next(Block.BlockHeight * 11 + 1);

            //校正重生位置(使其維持在格子內)
            posX -= posX % Block.BlockWidth;    
            posY -= posY % Block.BlockHeight;

            this.hitbox.Location = new Point(posX, posY);   //移動hitbox至該預選地(方便之後是否重生的判斷)
        }

        //選方向
        public void ChooseWay(int pX, int pY, int[,] obstacal)
        {
            //將AI座標轉換為索引值
            int aiX = this.boxCreature.Left / Block.BlockWidth;
            int aiY = this.boxCreature.Top / Block.BlockHeight;

            //將玩家座標轉換為索引值
            pX /= Block.BlockWidth;
            pY /= Block.BlockHeight;

            this.way = 4;   //初始化AI走的方向

            //當AI尚未抵達玩家所在處
            if (aiX != pX || aiY != pY)
            {
                //初始化
                for (int i = 0; i < Form1.MAP_SIZE; i++)
                {
                    for (int j = 0; j < Form1.MAP_SIZE; j++)
                    {
                        this.visited[i, j] = 0;
                    }
                }
                this.min = INF;

                this.visited[aiY, aiX] = 1; //紀錄AI當前座標為"已走過"

                //執行DFS演算法
                dfs(obstacal, this.visited, aiX, aiY, pX, pY, 0);

                //當無法抵達玩家位置
                if (!this.HaveWay)
                {
                    this.way = 4;   //靜止不動
                }

                //重置狀態
                this.HaveWay = false;
            }
        }

        //DFS演算法
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
                this.HaveWay = true;    //將狀態設為"可以抵達目的地"

                if (step < min)
                {
                    this.min = step;                        //紀錄最少步數
                    this.way = this.lstWay.ElementAt(0);    //紀錄走最短路徑時走的第一個方向
                }
                return;
            }

            for (int i = 0; i < next.Length; i++) //0上 1下 2左 3右
            {
                //移動一步
                tempX = curX + next[i][0];
                tempY = curY + next[i][1];

                //當下一步為"沒走過"且"沒障礙"
                if (ob[tempY, tempX] == 0 && vi[tempY, tempX] == 0)
                {
                    //標記該點為"走過"
                    vi[tempY, tempX] = 1;

                    //紀錄目前所走方向並記錄其索引值
                    this.lstWay.Add(i);
                    var index = this.lstWay.Count - 1;

                    //執行下一步
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
