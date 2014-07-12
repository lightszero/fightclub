using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using clgf.Anim;
using clgf;
using PointF = Microsoft.Xna.Framework.Vector2;
using System.Drawing;
namespace aniconv.xfl
{
    public class ProjParser
    {
        private string resourcepath;

        Dictionary<string, string> seed2dummy = new Dictionary<string, string>();
        public ProjParser(ILogger _logger, string respath, Sprite sprite)
        {
            logger = _logger;
            resourcepath = respath;
            cursprite = sprite;

        }
        public void TestDummySeed(string path)
        {

            string dummyseedpath = System.IO.Path.Combine(path, "seeddummy.txt");
            if (System.IO.File.Exists(dummyseedpath))
            {
                var lines = System.IO.File.ReadAllLines(dummyseedpath);
                foreach (var l in lines)
                {
                    var ws = l.Split(new string[] { "seed:", "->dummy:" }, StringSplitOptions.RemoveEmptyEntries);
                    seed2dummy[ws[0]] = ws[1];
                    logger.Log(LogLevel.Normal, "Seed:" + ws[0] + "->dummy:" + ws[1]);
                }
            }
        }
        private ILogger logger;
        //public data_project dataproj = null;
        //public data_asp dataasp = null;
        public data_domdoc datadom = null;
        public string mainpath;
        public string xflver = null;
        public bool Read(string path, ILogger _logger)
        {
            logger = _logger;

            mainpath = path;
            string pathname = System.IO.Path.GetFileName(mainpath);
            //string pathname=       System.IO.Path.GetDirectoryName(mainpath);

            string xflfile = System.IO.Path.Combine(path, pathname + ".xfl");

            xflver = System.IO.File.ReadAllText(xflfile);
            if (xflver == "PROXY-CS5" || xflver == "PROXY-CS5.5")
            {
                logger.Log(LogLevel.Normal, "xfl项目");
            }
            else
            {
                logger.Log(LogLevel.Error, "不是xfl项目");
                return false;
            }
            try
            {
                string dom = System.IO.Path.Combine(path, "DOMDocument.xml");
                using (System.IO.Stream s = System.IO.File.OpenRead(dom))
                {

                    XmlSerializer xmls = new XmlSerializer(typeof(data_domdoc));
                    datadom = xmls.Deserialize(s) as data_domdoc;
                }
            }
            catch (Exception err)
            {
                logger.Log(LogLevel.Waring, err.ToString());
            }
            //string texpath =
            string[] files = System.IO.Directory.GetFiles(System.IO.Path.Combine(path, "LIBRARY"), "*.xml");
            foreach (var f in files)
            {
                using (System.IO.Stream fs = System.IO.File.OpenRead(f))
                {
                    try
                    {
                        XmlSerializer xmls = new XmlSerializer(typeof(DOMSymbolItem));
                        var item = xmls.Deserialize(fs) as DOMSymbolItem;
                        GenSeed(item, path);
                    }
                    catch (Exception err)
                    {
                        logger.Log(LogLevel.Waring,"处理种子"+f+"失败"+ err.ToString());
                    }
                }
            }
            logger.Log(LogLevel.Normal, "加载成功");
            return true;
        }
        //public Dictionary<string, DOMSymbolItem> items = new Dictionary<string, DOMSymbolItem>();
        //public Dictionary<string, Seed> seeds = new Dictionary<string, Seed>();

