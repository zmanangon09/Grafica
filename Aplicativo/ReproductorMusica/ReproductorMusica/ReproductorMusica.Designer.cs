namespace ReproductorMusica
{
    partial class ReproductorMusica
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnPlayPause = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.trackBarProgreso = new System.Windows.Forms.TrackBar();
            this.btnPrev = new System.Windows.Forms.Button();
            this.pbVisualizer = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnVolumen = new System.Windows.Forms.Button();
            this.trackBarVolumen = new System.Windows.Forms.TrackBar();
            this.button1 = new System.Windows.Forms.Button();
            this.lblTiempo = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.pictureBoxCover = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarProgreso)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbVisualizer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolumen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCover)).BeginInit();
            this.SuspendLayout();
            // 
            // btnPlayPause
            // 
            this.btnPlayPause.Location = new System.Drawing.Point(113, 515);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(90, 36);
            this.btnPlayPause.TabIndex = 0;
            this.btnPlayPause.Text = "▶";
            this.btnPlayPause.UseVisualStyleBackColor = true;
            this.btnPlayPause.Click += new System.EventHandler(this.btnPlayPause_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(209, 515);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(90, 36);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "⏭";
            this.btnNext.UseVisualStyleBackColor = true;
            // 
            // trackBarProgreso
            // 
            this.trackBarProgreso.Location = new System.Drawing.Point(305, 515);
            this.trackBarProgreso.Name = "trackBarProgreso";
            this.trackBarProgreso.Size = new System.Drawing.Size(612, 69);
            this.trackBarProgreso.TabIndex = 3;
            this.trackBarProgreso.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(17, 515);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(90, 36);
            this.btnPrev.TabIndex = 5;
            this.btnPrev.Text = "⏮";
            this.btnPrev.UseVisualStyleBackColor = true;
            // 
            // pbVisualizer
            // 
            this.pbVisualizer.BackColor = System.Drawing.Color.Black;
            this.pbVisualizer.Dock = System.Windows.Forms.DockStyle.Top;
            this.pbVisualizer.Location = new System.Drawing.Point(0, 0);
            this.pbVisualizer.Name = "pbVisualizer";
            this.pbVisualizer.Size = new System.Drawing.Size(1132, 503);
            this.pbVisualizer.TabIndex = 6;
            this.pbVisualizer.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnVolumen
            // 
            this.btnVolumen.Location = new System.Drawing.Point(1044, 553);
            this.btnVolumen.Name = "btnVolumen";
            this.btnVolumen.Size = new System.Drawing.Size(52, 36);
            this.btnVolumen.TabIndex = 7;
            this.btnVolumen.Text = "🔊";
            this.btnVolumen.UseVisualStyleBackColor = true;
            this.btnVolumen.Click += new System.EventHandler(this.btnVolumen_Click);
            // 
            // trackBarVolumen
            // 
            this.trackBarVolumen.Location = new System.Drawing.Point(1016, 483);
            this.trackBarVolumen.Maximum = 100;
            this.trackBarVolumen.Name = "trackBarVolumen";
            this.trackBarVolumen.Size = new System.Drawing.Size(104, 69);
            this.trackBarVolumen.TabIndex = 8;
            this.trackBarVolumen.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarVolumen.Value = 50;
            this.trackBarVolumen.Visible = false;
            this.trackBarVolumen.Scroll += new System.EventHandler(this.trackBarVolumen_Scroll);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(17, 558);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(81, 31);
            this.button1.TabIndex = 9;
            this.button1.Text = "Cargar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnCargar_Click);
            // 
            // lblTiempo
            // 
            this.lblTiempo.AutoSize = true;
            this.lblTiempo.ForeColor = System.Drawing.Color.Snow;
            this.lblTiempo.Location = new System.Drawing.Point(923, 523);
            this.lblTiempo.Name = "lblTiempo";
            this.lblTiempo.Size = new System.Drawing.Size(0, 20);
            this.lblTiempo.TabIndex = 10;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(113, 558);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(81, 31);
            this.button2.TabIndex = 11;
            this.button2.Text = "Cargar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // pictureBoxCover
            // 
            this.pictureBoxCover.Location = new System.Drawing.Point(472, 156);
            this.pictureBoxCover.Name = "pictureBoxCover";
            this.pictureBoxCover.Size = new System.Drawing.Size(205, 199);
            this.pictureBoxCover.TabIndex = 12;
            this.pictureBoxCover.TabStop = false;
            // 
            // ReproductorMusica
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1132, 596);
            this.Controls.Add(this.pictureBoxCover);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.lblTiempo);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.trackBarVolumen);
            this.Controls.Add(this.btnVolumen);
            this.Controls.Add(this.pbVisualizer);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.trackBarProgreso);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPlayPause);
            this.Name = "ReproductorMusica";
            this.Text = "ReproductorMusica";
            ((System.ComponentModel.ISupportInitialize)(this.trackBarProgreso)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbVisualizer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolumen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCover)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPlayPause;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.TrackBar trackBarProgreso;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.PictureBox pbVisualizer;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnVolumen;
        private System.Windows.Forms.TrackBar trackBarVolumen;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblTiempo;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PictureBox pictureBoxCover;
    }
}