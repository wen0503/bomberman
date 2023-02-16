using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Timers;
using Microsoft.VisualBasic;//�ޥ�microsoft.visualbasic�R�W�Ŷ�
using Microsoft.VisualBasic.Devices;//�ޥ�microsoft.visualbasic.devices�R�W�Ŷ�
using System.Runtime.Intrinsics.X86;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;

namespace bomberman
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public const int MAP_SIZE = 11;     //�a�Ϥj�p

        int[,] obstacle = new int[MAP_SIZE, MAP_SIZE];  //�����a�Ϥ�����ê��        

        List<AI> AIs = new List<AI>();                      //�x�s�Ҧ�AI���}�C
        List<Creature> creatures = new List<Creature>();    //�x�s�Ҧ��ͪ����}�C

        Player player = new Player();   //�إߪ��a

        int point = 0;          //����

        bool start = false;     //�C���}�l���A(�O�_�w�}�l)

        Computer explode_wav = new Computer();  //�C����������

        public Form1()
        {
            InitializeComponent();
        }

        private void Bomberman_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;

            this.Size = new Size(1250, 830);        //�]�w�����j�p

            //��l�ƪ��a
            this.Controls.Add(player.boxCreature);  //�s�W���a
            this.Controls.Add(player.bomb.boxBomb); //�s�W���a���u
            for (int i = 0; i < 5; i++)
            {
                this.Controls.Add(player.explosions[i].boxExplosion);   //�s�W���a�z��
            }

            //��l��AI
            AIs.Add(new AI("red"));
            AIs.Add(new AI("green"));
            AIs.Add(new AI("yellow"));

            foreach(AI ai in AIs)
            {
                this.Controls.Add(ai.boxCreature);  //�s�WAI  
                this.Controls.Add(ai.bomb.boxBomb); //�s�WAI���u
                for (int j = 0; j < 5; j++)
                {
                    this.Controls.Add(ai.explosions[j].boxExplosion);   //�s�WAI�z��
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
            MapLoader(1);   //Ū���a�ϸ�T(Level1.txt)
        }

        private void butLevel2_Click(object sender, EventArgs e)
        {
            StartGame();
            MapLoader(2);   //Ū���a�ϸ�T(Level2.txt)
        }

        //�C���}�l
        private void StartGame()
        {
            start = true;   //�N�C�����A�]��"�}�l"
            Init();         //��l��         
        }

        //��l��
        private void Init()
        {           
            //���éҦ�Label
            LabStartup.Visible = false;
            LabGameover.Visible = false;

            //���èð��ΩҦ�Buttons
            butLevel1.Visible = false;
            butLevel2.Visible = false;
            butLevel1.Enabled = false;
            butLevel2.Enabled = false;

            //��l�ƪ��a
            player.BombPlaced = false;
            player.Explode = false;
            player.boxCreature.Visible = false;
            //�N���a�����u���èò��ܨ���
            player.bomb.boxBomb.Visible = false;
            player.bomb.boxBomb.Location = new Point(0, 0);
            for (int i = 0; i < 5; i++)
            {
                //�N���a���ͪ��z�����èò��ܨ���
                player.explosions[i].boxExplosion.Visible = false;
                player.explosions[i].boxExplosion.Location = new Point(0, 0);
            }

            //��l��AI
            foreach (AI ai in AIs)
            {
                ai.BombPlaced = false;
                ai.Explode = false;
                ai.boxCreature.Visible = false; //�NAI����

                //�NAI�����u���èò��ܨ���
                ai.bomb.boxBomb.Visible = false;
                ai.bomb.boxBomb.Location = new Point(0, 0);

                //�NAI���ͪ��z�����èò��ܨ���
                for (int i = 0; i < 5; i++)
                {
                    ai.explosions[i].boxExplosion.Visible = false;
                    ai.explosions[i].boxExplosion.Location = new Point(0, 0);
                }
            }

            //�R���Ҧ��������(����B��a�B�d�g)
            IEnumerable<PictureBox> pb = this.Controls.OfType<PictureBox>().Where(pb => !(pb.Tag == null) && (pb.Tag.ToString() == "wall" || pb.Tag.ToString() == "grass" || pb.Tag.ToString() == "dirt"));
            while (pb.Count() != 0)
            {
                foreach (Control control in pb)
                {
                    this.Controls.Remove(control);
                    control.Dispose();
                }
            }

            //�ҥΩҦ�Timer
            timeMain.Start();
            timeAI.Start();
            timeBomb.Start();
            timeExplosion.Start();

            point = 0;                  //���m����
            LabPoint.Visible = true;    //��ܤ���label
        }

        //�C������
        private void GameOver()
        {
            start = false; //�N�C�����A�]��"���}�l"

            //���ΩҦ�Timer
            timeMain.Stop();
            timeAI.Stop();
            timeBomb.Stop();
            timeExplosion.Stop();

            //�N�Ҧ�controls�v��
            foreach (Control control in this.Controls)
            {
                control.Visible = false;
            }

            //��ܩһ�label
            LabPoint.Visible = true;
            LabGameover.Visible = true;
            LabStartup.Visible = true;

            //��ܨñҥΩҦ�Button
            butLevel1.Visible = true;
            butLevel2.Visible = true;
            butLevel1.Enabled = true;
            butLevel2.Enabled = true;
        }

        //Ū�����d
        private void MapLoader(int level) 
        {
            int spawnX = 0, spawnY = 0; //��l���a�ͦ��y��

            string strLevel = string.Empty;
            switch (level)  //���d���
            {
                case 1: //���d1
                    player.Speed = 7;
                    foreach(AI ai in AIs)
                    {
                        ai.Speed = 10;
                        ai.step = Block.BlockWidth / ai.Speed;
                    }
                    spawnX = 140;           //�]�w���a�ͦ��y��
                    spawnY = 70;
                    AIs[0].Spawn(630, 70);  //�ͦ�AI
                    AIs[1].Spawn(70, 630);  //�ͦ�AI
                    AIs[2].Spawn(630, 630); //�ͦ�AI
                    strLevel = Properties.Resources.level1; //Ū���۹�������r��
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

            player.Spawn(spawnX, spawnY);   //�ͦ����a

            using (StringReader reader = new StringReader(strLevel))
            {
                int posX = 0, posY = 0; //��l�������m
                int i = 0, j = 0;       //��l���ޭ�

                string strings = string.Empty;
                while ((strings = reader.ReadLine()) != null)    //Ū��level1��r�ɤ�����r
                {
                    string[] str = strings.Split(' ');

                    foreach (string type in str)
                    {
                        Block block = new Block();
                        block.Spawn(type, posX, posY);      //�CŪ���@�r���N�إߤ@�ӷsbutton(�������)                        
                        this.Controls.Add(block.boxBlock);  //��m���
                        block = null;

                        posX += Block.BlockWidth;   //�V�k�첾�@�Ӥ���e

                        if (type == "N")    //�p�G����a���
                        {
                            obstacle[i, j] = 0; //�����D��ê��
                        }
                        else
                        {
                            obstacle[i, j] = 1; //������ê��
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
                            PlaceBomb(player);
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
        private void PlaceBomb(Creature cr)
        {
            cr.BombPlaced = true;                   //�]�w���a���A��"�w��m���u"
            cr.OnBomb = true;                       //�]�w���a���A��"�b���u�W"
            cr.fuze = Creature.DefaultFuze;         //���m���u�ޫH
            cr.duration = Creature.DefaultDuration; //���m�z������ɶ�

            cr.bomb.Spawn(cr.boxCreature.Left, cr.boxCreature.Top); //�ͦ����u(���ʬ��u)

            //�N���u�Ҧb�B�]����ê��(���u�|�׸�)
            obstacle[cr.bomb.bombY / Block.BlockHeight, cr.bomb.bombX / Block.BlockWidth] = 1;
        }

        //���u�z��
        private void Explode(Creature cr)
        {            
            explode_wav.Audio.Play(Properties.Resources.explode, AudioPlayMode.Background); //�z������
            
            cr.bomb.boxBomb.Visible = false;            //���ì��u
            cr.bomb.boxBomb.Location = new Point(0, 0); //�N���u���ܨ���(�קK�v�T�C��)

            cr.Explode = true;  //�N���A�]��"���u�w�z��"

            //�ͦ��Ӥ�쪺�z��(0�W 1�U 2�� 3�k 4��)
            cr.explosions[0].Spawn(cr.bomb.bombX, cr.bomb.bombY - Explosion.ExplosionHeight);
            cr.explosions[1].Spawn(cr.bomb.bombX, cr.bomb.bombY + Explosion.ExplosionHeight);
            cr.explosions[2].Spawn(cr.bomb.bombX - Explosion.ExplosionWidth, cr.bomb.bombY);
            cr.explosions[3].Spawn(cr.bomb.bombX + Explosion.ExplosionWidth, cr.bomb.bombY);
            cr.explosions[4].Spawn(cr.bomb.bombX, cr.bomb.bombY);
            for (int i = 0; i < 5; i++)
            {
                if (!CollisionCheck(cr.explosions[i].boxExplosion, "wall")) //�ˬd�z���O�_�I����
                {
                    cr.explosions[i].boxExplosion.Visible = true;  //�N�I�����z������
                }
            }

            //������ê��(���u�z����N���A�O��ê��)
            obstacle[cr.bomb.bombY / Block.BlockHeight, cr.bomb.bombX / Block.BlockWidth] = 0;
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

            foreach(AI ai in AIs)
            {
                if (CollisionCheck(ai.boxCreature, "explosion"))
                {
                    ai.Die();   //AI���`

                    point++;    //�[��

                    if (start == true)
                    {
                        do
                        {
                            ai.ChoosePlace();  //��ܭ��ͦa                                                     
                        } while (!CollisionCheck(ai.hitbox, "grass"));  //���ͦa������a�ɭ��s��ܭ��ͦa

                        do
                        {
                            Random rand = new Random();
                            ai.target = rand.Next(4);   //�H����ܧ����ؼ�
                        } while (ai.target == creatures.IndexOf(ai)); //������ؼЬ��ۤv�ɭ���

                        ai.step = Block.BlockHeight / ai.Speed;     //���m�B��
                        ai.Spawn(ai.hitbox.Left, ai.hitbox.Top);    //�ͦ�AI
                    }
                }               
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
                    obstacle[pY, pX] = 0;
                }
            }
        }

        private void timeBomb_Tick(object sender, EventArgs e)
        {
            //���u�w��m
            foreach (Creature cr in creatures)
            {
                if (cr.BombPlaced)
                {
                    cr.fuze--;
                    if (cr.fuze <= 0)
                    {
                        //�����z��
                        Explode(cr);
                    }
                }
            }
        }

        private void timeExplosion_Tick(object sender, EventArgs e)
        {
            //���u�w�z��
            foreach (Creature cr in creatures)
            {
                if (cr.Explode)
                {
                    cr.duration--;
                    if (cr.duration <= 0)
                    {
                        cr.BombPlaced = false;  //�N���A�]��"�w��񬵼u"
                        cr.Explode = false;     //�N���A�]��"��񪺬��u�|���z��"

                        for (int i = 0; i < 5; i++)
                        {
                            //�����z���ò��ܨ���
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
                if (ai.step == Block.BlockWidth / ai.Speed)   //��ʧ�����@��
                {
                    ai.step = 0;        //�B���k�s

                    if (!ai.BombPlaced) //�P�_AI�O�_�w��L���u(���W�O�_�s�bAI�����u),�קK���Ƨ�u
                    {
                        Random rand = new Random();

                        //�H���ͦ��@��0~9�����(�ΥH���AI��1/10���v�|�����u�ʧ@)
                        ai.place = rand.Next(10); 
                        if (ai.place == 1)
                        {
                            //�����u
                            PlaceBomb(ai);
                        }
                    }

                    //��ܤ�V
                    ai.ChooseWay(creatures[ai.target].boxCreature.Left, creatures[ai.target].boxCreature.Top, obstacle);
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
        }

        //------------------------------Timer_Tick END-------------------------------------------
    }
}
