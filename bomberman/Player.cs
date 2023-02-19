using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman
{
    internal class Player : Creature    //玩家
    {
        public Player() : base(7)
        {
            this.boxCreature.Image = Properties.Resources.player;   //設定玩家圖案
        }
    }
}
