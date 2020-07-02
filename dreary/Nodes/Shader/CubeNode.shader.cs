using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dreary.Nodes
{
    partial class CubeNode
    {
        private const string vertexCode = @"
#version 150
// element in vertex buffer. Vertex' position in model space.
in vec3 inPosition;
uniform mat4 mvpMatrix;

void main() {
    // transform vertex' position from model space to clip space.
    gl_Position = mvpMatrix * vec4(inPosition, 1.0); 
}
";

        private const string fragmentCode = @"
#version 330
uniform vec4 color;
uniform int renderMode;
uniform vec3 lightDir;
uniform vec3 viewPos;
uniform vec4 lightColor;
in vec3 normal;
in vec3 inPosition;
out vec4 outColor;
void main() {
    switch(renderMode) {
        case 0:
            float zeintensity;
            zeintensity = dot(lightDir,normalize(normal));
            vec4 zediffuse = zeintensity * lightColor;
            outColor = zediffuse * color; // fill the fragment with specified color.
            break;
        case 1:
            outColor = vec4(normal.x,normal.y,normal.z,1);
            break; 
        case 2:
            outColor = vec4(inPosition.x,inPosition.y,inPosition.z,1);
            break; 
        case 3:
            outColor = color;
            break;
        case 4:
            float intensity;
            intensity = dot(lightDir,normalize(normal));
            vec4 diffuse = intensity * lightColor;
            outColor = diffuse * color; // fill the fragment with specified color.
            float specularStrength = 0.5;
            vec3 viewDir = normalize(viewPos - vec3(gl_FragCoord.x,gl_FragCoord.y,gl_FragCoord.z));
            vec3 reflectDir = reflect(-lightDir, normal);  
            float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
            vec4 specular = specularStrength * spec * lightColor;  
            vec4 result = (diffuse + specular);
            outColor = vec4(result.x,result.y,result.z,1) * color;
            break;
    }
}
";
        private const string geometryCode = @"
#version 330

layout(triangles) in;
layout(triangle_strip, max_vertices=3) out;

out vec3 normal;

void main( void )
{
    vec3 a = ( gl_in[1].gl_Position - gl_in[0].gl_Position ).xyz;
    vec3 b = ( gl_in[2].gl_Position - gl_in[0].gl_Position ).xyz;
    vec3 N = normalize( cross( b, a ) );

    for( int i=0; i<gl_in.length( ); ++i )
    {
        gl_Position = gl_in[i].gl_Position;
        normal = N;
        EmitVertex( );
    }

    EndPrimitive( );
}";
    }
}
