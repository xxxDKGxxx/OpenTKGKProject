#version 330 core

layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec3 gNormal;
layout (location = 2) out vec4 gColor;

in vec3 ourColor;
in vec3 normal;
in vec3 fragPos;

void main() {
    gPosition = FragPos;
    gNormal = normalize(Normal);
    gColor = ObjectColor;
}
