﻿namespace MDPlayer.form
{
    public partial class ucSettingInstruments : UserControl
    {
        public ucSettingInstruments()
        {
            InitializeComponent();
        }

        private void cbSendWait_CheckedChanged(object sender, EventArgs e)
        {
            cbTwice.Enabled = cbSendWait.Checked;
        }

    }
}
