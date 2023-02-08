using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Timers;
using Microsoft.VisualBasic;//�ޥ�microsoft.visualbasic�R�W�Ŷ�
using Microsoft.VisualBasic.Devices;//�ޥ�microsoft.visualbasic.devices�R�W�Ŷ�

namespace bomberman
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        List<Explosion> Player_explosions = new List<Explosion>();  //�z�����}�C(�s��Ҧ����a��Explosion)
        List<Explosion> AI_explosions = new List<Explosion>();      //�z�����}�C(�s��Ҧ�AI��Explosion)

        Bomb PlayerBomb = new Bomb();   //�إߪ��a���u
        Bomb AIBomb = new Bomb();       //�إ�AI���u
        Player player = new Player();   //�إߪ��a
        AI ai = new AI();   //�إ߹q��

        int Player_duration = Explosion.duration;   //���a�z�������ɶ�
        int AI_duration = Explosion.duration;       //AI�z�������ɶ�

        int Player_fuze = Bomb.fuze;    //���a���u���ޫH
        int AI_fuze = Bomb.fuze;        //AI���u���ޫH

        int step = 0;   //AI�����B��
        int way = 0;    //AI������V
        int place = 0;  //AI�O�_�񬵼u
        int point = 0;  //����

        bool start = false;             //�C���}�l���A(�O�_�w�}�l)
        bool finish = true;             //AI��������@�檺���A(�O�_�����@��)
        bool Player_BombPlaced = false; //���a�O�_�w��U���u(���W�O�_�w�s�b���a�����u)
        bool AI_BombPlaced = false;     //AI�O�_�w��U���u(���W�O�_�w�s�bAI�����u)
        bool PlayeronBomb = false;      //���a�O�_�b���u�W��(�ΨӸѨM��m���u��U�L�k���ʪ����D)
        bool AIonBomb = false;          //AI�O�_�b���u�W��(�P�W)

        //Computer ENTER_GAME = new Computer();  //�C���i�J����
        Computer GAME_starting = new Computer();  //�C����������
        Computer bomb_wav = new Computer();  //�C����������

        public Form1()
        {
            InitializeComponent();
        }

        private void Bomberman_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;

            this.Size = new Size(1250, 830);

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

        private void StartGame()
        {
            start = true;   //�N�C�����A�]��"�}�l"
            Init();
            MapLoader(1);   //Ū���a�ϸ�T(Level1.txt)
            timeMain.Start();
            timeAI.Start();
            LabPoint.Visible = true;
            GAME_starting.Audio.Play("GAME_starting.wav", AudioPlayMode.BackgroundLoop);//���U�}�l�C�����򼽩񭵼�
        }

        private void Init()
        {
            //���éҦ�Label
            LabStartup.Visible = false;
            LabGameover.Visible = false;

            //��l��
            Player_BombPlaced = false;
            AI_BombPlaced = false;
            finish = true;
            step = 0;
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

        private void GameOver()//�C������
        {
            start = false; //�N�C�����A�]��"���}�l"

            //���ΩҦ�Timer
            timeMain.Stop();
            timeAI.Stop();
            timeBomb_AI.Stop();
            timeBomb_Player.Stop();
            timeExplosion_AI.Stop();
            timeExplosion_Player.Stop();

            foreach (Control control in this.Controls)  //�N�Ҧ�controls�v��
            {
                control.Visible = false;
            }

            LabGameover.Visible = true;
            LabStartup.Visible = true;           
        }

        private void MapLoader(int level) //Ū�����d
        {
            int spawnX = 0, spawnY = 0; //��l���a�ͦ��y��

            string strLevel = string.Empty;
            switch (level)  //���d���
            {
                case 1: //���d1                    
                    spawnX = 70;    //�]�w���a�ͦ��y��
                    spawnY = 70;
                    ai.Spawn(spawnX + 140, spawnY + 210);   //�ͦ��q��
                    strLevel = Properties.Resources.level1; //Ū���۹�������r��
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

            player.Spawn(spawnX, spawnY);   //�ͦ����a            



            using (StringReader reader = new StringReader(strLevel))
            {
                int posX = 0, posY = 0; //��l�������m

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
                    }
                    posX = 0;                   //�^���l��m(����)
                    posY += Block.BlockWidth;   //�V�U�첾�@�Ӥ����
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Keys input;
            input = e.KeyCode;

            if (input == Keys.Enter && start == false)   //���a���UENTER�C���}�l
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
                        if (!Player_BombPlaced)
                        {
                            PlaceBomb(player.boxCreature.Left, player.boxCreature.Top, timeBomb_Player, "player", PlayerBomb);
                        }
                        break;

                    //����
                    case Keys.W:
                        player.hitbox.Top -= player.Speed; //����hitbox����
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

                //�P�_hitbox�O�_����"���"�M"���u"�M"�d�g"�A�p�G�S����h���ʪ��a��hitbox��m
                if (!CollisionCheck(player.hitbox, "wall") && !CollisionCheck(player.hitbox, "dirt") && (!CollisionCheck(player.hitbox, "bomb") || PlayeronBomb == true))
                {
                    player.boxCreature.Left = player.hitbox.Left;
                    player.boxCreature.Top = player.hitbox.Top;
                }
            }
        }

        //�I���˴�(�P�_�⪫�O�_���|)
        public bool CollisionCheck(PictureBox box, string tag)//�P�_�O�_�I�������
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

        //�񬵼u
        private void PlaceBomb(int posX, int posY, System.Windows.Forms.Timer timer, string type, Bomb bomb)
        {
            switch (type)   //�P�_�O�֩񪺬��u
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

            bomb.Spawn(posX, posY); //�ͦ����u(���ʬ��u)
            System.Diagnostics.Debug.WriteLine("bombX = " + bomb.bombX + ", bombY = " + bomb.bombY);

            timer.Start();  //�}�l���u��Timer
        }

        //���u�z��
        private void Explode(List<Explosion> ex, System.Windows.Forms.Timer timer, Bomb bomb)
        {
            timer.Start();

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
        }

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

                while (!CollisionCheck(ai.hitbox, "grass")) //���ͦa������a�ɭ��s��ܭ��ͦa
                {
                    ai.ChoosePlace();  //��ܭ��ͦa
                }

                step = 0;       //�B���k�s
                finish = true;  //�N���A�]��"�w�ʧ�����@��"

                if(start == true)
                {
                    ai.Spawn(ai.hitbox.Left, ai.hitbox.Top);    //�ͦ�AI
                }
            }

            //�ˬd���a��U�O�_�b���u�W(�ΨӸѨM��m���u��U�L�k���ʪ����D)
            if (CollisionCheck(ai.boxCreature, "bomb") && AIonBomb == false)
            {
                AIonBomb = true;
            }
            else
            {
                AIonBomb = false;
            }

            //�ˬdAI��U�O�_�b���u�W(�ΨӸѨM��m���u��U�L�k���ʪ����D)
            if (CollisionCheck(player.boxCreature, "bomb") && PlayeronBomb == false)
            {
                PlayeronBomb = true;
            }
            else
            {
                PlayeronBomb = false;
            }

            //�C�|�Xform���Ҧ�Tag��"dirt"���Ҧ�Picturebox
            foreach (PictureBox picturebox in
                this.Controls.OfType<PictureBox>()
                .Where(pb => !(pb.Tag == null) && pb.Tag.ToString() == "dirt"))
            {
                if (CollisionCheck(picturebox, "explosion"))  //�P�_�d�g�O�_�Q����
                {
                    //�N�d�g��������a
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
                //�����z��
                if (start == true)
                {
                    Explode(Player_explosions, timeExplosion_Player, PlayerBomb);
                    bomb_wav.Audio.Play("bomb.wav", AudioPlayMode.Background); //�z������
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
                    //���ê��a���u�ò��ܨ���
                    Player_explosions[i].boxExplosion.Visible = false;
                    Player_explosions[i].boxExplosion.Location = new Point(0, 0);

                    //�N���A�]��"���a�w��񬵼u"
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
                //�����z��
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
                    //����AI���u�ò��ܨ���
                    AI_explosions[i].boxExplosion.Visible = false;
                    AI_explosions[i].boxExplosion.Location = new Point(0, 0);

                    //�N���A�]��"AI�w��񬵼u"
                    AI_BombPlaced = false;
                }
            }
        }

        private void timeAI_Tick(object sender, EventArgs e)
        {
            Random random = new Random();

            if (step == Block.BlockHeight / ai.Speed)   //��ʧ�����@��
            {
                step = 0;       //�B���k�s
                finish = true;  //�N���A�]��"�w�ʧ�����@��"

                if (!AI_BombPlaced)  //�P�_AI�O�_�w��L���u(���W�O�_�s�bAI�����u),�קK���Ƨ�u
                {
                    place = random.Next(2); //�H���ͦ��@��0~1�����(�ΥH�P�_AI�O�_�����u�ʧ@)
                    if (place == 1)
                    {
                        //�����u
                        PlaceBomb(ai.boxCreature.Left, ai.boxCreature.Top, timeBomb_AI, "AI", AIBomb);
                    }
                }
            }
            if (finish) //���A��"�w�ʧ�����@��"
            {
                //�H���ͦ��@��0~3�����(�ΥH�P�_AI���ʤ��)
                way = random.Next(4);
                System.Diagnostics.Debug.WriteLine("way = " + way);

                ai.Movement(way);

                //�P�_hitbox�O�_����"���"�M"���u"�M"�d�g"�A�p�G�S����h����AI��hitbox��m
                if (!CollisionCheck(ai.hitbox, "wall") && !CollisionCheck(ai.hitbox, "dirt") && !CollisionCheck(ai.hitbox, "explosion") && (!CollisionCheck(ai.hitbox, "bomb") || AIonBomb == true))
                {
                    finish = false;
                }
            }
            else
            {
                //����©ҿ��V����(����ʧ�����@��)
                ai.Movement(way);
                ai.boxCreature.Left = ai.hitbox.Left;
                ai.boxCreature.Top = ai.hitbox.Top;
                step++;
            }
        }
    }
}
