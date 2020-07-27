using CSharpGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dreary.Nodes
{
    [Editor(typeof(PropertyGridEditor), typeof(UITypeEditor))]
    public partial class CubeNode : ModernNode, IRenderable
    {
        [Category("CubeNode")]
        [Description("The color of this instance.")]
        public vec4 Color { get; set; }
        [Category("CubeNode")]
        [Description("The direction of light hitting this instance.")]
        public vec3 LightDir { get; set; }
        [Category("CubeNode")]
        [Description("The color of light hitting this instance.")]
        public vec4 LightColor { get; set; }
        [Category("CubeNode")]
        [Description("The primitive render mode of this instance.")]
        public PrimitiveRenderMode renderMode { get; set; }
        public static CubeNode Create()
        {
            // model provides vertex buffer and index buffer(within an IDrawCommand).
            var model = new CubeModel();
            // vertex shader and fragment shader.
            var vs = new VertexShader(vertexCode);
            var fs = new FragmentShader(fragmentCode);
            var gs = new GeometryShader(geometryCode);
            var array = new ShaderArray(vs, fs, gs);
            // which vertex buffer maps to which attribute in shader.
            var map = new AttributeMap();
            map.Add("inPosition", CubeModel.strPosition);
            // help to build a render method.
            var builder = new RenderMethodBuilder(array, map);
            // create node.
            var node = new CubeNode(model, builder);
            // initialize node.
            node.Initialize();
            
            return node;
        }

        private CubeNode(IBufferSource model, params RenderMethodBuilder[] builders)
            : base(model, builders)
        {
            Color = new vec4(0.75f, 0.75f, 0.75f, 1f);
            LightDir = new vec3(0f, 0.5f, -0.5f);
            LightColor = new vec4(0.5f, 0.5f, 0.5f, 1f);
            renderMode = PrimitiveRenderMode.Diffuse;
        }

        // render this before render children. Call RenderBeforeChildren();
        // render children.
        // not Call RenderAfterChildren();
        private ThreeFlags enableRendering = ThreeFlags.BeforeChildren | ThreeFlags.Children;
        public ThreeFlags EnableRendering
        {
            get { return enableRendering; }
            set { enableRendering = value; }
        }

        public void RenderBeforeChildren(RenderEventArgs arg)
        {
            // gets mvpMatrix.
            ICamera camera = arg.Camera;
            mat4 projectionMat = camera.GetProjectionMatrix();
            mat4 viewMat = camera.GetViewMatrix();
            mat4 modelMat = this.GetModelMatrix();
            mat4 mvpMatrix = projectionMat * viewMat * modelMat;
            // a render uint wraps everything(model data, shaders, glswitches, etc.) for rendering.
            ModernRenderUnit unit = this.RenderUnit;
            // gets render method.
            // There could be more than 1 method(vertex shader + fragment shader) to render the same model data. Thus we need an method array.
            RenderMethod method = unit.Methods[0];
            // shader program wraps vertex shader and fragment shader.
            ShaderProgram program = method.Program;
            //set value for 'uniform mat4 mvpMatrix'; in shader.
            program.SetUniform("mvpMatrix", mvpMatrix);
            program.SetUniform("color", Color);
            program.SetUniform("lightDir", LightDir);
            program.SetUniform("lightColor", LightColor);
            program.SetUniform("renderMode", (int)renderMode);
            program.SetUniform("viewPos", arg.Camera.Position);
            // render the cube model via OpenGL.
            method.Render();
        }

        public void RenderAfterChildren(RenderEventArgs arg)
        {
        }
    }

    public enum PrimitiveRenderMode : int
    {
        Diffuse,
        Normal,
        Pos,
        Flat,
        Specular, // acts like a better version of diffuse for some reason. no phong just better diffuse
    }
}
