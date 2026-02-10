#version 330 core
out vec4 FragColor;

struct Light {
    int type; // 0 point, 1 spotlight, 2 directional
    vec3 position;
    vec3 direction;
    vec3 color;

    mat4 lightSpaceMatrix;
    int shadowMapLayerIndex;
    
    // attenuation
    float constant;
    float linear;
    float quadratic;
    
    // reflector
    float cutOff;
    float outerCutOff;
};

uniform int renderMode; // 0 - full, 1 - depth, 2 - normals, 3 - color

uniform sampler2D gDepth;
uniform sampler2D gNormal;
uniform sampler2D gColor;
uniform sampler2DArray gShadow;

const int MAX_LIGHTS = 32;

uniform Light[MAX_LIGHTS] lights;
uniform int lightCount;

uniform vec3 fogColor;
uniform float fogStart;
uniform float fogEnd;

uniform mat4 invView;
uniform mat4 invProj;
uniform vec3 viewPos;

uniform int isPerspective;
uniform float near;
uniform float far;

in vec2 TexCoords;

vec3 CalculateDiffuseAndSpecular(Light light, vec3 normal, vec3 lightDir, vec3 viewPos, vec3 fragPos, vec3 objColor);
vec3 CalcPointLight(Light light, vec3 normal, vec3 fragPos, vec3 viewPos, vec3 objColor);
vec3 CalcSpotlight(Light light, vec3 normal, vec3 fragPos, vec3 viewPos, vec3 objColor);
vec3 CalcDirLight(Light light, vec3 normal, vec3 fragPos, vec3 viewPos, vec3 objColor);
float CalcShadow(vec4 fragPosLightSpace, vec3 normal, vec3 lightDir, int layerIndex);
float LinearizeDepth(float depth);

void main()
{
    if (renderMode == 1) { // depth
        float depth = texture(gDepth, TexCoords).r;
        float visualization = isPerspective == 1 ? LinearizeDepth(depth) / far : depth;
        vec3 visualization_vector = vec3(visualization);
        FragColor = vec4(visualization_vector, 1.0);
        return;
    }
    
    if (renderMode == 2) { // normals
        FragColor = (texture(gNormal, TexCoords) + 1) / 2;
        return;
    }
    
    if (renderMode == 3) { // color
        FragColor = texture(gColor, TexCoords);
        return;
    }
    
    vec3 normal = texture(gNormal, TexCoords).xyz;
    float fragDepth = texture(gDepth, TexCoords).r;
    vec4 fragPos = invProj * vec4(TexCoords.x * 2.0 - 1.0, TexCoords.y * 2.0 - 1.0,  fragDepth * 2.0 - 1.0, 1.0);
    
    fragPos /= fragPos.w;
    fragPos = invView * fragPos;
    fragPos /= fragPos.w;
    
    vec3 objColor = texture(gColor, TexCoords).xyz;
    vec3 fragColor = vec3(0.05) * objColor; // ambient
    
    for (int i = 0; i < lightCount; i++)
    {
        if (lights[i].type == 0) {
            fragColor += CalcPointLight(lights[i], normal, fragPos.xyz, viewPos, objColor);
        } // point light
        
        if (lights[i].type == 1) {
            fragColor += CalcSpotlight(lights[i], normal, fragPos.xyz, viewPos, objColor);
        } // spotlight
        
        if (lights[i].type == 2)
        {
            fragColor += CalcDirLight(lights[i], normal, fragPos.xyz, viewPos, objColor);
        } // directional
    }
    
    // fog
    float dist = length(viewPos - fragPos.xyz);
    float fogFactor = (fogEnd - dist) / (fogEnd - fogStart);
    
    fogFactor = clamp(fogFactor, 0.0, 1.0);
    
    vec3 finalColor = mix(fogColor, fragColor, fogFactor);
    
    FragColor = vec4(finalColor, 1.0);
}

float CalcShadow(vec4 fragPosLightSpace, vec3 normal, vec3 lightDir, int layerIndex)
{
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    
    projCoords = projCoords * 0.5 + 0.5;
    
    if(projCoords.z > 1.0)
        return 0.0;
    
    float bias = max(0.005 * (1.0 - dot(normal, lightDir)), 0.001);
    
    float shadow = 0.0;
    vec2 texelSize = 1.0 / textureSize(gShadow, 0).xy;

    for(int x = -2; x <= 2; ++x)
    {
        for(int y = -2; y <= 2; ++y)
        {
            float pcfDepth = texture(gShadow, vec3(projCoords.xy + vec2(x, y) * texelSize, layerIndex)).r;
            shadow += (projCoords.z - bias > pcfDepth ? 1.0 : 0.0);
        }
    }
    
    shadow /= 25.0;

    return shadow;
}

float LinearizeDepth(float depth)
{
    float z = depth * 2.0 - 1.0;
    return (2.0 * near * far) / (far + near - z * (far - near));
}

vec3 CalcDirLight(Light light, vec3 normal, vec3 fragPos, vec3 viewPos, vec3 objColor)
{
    vec3 lightDir = normalize(-light.direction);
    
    return CalculateDiffuseAndSpecular(light, normal, lightDir, viewPos, fragPos, objColor);
}

vec3 CalcSpotlight(Light light, vec3 normal, vec3 fragPos, vec3 viewPos, vec3 objColor)
{
    vec3 lightDir = normalize(light.position - fragPos);
    
    float theta = dot(lightDir, normalize(-light.direction));
    float epsilon = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
    
    return CalcPointLight(light, normal, fragPos, viewPos, objColor) * intensity;
}

vec3 CalcPointLight(Light light, vec3 normal, vec3 fragPos, vec3 viewPos, vec3 objColor)
{
    // attenuation
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
    
    vec3 lightDir = normalize(light.position - fragPos);
    vec3 result = CalculateDiffuseAndSpecular(light, normal, lightDir, viewPos, fragPos, objColor) * attenuation;

    return result;
}

vec3 CalculateDiffuseAndSpecular(Light light, vec3 normal, vec3 lightDir, vec3 viewPos, vec3 fragPos, vec3 objColor)
{
    vec3 norm = normalize(normal);
    lightDir = normalize(lightDir);
    
    // diffuse
    float diffStrength = 1;
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diffStrength * diff * light.color;
    
    // specular
    float specularStrength = 0.5;
    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = specularStrength * spec * light.color;
    
    float shadow = 0.0;

    if(light.shadowMapLayerIndex >= 0 && diff > 0.0)
    {
        vec4 fragPosLightSpace = light.lightSpaceMatrix * vec4(fragPos, 1.0);
        shadow = CalcShadow(fragPosLightSpace, normal, lightDir, light.shadowMapLayerIndex);
    }
    
    return (1.0 - shadow) * (diffuse * objColor + specular);
}