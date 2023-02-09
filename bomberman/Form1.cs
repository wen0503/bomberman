using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Timers;
using Microsoft.VisualBasic;//�ޥ�microsoft.visualbasic�R�W�Ŷ�
using Microsoft.VisualBasic.Devices;//�ޥ�microsoft.visualbasic.devices�R�W�Ŷ�
using System.Runtime.Intrinsics.X86;
using System;
using System.Diagnostics;

namespace bomberman
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public const int MAP_SIZE = 11;     //�a�Ϥj�p

        int[,] obstacal = new int[MAP_SIZE, MAP_SIZE];  //�����a�Ϥ�����ê��        

        List<Explosion> Player_explosions = new List<Explosion>();  //�z�����}�C(�s��Ҧ����a��Explosion)
        List<Explosion> AI_explosions = new List<Explosion>();      //�z�����}�C(�s��Ҧ�AI��Explosion)

        Bomb PlayerBomb = new Bomb();   //�إߪ��a���u
        Bomb AIBomb = new Bomb();       //�إ�AI���u
        Player player = new Player();   //�إߪ��a
        AI ai = new AI();               //�إ�AI

        int Player_duration = Explosion.duration;   //���a�z�������ɶ�
        int AI_duration = Explosion.duration;       //AI�z�������ɶ�

        int Player_fuze = Bomb.fuze;    //���a���u���ޫH
        int AI_fuze = Bomb.fuze;        //AI���u���ޫH

        int point = 0;  //����

        bool start = false;             //�C���}�l���A(�O�_�w�}�l)

        //Computer ENTER_GAME = new Computer();  //�C���i�J����
        Computer explode_wav = new Computer();  //�C����������

        public Form1()
        {
            InitializeComponent();
        }

        private void Bomberman_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;

            this.Size = new Size(1250, 830);        //�]�w�����j�p

            this.Controls.Add(player.boxCreature);  //�s�W���a
            this.Controls.Add(ai.boxCreature);      //�s�WAI
            this.Controls.Add(PlayerBomb.boxBomb);  //�s�W���a���u
            this.Controls.Add(AIBomb.boxBomb);      //�s�WAI���u

            for (int i = 0; i < 5; i++) //0�W 1�U 2�� 3�k 4��
            {
                Player_explosions.Add(new Explosion());                 //�N���a���ͪ��z����J�}�C
                this.Controls.Add(Player_explosions[i].boxExplosion);   //�s�W���a�z��

                AI_explosions.Add(new Explosion());                 //�NAI���ͪ��z����J�}�C
                this.Controls.Add(AI_explosions[i].boxExplosion);   //�s�WAI�z��
            }

            Init();
            LabStartup.Visible = true;
        }

        //�C���}�l
        private void StartGame()
        {
            start = true;   //�N�C�����A�]��"�}�l"
            Init();         //��l��
            MapLoader(1);   //Ū���a�ϸ�T(Level1.txt)
            timeMain.Start();   //�}�l"�DTimer"
            timeAI.Start();     //�}�l"AITimer"
            LabPoint.Visible = true;    //��ܤ���label
        }

        //��l��
        private void Init()
        {
            //���éҦ�Label
            LabStartup.Visible = false;
            LabGameover.Visible = false;

            //��l��
            player.BombPlaced = false;
            ai.BombPlaced = false;
            ai.step = Block.BlockWidth / ai.Speed;
            point = 0;

            //�N���a����
            player.boxCreature.Visible = false;

            //�NAI����
            ai.boxCreature.Visible = false;

            //�N���a�����u���èò��ܨ���
            PlayerBomb.boxBomb.Visible = false;
            PlayerBomb.boxBomb.Location = new Point(0, 0);

            //�NAI�����u���èò��ܨ���
            AIBomb.boxBomb.Visible = false;
            AIBomb.boxBomb.Location = new Point(0, 0);


            for (int i = 0; i < 5; i++) //0�W 1�U 2�� 3�k 4��
            {
                //�N���a���ͪ��z�����èò��ܨ���
                Player_explosions[i].boxExplosion.Visible = false;
                Player_explosions[i].boxExplosion.Location = new Point(0, 0);

                //�NAI���ͪ��z�����èò��ܨ���
                AI_explosions[i].boxExplosion.Visible = false;
                AI_explosions[i].boxExplosion.Location = new Point(0, 0);
            }

            //�R���Ҧ��������(����B��a�B�d�g)
            foreach (PictureBox picturebox in
                this.Controls.OfType<PictureBox>()
                .Where(pb => !(pb.Tag == null) && (pb.Tag.ToString() == "wall" || pb.Tag.ToString() == "grass" || pb.Tag.ToString() == "dirt")))
            {
                this.Controls.Remove(picturebox);
            }
        }

        //�C������
        private void GameOver()
        {
            start = false; //�N�C�����A�]��"���}�l"

            //���ΩҦ�Timer
            timeMain.Stop();
            timeAI.Stop();
            timeBomb_AI.Stop();
            timeBomb_Player.Stop();
            timeExplosion_AI.Stop();
            timeExplosion_Player.Stop();

            //�N�Ҧ�controls�v��
            foreach (Control control in this.Controls)  
            {
                control.Visible = false;
            }

            //��ܩһ�label
            LabPoint.Visible = true;
            LabGameover.Visible = true;
            LabStartup.Visible = true;           
        }

        //Ū�����d
        private void MapLoader(int level) 
        {
            int spawnX = 0, spawnY = 0; //��l���a�ͦ��y��

            string strLevel = string.Empty;
            switch (level)  //���d���
            {
                case 1: //���d1                    
                    spawnX = 140;    //�]�w���a�ͦ��y��
                    spawnY = 70;
                    ai.Spawn(210, 280);   //�ͦ��q��
                    strLevel = Properties.Resources.level1; //Ū���۹�������r��
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

            player.Spawn(spawnX, spawnY);   //�ͦ����a

            using (StringReader reader = new StringReader(strLevel))
            {
                int posX = 0, posY = 0; //��l�������m
                int i = 0, j = 0;       //��l���ޭ�

                string strings = string.Empty;
                while ((strings = reader.ReadLine()) != null)    //Ū��level1��r�ɤ�����r
                {
                    string[] blocks = strings.Split(' ');

                    foreach (string type in blocks)
                    {
                        Block block = new Block();
                        block.Spawn(type, posX, posY);      //�CŪ���@�r���N�إߤ@�ӷsbutton(�������)                        
                        this.Controls.Add(block.boxBlock);  //��m���
                        posX += Block.BlockWidth;   //�V�k�첾�@�Ӥ���e

                        if (type == "N")    //�p�G����a���
                        {
                            obstacal[i, j] = 0; //�����D��ê��
                        }
                        else
                        {
                            obstacal[i, j] = 1; //������ê��
                        }
                        j++;
                    }
                    posX = 0;                   //�^���l��m(����)
                    posY += Block.BlockHeight;   //�V�U�첾�@�Ӥ����

                    j = 0;
                    i++;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Keys input;
            input = e.KeyCode;

            //���a���UENTER�C���}�l
            if (input == Keys.Enter && start == false)
            {
                StartGame();
            }

            if (start == true)
            {
                player.hitbox.Left = player.boxCreature.Left;  //���Nhitbox���ܪ��a��m
                player.hitbox.Top = player.boxCreature.Top;

                switch (input)
                {
                    //�񬵼u
                    case Keys.Space:
                        if (!player.BombPlaced)
                        {
                            //��m���u
                            PlaceBomb(player.boxCreature.Left, player.boxCreature.Top, timeBomb_Player, "player", PlayerBomb);
                        }
                        break;

                    //����
                    case Keys.W:
                        player.hitbox.Top -= player.Speed; //����hitbox����                       
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

                //�P�_hitbox�O�_����"���"�M"���u"�M"�d�g"�A�p�G�S����h���ʪ��a��hitbox��m
                if (!CollisionCheck(player.hitbox, "wall") && !CollisionCheck(player.hitbox, "dirt") && (!CollisionCheck(player.hitbox, "bomb") || player.OnBomb))
                {
                    player.boxCreature.Left = player.hitbox.Left;
                    player.boxCreature.Top = player.hitbox.Top;
                }
            }
        }

        //�I���˴�(�P�_�⪫�O�_���|)
        public bool CollisionCheck(PictureBox box, string tag)
        {
            //�C�|�Xform���Ҧ�Tag��tag���Ҧ�Picturebox
            foreach (PictureBox picturebox in
                this.Controls.OfType<PictureBox>()
                .Where(pb => !(pb.Tag == null) && pb.Tag.ToString() == tag))
            {
                if (box.Bounds.IntersectsWith(picturebox.Bounds))  //�P�_��̬O�_���|
                {
                    return true;//�p���|�h�^��ture
                }
            }
            return false;//�p�L���|�h�^��false
        }


        //------------------------------Bomb Function BEGIN-------------------------------------------

        //�񬵼u
        private void PlaceBomb(int posX, int posY, System.Windows.Forms.Timer timer, string type, Bomb bomb)
        {           
            switch (type)   //�P�_�O�֩񪺬��u
            {
                case "player":
                    player.BombPlaced = true;               //�]�w���a���A��"�w��m���u"
                    player.OnBomb = true;                   //�]�w���a���A��"�b���u�W"
                    Player_duration = Explosion.duration;   //��l�z������ɶ�
                    Player_fuze = Bomb.fuze;                //��l���u�ޫH
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

            bomb.Spawn(posX, posY); //�ͦ����u(���ʬ��u)

            //�N���u�Ҧb�B�]����ê��(���u�|�׸�)
            obstacal[bomb.bombY / Block.BlockHeight, bomb.bombX / Block.BlockWidth] = 1;

            timer.Start();  //�}�l���u��Timer
        }

        //���u�z��
        private void Explode(List<Explosion> ex, System.Windows.Forms.Timer timer, Bomb bomb)
        {            
            explode_wav.Audio.Play(Properties.Resources.explode, AudioPlayMode.Background); //�z������
            
            bomb.boxBomb.Visible = false;   //���ì��u
            bomb.boxBomb.Location = new Point(0, 0); //�N���u���ܨ���(�קK�v�T�C��)

            //�ͦ��Ӥ�쪺�z��(0�W 1�U 2�� 3�k 4��)
            ex[0].Spawn(bomb.bombX, bomb.bombY - Explosion.ExplosionHeight);
            ex[1].Spawn(bomb.bombX, bomb.bombY + Explosion.ExplosionHeight);
            ex[2].Spawn(bomb.bombX - Explosion.ExplosionWidth, bomb.bombY);
            ex[3].Spawn(bomb.bombX + Explosion.ExplosionWidth, bomb.bombY);
            ex[4].Spawn(bomb.bombX, bomb.bombY);
            for (int i = 0; i < 5; i++)
            {
                if (!CollisionCheck(ex[i].boxExplosion, "wall")) //�ˬd�z���O�_�I����
                {
                    ex[i].boxExplosion.Visible = true;  //�N�I�����z������
                }
            }

            //������ê��(���u�z����N���A�O��ê��)
            obstacal[bomb.bombY / Block.BlockHeight, bomb.bombX / Block.BlockWidth] = 0;

            timer.Start();  //�}�l�z����Timer
        }

        //------------------------------Bomb Function END-------------------------------------------

        //------------------------------Timer_Tick BEGIN-------------------------------------------
        private void timeMain_Tick(object sender, EventArgs e)
        {

            LabPoint.Text = "Your points : " + point;   //�N���ƥ[�bLabel�W            

            if (CollisionCheck(player.boxCreature, "explosion"))    //�ˬd���a�O�_�I���z��
            {
                player.Die();   //���a���`
                GameOver();     //�C������
            }

            if (CollisionCheck(ai.boxCreature, "explosion"))
            {
                ai.Die();   //AI���`

                point++;    //�[��

                if(start == true)
                {
                    while (!CollisionCheck(ai.hitbox, "grass")) //���ͦa������a�ɭ��s��ܭ��ͦa
                    {
                        ai.ChoosePlace();  //��ܭ��ͦa
                    }

                    ai.step = Block.BlockHeight / ai.Speed;     //���m�B��
                    ai.Spawn(ai.hitbox.Left, ai.hitbox.Top);    //�ͦ�AI
                }
            }

            //�ˬdAI��U�O�_�b���u�W(�ΨӸѨM��m���u��U�L�k���ʪ����D)
            if (!CollisionCheck(ai.boxCreature, "bomb"))
            {
                ai.OnBomb = false;
            }
            else
            {
                ai.OnBomb = true;
            }

            //�ˬd���a��U�O�_�b���u�W(�ΨӸѨM��m���u��U�L�k���ʪ����D)
            if (!CollisionCheck(player.boxCreature, "bomb"))
            {
                player.OnBomb = false;
            }
            else
            {
                player.OnBomb = true;
            }

            //�C�|�Xform���Ҧ�Tag��"dirt"���Ҧ�Picturebox
            foreach (PictureBox picturebox in
                this.Controls.OfType<PictureBox>()
                .Where(pb => !(pb.Tag == null) && pb.Tag.ToString() == "dirt"))
            {
                if (CollisionCheck(picturebox, "explosion"))  //�P�_�d�g�O�_�Q����
                {
                    //�N���u�y���ഫ�����ޭ�
                    int pX = picturebox.Left / Block.BlockWidth;
                    int pY = picturebox.Top / Block.BlockHeight;

                    //�N�d�g��������a
                    picturebox.Image = Properties.Resources.grass;
                    picturebox.Tag = "grass";

                    //������ê��(�d�g�Q�}�a��N���A�O��ê��)
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
                    //�����z��
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
                    //���ê��a���u�ò��ܨ���
                    Player_explosions[i].boxExplosion.Visible = false;
                    Player_explosions[i].boxExplosion.Location = new Point(0, 0);

                    //�N���A�]��"���a�w��񬵼u"
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
                    //�����z��
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
                    //����AI���u�ò��ܨ���
                    AI_explosions[i].boxExplosion.Visible = false;
                    AI_explosions[i].boxExplosion.Location = new Point(0, 0);

                    //�N���A�]��"AI�w��񬵼u"
                    ai.BombPlaced = false;
                }
            }
        }

        private void timeAI_Tick(object sender, EventArgs e)
        {
            if (ai.step == Block.BlockWidth / ai.Speed)   //��ʧ�����@��
            {
                ai.step = 0;        //�B���k�s

                if (!ai.BombPlaced) //�P�_AI�O�_�w��L���u(���W�O�_�s�bAI�����u),�קK���Ƨ�u
                {
                    Random random = new Random();

                    ai.place = random.Next(2); //�H���ͦ��@��0~1�����(�ΥH�P�_AI�O�_�����u�ʧ@)
                    if (ai.place == 1)
                    {
                        //�����u
                        PlaceBomb(ai.boxCreature.Left, ai.boxCreature.Top, timeBomb_AI, "AI", AIBomb);
                    }
                }

                //��ܤ�V
                ai.ChooseWay(player.boxCreature.Left, player.boxCreature.Top, obstacal);
            }
            else
            {
                //����©ҿ��V����(����ʧ�����@��)
                ai.Move();               
                ai.boxCreature.Left = ai.hitbox.Left;
                ai.boxCreature.Top = ai.hitbox.Top;
                ai.step++;                             
            }                      
        }
        //------------------------------Timer_Tick END-------------------------------------------
    }
}