        private Sprite cursprite;
        string GetBestFile(string basepath, string filename)
        {

            string[] file = System.IO.Directory.GetFiles(resourcepath, filename, System.IO.SearchOption.AllDirectories);
            if (file.Length == 1) return file[0];
            if (file.Length == 0) return null;

            string str = null;
            string[] sseg = basepath.Split(new char[] { System.IO.Path.DirectorySeparatorChar });
            int samevalue = 0;
            foreach (var f in file)
            {
                string[] seg = f.Split(new char[] { System.IO.Path.DirectorySeparatorChar });
                int count = Math.Min(seg.Length, sseg.Length);
                int _same = 0;
                for (int i = 0; i < count; i++)
                {


                    if (seg[i] != sseg[i])
                    {

                        break;
                    }
                    _same = i + 1;
                }
                if (_same > samevalue)
                {
                    samevalue = _same;
                    str = f;
                }
            }
            return str;
        }
        Dictionary<string, string> seedchange = new Dictionary<string, string>();
        void GenSeed(DOMSymbolItem item, string _path)
        {
            //if (cursprite.seeds.ContainsKey(item.name))
            //{
            //    return;
            //}
            //else
            {
                Seed seed = new Seed();
                aniconv.xfl.DOMSymbolItem.DOMTimeline.DOMLayer.DOMFrame.DOMBitmapInstance binst = item.m_timeline.m_DOMTimeline.layers[0].frames[0].elements[0];
                string bestfile = GetBestFile(_path, binst.libraryItemName);
                if (string.IsNullOrEmpty(bestfile))
                {
                    throw new Exception(binst.libraryItemName + ":少了张图");
                }
                seed.texname = bestfile.Substring(resourcepath.Length + 1);
                string seedname = System.IO.Path.GetFileNameWithoutExtension(seed.texname);

                if (seedname.IndexOf("_dummy") == 0)
                {
                    //dummyseed 不导出
                    return;
                }
                var image = Image.FromFile(bestfile);
                seed.size.X = image.Width;
                seed.size.Y = image.Height;
                image.Dispose();
                if (binst.matrix.Count > 0)
                {
                    seed.orient.X = -binst.matrix[0].tx;
                    seed.orient.Y = -binst.matrix[0].ty;
                }

                if (cursprite.seeds.ContainsKey(seedname))
                {
                    //
                    Console.WriteLine("Seed以有，对比");
                    var oldseed = cursprite.seeds[seedname];



                    if (System.IO.Path.GetFileName(oldseed.texname) != System.IO.Path.GetFileName(seed.texname))
                    {
                        throw new Exception(seed.texname + ":seed源图冲突");
                    }
                    else if (oldseed.size.Equals(seed.size) == false)
                    {
                        throw new Exception(System.IO.Path.GetFileName(seed.texname) + ":seed尺寸冲突");
                    }
                    else if (oldseed.orient.Equals(seed.orient) == false)
                    {
                      
                        //seed 锚点冲突
                        //Console.WriteLine("Seed冲突,需要新建");
                        string subname = System.IO.Path.GetFileName(_path);
                        seedname += "_" + subname;
                        logger.Log(LogLevel.Waring, seedname +":Seed 冲突,已解决");
                        cursprite.seeds[seedname] = seed;
                        seedchange[item.name+"_"+subname] = seedname;
                    }
                    else
                    {
                        return;
                    }


                }
                cursprite.seeds[seedname] = seed;
                seedchange[item.name] = seedname;
            }
        }
        void GenSeed_R(string filename, string _path)
        {

            string seedoname = System.IO.Path.GetFileNameWithoutExtension(filename);
            string seedname = seedoname + "_b";

            if (seedname.IndexOf("_dummy") == 0)
            {
                //dummyseed 不导出
                return;
            }
            if (cursprite.seeds.ContainsKey(seedname))
            {
                return;
            }
            else
            {
                Seed seed = new Seed();

                string bestfile = GetBestFile(_path, filename);
                if (string.IsNullOrEmpty(bestfile))
                {
                    throw new Exception(filename + ":少了张图");
                }
                seed.texname = bestfile.Substring(resourcepath.Length + 1);

                var image = Image.FromFile(bestfile);
                seed.size.X = image.Width;
                seed.size.Y = image.Height;
                image.Dispose();

                cursprite.seeds[seedname] = seed;

            }
        }
        public Anim FillAni(string aniname)
        {
            Anim anim = new Anim(cursprite);
            if (this.datadom.listTimeLine.Count == 0)
            {
                logger.Log(LogLevel.Error, "this.datadom.listTimeLine.Count == 0");
                return null;
            }
            anim.size.X = datadom.width;
            anim.size.Y = datadom.height;
            int maxframe = 0;

            //Dictionary<string, string> part2bone = new Dictionary<string, string>();
            //foreach (var p in data.Parts)
            //{
            //    part2bone[p.type] = p.bone;
            //}

            foreach (var l in this.datadom.listTimeLine[0].layers)//不使用层名称
            {
                int frameid = 0;
                // List<aniconv.xfl.data_domdoc.DOMTimeline.DOMLayer.DOMFrame> framesneedcopy = new List<data_domdoc.DOMTimeline.DOMLayer.DOMFrame>();
                foreach (var f in l.frames)
                {
                    if (f.duration > 1)
                    {
                        frameid = f.index + f.duration - 1;
                        //framesneedcopy.Add(f);
                    }
                    frameid = Math.Max(f.index, frameid);
                }
                maxframe = Math.Max(frameid, maxframe);
            }

            anim.frames.Clear();
            while (anim.frames.Count <= maxframe)
            {
                anim.frames.Add(new Frame());
            }

            // anim.ResetFrameCount(maxframe);



            foreach (var l in this.datadom.listTimeLine[0].layers)
            {
                foreach (var f in l.frames)
                {
                    if (f.element.listbelements.Count > 0)
                    {
                        Element elem = new Element();
                        anim.frames[f.index].elems.Add(elem);

                        Matrix mat;
                        if (f.__elements[0].matrix.Count > 0)
                            mat = f.__elements[0].matrix[0];
                        else
                            mat = new Matrix();
                        elem.pos.X = mat.tx;// +f.elements[0].transformationPoint.;// -data.SizeX / 2;
                        elem.pos.Y = mat.ty;// +data.SizeY;

                        var s = mat.GetScale();
                        elem.scale.X = s.X;
                        elem.scale.Y = s.Y;
                        if (s.Y < 0)
                        {

                        }
                        elem.rotate = -mat.GetRotate();// / (float)Math.PI * -180.0f;

                        //f.elements[0].libraryItemName 类型
                        string seedoname = System.IO.Path.GetFileNameWithoutExtension(f.__elements[0].libraryItemName);
                        string seedname = seedoname + "_b";
                        GenSeed_R(f.__elements[0].libraryItemName, mainpath);
                        if (cursprite.seeds.ContainsKey(seedname) == false)
                        {
                            if (cursprite.seeds.ContainsKey(seedoname))
                            {
                                Seed seed = new Seed();

                                seed.size = cursprite.seeds[seedoname].size;
                                seed.orient = new Microsoft.Xna.Framework.Vector2();
                                seed.texname = cursprite.seeds[seedoname].texname;
                                cursprite.seeds[seedname] = seed;
                            }
                        }
                        elem.seed = seedname;
                        elem.seednow = elem.seed;
                        elem.sound = f.soundName;
                        elem.tag = f.name;
                        //if (f._elements[0].color.Count > 0)
                        //{
                        //    elem.color.A = (byte)(f.__elements[0].color[0].alphaMultiplier * 255);
                        //    elem.color.R = (byte)(f.__elements[0].color[0].redMultiplier * 255);
                        //    elem.color.G = (byte)(f.__elements[0].color[0].greenMultiplier * 255);
                        //    elem.color.B = (byte)(f.__elements[0].color[0].blueMultiplier * 255);
                        //    int i = f._elements[0].color[0].redOffset;
                        //    if (i > 255) i = 255;
                        //    elem.coloradd.R = (byte)i;
                        //    i = f._elements[0].color[0].greenOffset;
                        //    if (i > 255) i = 255;
                        //    elem.coloradd.G = (byte)i;
                        //    i = f._elements[0].color[0].blueOffset;
                        //    if (i > 255) i = 255;
                        //    elem.coloradd.B = (byte)i;

                        //}
                        if (f.duration > 0)
                        {
                            for (int i = 1; i < f.duration; i++)
                            {
                                anim.frames[f.index + i].elems.Add(elem.Copy());
                            }
                        }
                    }
                    else if (f._elements.Count > 0)
                    {
                        Element elem = new Element();
                        anim.frames[f.index].elems.Add(elem);

                        Matrix mat;
                        if (f._elements[0].matrix.Count > 0)
                            mat = f._elements[0].matrix[0];
                        else
                            mat = new Matrix();
                        elem.pos.X = mat.tx;// +f.elements[0].transformationPoint.;// -data.SizeX / 2;
                        elem.pos.Y = mat.ty;// +data.SizeY;

                        var s = mat.GetScale();
                        elem.scale.X = s.X;
                        elem.scale.Y = s.Y;
                        if (s.Y < 0)
                        {

                        }
                        elem.rotate = -mat.GetRotate();// / (float)Math.PI * -180.0f;

                        //f.elements[0].libraryItemName 类型
                        string itemname = f._elements[0].libraryItemName;
                        string itemfullname = itemname + "_" + aniname;
                        if (seedchange.ContainsKey(itemfullname))
                            elem.seed = seedchange[itemfullname];
                        else
                            elem.seed = seedchange[itemname];
                        if (seed2dummy.ContainsKey(elem.seed))
                            elem.seedasdummy = seed2dummy[elem.seed];
                        //elem.seed = seedchange[f._elements[0].libraryItemName];// cursprite.seeds[f.elements[0].libraryItemName];
                        elem.seednow = elem.seed;
                        elem.sound = f.soundName;
                        elem.tag = f.name;
                        if (f._elements[0].color.Count > 0)
                        {
                            elem.color.A = (byte)(f._elements[0].color[0].alphaMultiplier * 255);
                            elem.color.R = (byte)(f._elements[0].color[0].redMultiplier * 255);
                            elem.color.G = (byte)(f._elements[0].color[0].greenMultiplier * 255);
                            elem.color.B = (byte)(f._elements[0].color[0].blueMultiplier * 255);
                            int i = f._elements[0].color[0].redOffset;
                            if (i > 255) i = 255;
                            elem.coloradd.R = (byte)i;
                            i = f._elements[0].color[0].greenOffset;
                            if (i > 255) i = 255;
                            elem.coloradd.G = (byte)i;
                            i = f._elements[0].color[0].blueOffset;
                            if (i > 255) i = 255;
                            elem.coloradd.B = (byte)i;

                        }
                        if (f.duration > 0)
                        {
                            for (int i = 1; i < f.duration; i++)
                            {
                                anim.frames[f.index + i].elems.Add(elem.Copy());
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(f.name) == false)
                    {
                        anim.frames[f.index].AddTag(f.name);
                        for (int i = 1; i < f.duration; i++)
                        {
                            anim.frames[f.index + i].AddTag(f.name);
                        }
                    }

                }

            }
            return anim;
        }
        //string strproj = path + "\\.project";
        //string strasp = path + "\\.actionScriptProperties";
    }
    public class Matrix
    {
        [XmlAttribute]
        public float tx = 0;
        [XmlAttribute]
        public float ty = 0;
        [XmlAttribute]
        public float a = 1;
        [XmlAttribute]
        public float b = 0;
        [XmlAttribute]
        public float c = 0;
        [XmlAttribute]
        public float d = 1;

        public void SetRotate(double rotate)
        {
            a = (float)Math.Cos(rotate);
            b = (float)-Math.Sin(rotate);
            c = -b;
            d = a;

        }
        public void SetTranspos(float x, float y)
        {
            tx = x * a + y * c;
            ty = x * b + y * d;
        }
        public PointF GetScale()
        {
            //PointF tf = new PointF(1, 0);
            //PointF tu = new PointF(0, 1);
            //PointF tft = new PointF(1 * a + 0 * c, +1 * b + 0 * d);
            //PointF tfu = new PointF(0 * a + 1 * c, 0 * b + 1 * d);

            PointF t = new PointF(1 * a, 1 * b);
            double lenX = Math.Sqrt(t.X * t.X + t.Y * t.Y);
            PointF tu = new PointF(1 * c, 1 * d);
            double lenY = Math.Sqrt(tu.X * tu.X + tu.Y * tu.Y);
            float d1 = 1;
            float d2 = 1;
            float d3 = 1;
            float d4 = 1;
            if (t.Y < 0) d1 = -1;
            if (tu.Y < 0) d2 = -1;
            if (t.X < 0) d3 = -1;
            if (tu.Y < 0) d4 = -1;
            if (d1 * d2 < 0 || d3 * d4 < 0)
            {
                //lenX *= -1;
            }
            return new PointF((float)lenX, (float)lenY);
        }
        public float GetRotate()
        {
            PointF f = GetScale();
            float na = a / (f.X);
            float nb = b / f.Y;
            PointF fdir = new PointF(1 * na, -1 * nb);
            double ax = Math.Acos(fdir.X);
            double ay = Math.Asin(fdir.Y);
            if (fdir.Y < 0) ax = Math.PI * 2 - ax;
            return (float)ax;
        }
    }
    public class Point
    {
        [XmlAttribute]
        public float x = 0;

        [XmlAttribute]
        public float y = 0;
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    [System.Xml.Serialization.XmlRootAttribute("DOMDocument", Namespace = "http://ns.adobe.com/xfl/2008/")]
    //[XmlRoot("DOMDocument")]
    public class data_domdoc
    {
        public data_domdoc()
        {
            //xmlns.Add("xsi", "xmlns");

        }
        [XmlAttribute]
        public int width = 0;
        [XmlAttribute]
        public int height = 0;

        [XmlAttribute]
        public int currentTimeline = 1;
        [XmlAttribute]
        public string xflVersion = "2.1";
        [XmlAttribute]
        public string creatorInfo = "Adobe Flash Professional CS5.5";
        [XmlAttribute]
        public string platform = "Windows";

        [XmlAttribute]
        public string versionInfo = "Saved by Adobe Flash Windows 11.5 build 325";
        [XmlAttribute]
        public string majorVersion = "11";
        [XmlAttribute]
        public string minorVersion = "5";
        [XmlAttribute]
        public string buildNumber = "325";
        [XmlAttribute]
        public string viewAngle3D = "12.9595747283025";
        [XmlAttribute]
        public string nextSceneIdentifier = "2";
        [XmlAttribute]
        public string playOptionsPlayLoop = "false";
        [XmlAttribute]
        public string playOptionsPlayPages = "false";
        [XmlAttribute]
        public string playOptionsPlayFrameActions = "false";

        public class DOMBitmapItem
        {
            [XmlAttribute]
            public bool allowSmoothing = true;
            [XmlAttribute]
            public string compressionType = "lossless";
            [XmlAttribute]
            public string name = "PremiumTile_Shield-hd.png";
            [XmlAttribute]
            public string itemID = "50319ad5-00000270";
            [XmlAttribute]
            public string sourceExternalFilepath = "./LIBRARY/PremiumTile_Shield-hd.png";
            [XmlAttribute]
            public string sourceLastImported = "1341209140";
            [XmlAttribute]
            public int externalFileSize = 12317;
            [XmlAttribute]
            public string originalCompressionType = "lossless";
            [XmlAttribute]
            public string quality = "50";
            [XmlAttribute]
            public string href = "PremiumTile_Shield-hd.png";
            [XmlAttribute]
            public string bitmapDataHRef = "M 1 1345427941.dat";
            [XmlAttribute]
            public int frameRight = 1640;
            [XmlAttribute]
            public int frameBottom = 1580;
        }

        [XmlArray("media", Namespace = "http://ns.adobe.com/xfl/2008/")]
        public List<DOMBitmapItem> listDOMBitmap = new List<DOMBitmapItem>();

        public class Include
        {
            [XmlAttribute]
            public string href = "补间 1.xml";
            [XmlAttribute]
            public string itemIcon = "1";
            [XmlAttribute]
            public string loadImmediate = "false";
            [XmlAttribute]
            public string itemID = "50319a83-00000263";
            [XmlAttribute]
            public string lastModified = "1345428099";
        }
        [XmlArray("symbols", Namespace = "http://ns.adobe.com/xfl/2008/")]
        public List<Include> listSymbols = new List<Include>();

        public class DOMTimeline
        {
            [XmlAttribute]
            public string name = "场景 1";
            [XmlAttribute]
            public int currentFrame = 0;

            public class DOMLayer
            {
                [XmlAttribute]
                public string name = "图层 3";
                [XmlAttribute]
                public string color = "#FF800A";
                [XmlAttribute]
                public bool current = true;
                [XmlAttribute]
                public bool isSelected = true;

                public class DOMFrame
                {
                    [XmlAttribute]
                    public string name = "";
                    [XmlAttribute]
                    public string soundName = "";
                    [XmlAttribute]
                    public int index = 0;
                    [XmlAttribute]
                    public string tweenType = "motion";
                    [XmlAttribute]
                    public string motionTweenSnap = "true";
                    [XmlAttribute]
                    public int keyMode = 22017;
                    [XmlAttribute]
                    public int duration = 0;
                    public class DOMSymbolInstance
                    {
                        [XmlAttribute]
                        public string libraryItemName = "补间 3";
                        [XmlAttribute]
                        public string itemID = "补间 3";
                        [XmlAttribute]
                        public string symbolType = "graphic";
                        [XmlAttribute]
                        public string loop = "loop";



                        [XmlArray("matrix")]
                        public List<Matrix> matrix = new List<Matrix>();
                        [XmlArray("transformationPoint")]
                        public List<Point> transformationPoint = new List<Point>();
                        [XmlArray("color")]
                        public List<Color> color = new List<Color>();

                        public class Color
                        {
                            [XmlAttribute]
                            public byte redOffset = 0;
                            [XmlAttribute]
                            public byte greenOffset = 0;
                            [XmlAttribute]
                            public byte blueOffset = 0;
                            [XmlAttribute]
                            public float alphaMultiplier = 1.0f;
                            [XmlAttribute]
                            public float redMultiplier = 1.0f;
                            [XmlAttribute]
                            public float greenMultiplier = 1.0f;
                            [XmlAttribute]
                            public float blueMultiplier = 1.0f;
                        }
                    }
                    public class elements
                    {
                        [XmlElement("DOMSymbolInstance")]
                        public List<DOMSymbolInstance> listelements = new List<DOMSymbolInstance>();
                        [XmlElement("DOMBitmapInstance")]
                        public List<aniconv.xfl.DOMSymbolItem.DOMTimeline.DOMLayer.DOMFrame.DOMBitmapInstance> listbelements = new List<aniconv.xfl.DOMSymbolItem.DOMTimeline.DOMLayer.DOMFrame.DOMBitmapInstance>();
                    }
                    [XmlElement("elements")]
                    public elements element;

                    [XmlIgnore]
                    public List<DOMSymbolInstance> _elements
                    {
                        get
                        {
                            return element.listelements;
                        }
                    }


                    [XmlIgnore]
                    public List<aniconv.xfl.DOMSymbolItem.DOMTimeline.DOMLayer.DOMFrame.DOMBitmapInstance> __elements
                    {
                        get
                        {
                            return element.listbelements;
                        }
                    }
                }
                [XmlArray("frames")]
                public List<DOMFrame> frames = new List<DOMFrame>();
            }

            [XmlArray("layers")]
            public List<DOMLayer> layers = new List<DOMLayer>();
        }

        [XmlArray("timelines", Namespace = "http://ns.adobe.com/xfl/2008/")]
        public List<DOMTimeline> listTimeLine = new List<DOMTimeline>();

        public class swatchList
        {
        }

        [XmlArray("swatchLists", Namespace = "http://ns.adobe.com/xfl/2008/")]
        public List<swatchList> listswatch = new List<swatchList>();


        [XmlArray("extendedSwatchLists", Namespace = "http://ns.adobe.com/xfl/2008/")]
        public List<swatchList> listextendedwatch = new List<swatchList>();

        public class Empty
        {
        }

        [XmlArray("PrinterSettings", Namespace = "http://ns.adobe.com/xfl/2008/")]
        public List<Empty> listPrinterSetting = new List<Empty>();

        [XmlArray("publishHistory", Namespace = "http://ns.adobe.com/xfl/2008/")]
        public List<Empty> listpublishHistory = new List<Empty>();
        //<PrinterSettings/>
        //<publishHistory/>
    };


    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    [System.Xml.Serialization.XmlRootAttribute("DOMSymbolItem", Namespace = "http://ns.adobe.com/xfl/2008/")]
    public class DOMSymbolItem
    {
        [XmlAttribute]
        public string name = "补间 2";
        [XmlAttribute]
        public string itemID = "50319a83-00000266";
        [XmlAttribute]
        public string symbolType = "graphic";
        [XmlAttribute]
        public string lastModified = "";



        public class DOMTimeline
        {
            [XmlAttribute]
            public string name = "场景 1";

            public class DOMLayer
            {
                [XmlAttribute]
                public string name = "图层 3";
                [XmlAttribute]
                public string color = "#FF800A";
                [XmlAttribute]
                public bool current = true;
                [XmlAttribute]
                public bool isSelected = true;

                public class DOMFrame
                {
                    [XmlAttribute]
                    public int index = 0;
                    [XmlAttribute]
                    public int keyMode = 22017;


                    public class DOMBitmapInstance
                    {
                        [XmlAttribute]
                        public bool selected;
                        [XmlAttribute]
                        public string libraryItemName = "补间 3";


                        [XmlArray("matrix")]
                        public List<Matrix> matrix = new List<Matrix>();
                        [XmlArray("transformationPoint")]
                        public List<Point> transformationPoint = new List<Point>();
                    }

                    [XmlArray("elements")]
                    public List<DOMBitmapInstance> elements = new List<DOMBitmapInstance>();
                }
                [XmlArray("frames")]
                public List<DOMFrame> frames = new List<DOMFrame>();
            }

            [XmlArray("layers")]
            public List<DOMLayer> layers = new List<DOMLayer>();
        }
        public class timeline
        {
            [XmlElement("DOMTimeline", Namespace = "http://ns.adobe.com/xfl/2008/")]
            public DOMTimeline m_DOMTimeline = new DOMTimeline();
        }

        [XmlElement("timeline", Namespace = "http://ns.adobe.com/xfl/2008/")]
        public timeline m_timeline = new timeline();
    }


    [XmlRoot("projectDescription")]
    public class data_project
    {
        public string name = "";
        public string comment = "";

        public List<project> projects = new List<project>();
        public class project
        {
            [XmlText]
            public string text;
        }
        public List<nature> natures = new List<nature>();
        public class nature
        {
            [XmlText]
            public string text;
        }
    }

    [XmlRoot("actionScriptProperties")]
    public class data_asp
    {
        [XmlAttribute]
        public bool analytics;
        [XmlAttribute]
        public string mainApplicationPath;
        [XmlAttribute]
        public string projectUUID;
        [XmlAttribute]
        public string version;

        [XmlElement("compiler")]
        public compiler m_compiler;
        public class compiler
        {
            // additionalCompilerArguments="-locale en_US" autoRSLOrdering="true" copyDependentFiles="true" fteInMXComponents="false" generateAccessible="true" htmlExpressInstall="true" htmlGenerate="false" htmlHistoryManagement="true" htmlPlayerVersionCheck="true" includeNetmonSwc="false" outputFolderPath="bin-debug" removeUnusedRSL="true" sourceFolderPath="src" strict="true" targetPlayerVersion="0.0.0" useApolloConfig="false" useDebugRSLSwfs="true" verifyDigests="true" warn="true"
            // <compilerSourcePath/>
            [XmlElement("libraryPath")]
            public libraryPath m_libraryPath;
            public class libraryPath
            {
                [XmlAttribute]
                public int defaultLinkType;

                [XmlElement("libraryPathEntry")]
                public List<libraryPathEntry> entrys = new List<libraryPathEntry>();
                public class libraryPathEntry
                {
                    [XmlAttribute]
                    public int kind;
                    [XmlAttribute]
                    public int linkType;
                    [XmlAttribute]
                    public string path;
                    [XmlAttribute]
                    public bool useDefaultLinkType;

                    [XmlElement("excludedEntries")]
                    public excludedEntries m_excludedEntries;
                    public class excludedEntries
                    {
                        [XmlElement("libraryPathEntry")]
                        public List<libraryPathEntry> entrys = new List<libraryPathEntry>();
                    }
                }
            }
            // <sourceAttachmentPath/>

        }
        public List<application> applications = new List<application>();
        public class application
        {
            [XmlAttribute]
            public string path;
        }
    }


}
