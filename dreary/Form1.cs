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
using dreary.Basplash;
using System.Reflection;
using System.Diagnostics;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using dreary.Properties;

namespace dreary
{
    public partial class Form1 : Form
    {
        public Scene scene; // the game scene
        public ActionList actionlist; // list of actions opengl has to do (acting funny for some reason)
        public Camera pcam; // player camera
        public DateTime time; // time since program start (gameinit being called and creating all the assets)
        public GroupNode rootElement;
        public bool dllMode;
        public Client client;
        public Server server;
        public TreeView treeView { get { return treeView1; } }
        public FirstPerspectiveManipulater camManip;
        public DrearyGameDll gameDll;
        public bool throwFatalInsteadOfMsg;

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
            try
            {
                if (!dllMode)
                {
                    InitGame(); // initgame creates pcam, actionlist, scene and time
                    string[] cfg = new string[] {

            };
                    try
                    {
                        cfg = File.ReadAllLines("game.cfg"); // try to read game.cfg
                    }
                    catch (Exception)
                    {

                    }
                    nodbg = false;
                    foreach (string i in cfg) // iterate through each line of cfg
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
#if DEBUG_CALLS
                Basplash.Basplash basplash = new Basplash.Basplash();
                basplash.Execute("object.RvspInt|abacba|1002;game.Print|vabacba");
#endif
                }
                else
                {
                    gameDll.DrearyCreateInstance("game.External", "External");
                    string[] gd = (string[])gameDll.DrearyCall("External", "getGameDetails");
                    Console.WriteLine("Loading " + gd[0]);
                    gameDll.DrearyCall("External", "gInit", pcam, time, rootElement, client, server, camManip, actionlist, scene, winGLCanvas1);
                }

            }
            catch (Exception ex)
            {
                if(throwFatalInsteadOfMsg)
                {
                    throw;
                }
                GameFatal gf = new GameFatal(ex);
                gf.Show();
            }
        }

