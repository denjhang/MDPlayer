﻿#if X64
using MDPlayerx64;
#else
using MDPlayer.Properties;
#endif

namespace MDPlayer.form
{
    public partial class frmYM3526 : frmBase
    {
        public bool isClosed = false;
        public int x = -1;
        public int y = -1;
        private int frameSizeW = 0;
        private int frameSizeH = 0;
        private int chipID = 0;
        private int zoom = 1;

        private MDChipParams.YM3526 newParam = null;
        private MDChipParams.YM3526 oldParam = null;
        private FrameBuffer frameBuffer = new FrameBuffer();

        public frmYM3526(frmMain frm, int chipID, int zoom, MDChipParams.YM3526 newParam, MDChipParams.YM3526 oldParam) : base(frm)
        {
            this.chipID = chipID;
            this.zoom = zoom;
            InitializeComponent();

            this.newParam = newParam;
            this.oldParam = oldParam;
            frameBuffer.Add(pbScreen, ResMng.ImgDic["planeYM3526"], null, zoom);
            bool YM3526Type = (chipID == 0)
                ? parent.setting.YM3526Type[0].UseReal[0]
                : parent.setting.YM3526Type[1].UseReal[0];
            int YM3526SoundLocation = (chipID == 0)
                ? parent.setting.YM3526Type[0].realChipInfo[0].SoundLocation
                : parent.setting.YM3526Type[1].realChipInfo[0].SoundLocation;
            int tp = !YM3526Type ? 0 : (YM3526SoundLocation < 0 ? 2 : 1);

            DrawBuff.screenInitYM3526(frameBuffer, tp);
            update();
        }

        public void update()
        {
            frameBuffer.Refresh(null);
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        private void frmYM3526_Load(object sender, EventArgs e)
        {
            this.Location = new Point(x, y);

            frameSizeW = this.Width - this.ClientSize.Width;
            frameSizeH = this.Height - this.ClientSize.Height;

            changeZoom();
        }

        private void frmYM3526_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                parent.setting.location.PosYm3526[chipID] = Location;
            }
            else
            {
                parent.setting.location.PosYm3526[chipID] = RestoreBounds.Location;
            }
            isClosed = true;
        }

        public void changeZoom()
        {
            this.MaximumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeYM3526"].Width * zoom, frameSizeH + ResMng.ImgDic["planeYM3526"].Height * zoom);
            this.MinimumSize = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeYM3526"].Width * zoom, frameSizeH + ResMng.ImgDic["planeYM3526"].Height * zoom);
            this.Size = new System.Drawing.Size(frameSizeW + ResMng.ImgDic["planeYM3526"].Width * zoom, frameSizeH + ResMng.ImgDic["planeYM3526"].Height * zoom);
            frmYM3526_Resize(null, null);
        }

        private void frmYM3526_Resize(object sender, EventArgs e)
        {

        }

        public void screenInit()
        {
            for (int c = 0; c < newParam.channels.Length; c++)
            {
                newParam.channels[c].note = -1;
            }
        }

        private int[] slot1Tbl = new int[] { 0, 1, 2, 6, 7, 8, 12, 13, 14 };
        private int[] slot2Tbl = new int[] { 3, 4, 5, 9, 10, 11, 15, 16, 17 };
        private static byte[] rhythmAdr = new byte[] { 0x53, 0x54, 0x52, 0x55, 0x51 };

