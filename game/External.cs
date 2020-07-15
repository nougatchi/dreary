using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSharpGL;

namespace game
{
    public class External
    {
        public void gInit(Camera pcam, DateTime time, SceneNodeBase rootElement, object client, object server, FirstPerspectiveManipulater camManip, ActionList actionlist, Scene scene, WinGLCanvas canvas)
        {
            var position = new vec3(5, 3, 4);
            var center = new vec3(0, 0, 0);
            var up = new vec3(0, 1, 0);
            pcam = new Camera(position, center, up, CameraType.Perspective, canvas.Width, canvas.Height); // create the camera
            rootElement = new GroupNode(); // this will be the rootnode of the scene
            //CreateGameTree(rootElement);
            scene = new Scene(pcam) // initialises the scene, use pcam as camera
            {
                RootNode = rootElement,
                ClearColor = Color.Black.ToVec4(),
            };
            actionlist = new ActionList();
            //treeView1.ExpandAll();
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
            camManip.Bind(pcam, canvas);
        }
    }
}
