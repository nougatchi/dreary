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

namespace dreary
{
    public partial class Form1 : Form
    {
        public Scene scene;
        public ActionList actionlist;
        public Camera pcam;
        public DateTime time;
        public Form1()
        {
            InitializeComponent();
            
            var position = new vec3(5, 3, 4);
            var center = new vec3(0, 0, 0);
            var up = new vec3(0, 1, 0);
            pcam = new Camera(position, center, up, CameraType.Perspective, this.winGLCanvas1.Width, this.winGLCanvas1.Height);
            GroupNode rootElement = new GroupNode();
            {
                CameraNode cameraNode = new CameraNode(pcam);
                rootElement.Children.Add(cameraNode);
                cameraNode.Initialize();
                TextBillboardNode billboardNode = TextBillboardNode.Create(200, 40, 100);
                billboardNode.EnableRendering = ThreeFlags.None;
                billboardNode.Color = Color.White.ToVec3();
                billboardNode.Text = "Hello";
                billboardNode.WorldPosition = new vec3(5, 0, 0);
                rootElement.Children.Add(billboardNode);
                Nodes.SkyboxNode skybox = Nodes.SkyboxNode.Create(new Bitmap(800, 800, System.Drawing.Imaging.PixelFormat.Format8bppIndexed));
                try
                {
                    skybox = Nodes.SkyboxNode.Create(new Bitmap(Image.FromFile("Content/e30cwk97.bmp")));
                } catch(Exception e) { 

                }
                rootElement.Children.Add(skybox);
            }
            scene = new Scene(pcam)
            {
                RootNode = rootElement,
                ClearColor = Color.Black.ToVec4(),
            };
            actionlist = new ActionList();
            Match(treeView1, scene.RootNode);
            treeView1.ExpandAll();
            time = new DateTime();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var list = new ActionList();
            var transformAction = new TransformAction(scene);
            list.Add(transformAction);
            var billboardSortAction = new BillboardSortAction(scene.RootNode, scene.Camera);
            var renderAction = new RenderAction(scene);
            list.Add(renderAction);
            var billboardRenderAction = new BillboardRenderAction(scene.Camera, billboardSortAction);
            list.Add(billboardRenderAction);
            actionlist = list;
            var manipulater = new FirstPerspectiveManipulater();
            manipulater.Bind(pcam, winGLCanvas1);
        }

        private void Match(TreeView treeView, SceneNodeBase nodeBase)
        {
            treeView.Nodes.Clear();
            var node = new TreeNode(nodeBase.ToString()) { Tag = nodeBase };
            treeView.Nodes.Add(node);
            Match(node, nodeBase);
        }

        private void Match(TreeNode node, SceneNodeBase nodeBase)
        {
            foreach (var item in nodeBase.Children)
            {
                var child = new TreeNode(item.ToString()) { Tag = item };
                node.Nodes.Add(child);
                Match(child, item);
            }
        }

        private void winGLCanvas1_OpenGLDraw(object sender, PaintEventArgs e)
        {
            ActionList list = actionlist;
            if (list != null)
            {
                vec4 clearColor = scene.ClearColor;
                GL.Instance.ClearColor(clearColor.x, clearColor.y, clearColor.z, clearColor.w);
                GL.Instance.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);
                list.Act(new ActionParams(Viewport.GetCurrent()));
                GL.Instance.DrawText(10, 10, Color.Red, "Arial", 12, "FPS: " + winGLCanvas1.FPS.ToString() + " Time " + (DateTime.Now - time).Seconds);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyGrid1.SelectedObject = e.Node.Tag;
        }
    }
}
