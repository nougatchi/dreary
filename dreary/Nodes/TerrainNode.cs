using CSharpGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dreary.Nodes
{
    [Editor(typeof(PropertyGridEditor), typeof(UITypeEditor))]
    public partial class TerrainNode : ModernNode, IRenderable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Category("TerrainNode")]
        [Description("The color of this instance.")]
        public vec4 Color { get; set; }
        [Category("TerrainNode")]
        [Description("The direction of light hitting this instance.")]
        public vec3 LightDir { get; set; }
        [Category("TerrainNode")]
        [Description("The color of light hitting this instance.")]
        public vec4 LightColor { get; set; }
        [Category("TerrainNode")]
        [Description("The primitive render mode of this instance.")]
        public PrimitiveRenderMode renderMode { get; set; }
        public static TerrainNode Create()
        {
            var model = new TerrainModel();
            RenderMethodBuilder defaultBuilder;
            {
                var vs = new VertexShader(vert);
                var fs = new FragmentShader(frag);
                var provider = new ShaderArray(vs, fs);
                var map = new AttributeMap();
                defaultBuilder = new RenderMethodBuilder(provider, map, new PolygonModeSwitch(PolygonMode.Line));
            }

            var node = new TerrainNode(model, defaultBuilder);
            node.Initialize();

            return node;
        }

        private TerrainNode(IBufferSource model, params RenderMethodBuilder[] builders)
            : base(model, builders)
        {
        }

        protected override void DoInitialize()
        {
            base.DoInitialize();

            RenderMethod method = this.RenderUnit.Methods[0];
            ShaderProgram program = method.Program;
            program.SetUniform("terrainSize", new ivec2(TerrainModel.TERRAIN_WIDTH, TerrainModel.TERRAIN_DEPTH));
            program.SetUniform("scale", (TerrainModel.TERRAIN_WIDTH + TerrainModel.TERRAIN_DEPTH) * 0.08f);
            
            var image = new Bitmap("Content/heightmap-test.png");
            this.UpdateHeightmap(image);
        }

        private ThreeFlags enableRendering = ThreeFlags.BeforeChildren | ThreeFlags.Children | ThreeFlags.AfterChildren;
        /// <summary>
        /// Render before/after children? Render children? 
        /// RenderAction cares about this property. Other actions, maybe, maybe not, your choice.
        /// </summary>
        public ThreeFlags EnableRendering
        {
            get { return this.enableRendering; }
            set { this.enableRendering = value; }
        }

        public void RenderBeforeChildren(RenderEventArgs arg)
        {
            ICamera camera = arg.Camera;
            mat4 projection = camera.GetProjectionMatrix();
            mat4 view = camera.GetViewMatrix();
            mat4 model = this.GetModelMatrix();
            ModernRenderUnit unit = this.RenderUnit;
            RenderMethod method = this.RenderUnit.Methods[0];
            ShaderProgram program = method.Program;
            program.SetUniform("mvpMat", projection * view * model);
            program.SetUniform("color", Color);
            program.SetUniform("renderMode", (int)renderMode);
            method.Render();
        }

        public void RenderAfterChildren(RenderEventArgs arg)
        {
        }

        private Texture heightTexture;

        /// <summary>
        /// Load a user defined heightmap
        /// </summary>
        /// <param name="image"></param>
        public void UpdateHeightmap(Bitmap image)
        {
            if (this.heightTexture != null)
            {
                this.heightTexture.Dispose();
            }

            var storage = new TexImageBitmap(image, GL.GL_RED);
            var heightMapTexture = new Texture(storage,
                //new TexParameteri(TexParameter.PropertyName.TextureWrapR, (int)GL.GL_CLAMP),
                new TexParameteri(TexParameter.PropertyName.TextureWrapS, (int)GL.GL_CLAMP),
                new TexParameteri(TexParameter.PropertyName.TextureWrapT, (int)GL.GL_CLAMP),
                new TexParameteri(TexParameter.PropertyName.TextureMinFilter, (int)GL.GL_NEAREST),
                new TexParameteri(TexParameter.PropertyName.TextureMagFilter, (int)GL.GL_NEAREST)
                );
            heightMapTexture.TextureUnitIndex = 0;
            heightMapTexture.Initialize();

            RenderMethod method = this.RenderUnit.Methods[0];
            ShaderProgram program = method.Program;
            program.SetUniform("heightMapTexture", heightMapTexture);

            this.heightTexture = heightMapTexture;
        }
    }
}
