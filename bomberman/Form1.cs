using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Timers;
using Microsoft.VisualBasic;//引用microsoft.visualbasic命名空間
using Microsoft.VisualBasic.Devices;//引用microsoft.visualbasic.devices命名空間
using System.Runtime.Intrinsics.X86;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;

namespace bomberman
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public const int MAP_SIZE = 11;     //地圖大小

        int[,] obstacle = new int[MAP_SIZE, MAP_SIZE];  //紀錄地圖中的障礙物        

        List<AI> AIs = new List<AI>();                      //儲存所有AI的陣列
        List<Creature> creatures = new List<Creature>();    //儲存所有生物的陣列

        Player player = new Player();   //建立玩家

        int point = 0;          //分數

        bool start = false;     //遊戲開始狀態(是否已開始)

        Computer explode_wav = new Computer();  //遊戲內部音樂

        public Form1()
        {
            InitializeComponent();
        }

        private void Bomberman_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;

            this.Size = new Size(1250, 830);        //設定視窗大小

            //初始化玩家
            this.Controls.Add(player.boxCreature);  //新增玩家
            this.Controls.Add(player.bomb.boxBomb); //新增玩家炸彈
            for (int i = 0; i < 5; i++)
            {
                this.Controls.Add(player.explosions[i].boxExplosion);   //新增玩家爆炸
            }

            //初始化AI
            AIs.Add(new AI("red"));
            AIs.Add(new AI("green"));
            AIs.Add(new AI("yellow"));

            foreach(AI ai in AIs)
            {
                this.Controls.Add(ai.boxCreature);  //新增AI  
                this.Controls.Add(ai.bomb.boxBomb); //新增AI炸彈
                for (int j = 0; j < 5; j++)
                {
                    this.Controls.Add(ai.explosions[j].boxExplosion);   //新增AI爆炸
                }
            }

            creatures.Add(player);
            foreach(AI ai in AIs)
            {
                creatures.Add(ai);
            }

            Init();
            LabStartup.Visible = true;
            butLevel1.Visible = true;
            butLevel2.Visible = true;
            butLevel1.Enabled = true;
            butLevel2.Enabled = true;
        }

        private void butLevel1_Click(object sender, EventArgs e)
        {
            StartGame();
            MapLoader(1);   //讀取地圖資訊(Level1.txt)
        }

        private void butLevel2_Click(object sender, EventArgs e)
        {
            StartGame();
            MapLoader(2);   //讀取地圖資訊(Level2.txt)
        }

        //遊戲開始
        private void StartGame()
        {
            start = true;   //將遊戲狀態設為"開始"
            Init();         //初始化         
        }

        //初始化
        private void Init()
        {           
            //隱藏所有Label
            LabStartup.Visible = false;
            LabGameover.Visible = false;

            //隱藏並停用所有Buttons
            butLevel1.Visible = false;
            butLevel2.Visible = false;
            butLevel1.Enabled = false;
            butLevel2.Enabled = false;

            //初始化玩家
            player.BombPlaced = false;
            player.Explode = false;
            player.boxCreature.Visible = false;
            //將玩家的炸彈隱藏並移至角落
            player.bomb.boxBomb.Visible = false;
            player.bomb.boxBomb.Location = new Point(0, 0);
            for (int i = 0; i < 5; i++)
            {
                //將玩家產生的爆炸隱藏並移至角落
                player.explosions[i].boxExplosion.Visible = false;
                player.explosions[i].boxExplosion.Location = new Point(0, 0);
            }

            //初始化AI
            foreach (AI ai in AIs)
            {
                ai.BombPlaced = false;
                ai.Explode = false;
                ai.boxCreature.Visible = false; //將AI隱藏

                //將AI的炸彈隱藏並移至角落
                ai.bomb.boxBomb.Visible = false;
                ai.bomb.boxBomb.Location = new Point(0, 0);

                //將AI產生的爆炸隱藏並移至角落
                for (int i = 0; i < 5; i++)
                {
                    ai.explosions[i].boxExplosion.Visible = false;
                    ai.explosions[i].boxExplosion.Location = new Point(0, 0);
                }
            }

            //刪除所有場景方塊(牆壁、草地、泥土)
            IEnumerable<PictureBox> pb = this.Controls.OfType<PictureBox>().Where(pb => !(pb.Tag == null) && (pb.Tag.ToString() == "wall" || pb.Tag.ToString() == "grass" || pb.Tag.ToString() == "dirt"));
            while (pb.Count() != 0)
            {
                foreach (Control control in pb)
                {
                    this.Controls.Remove(control);
                    control.Dispose();
                }
            }

            //啟用所有Timer
            timeMain.Start();
            timeAI.Start();
            timeBomb.Start();
            timeExplosion.Start();

            point = 0;                  //重置分數
            LabPoint.Visible = true;    //顯示分數label
        }

        //遊戲結束
        private void GameOver()
        {
            start = false; //將遊戲狀態設為"未開始"

            //停用所有Timer
            timeMain.Stop();
            timeAI.Stop();
            timeBomb.Stop();
            timeExplosion.Stop();

            //將所有controls影藏
            foreach (Control control in this.Controls)
            {
                control.Visible = false;
            }

            //顯示所需label
            LabPoint.Visible = true;
            LabGameover.Visible = true;
            LabStartup.Visible = true;

            //顯示並啟用所有Button
            butLevel1.Visible = true;
            butLevel2.Visible = true;
            butLevel1.Enabled = true;
            butLevel2.Enabled = true;
        }

        //讀取關卡
        private void MapLoader(int level) 
        {
            int spawnX = 0, spawnY = 0; //初始玩家生成座標

            string strLevel = string.Empty;
            switch (level)  //關卡選擇
            {
                case 1: //關卡1
                    player.Speed = 7;
                    foreach(AI ai in AIs)
                    {
                        ai.Speed = 10;
                        ai.step = Block.BlockWidth / ai.Speed;
                    }
                    spawnX = 140;           //設定玩家生成座標
                    spawnY = 70;
                    AIs[0].Spawn(630, 70);  //生成AI
                    AIs[1].Spawn(70, 630);  //生成AI
                    AIs[2].Spawn(630, 630); //生成AI
                    strLevel = Properties.Resources.level1; //讀取相對應的文字檔
                    break;

                case 2:
                    player.Speed = 5;
                    foreach (AI ai in AIs)
                    {
                        ai.Speed = 14;
                        ai.step = Block.BlockWidth / ai.Speed;
                    }
                    spawnX = 70;
                    spawnY = 70;
                    AIs[0].Spawn(630, 70);
                    AIs[1].Spawn(70, 630);
                    AIs[2].Spawn(630, 630);
                    strLevel = Properties.Resources.level2;
                    break;

                default:
                    Console.Error.WriteLine("Level Selection Error!");
                    break;
            }

            player.Spawn(spawnX, spawnY);   //生成玩家

            using (StringReader reader = new StringReader(strLevel))
            {
                int posX = 0, posY = 0; //初始方塊的位置
                int i = 0, j = 0;       //初始索引值

                string strings = string.Empty;
                while ((strings = reader.ReadLine()) != null)    //讀取level1文字檔中的文字
                {
                    string[] str = strings.Split(' ');

                    foreach (string type in str)
                    {
                        Block block = new Block();
                        block.Spawn(type, posX, posY);      //每讀取一字元就建立一個新button(場景方塊)                        
                        this.Controls.Add(block.boxBlock);  //放置方塊
                        block = null;

                        posX += Block.BlockWidth;   //向右位移一個方塊寬

                        if (type == "N")    //如果為草地方塊
                        {
                            obstacle[i, j] = 0; //視為非障礙物
                        }
                        else
                        {
                            obstacle[i, j] = 1; //視為障礙物
                        }
                        j++;
                    }
                    posX = 0;                   //回到初始位置(換行)
                    posY += Block.BlockHeight;   //向下位移一個方塊高

                    j = 0;
                    i++;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Keys input;
            input = e.KeyCode;

            if (start == true)
            {
                player.hitbox.Left = player.boxCreature.Left;  //先將hitbox移至玩家位置
                player.hitbox.Top = player.boxCreature.Top;

                switch (input)
                {
                    //放炸彈
                    case Keys.Space:
                        if (!player.BombPlaced)
                        {
                            //放置炸彈
                            PlaceBomb(player);
                        }
                        break;

                    //移動
                    case Keys.W:
                        player.hitbox.Top -= player.Speed; //先讓hitbox移動                       
                        player.boxCreature.Image= Properties.Resources.player_up;
                        break;

                    case Keys.A:
                        player.hitbox.Left -= player.Speed;
                        player.boxCreature.Image = Properties.Resources.player_left;
                        break;

                    case Keys.S:
                        player.hitbox.Top += player.Speed;
                        player.boxCreature.Image = Properties.Resources.player_down;
                        break;

                    case Keys.D:
                        player.hitbox.Left += player.Speed;
                        player.boxCreature.Image = Properties.Resources.player_right;
                        break;

                    default:
                        break;
                }

                //判斷hitbox是否撞到"牆壁"和"炸彈"和"泥土"，如果沒撞到則移動玩家到hitbox位置
                if (!CollisionCheck(player.hitbox, "wall") && !CollisionCheck(player.hitbox, "dirt") && (!CollisionCheck(player.hitbox, "bomb") || player.OnBomb))
                {
                    player.boxCreature.Left = player.hitbox.Left;
                    player.boxCreature.Top = player.hitbox.Top;
                }
            }
        }

        //碰撞檢測(判斷兩物是否重疊)
        public bool CollisionCheck(PictureBox box, string tag)
        {
            //列舉出form中所有Tag為tag的所有Picturebox
            foreach (PictureBox picturebox in
                this.Controls.OfType<PictureBox>()
                .Where(pb => !(pb.Tag == null) && pb.Tag.ToString() == tag))
            {
                if (box.Bounds.IntersectsWith(picturebox.Bounds))  //判斷兩者是否重疊
                {
                    return true;//如重疊則回傳ture
                }
            }
            return false;//如無重疊則回傳false
        }


        //------------------------------Bomb Function BEGIN-------------------------------------------

        //放炸彈
        private void PlaceBomb(Creature cr)
        {
            cr.BombPlaced = true;                   //設定玩家狀態為"已放置炸彈"
            cr.OnBomb = true;                       //設定玩家狀態為"在炸彈上"
            cr.fuze = Creature.DefaultFuze;         //重置炸彈引信
            cr.duration = Creature.DefaultDuration; //重置爆炸持續時間

            cr.bomb.Spawn(cr.boxCreature.Left, cr.boxCreature.Top); //生成炸彈(移動炸彈)

            //將炸彈所在處設為障礙物(炸彈會擋路)
            obstacle[cr.bomb.bombY / Block.BlockHeight, cr.bomb.bombX / Block.BlockWidth] = 1;
        }

        //炸彈爆炸
        private void Explode(Creature cr)
        {            
            explode_wav.Audio.Play(Properties.Resources.explode, AudioPlayMode.Background); //爆炸音效
            
            cr.bomb.boxBomb.Visible = false;            //隱藏炸彈
            cr.bomb.boxBomb.Location = new Point(0, 0); //將炸彈移至角落(避免影響遊戲)

            cr.Explode = true;  //將狀態設為"炸彈已爆炸"

            //生成個方位的爆炸(0上 1下 2左 3右 4中)
            cr.explosions[0].Spawn(cr.bomb.bombX, cr.bomb.bombY - Explosion.ExplosionHeight);
            cr.explosions[1].Spawn(cr.bomb.bombX, cr.bomb.bombY + Explosion.ExplosionHeight);
            cr.explosions[2].Spawn(cr.bomb.bombX - Explosion.ExplosionWidth, cr.bomb.bombY);
            cr.explosions[3].Spawn(cr.bomb.bombX + Explosion.ExplosionWidth, cr.bomb.bombY);
            cr.explosions[4].Spawn(cr.bomb.bombX, cr.bomb.bombY);
            for (int i = 0; i < 5; i++)
            {
                if (!CollisionCheck(cr.explosions[i].boxExplosion, "wall")) //檢查爆炸是否碰到牆
                {
                    cr.explosions[i].boxExplosion.Visible = true;  //將碰到牆的爆炸隱藏
                }
            }

            //移除障礙物(炸彈爆炸後就不再是障礙物)
            obstacle[cr.bomb.bombY / Block.BlockHeight, cr.bomb.bombX / Block.BlockWidth] = 0;
        }

        //------------------------------Bomb Function END-------------------------------------------

        //------------------------------Timer_Tick BEGIN-------------------------------------------

        private void timeMain_Tick(object sender, EventArgs e)
        {
            LabPoint.Text = "Your points : " + point;   //將分數加在Label上            

            if (CollisionCheck(player.boxCreature, "explosion"))    //檢查玩家是否碰到爆炸
            {
                player.Die();   //玩家死亡
                GameOver();     //遊戲結束
            }

            foreach(AI ai in AIs)
            {
                if (CollisionCheck(ai.boxCreature, "explosion"))
                {
                    ai.Die();   //AI死亡

                    point++;    //加分

                    if (start == true)
                    {
                        do
                        {
                            ai.ChoosePlace();  //選擇重生地                                                     
                        } while (!CollisionCheck(ai.hitbox, "grass"));  //當重生地不為草地時重新選擇重生地

                        do
                        {
                            Random rand = new Random();
                            ai.target = rand.Next(4);   //隨機選擇攻擊目標
                        } while (ai.target == creatures.IndexOf(ai)); //當攻擊目標為自己時重選

                        ai.step = Block.BlockHeight / ai.Speed;     //重置步數
                        ai.Spawn(ai.hitbox.Left, ai.hitbox.Top);    //生成AI
                    }
                }               
            }          

            //檢查玩家當下是否在炸彈上(用來解決放置炸彈當下無法移動的問題)
            if (!CollisionCheck(player.boxCreature, "bomb"))
            {
                player.OnBomb = false;
            }
            else
            {
                player.OnBomb = true;
            }           

            //列舉出form中所有Tag為"dirt"的所有Picturebox
            foreach (PictureBox picturebox in
                this.Controls.OfType<PictureBox>()
                .Where(pb => !(pb.Tag == null) && pb.Tag.ToString() == "dirt"))
            {
                if (CollisionCheck(picturebox, "explosion"))  //判斷泥土是否被炸到
                {
                    //將炸彈座標轉換成索引值
                    int pX = picturebox.Left / Block.BlockWidth;
                    int pY = picturebox.Top / Block.BlockHeight;

                    //將泥土替換為草地
                    picturebox.Image = Properties.Resources.grass;
                    picturebox.Tag = "grass";

                    //移除障礙物(泥土被破壞後就不再是障礙物)
                    obstacle[pY, pX] = 0;
                }
            }
        }

        private void timeBomb_Tick(object sender, EventArgs e)
        {
            //當炸彈已放置
            foreach (Creature cr in creatures)
            {
                if (cr.BombPlaced)
                {
                    cr.fuze--;
                    if (cr.fuze <= 0)
                    {
                        //執行爆炸
                        Explode(cr);
                    }
                }
            }
        }

        private void timeExplosion_Tick(object sender, EventArgs e)
        {
            //當炸彈已爆炸
            foreach (Creature cr in creatures)
            {
                if (cr.Explode)
                {
                    cr.duration--;
                    if (cr.duration <= 0)
                    {
                        cr.BombPlaced = false;  //將狀態設為"已投放炸彈"
                        cr.Explode = false;     //將狀態設為"投放的炸彈尚未爆炸"

                        for (int i = 0; i < 5; i++)
                        {
                            //隱藏爆炸並移至角落
                            cr.explosions[i].boxExplosion.Visible = false;
                            cr.explosions[i].boxExplosion.Location = new Point(0, 0);
                        }
                    }
                }
            }
        }

        private void timeAI_Tick(object sender, EventArgs e)
        {
            foreach (AI ai in AIs)
            {
                if (ai.step == Block.BlockWidth / ai.Speed)   //當動完完整一格
                {
                    ai.step = 0;        //步數歸零

                    if (!ai.BombPlaced) //判斷AI是否已放過炸彈(場上是否存在AI的炸彈),避免重複投彈
                    {
                        Random rand = new Random();

                        //隨機生成一個0~9的整數(用以表示AI有1/10機率會執行投彈動作)
                        ai.place = rand.Next(10); 
                        if (ai.place == 1)
                        {
                            //執行投彈
                            PlaceBomb(ai);
                        }
                    }

                    //選擇方向
                    ai.ChooseWay(creatures[ai.target].boxCreature.Left, creatures[ai.target].boxCreature.Top, obstacle);
                }
                else
                {
                    //持續朝所選方向移動(直到動完完整一格)
                    ai.Move();
                    ai.boxCreature.Left = ai.hitbox.Left;
                    ai.boxCreature.Top = ai.hitbox.Top;
                    ai.step++;
                }
            }                                  
        }

        //------------------------------Timer_Tick END-------------------------------------------
    }
}
