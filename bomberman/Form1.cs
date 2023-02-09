using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Timers;
using Microsoft.VisualBasic;//引用microsoft.visualbasic命名空間
using Microsoft.VisualBasic.Devices;//引用microsoft.visualbasic.devices命名空間
using System.Runtime.Intrinsics.X86;
using System;
using System.Diagnostics;

namespace bomberman
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public const int MAP_SIZE = 11;     //地圖大小

        int[,] obstacal = new int[MAP_SIZE, MAP_SIZE];  //紀錄地圖中的障礙物        

        List<Explosion> Player_explosions = new List<Explosion>();  //爆炸的陣列(存放所有玩家的Explosion)
        List<Explosion> AI_explosions = new List<Explosion>();      //爆炸的陣列(存放所有AI的Explosion)

        Bomb PlayerBomb = new Bomb();   //建立玩家炸彈
        Bomb AIBomb = new Bomb();       //建立AI炸彈
        Player player = new Player();   //建立玩家
        AI ai = new AI();               //建立AI

        int Player_duration = Explosion.duration;   //玩家爆炸後持續時間
        int AI_duration = Explosion.duration;       //AI爆炸後持續時間

        int Player_fuze = Bomb.fuze;    //玩家炸彈的引信
        int AI_fuze = Bomb.fuze;        //AI炸彈的引信

        int point = 0;  //分數

        bool start = false;             //遊戲開始狀態(是否已開始)

        //Computer ENTER_GAME = new Computer();  //遊戲進入音樂
        Computer explode_wav = new Computer();  //遊戲內部音樂

        public Form1()
        {
            InitializeComponent();
        }

        private void Bomberman_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;

            this.Size = new Size(1250, 830);        //設定視窗大小

            this.Controls.Add(player.boxCreature);  //新增玩家
            this.Controls.Add(ai.boxCreature);      //新增AI
            this.Controls.Add(PlayerBomb.boxBomb);  //新增玩家炸彈
            this.Controls.Add(AIBomb.boxBomb);      //新增AI炸彈

            for (int i = 0; i < 5; i++) //0上 1下 2左 3右 4中
            {
                Player_explosions.Add(new Explosion());                 //將玩家產生的爆炸放入陣列
                this.Controls.Add(Player_explosions[i].boxExplosion);   //新增玩家爆炸

                AI_explosions.Add(new Explosion());                 //將AI產生的爆炸放入陣列
                this.Controls.Add(AI_explosions[i].boxExplosion);   //新增AI爆炸
            }

            Init();
            LabStartup.Visible = true;
        }

        //遊戲開始
        private void StartGame()
        {
            start = true;   //將遊戲狀態設為"開始"
            Init();         //初始化
            MapLoader(1);   //讀取地圖資訊(Level1.txt)
            timeMain.Start();   //開始"主Timer"
            timeAI.Start();     //開始"AITimer"
            LabPoint.Visible = true;    //顯示分數label
        }

        //初始化
        private void Init()
        {
            //隱藏所有Label
            LabStartup.Visible = false;
            LabGameover.Visible = false;

            //初始化
            player.BombPlaced = false;
            ai.BombPlaced = false;
            ai.step = Block.BlockWidth / ai.Speed;
            point = 0;

            //將玩家隱藏
            player.boxCreature.Visible = false;

            //將AI隱藏
            ai.boxCreature.Visible = false;

            //將玩家的炸彈隱藏並移至角落
            PlayerBomb.boxBomb.Visible = false;
            PlayerBomb.boxBomb.Location = new Point(0, 0);

            //將AI的炸彈隱藏並移至角落
            AIBomb.boxBomb.Visible = false;
            AIBomb.boxBomb.Location = new Point(0, 0);


            for (int i = 0; i < 5; i++) //0上 1下 2左 3右 4中
            {
                //將玩家產生的爆炸隱藏並移至角落
                Player_explosions[i].boxExplosion.Visible = false;
                Player_explosions[i].boxExplosion.Location = new Point(0, 0);

                //將AI產生的爆炸隱藏並移至角落
                AI_explosions[i].boxExplosion.Visible = false;
                AI_explosions[i].boxExplosion.Location = new Point(0, 0);
            }

            //刪除所有場景方塊(牆壁、草地、泥土)
            foreach (PictureBox picturebox in
                this.Controls.OfType<PictureBox>()
                .Where(pb => !(pb.Tag == null) && (pb.Tag.ToString() == "wall" || pb.Tag.ToString() == "grass" || pb.Tag.ToString() == "dirt")))
            {
                this.Controls.Remove(picturebox);
            }
        }

        //遊戲結束
        private void GameOver()
        {
            start = false; //將遊戲狀態設為"未開始"

            //停用所有Timer
            timeMain.Stop();
            timeAI.Stop();
            timeBomb_AI.Stop();
            timeBomb_Player.Stop();
            timeExplosion_AI.Stop();
            timeExplosion_Player.Stop();

            //將所有controls影藏
            foreach (Control control in this.Controls)  
            {
                control.Visible = false;
            }

            //顯示所需label
            LabPoint.Visible = true;
            LabGameover.Visible = true;
            LabStartup.Visible = true;           
        }

        //讀取關卡
        private void MapLoader(int level) 
        {
            int spawnX = 0, spawnY = 0; //初始玩家生成座標

            string strLevel = string.Empty;
            switch (level)  //關卡選擇
            {
                case 1: //關卡1                    
                    spawnX = 140;    //設定玩家生成座標
                    spawnY = 70;
                    ai.Spawn(210, 280);   //生成電腦
                    strLevel = Properties.Resources.level1; //讀取相對應的文字檔
                    break;

                //case 2:
                //    spawnX = 70;
                //    spawnY = 70;
                //    ai.Spawn(210, 280);
                //    strLevel = Properties.Resources.level2;
                //    break;
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
                    string[] blocks = strings.Split(' ');

                    foreach (string type in blocks)
                    {
                        Block block = new Block();
                        block.Spawn(type, posX, posY);      //每讀取一字元就建立一個新button(場景方塊)                        
                        this.Controls.Add(block.boxBlock);  //放置方塊
                        posX += Block.BlockWidth;   //向右位移一個方塊寬

                        if (type == "N")    //如果為草地方塊
                        {
                            obstacal[i, j] = 0; //視為非障礙物
                        }
                        else
                        {
                            obstacal[i, j] = 1; //視為障礙物
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

            //當玩家按下ENTER遊戲開始
            if (input == Keys.Enter && start == false)
            {
                StartGame();
            }

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
                            PlaceBomb(player.boxCreature.Left, player.boxCreature.Top, timeBomb_Player, "player", PlayerBomb);
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
        private void PlaceBomb(int posX, int posY, System.Windows.Forms.Timer timer, string type, Bomb bomb)
        {           
            switch (type)   //判斷是誰放的炸彈
            {
                case "player":
                    player.BombPlaced = true;               //設定玩家狀態為"已放置炸彈"
                    player.OnBomb = true;                   //設定玩家狀態為"在炸彈上"
                    Player_duration = Explosion.duration;   //初始爆炸持續時間
                    Player_fuze = Bomb.fuze;                //初始炸彈引信
                    break;

                case "AI":
                    ai.BombPlaced = true;
                    ai.OnBomb = true;
                    AI_duration = Explosion.duration;
                    AI_fuze = Bomb.fuze;
                    break;

                default:
                    break;
            }

            bomb.Spawn(posX, posY); //生成炸彈(移動炸彈)

            //將炸彈所在處設為障礙物(炸彈會擋路)
            obstacal[bomb.bombY / Block.BlockHeight, bomb.bombX / Block.BlockWidth] = 1;

            timer.Start();  //開始炸彈的Timer
        }

        //炸彈爆炸
        private void Explode(List<Explosion> ex, System.Windows.Forms.Timer timer, Bomb bomb)
        {            
            explode_wav.Audio.Play(Properties.Resources.explode, AudioPlayMode.Background); //爆炸音效
            
            bomb.boxBomb.Visible = false;   //隱藏炸彈
            bomb.boxBomb.Location = new Point(0, 0); //將炸彈移至角落(避免影響遊戲)

            //生成個方位的爆炸(0上 1下 2左 3右 4中)
            ex[0].Spawn(bomb.bombX, bomb.bombY - Explosion.ExplosionHeight);
            ex[1].Spawn(bomb.bombX, bomb.bombY + Explosion.ExplosionHeight);
            ex[2].Spawn(bomb.bombX - Explosion.ExplosionWidth, bomb.bombY);
            ex[3].Spawn(bomb.bombX + Explosion.ExplosionWidth, bomb.bombY);
            ex[4].Spawn(bomb.bombX, bomb.bombY);
            for (int i = 0; i < 5; i++)
            {
                if (!CollisionCheck(ex[i].boxExplosion, "wall")) //檢查爆炸是否碰到牆
                {
                    ex[i].boxExplosion.Visible = true;  //將碰到牆的爆炸隱藏
                }
            }

            //移除障礙物(炸彈爆炸後就不再是障礙物)
            obstacal[bomb.bombY / Block.BlockHeight, bomb.bombX / Block.BlockWidth] = 0;

            timer.Start();  //開始爆炸的Timer
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

            if (CollisionCheck(ai.boxCreature, "explosion"))
            {
                ai.Die();   //AI死亡

                point++;    //加分

                if(start == true)
                {
                    while (!CollisionCheck(ai.hitbox, "grass")) //當重生地不為草地時重新選擇重生地
                    {
                        ai.ChoosePlace();  //選擇重生地
                    }

                    ai.step = Block.BlockHeight / ai.Speed;     //重置步數
                    ai.Spawn(ai.hitbox.Left, ai.hitbox.Top);    //生成AI
                }
            }

            //檢查AI當下是否在炸彈上(用來解決放置炸彈當下無法移動的問題)
            if (!CollisionCheck(ai.boxCreature, "bomb"))
            {
                ai.OnBomb = false;
            }
            else
            {
                ai.OnBomb = true;
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
                    obstacal[pY, pX] = 0;
                }
            }
        }

        private void timeBomb_Player_Tick(object sender, EventArgs e)
        {
            Player_fuze--;
            if (Player_fuze <= 0)
            {
                timeBomb_Player.Stop();
                
                if (start == true)
                {
                    //執行爆炸
                    Explode(Player_explosions, timeExplosion_Player, PlayerBomb);                    
                }
            }
        }

        private void timeExplosion_Player_Tick(object sender, EventArgs e)
        {
            Player_duration--;
            if (Player_duration <= 0)
            {
                timeExplosion_Player.Stop();

                for (int i = 0; i < 5; i++)
                {
                    //隱藏玩家炸彈並移至角落
                    Player_explosions[i].boxExplosion.Visible = false;
                    Player_explosions[i].boxExplosion.Location = new Point(0, 0);

                    //將狀態設為"玩家已投放炸彈"
                    player.BombPlaced = false;
                }
            }
        }

        private void timeBomb_AI_Tick(object sender, EventArgs e)
        {
            AI_fuze--;
            if (AI_fuze <= 0)
            {
                timeBomb_AI.Stop();
                
                if (start == true)
                {
                    //執行爆炸
                    Explode(AI_explosions, timeExplosion_AI, AIBomb);
                }
            }
        }

        private void timeExplosion_AI_Tick(object sender, EventArgs e)
        {
            AI_duration--;
            if (AI_duration <= 0)
            {
                timeExplosion_AI.Stop();

                for (int i = 0; i < 5; i++)
                {
                    //隱藏AI炸彈並移至角落
                    AI_explosions[i].boxExplosion.Visible = false;
                    AI_explosions[i].boxExplosion.Location = new Point(0, 0);

                    //將狀態設為"AI已投放炸彈"
                    ai.BombPlaced = false;
                }
            }
        }

        private void timeAI_Tick(object sender, EventArgs e)
        {
            if (ai.step == Block.BlockWidth / ai.Speed)   //當動完完整一格
            {
                ai.step = 0;        //步數歸零

                if (!ai.BombPlaced) //判斷AI是否已放過炸彈(場上是否存在AI的炸彈),避免重複投彈
                {
                    Random random = new Random();

                    ai.place = random.Next(2); //隨機生成一個0~1的整數(用以判斷AI是否執行投彈動作)
                    if (ai.place == 1)
                    {
                        //執行投彈
                        PlaceBomb(ai.boxCreature.Left, ai.boxCreature.Top, timeBomb_AI, "AI", AIBomb);
                    }
                }

                //選擇方向
                ai.ChooseWay(player.boxCreature.Left, player.boxCreature.Top, obstacal);
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
        //------------------------------Timer_Tick END-------------------------------------------
    }
}
