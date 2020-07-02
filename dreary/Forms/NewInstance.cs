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
                    string sn_skybox = "Content/skybox.png";
                    InputBoxes.ShowInputDialog(ref sn_skybox, "Skybox");
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
                case "CubeNode":
                    CubeNode cube = CubeNode.Create();
                    cube.Name = "NewCube";
                    cube.Parent = nodebase;
                    break;
                case "ShadowVolumeNode":
                    CubeModel model = new CubeModel();
                    string sv_pos = CubeModel.strPosition;
                    string sv_normal = "0 0 0";
                    InputBoxes.ShowInputDialog(ref sv_pos, "Volume Position");
                    InputBoxes.ShowInputDialog(ref sv_pos, "Volume Normal");
                    ShadowVolumeNode shadow = ShadowVolumeNode.Create(model, sv_pos, sv_normal, new vec3(1, 1, 1));
                    shadow.Name = "NewLight";
                    shadow.Parent = nodebase;
                    break;
            }
            src_form.Match(src_form.treeView,nodebase);
        }
    }
}
