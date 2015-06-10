using System.Windows.Forms;
namespace ApacsAdapterController
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.msgLabel = new System.Windows.Forms.Label();
            this.timeLabel = new System.Windows.Forms.Label();
            this.mainPhoto = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.mainPhoto)).BeginInit();
            this.SuspendLayout();
            // 
            // msgLabel
            // 
            this.msgLabel.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.msgLabel, "msgLabel");
            this.msgLabel.ForeColor = System.Drawing.Color.Black;
            this.msgLabel.Name = "msgLabel";
            this.msgLabel.UseMnemonic = false;
            // 
            // timeLabel
            // 
            this.timeLabel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.timeLabel, "timeLabel");
            this.timeLabel.ForeColor = System.Drawing.Color.White;
            this.timeLabel.Name = "timeLabel";
            // 
            // mainPhoto
            // 
            this.mainPhoto.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.mainPhoto, "mainPhoto");
            this.mainPhoto.Name = "mainPhoto";
            this.mainPhoto.TabStop = false;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.Black;
            this.ControlBox = false;
            this.Controls.Add(this.msgLabel);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.mainPhoto);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.mainPhoto)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Label msgLabel;
        private Label timeLabel;
        private PictureBox mainPhoto;
    }
}

