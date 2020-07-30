#version 330 core
 
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
