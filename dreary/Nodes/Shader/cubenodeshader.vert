#version 150
// element in vertex buffer. Vertex' position in model space.
in vec3 inPosition;
uniform mat4 mvpMatrix;

void main() {
    // transform vertex' position from model space to clip space.
    gl_Position = mvpMatrix * vec4(inPosition, 1.0); 
}