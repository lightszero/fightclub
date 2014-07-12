using System;
using System.Collections.Generic;

using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace ha_ani_tool.hadata
{
    public interface IEditAble
    {
        void Move(int x, int y);
        void Rotate(float value);
        void Scale(float x,float y);
        void UnDo();
        void ReDo();
        void Save();

    }


    public class Attachment:IEditAble
    {
        [XmlAttribute]
        public string name
        {
            get;
            set;
        }
        [XmlAttribute]
        public string textureName
        {
            get;
            set;
        }
        [XmlAttribute]
        public float rotation
        {
            get;
            set;
        }
        [XmlAttribute]
        public float translationX
        {
            get;
            set;
        }
        [XmlAttribute]
        public float translationY
        {
            get;
            set;
        }
        public override string ToString()
        {
            string outstr = name + ":" + textureName;
            if (System.IO.File.Exists(this.textureName) == false)
            {
                outstr = "<E>" +outstr;
            }
            return outstr;
        }

        public void Move(int x, int y)
        {
            this.translationY -= y;
            this.translationX += x;
            //Save();
                //Value v = new Value(this);
            //Save();
        }

        public void Rotate(float value)
        {
            //Save();
                this.rotation += value;
            //Save();
        }

        class Value
        {
            public Value(Attachment attm)
            {
                x = attm.translationX;
                y = attm.translationY;
                rotate = attm.rotation;
            }
            public float x;
            public float y;
            public float rotate;
            public override bool Equals(object obj)
            {
                Value v = obj as Value;
                if (v == null) return false;
                if (v.x == x && v.y == y && v.rotate == rotate) return true;

                return false;
            }
			public override int GetHashCode ()
			{
				return base.GetHashCode ();
			} 
        }  
        [XmlIgnore]
        List<Value> History = new List<Value>();
        [XmlIgnore]
        int stepid = 0;
        public void Save()
        {
            while (History.Count > (stepid+1)&&stepid>0)
            {
                History.RemoveAt(stepid + 1);
            }

            Value v = new Value(this);
            if (History.Count == 0)
            {
                History.Add(v);
                
            }
            else
            {
                if (v != History[History.Count - 1])
                {
                    History.Add(v);
                }
            }
            while (History.Count > 10)
            {
                History.RemoveAt(0);
            }
            stepid = History.Count - 1;
        }
        public void UnDo()
        {
            stepid--;
            if (stepid < 0) stepid = 0;
            if (History.Count <= stepid)
            {
                return;
            }
            this.translationX = History[stepid].x;
            this.translationY = History[stepid].y;
            this.rotation = History[stepid].rotate;
        }

        public void ReDo()
        {
            stepid++;
            if (stepid >= History.Count) stepid = History.Count-1;
            if (History.Count <= stepid)
            {
                return;
            }
            this.translationX = History[stepid].x;
            this.translationY = History[stepid].y;
            this.rotation = History[stepid].rotate;
        }


        public void Scale(float x, float y)
        {
            //throw new NotImplementedException();
        }
    }
    public class Part
    {
        [XmlAttribute]
        public string type
        {
            get;
            set;
        }
        [XmlAttribute]
        public string bone
        {
            get;
            set;
        }

        [XmlAttribute]
        public string defaultAttachment
        {
            get;
            set;
        }

        public class Replacement
        {
            [XmlAttribute]
            public string category
            {
                get;
                set;
            }

            
            [XmlAttribute]
            public string attachment
            {
                get;
                set;
            }
            public override string ToString()
            {
                return category + ":" + attachment;
            }
        }

        [XmlElement("Replacement")]
        public List<Replacement> Replacements = new List<Replacement>();
        public override string ToString()
        {
            return type + ":" + bone + ":" + defaultAttachment;
        }
    }
    public class Anim
    {
        [XmlAttribute]
        public string type
        {
            get;
            set;
        }


        [XmlAttribute]
        public string name
        {
            get;
            set;
        }

        [XmlElement("Tag")]
        public List<Tag> tags = new List<Tag>();

        public class Tag
        {
            public Tag()
            {
                Type = "";
            }
            [XmlAttribute]
            public float Time
            {
                get;
                set;
            }
            [XmlAttribute]
            public string Type
            {
                get;
                set;
            }
            [XmlAttribute]
            public string Filename
            {
                get;
                set;
            }
            [XmlAttribute]
            public string Bone
            {
                get;
                set;
            }
            [XmlAttribute]
            public string Part
            {
                get;
                set;
            }
            [XmlAttribute]
            public string Attachment
            {
                get;
                set;
            }
            public override string ToString()
            {
                return Type.ToString() + ":" + Time.ToString();
            }
            public class DataReplacement
            {
                [XmlAttribute]
                public string category
                {
                    get;
                    set;
                }
                [XmlAttribute]
                public string attachment
                {
                    get;
                    set;
                }
            }
            [XmlElement("Replacement")]
            public DataReplacement Replacement
            {
                get;
                set;
            }
        }

        public override string ToString()
        {
            return type + ":" + name;
        }
    }
    [XmlRoot("puppet")]
    public class puppetdata
    {
        public int SizeX
        {
            get;
            set;
        }
        public int SizeY
        {
            get;
            set;
        }
        public int CornerOffsetX
        {
            get;
            set;
        }
        public int CornerOffsetY
        {
            get;
            set;
        }

        public List<Attachment> Attachments = new List<Attachment>();

        [XmlElement("Part")]
        public List<Part> Parts = new List<Part>();

        [XmlElement("Anim")]
        public List<Anim> Anims = new List<Anim>();

        [XmlIgnore]
        public string _FileName;
        [XmlIgnore]
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName =  value.Replace(".puppet",".Puppet");  
            }
        }
        public static puppetdata LoadFormString(string xml)
        {
            XmlSerializer s = new XmlSerializer(typeof(puppetdata));
           
            
            using ( System.IO.StringReader reader =new System.IO.StringReader(xml))
            {

                var r = s.Deserialize(reader) as puppetdata;
                if (r != null)
                {
                    List<Anim> list = r.Anims;
                    for (int i = 0; i < list.Count; ++i)
                    {
                        Anim a = list[i];
                        a.name = a.name.Replace("\\", "/");
                    }


                    r.FileName = "whatever";
                    return r;
                }
            }
            return null;
        }
        public static puppetdata Load(string filename)
        {
            XmlSerializer s = new XmlSerializer(typeof(puppetdata));
            using (System.IO.Stream stream = System.IO.File.OpenRead(filename))
            {
                var r = s.Deserialize(stream) as puppetdata;
                if (r != null)
                {
                    List<Anim> list = r.Anims;
                    for (int i = 0; i < list.Count; ++i)
                    {
                        Anim a = list[i];
                        a.name = a.name.Replace("\\","/");
                    }


                    r.FileName = filename;
                    return r;
                }
            }
            return null;
        }
        public void Save()
        {
            List<Anim> list = this.Anims;
            for (int i = 0; i < list.Count; ++i)
            {
                Anim a = list[i];
                a.name = a.name.Replace("\\", "/");
            }

            XmlSerializer s = new XmlSerializer(typeof(puppetdata));
            using (System.IO.Stream stream = System.IO.File.Open(FileName,System.IO.FileMode.Create))
            {
                s.Serialize(stream, this);
            }
        }
    }
}
