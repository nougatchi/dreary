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
            treeView1.ImageList = DeviconServe.GenImageList();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitGame();
        }

        private void Match(TreeView treeView, SceneNodeBase nodeBase)
        {
            treeView.Nodes.Clear();
            var node = new TreeNode(nodeBase.Name) { Tag = nodeBase };
            treeView.Nodes.Add(node);
            node.ImageIndex = DeviconServe.GetDeviconIndex(nodeBase.GetType().Name);
            node.SelectedImageIndex = DeviconServe.GetDeviconIndex(nodeBase.GetType().Name);
            var anode = new TreeNode(actionlist.GetType().Name) { Tag = actionlist };
            treeView.Nodes.Add(anode);
            anode.ImageIndex = DeviconServe.GetDeviconIndex(actionlist.GetType().Name);
            anode.SelectedImageIndex = DeviconServe.GetDeviconIndex(actionlist.GetType().Name);
            Match(node, nodeBase);
            Match(anode, actionlist);
        }

        private void Match(TreeNode node, SceneNodeBase nodeBase)
        {
            foreach (var item in nodeBase.Children)
            {
                var child = new TreeNode(item.Name) { Tag = item };
                node.Nodes.Add(child);
                child.ImageIndex = DeviconServe.GetDeviconIndex(item.GetType().Name);
                child.SelectedImageIndex = DeviconServe.GetDeviconIndex(item.GetType().Name);
                Match(child, item);
            }
        }
        private void Match(TreeNode node, ActionList nodeBase)
        {
            foreach (var item in nodeBase)
            {
                var child = new TreeNode(item.GetType().Name) { Tag = item };
                child.ImageIndex = DeviconServe.GetDeviconIndex(item.GetType().Name);
                child.SelectedImageIndex = DeviconServe.GetDeviconIndex(item.GetType().Name);
                node.Nodes.Add(child);
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
                GL.Instance.DrawText(10, 22, Color.Red, "Arial", 12, "List Size: " + list.Count);
                string tasks = "Tasks: ";
                foreach(object i in list)
                {
                    tasks += $"{i.GetType().Name} ";
                }
                GL.Instance.DrawText(10, 34, Color.Red, "Arial", 12, tasks);
                list.Act(new ActionParams(Viewport.GetCurrent()));
                GL.Instance.DrawText(10, 10, Color.Red, "Arial", 12, "FPS: " + winGLCanvas1.FPS.ToString() + " Time " + (DateTime.Now - time).Seconds);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyGrid1.SelectedObject = e.Node.Tag;
        }

        private void InitGame()
        {
            var position = new vec3(5, 3, 4);
            var center = new vec3(0, 0, 0);
            var up = new vec3(0, 1, 0);
            pcam = new Camera(position, center, up, CameraType.Perspective, this.winGLCanvas1.Width, this.winGLCanvas1.Height);
            GroupNode rootElement = new GroupNode();
            CreateGameTree(rootElement);
            scene = new Scene(pcam)
            {
                RootNode = rootElement,
                ClearColor = Color.Black.ToVec4(),
            };
            actionlist = new ActionList();
            treeView1.ExpandAll();
            time = new DateTime();

            var list = new ActionList();
            var transformAction = new TransformAction(scene);
            list.Add(transformAction);
            var billboardSortAction = new BillboardSortAction(scene.RootNode, scene.Camera);
            list.Add(billboardSortAction);
            var renderAction = new RenderAction(scene);
            list.Add(renderAction);
            var billboardRenderAction = new BillboardRenderAction(scene.Camera, billboardSortAction);
            list.Add(billboardRenderAction);
            actionlist = list;
            var manipulater = new FirstPerspectiveManipulater();
            manipulater.BindingMouseButtons = GLMouseButtons.Right;
            manipulater.StepLength = 0.1f;
            manipulater.Bind(pcam, winGLCanvas1);
            Match(treeView1, scene.RootNode);
        }

        private void CreateGameTree(GroupNode node)
        {
            node.Initialize();
            /*CameraNode cameraNode = new CameraNode(pcam);
            cameraNode.Name = "Camera";
            node.Children.Add(cameraNode);
            cameraNode.Initialize();*/
            TextBillboardNode billboardNode = TextBillboardNode.Create(200, 40, 100);
            billboardNode.EnableRendering = ThreeFlags.None;
            billboardNode.Color = Color.White.ToVec3();
            billboardNode.Text = "Hello";
            billboardNode.WorldPosition = new vec3(5, 0, 0);
            billboardNode.Name = "Billboard_Dbg";
            node.Children.Add(billboardNode);
            Nodes.SkyboxNode skybox = Nodes.SkyboxNode.Create(new Bitmap(800, 800, System.Drawing.Imaging.PixelFormat.Format8bppIndexed));
            try
            {
                skybox = Nodes.SkyboxNode.Create(new Bitmap(Image.FromFile("Content/e30cwk97.bmp")));
            }
            catch (Exception e)
            {

            }
            skybox.Name = "Skybox";
            node.Children.Add(skybox);
        }
    }
}
