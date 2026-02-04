#version 330 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 color;
layout (location = 2) in vec3 aNormal;

out vec3 ourColor;
out vec3 normal;
out vec3 fragPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main() {
    gl_Position = projection * view * model * vec4(position, 1.0);
    ourColor = color;
    normal = mat3(transpose(inverse(model))) * aNormal;
    fragPos = vec3(model * vec4(position, 1.0));
}
