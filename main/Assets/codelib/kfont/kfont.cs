using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class kfont
{
    public kfont_header header;
    List<font_info> fontinfo = null;
    List<byte[]> fontdata = null;
    Dictionary<Int32, KeyValuePair<Int32, UInt32>> chardata = new Dictionary<int, KeyValuePair<int, uint>>();
    static int MakeFourCC(int ch0, int ch1, int ch2, int ch3)
    {
        return ((int)(byte)(ch0) | ((int)(byte)(ch1) << 8) | ((int)(byte)(ch2) << 16) | ((int)(byte)(ch3) << 24));
    }
    public void Read(System.IO.Stream instream)
    {
        {//Header
            byte[] b = new byte[28];
            instream.Read(b, 0, 28);
            header.fourcc = BitConverter.ToUInt32(b, 0);
            int fourcc = MakeFourCC('K', 'F', 'N', 'T');
            header.version = BitConverter.ToUInt32(b, 4);
            header.start_ptr = BitConverter.ToUInt32(b, 8);
            header.validate_chars = BitConverter.ToUInt32(b, 12);
            header.non_empty_chars = BitConverter.ToUInt32(b, 16);
            header.char_size = BitConverter.ToUInt32(b, 20);
            header._base = BitConverter.ToInt16(b, 24);
            header.scale = BitConverter.ToInt16(b, 25);
        }
        {
            //instream.Seek(header.start_ptr, System.IO.SeekOrigin.Begin);
            Dictionary<Int32, Int32> nedata = new Dictionary<int, int>();
            for (int i = 0; i < header.non_empty_chars; i++)
            {
                byte[] buf = new byte[8];
                instream.Read(buf, 0, 8);
                int l = BitConverter.ToInt32(buf, 0);
                int r = BitConverter.ToInt32(buf, 4);
                chardata[l] = new KeyValuePair<int, uint>(r, 0);
                //nedata[l] = r;
            }
            Dictionary<Int32, UInt32> vdata = new Dictionary<int, UInt32>();
            for (int i = 0; i < header.validate_chars; i++)
            {
                byte[] buf = new byte[8];
                instream.Read(buf, 0, 8);
                int l = BitConverter.ToInt32(buf, 0);
                uint r = BitConverter.ToUInt32(buf, 4);
                if (chardata.ContainsKey(l))
                {
                    chardata[l] = new KeyValuePair<int, uint>(chardata[l].Key, r);
                }
                else
                {
                    chardata[l] = new KeyValuePair<int, uint>(-1, r);
                }
                //vdata[l] = r;
            }
        }
        {//FontInfo
            fontinfo = new List<font_info>();
            for (int i = 0; i < header.non_empty_chars; i++)
            {
                byte[] buf = new byte[8];
                instream.Read(buf, 0, 8);
                font_info info;
                info.top = BitConverter.ToInt16(buf, 0);
                info.left = BitConverter.ToInt16(buf, 2);
                info.width = BitConverter.ToUInt16(buf, 4);
                info.height = BitConverter.ToUInt16(buf, 6);
                fontinfo.Add(info);
            }
        }
        {//FontData
            float timer = 0;
            fontdata = new List<byte[]>();
            for (int i = 0; i < header.non_empty_chars; i++)
            {
                DateTime t = DateTime.Now;
                byte[] buf = new byte[8];
                instream.Read(buf, 0, 8);
                ulong len = BitConverter.ToUInt64(buf, 0);
                byte[] data = new byte[len];
                instream.Read(data, 0, (int)len);


                System.IO.MemoryStream sin = new System.IO.MemoryStream(data);

                System.IO.MemoryStream sout = new System.IO.MemoryStream();
                SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
                byte[] properties = new byte[5];
                if (sin.Read(properties, 0, 5) != 5)
                    throw (new Exception("input .lzma is too short"));
                decoder.SetDecoderProperties(properties);

                //sin.Read(buf, 0,2);
                int len2 = (int)(header.char_size * header.char_size);

                decoder.Code(sin, sout, (long)len - 5, (int)len2, null);
                sout.Position = 0;
                byte[] decodedata = new byte[len2];
                sout.Read(decodedata, 0, len2);

                fontdata.Add(decodedata);
                DateTime t2 = DateTime.Now;
                float delta = (float)(t2 - t).TotalSeconds;
                timer += delta;
            }
            Debug.Log("Load time total:" + timer + " per:" + (timer / header.non_empty_chars));
        }
    }
    public byte[] GetCharData(char c)
    {
            if(chardata.ContainsKey(c))
            {
                int use = chardata[c].Key;
                if (use == -1) use = (int)chardata[c].Value;

                Debug.Log(chardata[c].Key + "|" + chardata[c].Value+"use="+use);

                return fontdata[use];
            }
            else
            {
                return null;
            }
    }
    public struct kfont_header
    {

        public UInt32 fourcc;
        public UInt32 version;
        public UInt32 start_ptr;
        public UInt32 validate_chars;
        public UInt32 non_empty_chars;
        public UInt32 char_size;

        public Int16 _base;
        public Int16 scale;
    };
    public struct font_info
    {
        public Int16 top;
        public Int16 left;
        public UInt16 width;
        public UInt16 height;
    };
}