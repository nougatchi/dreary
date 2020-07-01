using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSharpGL;
using dreary.Nodes;

namespace dreary.Forms
{
    public partial class NewInstance : Form
    {
        SceneNodeBase nodebase;
        Form1 src_form;
        public NewInstance(SceneNodeBase select, Form1 form)
        {
            InitializeComponent();
            nodebase = select;
            src_form = form;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch(listBox1.SelectedItem)
            {
                case "SkyboxNode":
                    string sn_skybox = "Content/e30cwk97.bmp";
                    InputBoxes.ShowInputDialog(ref sn_skybox, sn_skybox);
                    SkyboxNode node = SkyboxNode.Create(new Bitmap(Image.FromFile(sn_skybox)));
                    node.Name = "NewSkybox";
                    node.Parent = nodebase;
                    break;
                case "TextBillboardNode":
                    string sn_billboard = "Hello World!";
                    TextBillboardNode textBillboardNode = TextBillboardNode.Create(200, 40, 65535);
                    textBillboardNode.Name = "NewBillboard";
                    textBillboardNode.Parent = nodebase;
                    textBillboardNode.Text = sn_billboard;
                    textBillboardNode.EnableRendering = ThreeFlags.None;
                    break;
            }
            src_form.Match(src_form.treeView,nodebase);
        }
    }
}
