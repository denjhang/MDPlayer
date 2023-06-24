﻿/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2014 Leandro Nini <drfiemost@users.sourceforge.net>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

namespace Driver.libsidplayfp.utils
{
    public sealed class md5Internal : iMd5
    {




        //# include "iMd5.h"
        //# include "MD5/MD5.h"
        //# include "sidcxx11.h"


        private MD5.MD5 hd = new MD5.MD5();

        public override void append(byte[] data, int nbytes) { hd.append(data, nbytes); }

        public override void finish() { hd.finish(); }

        public override byte[] getDigest() { return hd.getDigest(); }

        public override void reset() { hd.reset(); }





    }
}