        public void screenChangeParams()
        {
            int[] ym3526Register = Audio.GetYM3526Register(chipID);
            MDChipParams.Channel nyc;
            int slot = 0;
            ChipKeyInfo ki = Audio.GetYM3526KeyInfo(chipID);

            MDSound.MDSound.Chip chipInfo = Audio.GetMDSChipInfo(MDSound.MDSound.enmInstrumentType.YM3526);
            uint masterClock = chipInfo == null ? 3579545 : chipInfo.Clock; //3579545 -> Default master clock

            //FM
            for (int c = 0; c < 9; c++)
            {
                nyc = newParam.channels[c];
                for (int i = 0; i < 2; i++)
                {

                    if (i == 0)
                    {
                        slot = slot1Tbl[c];
                    }
                    else
                    {
                        slot = slot2Tbl[c];
                    }
                    slot = (slot % 6) + 8 * (slot / 6);

                    //AR
                    nyc.inst[0 + i * 17] = ym3526Register[0x60 + slot] >> 4;
                    //DR
                    nyc.inst[1 + i * 17] = ym3526Register[0x60 + slot] & 0xf;
                    //SL
                    nyc.inst[2 + i * 17] = ym3526Register[0x80 + slot] >> 4;
                    //RR
                    nyc.inst[3 + i * 17] = ym3526Register[0x80 + slot] & 0xf;
                    //KL
                    nyc.inst[4 + i * 17] = ym3526Register[0x40 + slot] >> 6;
                    //TL
                    nyc.inst[5 + i * 17] = ym3526Register[0x40 + slot] & 0x3f;
                    //MT
                    nyc.inst[6 + i * 17] = ym3526Register[0x20 + slot] & 0xf;
                    //AM
                    nyc.inst[7 + i * 17] = ym3526Register[0x20 + slot] >> 7;
                    //VB
                    nyc.inst[8 + i * 17] = (ym3526Register[0x20 + slot] >> 6) & 1;
                    //EG
                    nyc.inst[9 + i * 17] = (ym3526Register[0x20 + slot] >> 5) & 1;
                    //KR
                    nyc.inst[10 + i * 17] = (ym3526Register[0x20 + slot] >> 4) & 1;
                }

                //BL
                nyc.inst[11] = (ym3526Register[0xb0 + c] >> 2) & 7;
                //FNUM
                nyc.inst[12] = ym3526Register[0xa0 + c]
                    + ((ym3526Register[0xb0 + c] & 3) << 8);

                //FB
                nyc.inst[15] = (ym3526Register[0xc0 + c] >> 1) & 7;
                //CN
                nyc.inst[14] = (ym3526Register[0xc0 + c] & 1);

                // FNUM / (2^19) * (mClock/72) * (2 ^ (block - 1)) 
                double fmus = (double)nyc.inst[12] / (1 << 19) * (masterClock / 72.0) * (1 << nyc.inst[11]);
                nyc.note = Common.searchSegaPCMNote(fmus / 523.3);//523.3 -> c4

                if (ki.On[c])
                {
                    int tl1 = nyc.inst[5 + 0 * 17];
                    int tl2 = nyc.inst[5 + 1 * 17];
                    int tl = tl2;
                    if (nyc.inst[14] != 0)
                    {
                        tl = Math.Min(tl1, tl2);
                    }
                    nyc.volume = (19 * (64 - tl) / 64);
                }
                else
                {
                    if ((ym3526Register[0xb0 + c] & 0x20) == 0) nyc.note = -1;
                    nyc.volume--; if (nyc.volume < 0) nyc.volume = 0;
                }


            }
            newParam.channels[9].dda = ((ym3526Register[0xbd] >> 7) & 0x01) != 0;//DA
            newParam.channels[10].dda = ((ym3526Register[0xbd] >> 6) & 0x01) != 0;//DV

            #region リズム情報の取得

            //slot14 TL 0x51 HH
            //slot15 TL 0x52 TOM
            //slot16 TL 0x53 BD
            //slot17 TL 0x54 SD
            //slot18 TL 0x55 CYM

            for (int i = 0; i < 5; i++)
            {
                if (ki.On[i + 9])
                {
                    newParam.channels[i + 9].volume = 19 - ((ym3526Register[rhythmAdr[i]] & 0x3f) >> 2);
                }
                else
                {
                    newParam.channels[i + 9].volume--;
                    if (newParam.channels[i + 9].volume < 0) newParam.channels[i + 9].volume = 0;
                }
            }

            #endregion
        }

