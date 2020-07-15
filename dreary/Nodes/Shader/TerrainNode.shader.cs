using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dreary.Nodes
{
    public partial class TerrainNode
    {
        private const string vert = @"#version 330 core
  
//uniforms
uniform mat4 mvpMat;					//combined modelview projection matrix
uniform ivec2 terrainSize;	//half terrain size
uniform sampler2D heightMapTexture;	//heightmap texture
uniform float scale;				//scale for the heightmap height
out vec4 position; // for frag
void main()
{   
    float u = float(gl_VertexID % terrainSize.x) / float(terrainSize.x - 1);
    float v = float(gl_VertexID / terrainSize.x) / float(terrainSize.y - 1);
	float height = (texture(heightMapTexture, vec2(u, v)).r - 0.5) * scale;
    float x = (u - 0.5) * terrainSize.x;
    float z = (v - 0.5) * terrainSize.y;
	gl_Position = mvpMat*vec4(x, height, z, 1);		
	position = mvpMat*vec4(x, height, z, 1);			
}
";

        private const string frag = @"#version 330 core
 
layout (location=0) out vec4 outColor;
uniform vec4 color;
uniform int renderMode;
in vec4 position;
void main()
{
    switch(renderMode) {
        case 0:
            
            break;
        case 2:
            outColor = color - vec4(position.y,position.y,position.y,0);
            break;
        case 3:
            outColor = color;
            break;
    }
}
";
    }
}