        public void Match(TreeView treeView, SceneNodeBase nodeBase)
        {
            try
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
                cam.ImageIndex = DeviconServe.GetDeviconIndex(pcam.GetType().Name);
                cam.SelectedImageIndex = DeviconServe.GetDeviconIndex(pcam.GetType().Name);
                var scn = new TreeNode(scene.GetType().Name) { Tag = scene };
                treeView.Nodes.Add(scn);
                scn.ImageIndex = DeviconServe.GetDeviconIndex(scn.GetType().Name);
                scn.SelectedImageIndex = DeviconServe.GetDeviconIndex(scn.GetType().Name);
                Match(node, nodeBase); // call for each child
                Match(anode, actionlist); // call for each child
                treeView.ExpandAll();
            }
            catch (Exception ex)
            {
                GameFatal gf = new GameFatal(ex);
                gf.Show();
            }
        }

        private void Match(TreeNode node, SceneNodeBase nodeBase)
        {
            try
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
            catch (Exception ex)
            {
                if (throwFatalInsteadOfMsg)
                {
                    throw;
                }
                GameFatal gf = new GameFatal(ex);
                gf.Show();
            }
        }
        private void Match(TreeNode node, ActionList nodeBase) 
        {
            try
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
            catch (Exception ex)
            {
                GameFatal gf = new GameFatal(ex);
                gf.Show();
            }
        }

        private void winGLCanvas1_OpenGLDraw(object sender, PaintEventArgs e) // draw code
        {
            try
            {
                ActionList list = actionlist;
                if (list != null)
                {
                    vec4 clearColor = scene.ClearColor;
                    GL.Instance.ClearColor(clearColor.x, clearColor.y, clearColor.z, clearColor.w);
                    GL.Instance.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);
                    list.Act(new ActionParams(Viewport.GetCurrent())); // fixed
                    if (!nodbg) // nodbg will toggle this
                    {
                        string tasks = "Tasks: ";
                        foreach (object i in list)
                        {
                            tasks += $"{i.GetType().Name} ";
                        }
                        GL.Instance.DrawText(10, 58, Color.Red, "Verdana", 12, $"RAM Usage (no gc): {GC.GetTotalMemory(false)/1024}"); 
                        GL.Instance.DrawText(10, 22, Color.Red, "Verdana", 12, "List Size: " + list.Count);
                        GL.Instance.DrawText(10, 34, Color.Red, "Verdana", 12, tasks);
                        GL.Instance.DrawText(10, 10, Color.Red, "Verdana", 12, "FPS: " + winGLCanvas1.FPS.ToString() + " Time " + (DateTime.Now - time).Seconds);
                    }
                    if (StatusmessageEnabled)
                    {
                        GL.Instance.DrawText(10, 34, Color.White, "Verdana", 24, Statusmessage);
                    }
                }
            } catch(Exception ex)
            {
                if (throwFatalInsteadOfMsg)
                {
                    throw;
                }
                GameFatal gf = new GameFatal(ex);
                gf.Show();
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
            try
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
            } catch(Exception e)
            {
                if (throwFatalInsteadOfMsg)
                {
                    throw;
                }
                GameFatal gf = new GameFatal(e);
                gf.Show();
            }
        }

        /// <summary>
        /// This places all of the smaller classes in the root node
        /// </summary>
        /// <param name="node">The root node in question</param>
        private void CreateGameTree(GroupNode node)
        {
            try
            {
                node.Name = "Workspace"; // set the name so it will appear in the browser properly
                node.Initialize(); // initialise root node
                Image skbx = new Bitmap(800, 800, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                Nodes.SkyboxNode skybox = Nodes.SkyboxNode.Create(new Bitmap(skbx)); // create the skybox
                try
                {
                    skybox = Nodes.SkyboxNode.Create(new Bitmap(Image.FromFile("Content/skybox.png"))); // attempt loading the skybox "skybox"
                }
                catch (Exception e)
                {
                    skybox = Nodes.SkyboxNode.Create(Resources.NoSkyDefault); // otherwise load the noskydefault
                }
                skybox.Name = "Skybox";
                node.Children.Add(skybox); // add this to the rootnode
            } catch(Exception e)
            {
                if (throwFatalInsteadOfMsg)
                {
                    throw;
                }
                GameFatal gf = new GameFatal(e);
                gf.Show();
            }
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
                if (throwFatalInsteadOfMsg)
                {
                    throw;
                }
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
            if(!(scene is null))
            {
                scene.Camera.AspectRatio = ((float)winGLCanvas1.Width) / ((float)winGLCanvas1.Height);
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client = new Client(this);
            client.Connect("127.0.0.1", 3398);
        }

        private void hostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            server = new Server(3398, this);
            Statusmessage = "Server up.";
            StatusmessageEnabled = true;
            server.Start();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte[] gdata;

        }

        private byte[] SerializeData(SceneNodeBase node)
        {
            byte[] data = new byte[1024];
            foreach(SceneNodeBase child in node.Children)
            {
                SerializeData(child);
            }

            return data;
        }

        private string TransmorgifyCode(string line, bool latent = false)
        {
            if(latent)
            {
                return "using System;" +
                    "using dreary;" +
                    "using System.Windows.Forms;" +
                    "using dreary.Net;" +
                    "using dreary.Basplash;" +
                    "using dreary.Nodes;" +
                    "using CSharpGL;" +
                    "using System.Collections.Generic;" +
                    "using dreary.Script;" +
                    "namespace RuntimeCode {" +
                    "public static class Code {" +
                    line +
                    "}" +
                    "}";
            }
            return "using System;" +
                "using dreary;" +
                "using dreary.Net;" +
                "using dreary.Basplash;" + 
                "using dreary.Nodes;" +
                "using CSharpGL;" +
                "using System.Collections.Generic;" +
                "namespace RuntimeCode {" +
                "public static class Code {" +
                "public static void Entry(Scene scene, Camera cam) {" +
                line +
                "}" +
                "}" +
                "}";
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                CompilerParameters parameters = new CompilerParameters();
                parameters.GenerateInMemory = true;
                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("dal.dll");
                parameters.ReferencedAssemblies.Add("dgl.dll");
                parameters.ReferencedAssemblies.Add("dglw.dll");
                parameters.ReferencedAssemblies.Add("dreary.exe");
                Console.WriteLine("Code transmorgified: \n" + TransmorgifyCode(toolStripTextBox1.Text));
                CompilerResults results = provider.CompileAssemblyFromSource(parameters, TransmorgifyCode(toolStripTextBox1.Text));
                foreach (string i in results.Output)
                {
                    Console.WriteLine(i);
                }
                foreach (CompilerError i in results.Errors)
                {
                    Console.WriteLine("[ERROR in JIT Code] " + i.ErrorText);
                }
                results.CompiledAssembly.GetType("RuntimeCode.Code").GetMethod("Entry").Invoke(null, new object[] { scene, pcam });
                provider.Dispose();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString(), "Script Error");
            }
        }

        private void classToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void createOneExecuteScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Forms.OneExecute ox = new Forms.OneExecute();
                ox.ShowDialog();
                CSharpCodeProvider provider = new CSharpCodeProvider();
                CompilerParameters parameters = new CompilerParameters();
                parameters.GenerateInMemory = true;
                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
                parameters.ReferencedAssemblies.Add("dal.dll");
                parameters.ReferencedAssemblies.Add("dgl.dll");
                parameters.ReferencedAssemblies.Add("dglw.dll");
                parameters.ReferencedAssemblies.Add("dreary.exe");
                Console.WriteLine("Code transmorgified: \n" + TransmorgifyCode(ox.output, true));
                CompilerResults results = provider.CompileAssemblyFromSource(parameters, TransmorgifyCode(ox.output, true));
                foreach (string i in results.Output)
                {
                    Console.WriteLine(i);
                }
                foreach (CompilerError i in results.Errors)
                {
                    Console.WriteLine("[ERROR in JIT Code] " + i.ErrorText);
                }
                results.CompiledAssembly.GetType("RuntimeCode.Code").GetMethod("Entry").Invoke(null, new object[] { scene, pcam, this, 1.0 });
                provider.Dispose();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString(), "Script Error");
            }
        }
    }
}
