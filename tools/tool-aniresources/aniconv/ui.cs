
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using clgf;
using aniconv.xfl;
using clgf.Anim;
using System;
using Microsoft.Xna.Framework.Graphics;
using clgf.Texture;
using Microsoft.Xna.Framework;

namespace aniconv
{
    public partial class ui : Form, ILogger
    {
        Game1 game = null;
        public ui(Game1 _game)
        {
            InitializeComponent();
            game = _game;
        }
        public delegate void deleOnResize(int x, int y);
        public event deleOnResize OnGraphReszie;
        public void Init(Form subForm)
        {
            subForm.Enabled = false;
            subForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            subForm.TopLevel = false;
            subForm.Dock = DockStyle.Fill;
            this.pictureBox1.Controls.Add(subForm);
            this.pictureBox1.Resize += (s, e) =>
            {
                OnGraphReszie(this.pictureBox1.Width, this.pictureBox1.Height);
            };
            OnGraphReszie(this.pictureBox1.Width, this.pictureBox1.Height);
        }

        private void UpdateAnimTree()
        {
            treeView1.Nodes.Clear();
            UpdateAnimTree_Sub("char");
            UpdateAnimTree_Sub("scene");
            UpdateAnimTree_Sub("webgame");
            treeView1.ExpandAll();
        }
        private void UpdateAnimTree_Sub(string name)
        {
            if (System.IO.Directory.Exists("resources\\" + name) == false) return;
            TreeNode tnode = new TreeNode(name);
            treeView1.Nodes.Add(tnode);
            string[] dirs = System.IO.Directory.GetDirectories("resources\\" + name);
            foreach (var d in dirs)
            {
                string pathname = System.IO.Path.GetFileName(d);
                TreeNode tchar = new TreeNode(pathname);
                tchar.Tag = "char:" + d;
                tnode.Nodes.Add(tchar);

                try
                {
                    string[] files = System.IO.Directory.GetFiles(d + "\\action-srcs", "*.xfl", System.IO.SearchOption.AllDirectories);
                    foreach (var f in files)
                    {
                        string aniname = System.IO.Path.GetFileNameWithoutExtension(f);
                        TreeNode tanim = new TreeNode(aniname);
                        tanim.Tag = "anim:" + f;
                        tchar.Nodes.Add(tanim);
                    }
                }
                catch (Exception err)
                {
                }
            }
        }
        private void ui_Load(object sender, System.EventArgs e)
        {
            UpdateAnimTree();


        }


        private void ui_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void ui_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        public void Log(LogLevel level, string text)
        {
            this.listBox1.Items.Add(level.ToString() + ":" + text);
        }
        string setcharpath;
        string setchar;
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            if (e.Node.Tag == null) return;
            string str = e.Node.Tag as string;
            if (str.Contains("char:"))//选中角色
            {
                setchar = e.Node.Text;
                setcharpath = str.Substring(5);
                this.Text = setcharpath;
                if (anis.ContainsKey(setchar) == false) return;
                UpdateAnim(anis[setchar].anims.Values);
                lastani = null;
                UpdateAniInfo();
            }
            else if (str.Contains("anim:"))
            {
                if (e.Node.Parent.Tag == null) return;
                //string pstr = e.Node.Parent.Tag as string;
                if (setchar == e.Node.Parent.Text)
                {
                    if (anis.ContainsKey(setchar) == false) return;
                    //选中了一个动画


                    var anim = anis[setchar].anims[e.Node.Text];
                    UpdateAnim(new Anim[] { anim });
                    UpdateAniInfo();
                    //game.DrawSprite(SpriteAni.Create(anim, game.tmgr, "tmpcharbitmap"),new Vector2(anim.size.X/2,anim.size.Y));
                }
            }
            else
            {
                setcharpath = null;
                setchar = null;
            }
        }
        Dictionary<string, Sprite> anis = new Dictionary<string, Sprite>();
        private void button1_Click(object sender, EventArgs __e)
        {//加载角色
            if (setcharpath == null) return;
            anis[setchar] = Sprite.Load(null);
            ProjParser proj = new ProjParser(this, "resources", anis[setchar]);
            game.ClearSprite();
            proj.TestDummySeed(setcharpath);
            string[] files = System.IO.Directory.GetFiles(setcharpath, "*.xfl", System.IO.SearchOption.AllDirectories);
            List<string> texs = new List<string>();
            foreach (var p in files)
            {
                string path = System.IO.Path.GetDirectoryName(p);

                proj.Read(path, this);
                string aniname = System.IO.Path.GetFileNameWithoutExtension(p);
                try
                {
                    var ani = proj.FillAni(aniname);
                    anis[setchar].anims[aniname] = ani;
                }
                catch (Exception err)
                {
                    this.Log(LogLevel.Error, "处理动画"+aniname+"出错");
                }

            }

            //unused.Clear();
            foreach (var k in anis[setchar].seeds.Keys)
            {


                //if (texs.Contains(anis[setchar].seeds[k].texname) == false)
                {
                    //unused[k] = anis[setchar].seeds[k];
                    texs.Add(anis[setchar].seeds[k].texname);
                }
            }
            if (System.IO.Directory.Exists("content\\tmpcharbitmap"))
            {
                //System.IO.Directory.Delete("content\\tmpcharbitmap",true);
            }
            //System.IO.Directory.CreateDirectory("content\\tmpcharbitmap");
            foreach (var tname in texs)
            {
                string outname = System.IO.Path.Combine("content\\tmpcharbitmap", tname);
                string outpath = System.IO.Path.GetDirectoryName(outname);
                if (System.IO.Directory.Exists(outpath) == false)
                {
                    System.IO.Directory.CreateDirectory(outpath);
                }
                System.IO.File.Copy(System.IO.Path.Combine("resources", tname), outname, true);

            }
            //在此之前，应该把所需贴图复制到某个路径

            game.tmgr.ClearPacketInfo();
            float y = 0;
            float x = 0;
            foreach (var ani in anis[setchar].anims.Values)
            {
                x = Math.Max(ani.size.X, x);
            }
            UpdateAnim(anis[setchar].anims.Values);
            UpdateAniInfo();
            //foreach (var ani in anis[setchar].anims.Values)
            //{
            //    y += ani.size.Y;
            //    game.DrawSprite(SpriteAni.Create(ani, game.tmgr, "tmpcharbitmap"), new Vector2(x / 2, y));
            //    //game.DrawSprite(SpriteAni.Create(ani, game.tmgr, ""));
            //}
        }
        List<string> tags = new List<string>();
        class TagInfo
        {
            public TagInfo(string tag)
            {
                this.tag = tag;
            }
            public string tag;
            public override string ToString()
            {
                string outstr = "";
                if (tag == "") outstr = "tag:" + "<all>";
                else outstr = "tag:" + tag;
                if (color != colorstyle.style_null)
                {
                    outstr += "|" + color.ToString();
                }
                return outstr;
                //base.ToString();
            }
            public colorstyle color = colorstyle.style_null;
            public enum colorstyle
            {
                style_null,
                style_red,
                style_green,
                style_blue,
            }
        }

