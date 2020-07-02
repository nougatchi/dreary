using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CSharpGL;
using dreary.Net;

namespace dreary
{
    public partial class Form1 : Form
    {
        public Scene scene; // the game scene
        public ActionList actionlist; // list of actions opengl has to do (acting funny for some reason)
        public Camera pcam; // player camera
        public DateTime time; // time since program start (gameinit being called and creating all the assets)
        public GroupNode rootElement;
        public Client client;
        public Server server;
        public TreeView treeView { get { return treeView1; } }
        public FirstPerspectiveManipulater camManip;

        public bool StatusmessageEnabled;
        public string Statusmessage;

        private bool nodbg;

        public Form1()
        {
            InitializeComponent();
            treeView1.ImageList = DeviconServe.GenImageList(); // generate the image list

        }
        
        public void NobgTrue()
        {
            splitContainer1.Panel1Collapsed = true;
            menuStrip1.Visible = false;
            nodbg = true;
        }

        public void Reenter()
        {
            splitContainer1.Panel1Collapsed = false;
            menuStrip1.Visible = true;
            camManip.Bind(pcam, winGLCanvas1);
        }

        public void ConnectModeEnable()
        {
            NobgTrue();
            camManip.Unbind();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitGame(); // initgame creates pcam, actionlist, scene and time
            string[] cfg = new string[] {

            };
            try
            {
                cfg = File.ReadAllLines("game.cfg"); // try to read game.cfg
            } catch (Exception)
            {

            }
            nodbg = false;
            foreach(string i in cfg) // iterate through each line of cfg
            {
                string cmd = i.Split(' ')[0];
                switch (cmd)
                {
                    case "nodbg": // nodbg removes all of the explorer and menustrip assets, leaving only the GL window behind
                        splitContainer1.Panel1Collapsed = true;
                        menuStrip1.Visible = false;
                        nodbg = true;
                        break;
                }
            }
        }

        public void Match(TreeView treeView, SceneNodeBase nodeBase)
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
            var cam = new TreeNode(pcam.GetType().Name) { Tag = pcam };
            treeView.Nodes.Add(cam);
            treeView.ImageIndex = DeviconServe.GetDeviconIndex(pcam.GetType().Name);
            treeView.SelectedImageIndex = DeviconServe.GetDeviconIndex(pcam.GetType().Name);
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
                list.Act(new ActionParams(Viewport.GetCurrent())); // fixed
                if (!nodbg)
                {
                    string tasks = "Tasks: ";
                    foreach (object i in list)
                    {
                        tasks += $"{i.GetType().Name} ";
                    }
                    GL.Instance.DrawText(10, 22, Color.Red, "Arial", 12, "List Size: " + list.Count);
                    GL.Instance.DrawText(10, 34, Color.Red, "Arial", 12, tasks);
                    GL.Instance.DrawText(10, 10, Color.Red, "Arial", 12, "FPS: " + winGLCanvas1.FPS.ToString() + " Time " + (DateTime.Now - time).Seconds);
                }
                if(StatusmessageEnabled)
                {
                    GL.Instance.DrawText(10, 34, Color.White, "Verdana", 24, Statusmessage);
                }
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
            rootElement = new GroupNode(); // this will be the rootnode of the scene
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
            camManip = new FirstPerspectiveManipulater(); // allows moving the camera
            camManip.BindingMouseButtons = GLMouseButtons.Right;
            camManip.StepLength = 0.1f;
            camManip.Bind(pcam, winGLCanvas1);
            Match(treeView1, scene.RootNode); // update the treeview
        }

        /// <summary>
        /// This places all of the smaller classes in the root node
        /// </summary>
        /// <param name="node">The root node in question</param>
        private void CreateGameTree(GroupNode node)
        {
            node.Name = "Workspace"; // set the name so it will appear in the browser properly
            node.Initialize(); // initialise root node
            // apparently cameranode doesnt work.
            /*CameraNode cameraNode = new CameraNode(pcam); // bind cameranode to pcam
            cameraNode.Name = "Camera";
            node.Children.Add(cameraNode); // add this to the rootnode
            cameraNode.Initialize(); // init camnode*/
            Nodes.SkyboxNode skybox = Nodes.SkyboxNode.Create(new Bitmap(800, 800, System.Drawing.Imaging.PixelFormat.Format8bppIndexed)); // create the skybox
            try
            {
                skybox = Nodes.SkyboxNode.Create(new Bitmap(Image.FromFile("Content/skybox.png"))); // attempt loading the skybox "skybox"
            }
            catch (Exception e)
            {

            }
            skybox.Name = "Skybox";
            node.Children.Add(skybox); // add this to the rootnode
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Forms.NewInstance instance = new Forms.NewInstance(rootElement, this);
                instance.ShowDialog();
            } catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Match(treeView1, rootElement);
            }
        }

        private void destroyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SceneNodeBase node = (SceneNodeBase)treeView1.SelectedNode.Tag;
                if (node != scene.RootNode)
                {
                    node.Parent.Children.Remove(node);
                    node.Dispose();
                    Match(treeView1, rootElement);
                }
                else
                {
                    MessageBox.Show("You cant delete the workspace, silly!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void winGLCanvas1_Resize(object sender, EventArgs e)
        {
            scene.Camera.AspectRatio = ((float)winGLCanvas1.Width) / ((float)winGLCanvas1.Height);
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client = new Client(this);
            client.Connect("127.0.0.1", 3398);
        }

        private void hostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            server = new Server(3398);
            server.Start();
        }
    }
}