        public void screenDrawParams()
        {
            bool YM3526Type = (chipID == 0)
                ? parent.setting.YM3526Type[0].UseReal[0]
                : parent.setting.YM3526Type[1].UseReal[0];
            int YM3526SoundLocation = (chipID == 0)
                ? parent.setting.YM3526Type[0].realChipInfo[0].SoundLocation
                : parent.setting.YM3526Type[1].realChipInfo[0].SoundLocation;
            int tp = !YM3526Type ? 0 : (YM3526SoundLocation < 0 ? 2 : 1);
            MDChipParams.Channel oyc;
            MDChipParams.Channel nyc;

            //FM
            for (int c = 0; c < 9; c++)
            {

                oyc = oldParam.channels[c];
                nyc = newParam.channels[c];

                for (int i = 0; i < 2; i++)
                {
                    DrawBuff.font4Int2(frameBuffer, 16 + 4 + i * 132, c * 8 + 96, 0, 0, ref oyc.inst[0 + i * 17], nyc.inst[0 + i * 17]);//AR
                    DrawBuff.font4Int2(frameBuffer, 16 + 12 + i * 132, c * 8 + 96, 0, 0, ref oyc.inst[1 + i * 17], nyc.inst[1 + i * 17]);//DR
                    DrawBuff.font4Int2(frameBuffer, 16 + 20 + i * 132, c * 8 + 96, 0, 0, ref oyc.inst[2 + i * 17], nyc.inst[2 + i * 17]);//SL
                    DrawBuff.font4Int2(frameBuffer, 16 + 28 + i * 132, c * 8 + 96, 0, 0, ref oyc.inst[3 + i * 17], nyc.inst[3 + i * 17]);//RR

                    DrawBuff.font4Int2(frameBuffer, 16 + 40 + i * 132, c * 8 + 96, 0, 0, ref oyc.inst[4 + i * 17], nyc.inst[4 + i * 17]);//KL
                    DrawBuff.font4Int2(frameBuffer, 16 + 48 + i * 132, c * 8 + 96, 0, 0, ref oyc.inst[5 + i * 17], nyc.inst[5 + i * 17]);//TL

                    DrawBuff.font4Int2(frameBuffer, 16 + 60 + i * 132, c * 8 + 96, 0, 0, ref oyc.inst[6 + i * 17], nyc.inst[6 + i * 17]);//MT

                    DrawBuff.font4Int2(frameBuffer, 16 + 72 + i * 132, c * 8 + 96, 0, 0, ref oyc.inst[7 + i * 17], nyc.inst[7 + i * 17]);//AM
                    DrawBuff.font4Int2(frameBuffer, 16 + 80 + i * 132, c * 8 + 96, 0, 0, ref oyc.inst[8 + i * 17], nyc.inst[8 + i * 17]);//VB
                    DrawBuff.font4Int2(frameBuffer, 16 + 88 + i * 132, c * 8 + 96, 0, 0, ref oyc.inst[9 + i * 17], nyc.inst[9 + i * 17]);//EG
                    DrawBuff.font4Int2(frameBuffer, 16 + 96 + i * 132, c * 8 + 96, 0, 0, ref oyc.inst[10 + i * 17], nyc.inst[10 + i * 17]);//KR
                }

                DrawBuff.font4Int2(frameBuffer, 16 + 4 * 64, c * 8 + 96, 0, 0, ref oyc.inst[11], nyc.inst[11]);//BL
                DrawBuff.font4Hex12Bit(frameBuffer, 16 + 4 * 68, c * 8 + 96, 0, ref oyc.inst[12], nyc.inst[12]);//F-Num
                DrawBuff.font4Int2(frameBuffer, 16 + 4 * 72, c * 8 + 96, 0, 0, ref oyc.inst[14], nyc.inst[14]);//CN
                DrawBuff.font4Int2(frameBuffer, 16 + 4 * 75, c * 8 + 96, 0, 0, ref oyc.inst[15], nyc.inst[15]);//FB
                DrawBuff.KeyBoard(frameBuffer, c, ref oyc.note, nyc.note, tp);
                DrawBuff.VolumeXY(frameBuffer, 64, c * 2 + 2, 0, ref oyc.volume, nyc.volume, tp);
                DrawBuff.ChYM3526(frameBuffer, c, ref oyc.mask, nyc.mask, tp);

            }

            DrawBuff.drawNESSw(frameBuffer, 76 * 4, 10 * 8, ref oldParam.channels[9].dda, newParam.channels[9].dda);//DA
            DrawBuff.drawNESSw(frameBuffer, 80 * 4, 10 * 8, ref oldParam.channels[10].dda, newParam.channels[10].dda);//DV

            for (int c = 9; c < 14; c++)
            {
                DrawBuff.ChYM3526(frameBuffer, c, ref oldParam.channels[c].mask, newParam.channels[c].mask, tp);
                DrawBuff.VolumeXY(frameBuffer, 3 + (c - 9) * 15, 10 * 2, 0, ref oldParam.channels[c].volume, newParam.channels[c].volume, tp);
            }
        }

        private void pbScreen_MouseClick(object sender, MouseEventArgs e)
        {
            int px = e.Location.X / zoom;
            int py = e.Location.Y / zoom;
            int ch;
            //上部のラベル行の場合は何もしない
            if (py < 1 * 8)
            {
                //但しchをクリックした場合はマスク反転
                if (px < 8)
                {
                    for (ch = 0; ch < 9 + 5; ch++)
                    {
                        if (newParam.channels[ch].mask == true)
                            parent.ResetChannelMask(EnmChip.YM3526, chipID, ch);
                        else
                            parent.SetChannelMask(EnmChip.YM3526, chipID, ch);
                    }
                }
                return;
            }

            //鍵盤 FM & RHM
            ch = (py / 8) - 1;
            if (ch < 0) return;

            if (ch == 9)
            {
                int x = (px / 4 - 1);
                if (x < 0) return;
                x /= 15;
                if (x > 4) return;
                ch += x;
            }

            if (e.Button == MouseButtons.Left)
            {
                //マスク
                parent.SetChannelMask(EnmChip.YM3526, chipID, ch);
                return;
            }

            //マスク解除
            for (ch = 0; ch < 9 + 5; ch++) parent.ResetChannelMask(EnmChip.YM3526, chipID, ch);
            return;
        }

    }
}
