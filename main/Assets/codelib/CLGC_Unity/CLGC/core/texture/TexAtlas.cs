using System;
using System.Collections.Generic;

using System.Text;
using System.Xml.Serialization;

    public class TextureAtlas
    {
        [XmlAttribute]
        public float height;
        [XmlAttribute]
        public float width;
        [XmlAttribute]
        public string imagePath;
        //

        //[XmlAttribute]
        public class sprite
        {
            [XmlAttribute]
            public int h;
            [XmlAttribute]
            public int w;
            [XmlAttribute]
            public int x;
            [XmlAttribute]
            public int y;
            [XmlAttribute]
            public string n;
        }
        [XmlElement("sprite")]
        public List<sprite> sprites = new List<sprite>();

        public static TextureAtlas CreateFromStringXML(string xml)
        {
            System.IO.StringReader reader = new System.IO.StringReader(xml);
            XmlSerializer xmls = new XmlSerializer(typeof(TextureAtlas));
            return xmls.Deserialize(reader) as TextureAtlas;
        }
        public static TextureAtlas CreateFromStringCSV(string csv)
        {
            TextureAtlas ta = new TextureAtlas();
            ta.imagePath = "";
            ta.width = 0;
            ta.height = 0;
            var lines=        csv.Split(new char[]{'\r', '\n'},StringSplitOptions.RemoveEmptyEntries);
            foreach (var l in lines)
            {
                var ws = l.Split(new char[] { ',' });
                sprite s =new sprite();
                s.n= ws[0];
                s.x = int.Parse(ws[1]);
                s.y = int.Parse(ws[2]);
                s.w = int.Parse(ws[3]);
                s.h = int.Parse(ws[4]);
                ta.sprites.Add(s);
            }
            return ta;
        }
    }