        void UpdateAnim(IEnumerable<Anim> anim)
        {
            game.ClearSprite();
            float x = 50;
            tags.Clear();

            foreach (var ani in anim)
            {
                var drawani = SpriteAni.CreateAni(ani, game.tmgr, "tmpcharbitmap");
                lastani = drawani;
                for (int i = 0; i < ani.frames.Count; i++)
                {
                    foreach (var tag in drawani.GetFrameByID(i).bounds.Keys)
                    {
                        if (tags.Contains(tag) == false) tags.Add(tag);
                    }
                }
                game.DrawSprite(drawani, new Vector2(x + ani.size.X / 2, 300));
                x += ani.size.X * 1.5f;
            }
            if (tags.Contains("") == false)
                tags.Add("");
            this.checkedListBox1.Items.Clear();
            this.listBox2.Items.Clear();
            foreach (var l in tags)
            {
                this.checkedListBox1.Items.Add(new TagInfo(l));
                this.listBox2.Items.Add(new TagInfo(l));

            }

            //this.checkedListBox1.SelectedItems.Clear();
            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }


            checkedListBox2.Items.Clear();
            foreach (var i in game.dummyshow)
            {
                checkedListBox2.Items.Add(i.Key);
                checkedListBox2.SetItemChecked(checkedListBox2.Items.Count - 1, i.Value);
            }

        }

        private void button2_Click(object sender, EventArgs _e)
        {//导出角色
            if (setchar == null) return;
            List<string> texs = new List<string>();
            string outstr = setcharpath.Replace("resources", "content");
            anis[setchar].Save(outstr);
            //foreach (var i in anis[setchar].anims)
            //{
            //    //string outpath = "resources\\char\\" + setchar + "\\" + i.Key + ".anim.bin";
            //    //using (System.IO.Stream s = System.IO.File.Create(outpath))
            //    //{
            //    //    i.Value.Write(s);

            //    //}
            //    foreach (var f in i.Value.frames)
            //    {
            //        foreach (var e in f.elems)
            //        {
            //            if (texs.Contains(e.seed.texname) == false)
            //            {
            //                texs.Add(e.seed.texname);
            //            }
            //        }
            //    }
            //}
            foreach (var i in anis[setchar].seeds.Values)
            {
                texs.Add(i.texname);
            }
            PacketTexture(texs, setchar, System.IO.Path.GetDirectoryName(outstr));
        }
        void PacketTexture(List<string> filename, string outputname, string outputpath)
        {
            Dictionary<string, Texture2D> texs = new Dictionary<string, Texture2D>();

            foreach (var f in filename)
            {
                using (System.IO.Stream s = System.IO.File.OpenRead(System.IO.Path.Combine("resources", f)))
                {
                    Texture2D t = Texture2D.FromStream(game.GraphicsDevice, s);
                    texs[f] = t;
                }
            }
            IList<TexturePacket> packet = TexturePacket.PacketTexture(game.GraphicsDevice, texs);
            int i = 0;
            foreach (var p in packet)
            {
                i++;
                string outfile = System.IO.Path.Combine(outputpath, outputname);
                if (i >= 2)
                    outfile += "_" + i;

                using (System.IO.Stream s = System.IO.File.Create(outfile + ".png"))
                {
                    p.texture.SaveAsPng(s, p.texture.Width, p.texture.Height);
                }
                using (System.IO.Stream s = System.IO.File.Create(outfile + ".packet.csv.txt"))
                {
                    p.Save(s);
                }
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            game.shadowtags.Clear();
            TagInfo i = checkedListBox1.Items[e.Index] as TagInfo;
            foreach (TagInfo _info in checkedListBox1.CheckedItems)
            {
                if (_info == i) continue;
                game.shadowtags.Add(_info.tag);
            }
            if (e.NewValue == CheckState.Checked)
            {
                game.shadowtags.Add(i.tag);
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        void updatebound()
        {
            game.boundcolor.Clear();
            List<TagInfo> tags = new List<TagInfo>();
            for (int i = 0; i < listBox2.Items.Count; i++)
            {
                tags.Add(listBox2.Items[i] as TagInfo);
            }
            listBox2.Items.Clear();


            foreach (TagInfo i in tags)
            {
                listBox2.Items.Add(i);
                if (i.color == TagInfo.colorstyle.style_red)
                {
                    game.boundcolor[i.tag] = new Microsoft.Xna.Framework.Color(1.0f, 0, 0, 0.5f);
                }
                if (i.color == TagInfo.colorstyle.style_green)
                {
                    game.boundcolor[i.tag] = new Microsoft.Xna.Framework.Color(0, 1.0f, 0, 0.5f);
                }
                if (i.color == TagInfo.colorstyle.style_blue)
                {
                    game.boundcolor[i.tag] = new Microsoft.Xna.Framework.Color(0, 0, 1.0f, 0.5f);
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {//color clear
            TagInfo t = listBox2.SelectedItem as TagInfo;
            if (t == null) return;
            t.color = TagInfo.colorstyle.style_null;
            updatebound();
        }

        private void button4_Click(object sender, EventArgs e)
        {//color red;
            //for (int i = 0; i < listBox2.Items.Count; i++)
            //{
            //    TagInfo t = listBox2.Items[i] as TagInfo;
            //    t.color = TagInfo.colorstyle.style_null;
            //}
            TagInfo t = listBox2.SelectedItem as TagInfo;
            if (t == null) return;
            t.color = TagInfo.colorstyle.style_red;
            updatebound();
        }

        private void button5_Click(object sender, EventArgs e)
        {//color green
            //for (int i = 0; i < listBox2.Items.Count; i++)
            //{
            //    TagInfo t = listBox2.Items[i] as TagInfo;
            //    t.color = TagInfo.colorstyle.style_null;
            //}
            TagInfo t = listBox2.SelectedItem as TagInfo;
            if (t == null) return;
            t.color = TagInfo.colorstyle.style_green;
            updatebound();
        }

        private void button6_Click(object sender, EventArgs e)
        {//color blue
            //for (int i = 0; i < listBox2.Items.Count; i++)
            //{
            //    TagInfo t = listBox2.Items[i] as TagInfo;
            //    t.color = TagInfo.colorstyle.style_null;
            //}
            TagInfo t = listBox2.SelectedItem as TagInfo;
            if (t == null) return;
            t.color = TagInfo.colorstyle.style_blue;
            updatebound();
        }
        SpriteAni lastani = null;
        void UpdateAniInfo()
        {
            listBox3.Items.Clear();
            if (lastani == null) return;
            listBox3.Items.Add(new KeyValuePair<string, AniElement>("<all>", lastani.GetElement(null)));
            foreach (var e in lastani.elements)
            {
                listBox3.Items.Add(e);

            }



        }
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox3.SelectedItem == null) return;
            KeyValuePair<string, AniElement> pair = (KeyValuePair<string, AniElement>)listBox3.SelectedItem;
            if (game.onlycontroller == null) return;
            if (pair.Value.loop)
            {


                game.onlycontroller.State(pair.Value);
            }
            else
            {
                game.onlycontroller.Play(pair.Value, SpriteAniController.playtag.play_wait);
                game.onlycontroller.State(null);
            }
        }

        private void checkedListBox2_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string tag = checkedListBox2.Items[e.Index].ToString();

            if (game.dummyshow.ContainsKey(tag))
            {
                game.dummyshow[tag] = (e.NewValue == CheckState.Checked);
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            checkedListBox2.Items.Clear();
            foreach (var i in game.dummyshow)
            {
                checkedListBox2.Items.Add(i.Key);
                checkedListBox2.SetItemChecked(checkedListBox2.Items.Count - 1, i.Value);
            }
        }
    }
}
