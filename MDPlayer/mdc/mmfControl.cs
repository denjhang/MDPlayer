﻿using System;
using System.IO.MemoryMappedFiles;
using System.Text;


namespace mdc
{
    public class mmfControl : KumaCom
    {
        private object lockobj = new object();
        private MemoryMappedFile _map = null;
        private byte[] mmfBuf;
        public string mmfName = "dummy";
        public int mmfSize = 1024;

        public mmfControl()
        {
        }

        public mmfControl(bool isClient, string mmfName, int mmfSize)
        {
            this.mmfName = mmfName;
            this.mmfSize = mmfSize;
            if (!isClient) Open(mmfName, mmfSize);
        }

        public override bool Open(string mmfName, int mmfSize)
        {
            try
            {
                //mmfBuf = new byte[mmfSize];

                //lock (lockobj)
                //{
                //    _map = MemoryMappedFile.CreateNew(mmfName, mmfSize);
                //    try
                //    {
                //        MemoryMappedFileSecurity permission = _map.GetAccessControl();
                //        permission.AddAccessRule(
                //          new AccessRule<MemoryMappedFileRights>("Everyone",
                //            MemoryMappedFileRights.FullControl, AccessControlType.Allow));
                //        _map.SetAccessControl(permission);
                //    }
                //    catch (Exception ex)
                //    {
                //        log.Write(ex.Message + ex.StackTrace);
                //    }
                //}
                return true;
            }
            catch (Exception ex)
            {
                log.Write(ex.Message + ex.StackTrace);
                return false;
            }
        }

        public override void Close()
        {
            //lock (lockobj)
            //{
            //    if (_map == null) return;
            //    try
            //    {
            //        _map.Dispose();
            //    }
            //    catch (Exception ex)
            //    {
            //        log.Write(ex.Message + ex.StackTrace);
            //    }
            //}
        }

        public override string GetMessage()
        {
            string msg = "";

            try
            {
                lock (lockobj)
                {
                    using (MemoryMappedViewAccessor view = _map.CreateViewAccessor())
                    {
                        view.ReadArray(0, mmfBuf, 0, mmfBuf.Length);
                        msg = Encoding.Unicode.GetString(mmfBuf);
                        msg = msg.Substring(0, msg.IndexOf('\0'));
                        Array.Clear(mmfBuf, 0, mmfBuf.Length);
                        view.WriteArray(0, mmfBuf, 0, mmfBuf.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Write(ex.Message + ex.StackTrace);
            }

            return msg;
        }
        public override byte[] GetBytes()
        {
            try
            {
                lock (lockobj)
                {
                    using (var map = MemoryMappedFile.OpenExisting(mmfName))
                    using (MemoryMappedViewAccessor view = map.CreateViewAccessor())
                    {
                        mmfBuf = new byte[mmfSize];
                        view.ReadArray(0, mmfBuf, 0, mmfBuf.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Write(ex.Message + ex.StackTrace);
            }

            return mmfBuf;
        }

        public override void SendMessage(string msg)
        {
            try
            {
                byte[] ary = Encoding.Unicode.GetBytes(msg);
                if (ary.Length > mmfSize) throw new ArgumentOutOfRangeException();

                using (var map = MemoryMappedFile.OpenExisting(mmfName))
                using (var view = map.CreateViewAccessor())
                    view.WriteArray(0, ary, 0, ary.Length);
            }
            catch (Exception ex)
            {
                log.Write(ex.Message + ex.StackTrace);
            }
        }

    }
}
