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
        public Scene scene; // the game scene
        public ActionList actionlist; // list of actions opengl has to do (acting funny for some reason)
        public Camera pcam; // player camera
        public DateTime time; // time since program start (gameinit being called and creating all the assets)
        public Form1()
        {
            InitializeComponent();
            treeView1.ImageList = DeviconServe.GenImageList();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitGame(); // initgame creates pcam, actionlist, scene and time
        }

        private void Match(TreeView treeView, SceneNodeBase nodeBase)
        {
            treeView.Nodes.Clear(); // clear everything in the treeview
            var node = new TreeNode(nodeBase.Name) { Tag = nodeBase }; // add a node, set its name to the Name field of nodebase
            treeView.Nodes.Add(node);
            node.ImageIndex = DeviconServe.GetDeviconIndex(nodeBase.GetType().Name); // get the devicon of this node
            node.SelectedImageIndex = DeviconServe.GetDeviconIndex(nodeBase.GetType().Name);
            var anode = new TreeNode(actionlist.GetType().Name) { Tag = actionlist }; // anode is just the action list
            treeView.Nodes.Add(anode);
            anode.ImageIndex = DeviconServe.GetDeviconIndex(actionlist.GetType().Name); // get the devicon of anode
            anode.SelectedImageIndex = DeviconServe.GetDeviconIndex(actionlist.GetType().Name);
            Match(node, nodeBase); // call for each child
            Match(anode, actionlist); // call for each child
        }

        private void Match(TreeNode node, SceneNodeBase nodeBase)
        {
            foreach (var item in nodeBase.Children) // item is based off of SceneNodeBase
            {
                var child = new TreeNode(item.Name) { Tag = item }; // add a node, set its name to the Name field of item
                node.Nodes.Add(child);
                child.ImageIndex = DeviconServe.GetDeviconIndex(item.GetType().Name);
                child.SelectedImageIndex = DeviconServe.GetDeviconIndex(item.GetType().Name);
                Match(child, item);
            }
        }
        private void Match(TreeNode node, ActionList nodeBase) 
        {
            foreach (var item in nodeBase) // item is a task that is put in ActionList
            {
                var child = new TreeNode(item.GetType().Name) { Tag = item }; // add a node, set its name to the type of its class
                child.ImageIndex = DeviconServe.GetDeviconIndex(item.GetType().Name);
                child.SelectedImageIndex = DeviconServe.GetDeviconIndex(item.GetType().Name);
                node.Nodes.Add(child);
                // dont call Match at all because the tasks itself dont contain children
            }
        }

        private void winGLCanvas1_OpenGLDraw(object sender, PaintEventArgs e) // draw code
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
                list.Act(new ActionParams(Viewport.GetCurrent())); // this apparently wont work. gl is known to work as it draws above and below but this wont budge at all
                GL.Instance.DrawText(10, 10, Color.Red, "Arial", 12, "FPS: " + winGLCanvas1.FPS.ToString() + " Time " + (DateTime.Now - time).Seconds);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyGrid1.SelectedObject = e.Node.Tag;
        }

        /// <summary>
        /// InitGame initialises all of the classes (pcam, scene, time, the game scene root). It must be called.
        /// </summary>
        private void InitGame()
        {
            var position = new vec3(5, 3, 4);
            var center = new vec3(0, 0, 0);
            var up = new vec3(0, 1, 0);
            pcam = new Camera(position, center, up, CameraType.Perspective, this.winGLCanvas1.Width, this.winGLCanvas1.Height); // create the camera
            GroupNode rootElement = new GroupNode(); // this will be the rootnode of the scene
            CreateGameTree(rootElement);
            scene = new Scene(pcam) // initialises the scene, use pcam as camera
            {
                RootNode = rootElement,
                ClearColor = Color.Black.ToVec4(),
            };
            actionlist = new ActionList();
            treeView1.ExpandAll();
            time = new DateTime();

            var list = new ActionList(); // iniitialise the actionlist
            var transformAction = new TransformAction(scene); // transform action
            list.Add(transformAction);
            var billboardSortAction = new BillboardSortAction(scene.RootNode, scene.Camera); // billboard sort
            list.Add(billboardSortAction);
            var renderAction = new RenderAction(scene); // render
            list.Add(renderAction);
            var billboardRenderAction = new BillboardRenderAction(scene.Camera, billboardSortAction); // billboard render
            list.Add(billboardRenderAction);
            actionlist = list;
            var manipulater = new FirstPerspectiveManipulater(); // allows moving the camera
            manipulater.BindingMouseButtons = GLMouseButtons.Right;
            manipulater.StepLength = 0.1f;
            manipulater.Bind(pcam, winGLCanvas1);
            Match(treeView1, scene.RootNode); // update the treeview
        }

        /// <summary>
        /// This places all of the smaller classes in the root node
        /// </summary>
        /// <param name="node">The root node in question</param>
        private void CreateGameTree(GroupNode node)
        {
            node.Initialize(); // initialise root node
            CameraNode cameraNode = new CameraNode(pcam); // bind cameranode to pcam
            cameraNode.Name = "Camera";
            node.Children.Add(cameraNode); // add this to the rootnode
            cameraNode.Initialize(); // init camnode
            TextBillboardNode billboardNode = TextBillboardNode.Create(200, 40, 100); // create a billboard
            billboardNode.EnableRendering = ThreeFlags.None;
            billboardNode.Color = Color.White.ToVec3();
            billboardNode.Text = "Hello"; // sets the billboard text to Hello
            billboardNode.WorldPosition = new vec3(5, 0, 0);
            billboardNode.Name = "Billboard_Dbg"; // set the billboard name to Billboard_Dbg
            node.Children.Add(billboardNode); // add this to the rootnode
            Nodes.SkyboxNode skybox = Nodes.SkyboxNode.Create(new Bitmap(800, 800, System.Drawing.Imaging.PixelFormat.Format8bppIndexed)); // create the skybox
            try
            {
                skybox = Nodes.SkyboxNode.Create(new Bitmap(Image.FromFile("Content/e30cwk97.bmp"))); // attempt loading the skybox "e30cwk97.bmp"
            }
            catch (Exception e)
            {

            }
            skybox.Name = "Skybox";
            node.Children.Add(skybox); // add this to the rootnode
        }
    }
}
