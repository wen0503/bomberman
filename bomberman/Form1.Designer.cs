namespace bomberman
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timeMain = new System.Windows.Forms.Timer(this.components);
            this.LabStartup = new System.Windows.Forms.Label();
            this.timeAI = new System.Windows.Forms.Timer(this.components);
            this.timeExplosion_Player = new System.Windows.Forms.Timer(this.components);
            this.timeBomb_Player = new System.Windows.Forms.Timer(this.components);
            this.timeBomb_AI = new System.Windows.Forms.Timer(this.components);
            this.timeExplosion_AI = new System.Windows.Forms.Timer(this.components);
            this.LabGameover = new System.Windows.Forms.Label();
            this.LabPoint = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // timeMain
            // 
            this.timeMain.Tick += new System.EventHandler(this.timeMain_Tick);
            // 
            // LabStartup
            // 
            this.LabStartup.AutoSize = true;
            this.LabStartup.Font = new System.Drawing.Font("Microsoft JhengHei UI", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LabStartup.Location = new System.Drawing.Point(207, 415);
            this.LabStartup.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabStartup.Name = "LabStartup";
            this.LabStartup.Size = new System.Drawing.Size(399, 50);
            this.LabStartup.TabIndex = 0;
            this.LabStartup.Text = "Press ENTER to start";
            // 
            // timeAI
            // 
            this.timeAI.Tick += new System.EventHandler(this.timeAI_Tick);
            // 
            // timeExplosion_Player
            // 
            this.timeExplosion_Player.Interval = 1000;
            this.timeExplosion_Player.Tick += new System.EventHandler(this.timeExplosion_Player_Tick);
            // 
            // timeBomb_Player
            // 
            this.timeBomb_Player.Interval = 1000;
            this.timeBomb_Player.Tick += new System.EventHandler(this.timeBomb_Player_Tick);
            // 
            // timeBomb_AI
            // 
            this.timeBomb_AI.Interval = 1000;
            this.timeBomb_AI.Tick += new System.EventHandler(this.timeBomb_AI_Tick);
            // 
            // timeExplosion_AI
            // 
            this.timeExplosion_AI.Interval = 1000;
            this.timeExplosion_AI.Tick += new System.EventHandler(this.timeExplosion_AI_Tick);
            // 
            // LabGameover
            // 
            this.LabGameover.AutoSize = true;
            this.LabGameover.Font = new System.Drawing.Font("Microsoft JhengHei UI", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LabGameover.Location = new System.Drawing.Point(276, 273);
            this.LabGameover.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabGameover.Name = "LabGameover";
            this.LabGameover.Size = new System.Drawing.Size(236, 50);
            this.LabGameover.TabIndex = 1;
            this.LabGameover.Text = "Game Over";
            this.LabGameover.Visible = false;
            // 
            // LabPoint
            // 
            this.LabPoint.AutoSize = true;
            this.LabPoint.Font = new System.Drawing.Font("Microsoft JhengHei UI", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LabPoint.Location = new System.Drawing.Point(843, 127);
            this.LabPoint.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabPoint.Name = "LabPoint";
            this.LabPoint.Size = new System.Drawing.Size(256, 50);
            this.LabPoint.TabIndex = 3;
            this.LabPoint.Text = "Your points :";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 791);
            this.Controls.Add(this.LabPoint);
            this.Controls.Add(this.LabGameover);
            this.Controls.Add(this.LabStartup);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Bomberman";
            this.Load += new System.EventHandler(this.Bomberman_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timeMain;
        private Label LabStartup;
        private System.Windows.Forms.Timer timeAI;
        private System.Windows.Forms.Timer timeExplosion_Player;
        private System.Windows.Forms.Timer timeBomb_Player;
        private System.Windows.Forms.Timer timeBomb_AI;
        private System.Windows.Forms.Timer timeExplosion_AI;
        private Label LabGameover;
        private Label LabPoint;
    }
}