﻿#if X64
using MDPlayerx64.Properties;
#else
using MDPlayer.Properties;
#endif
namespace MDPlayer.form
{
    partial class frmOKIM6258
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOKIM6258));
            pbScreen = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pbScreen).BeginInit();
            SuspendLayout();
            // 
            // pbScreen
            // 
            pbScreen.Location = new Point(0, 0);
            pbScreen.Margin = new Padding(4, 4, 4, 4);
            pbScreen.Name = "pbScreen";
            pbScreen.Size = new Size(373, 20);
            pbScreen.TabIndex = 0;
            pbScreen.TabStop = false;
            pbScreen.MouseClick += pbScreen_MouseClick;
            // 
            // frmOKIM6258
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(373, 20);
            Controls.Add(pbScreen);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5, 5, 5, 5);
            MaximizeBox = false;
            Name = "frmOKIM6258";
            Text = "OKIM6258";
            FormClosed += frmOKIM6258_FormClosed;
            Load += frmOKIM6258_Load;
            ((System.ComponentModel.ISupportInitialize)pbScreen).EndInit();
            ResumeLayout(false);
        }

        #endregion

        public PictureBox pbScreen;
    }
}