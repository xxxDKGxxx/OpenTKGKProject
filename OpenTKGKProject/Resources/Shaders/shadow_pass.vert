#version 330 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 color;
layout (location = 2) in vec3 aNormal;

uniform mat4 lightProjViewMatrix;
uniform mat4 model;

void main() {
    gl_Position = lightProjViewMatrix * model * vec4(position, 1.0);
}
