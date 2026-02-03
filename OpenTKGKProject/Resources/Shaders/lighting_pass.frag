#version 330 core
out vec4 FragColor;

uniform int renderMode; // 0 - full, 1 - position, 2 - normals, 3 - color
uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gColor;

in vec2 TexCoords;

void main()
{
    if (renderMode == 1) {
        FragColor = texture(gPosition, TexCoords);
        return;
    }
    
    if (renderMode == 2) {
        FragColor = texture(gNormal, TexCoords);
        return;
    }
    
    if (renderMode == 3) {
        FragColor = texture(gColor, TexCoords);
        return;
    }
    
    FragColor = vec4(1.0);
    
//    float ambientStrength = 0.1;
//    vec3 ambient = ambientStrength * lightColor;
//    
//    vec3 norm = normalize(normal);
//    vec3 lightDir = normalize(lightPos - fragPos);
//    
//    float diff = max(dot(norm, lightDir), 0.0);
//    vec3 diffuse = diff * lightColor;
//    
//    float specularStrength = 0.5;
//    vec3 viewDir = normalize(viewPos - fragPos);
//    vec3 reflectDir = reflect(-lightDir, norm);
//    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
//    vec3 specular = specularStrength * spec * lightColor;
//    
//    vec3 result = (ambient + diffuse + specular) * ourColor;
//    FragColor = vec4(result, 1.0);
}