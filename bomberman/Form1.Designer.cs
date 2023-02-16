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
            this.timeExplosion = new System.Windows.Forms.Timer(this.components);
            this.timeBomb = new System.Windows.Forms.Timer(this.components);
            this.LabGameover = new System.Windows.Forms.Label();
            this.LabPoint = new System.Windows.Forms.Label();
            this.butLevel1 = new System.Windows.Forms.Button();
            this.butLevel2 = new System.Windows.Forms.Button();
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
            this.LabStartup.Location = new System.Drawing.Point(240, 229);
            this.LabStartup.Name = "LabStartup";
            this.LabStartup.Size = new System.Drawing.Size(534, 64);
            this.LabStartup.TabIndex = 0;
            this.LabStartup.Text = "Please select the level";
            // 
            // timeAI
            // 
            this.timeAI.Tick += new System.EventHandler(this.timeAI_Tick);
            // 
            // timeExplosion
            // 
            this.timeExplosion.Interval = 1000;
            this.timeExplosion.Tick += new System.EventHandler(this.timeExplosion_Tick);
            // 
            // timeBomb
            // 
            this.timeBomb.Interval = 1000;
            this.timeBomb.Tick += new System.EventHandler(this.timeBomb_Tick);
            // 
            // LabGameover
            // 
            this.LabGameover.AutoSize = true;
            this.LabGameover.Font = new System.Drawing.Font("Microsoft JhengHei UI", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LabGameover.Location = new System.Drawing.Point(355, 346);
            this.LabGameover.Name = "LabGameover";
            this.LabGameover.Size = new System.Drawing.Size(292, 64);
            this.LabGameover.TabIndex = 1;
            this.LabGameover.Text = "Game Over";
            this.LabGameover.Visible = false;
            // 
            // LabPoint
            // 
            this.LabPoint.AutoSize = true;
            this.LabPoint.Font = new System.Drawing.Font("Microsoft JhengHei UI", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LabPoint.Location = new System.Drawing.Point(1084, 161);
            this.LabPoint.Name = "LabPoint";
            this.LabPoint.Size = new System.Drawing.Size(324, 64);
            this.LabPoint.TabIndex = 3;
            this.LabPoint.Text = "Your points :";
            // 
            // butLevel1
            // 
            this.butLevel1.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.butLevel1.Location = new System.Drawing.Point(280, 468);
            this.butLevel1.Name = "butLevel1";
            this.butLevel1.Size = new System.Drawing.Size(150, 50);
            this.butLevel1.TabIndex = 4;
            this.butLevel1.Text = "Level1";
            this.butLevel1.UseVisualStyleBackColor = true;
            this.butLevel1.Click += new System.EventHandler(this.butLevel1_Click);
            // 
            // butLevel2
            // 
            this.butLevel2.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.butLevel2.Location = new System.Drawing.Point(554, 468);
            this.butLevel2.Name = "butLevel2";
            this.butLevel2.Size = new System.Drawing.Size(150, 50);
            this.butLevel2.TabIndex = 5;
            this.butLevel2.Text = "Level2";
            this.butLevel2.UseVisualStyleBackColor = true;
            this.butLevel2.Click += new System.EventHandler(this.butLevel2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1587, 1002);
            this.Controls.Add(this.butLevel2);
            this.Controls.Add(this.butLevel1);
            this.Controls.Add(this.LabPoint);
            this.Controls.Add(this.LabGameover);
            this.Controls.Add(this.LabStartup);
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
        private System.Windows.Forms.Timer timeExplosion;
        private System.Windows.Forms.Timer timeBomb;
        private Label LabGameover;
        private Label LabPoint;
        private Button butLevel1;
        private Button butLevel2;
    }
}