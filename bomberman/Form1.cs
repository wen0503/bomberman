using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Timers;
using Microsoft.VisualBasic;//引用microsoft.visualbasic命名空間
using Microsoft.VisualBasic.Devices;//引用microsoft.visualbasic.devices命名空間

namespace bomberman
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        List<Explosion> Player_explosions = new List<Explosion>();  //爆炸的陣列(存放所有玩家的Explosion)
        List<Explosion> AI_explosions = new List<Explosion>();      //爆炸的陣列(存放所有AI的Explosion)

        Bomb PlayerBomb = new Bomb();   //建立玩家炸彈
        Bomb AIBomb = new Bomb();       //建立AI炸彈
        Player player = new Player();   //建立玩家
        AI ai = new AI();   //建立電腦

        int Player_duration = Explosion.duration;   //玩家爆炸後持續時間
        int AI_duration = Explosion.duration;       //AI爆炸後持續時間

        int Player_fuze = Bomb.fuze;    //玩家炸彈的引信
        int AI_fuze = Bomb.fuze;        //AI炸彈的引信

        int step = 0;   //AI走的步數
        int way = 0;    //AI走的方向
        int place = 0;  //AI是否放炸彈
        int point = 0;  //分數

        bool start = false;             //遊戲開始狀態(是否已開始)
        bool finish = true;             //AI走完完整一格的狀態(是否走完一格)
        bool Player_BombPlaced = false; //玩家是否已放下炸彈(場上是否已存在玩家的炸彈)
        bool AI_BombPlaced = false;     //AI是否已放下炸彈(場上是否已存在AI的炸彈)
        bool PlayeronBomb = false;      //玩家是否在炸彈上方(用來解決放置炸彈當下無法移動的問題)
        bool AIonBomb = false;          //AI是否在炸彈上方(同上)

        //Computer ENTER_GAME = new Computer();  //遊戲進入音樂
        Computer GAME_starting = new Computer();  //遊戲內部音樂
        Computer bomb_wav = new Computer();  //遊戲內部音樂

        public Form1()
        {
            InitializeComponent();
        }

        private void Bomberman_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;

            this.Size = new Size(1250, 830);

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

        private void StartGame()
        {
            start = true;   //將遊戲狀態設為"開始"
            Init();
            MapLoader(1);   //讀取地圖資訊(Level1.txt)
            timeMain.Start();
            timeAI.Start();
            LabPoint.Visible = true;
            GAME_starting.Audio.Play("GAME_starting.wav", AudioPlayMode.BackgroundLoop);//按下開始遊戲持續播放音樂
        }

        private void Init()
        {
            //隱藏所有Label
            LabStartup.Visible = false;
            LabGameover.Visible = false;

            //初始化
            Player_BombPlaced = false;
            AI_BombPlaced = false;
            finish = true;
            step = 0;
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

        private void GameOver()//遊戲結束
        {
            start = false; //將遊戲狀態設為"未開始"

            //停用所有Timer
            timeMain.Stop();
            timeAI.Stop();
            timeBomb_AI.Stop();
            timeBomb_Player.Stop();
            timeExplosion_AI.Stop();
            timeExplosion_Player.Stop();

            foreach (Control control in this.Controls)  //將所有controls影藏
            {
                control.Visible = false;
            }

            LabGameover.Visible = true;
            LabStartup.Visible = true;           
        }

        private void MapLoader(int level) //讀取關卡
        {
            int spawnX = 0, spawnY = 0; //初始玩家生成座標

            string strLevel = string.Empty;
            switch (level)  //關卡選擇
            {
                case 1: //關卡1                    
                    spawnX = 70;    //設定玩家生成座標
                    spawnY = 70;
                    ai.Spawn(spawnX + 140, spawnY + 210);   //生成電腦
                    strLevel = Properties.Resources.level1; //讀取相對應的文字檔
                    break;

                //case 2:
                //    spawnX = 70;
                //    spawnY = 70;
                //    ai.Spawn(spawnX + 140, spawnY + 210);
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
                    }
                    posX = 0;                   //回到初始位置(換行)
                    posY += Block.BlockWidth;   //向下位移一個方塊高
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Keys input;
            input = e.KeyCode;

            if (input == Keys.Enter && start == false)   //當玩家按下ENTER遊戲開始
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
                        if (!Player_BombPlaced)
                        {
                            PlaceBomb(player.boxCreature.Left, player.boxCreature.Top, timeBomb_Player, "player", PlayerBomb);
                        }
                        break;

                    //移動
                    case Keys.W:
                        player.hitbox.Top -= player.Speed; //先讓hitbox移動
                        break;

                    case Keys.A:
                        player.hitbox.Left -= player.Speed;
                        break;

                    case Keys.S:
                        player.hitbox.Top += player.Speed;
                        break;

                    case Keys.D:
                        player.hitbox.Left += player.Speed;
                        break;

                    default:
                        break;
                }

                //判斷hitbox是否撞到"牆壁"和"炸彈"和"泥土"，如果沒撞到則移動玩家到hitbox位置
                if (!CollisionCheck(player.hitbox, "wall") && !CollisionCheck(player.hitbox, "dirt") && (!CollisionCheck(player.hitbox, "bomb") || PlayeronBomb == true))
                {
                    player.boxCreature.Left = player.hitbox.Left;
                    player.boxCreature.Top = player.hitbox.Top;
                }
            }
        }

        //碰撞檢測(判斷兩物是否重疊)
        public bool CollisionCheck(PictureBox box, string tag)//判斷是否碰撞到牆壁
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

        //放炸彈
        private void PlaceBomb(int posX, int posY, System.Windows.Forms.Timer timer, string type, Bomb bomb)
        {
            switch (type)   //判斷是誰放的炸彈
            {
                case "player":
                    Player_BombPlaced = true;
                    Player_duration = Explosion.duration;
                    Player_fuze = Bomb.fuze;
                    break;

                case "AI":
                    AI_BombPlaced = true;
                    AI_duration = Explosion.duration;
                    AI_fuze = Bomb.fuze;
                    break;

                default:
                    break;
            }

            bomb.Spawn(posX, posY); //生成炸彈(移動炸彈)
            System.Diagnostics.Debug.WriteLine("bombX = " + bomb.bombX + ", bombY = " + bomb.bombY);

            timer.Start();  //開始炸彈的Timer
        }

        //炸彈爆炸
        private void Explode(List<Explosion> ex, System.Windows.Forms.Timer timer, Bomb bomb)
        {
            timer.Start();

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
        }

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

                while (!CollisionCheck(ai.hitbox, "grass")) //當重生地不為草地時重新選擇重生地
                {
                    ai.ChoosePlace();  //選擇重生地
                }

                step = 0;       //步數歸零
                finish = true;  //將狀態設為"已動完完整一格"

                if(start == true)
                {
                    ai.Spawn(ai.hitbox.Left, ai.hitbox.Top);    //生成AI
                }
            }

            //檢查玩家當下是否在炸彈上(用來解決放置炸彈當下無法移動的問題)
            if (CollisionCheck(ai.boxCreature, "bomb") && AIonBomb == false)
            {
                AIonBomb = true;
            }
            else
            {
                AIonBomb = false;
            }

            //檢查AI當下是否在炸彈上(用來解決放置炸彈當下無法移動的問題)
            if (CollisionCheck(player.boxCreature, "bomb") && PlayeronBomb == false)
            {
                PlayeronBomb = true;
            }
            else
            {
                PlayeronBomb = false;
            }

            //列舉出form中所有Tag為"dirt"的所有Picturebox
            foreach (PictureBox picturebox in
                this.Controls.OfType<PictureBox>()
                .Where(pb => !(pb.Tag == null) && pb.Tag.ToString() == "dirt"))
            {
                if (CollisionCheck(picturebox, "explosion"))  //判斷泥土是否被炸到
                {
                    //將泥土替換為草地
                    picturebox.Image = Properties.Resources.grass;
                    picturebox.Tag = "grass";
                }
            }
        }

        private void timeBomb_Player_Tick(object sender, EventArgs e)
        {
            Player_fuze--;
            System.Diagnostics.Debug.WriteLine("Player_fuze = " + Player_fuze);
            if (Player_fuze <= 0)
            {
                timeBomb_Player.Stop();
                //執行爆炸
                if (start == true)
                {
                    Explode(Player_explosions, timeExplosion_Player, PlayerBomb);
                    bomb_wav.Audio.Play("bomb.wav", AudioPlayMode.Background); //爆炸音效
                }
            }
        }


        private void timeExplosion_Player_Tick(object sender, EventArgs e)
        {
            Player_duration--;
            System.Diagnostics.Debug.WriteLine("Player_duration = " + Player_duration);
            if (Player_duration <= 0)
            {
                timeExplosion_Player.Stop();
                for (int i = 0; i < 5; i++)
                {
                    //隱藏玩家炸彈並移至角落
                    Player_explosions[i].boxExplosion.Visible = false;
                    Player_explosions[i].boxExplosion.Location = new Point(0, 0);

                    //將狀態設為"玩家已投放炸彈"
                    Player_BombPlaced = false;
                }
            }
        }

        private void timeBomb_AI_Tick(object sender, EventArgs e)
        {
            AI_fuze--;
            System.Diagnostics.Debug.WriteLine("AI_fuze = " + AI_fuze);
            if (AI_fuze <= 0)
            {
                timeBomb_AI.Stop();
                //執行爆炸
                if (start == true)
                {
                    Explode(AI_explosions, timeExplosion_AI, AIBomb);
                }
            }
        }

        private void timeExplosion_AI_Tick(object sender, EventArgs e)
        {
            AI_duration--;
            System.Diagnostics.Debug.WriteLine("AIduration = " + AI_duration);
            if (AI_duration <= 0)
            {
                timeExplosion_AI.Stop();
                for (int i = 0; i < 5; i++)
                {
                    //隱藏AI炸彈並移至角落
                    AI_explosions[i].boxExplosion.Visible = false;
                    AI_explosions[i].boxExplosion.Location = new Point(0, 0);

                    //將狀態設為"AI已投放炸彈"
                    AI_BombPlaced = false;
                }
            }
        }

        private void timeAI_Tick(object sender, EventArgs e)
        {
            Random random = new Random();

            if (step == Block.BlockHeight / ai.Speed)   //當動完完整一格
            {
                step = 0;       //步數歸零
                finish = true;  //將狀態設為"已動完完整一格"

                if (!AI_BombPlaced)  //判斷AI是否已放過炸彈(場上是否存在AI的炸彈),避免重複投彈
                {
                    place = random.Next(2); //隨機生成一個0~1的整數(用以判斷AI是否執行投彈動作)
                    if (place == 1)
                    {
                        //執行投彈
                        PlaceBomb(ai.boxCreature.Left, ai.boxCreature.Top, timeBomb_AI, "AI", AIBomb);
                    }
                }
            }
            if (finish) //當狀態為"已動完完整一格"
            {
                //隨機生成一個0~3的整數(用以判斷AI移動方位)
                way = random.Next(4);
                System.Diagnostics.Debug.WriteLine("way = " + way);

                ai.Movement(way);

                //判斷hitbox是否撞到"牆壁"和"炸彈"和"泥土"，如果沒撞到則移動AI到hitbox位置
                if (!CollisionCheck(ai.hitbox, "wall") && !CollisionCheck(ai.hitbox, "dirt") && !CollisionCheck(ai.hitbox, "explosion") && (!CollisionCheck(ai.hitbox, "bomb") || AIonBomb == true))
                {
                    finish = false;
                }
            }
            else
            {
                //持續朝所選方向移動(直到動完完整一格)
                ai.Movement(way);
                ai.boxCreature.Left = ai.hitbox.Left;
                ai.boxCreature.Top = ai.hitbox.Top;
                step++;
            }
        }
    }
}
