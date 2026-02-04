#version 330 core

layout (location = 0) out vec3 gNormal;
layout (location = 1) out vec3 gColor;

in vec3 ourColor;
in vec3 normal;
in vec3 fragPos;

void main() {
    gNormal = normalize(normal);
    gColor = ourColor;
}
