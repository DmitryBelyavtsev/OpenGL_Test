#version 330 core

//Defines
#define NR_POINT_LIGHTS 4

//Classes//
struct Material 
{
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
};

struct DirectionLight
{
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct PointLight
{
    vec3 position;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct SpotLight
{
    vec3 position;
    vec3 direction;
    float cutOff;
    float outerCutOff;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;
};

//Uniforms//
uniform DirectionLight directionLight;
uniform PointLight pointLights[NR_POINT_LIGHTS];
uniform SpotLight spotLight;

uniform Material material;

uniform vec3 viewPos;

//IN//
in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;

//OUT//
out vec4 FragColor;


//Prototypes//
vec3 CalcDirLight(vec3 normal, vec3 viewDir);
vec3 CalcPointLight(int i, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalcSpotLight(vec3 normal, vec3 fragPos, vec3 viewDir);
//Functions//
void main()
{
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);

    vec3 result = CalcDirLight(norm, viewDir);
    
    for(int i = 0; i < NR_POINT_LIGHTS; i++)
    {
        result += CalcPointLight(i, norm, FragPos, viewDir);
    }

    result += CalcSpotLight(norm, FragPos, viewDir);
    
    FragColor = vec4(result, 1.0);
}

vec3 CalcDirLight(vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-directionLight.direction);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

    vec3 ambient = directionLight.ambient * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse = directionLight.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = directionLight.specular * spec * vec3(texture(material.specular, TexCoords));
    
    return ambient + diffuse + specular;
}

vec3 CalcPointLight(int i, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(pointLights[i].position - fragPos);

    float diff = max(dot(normal, lightDir), 0.0);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

    float distance = length(pointLights[i].position - fragPos);
    float attenuation = 1.0 / (pointLights[i].constant + 
    pointLights[i].linear * distance + 
    pointLights[i].quadratic * (distance * distance));

    vec3 ambient = pointLights[i].ambient * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse = pointLights[i].diffuse * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = pointLights[i].specular * spec * vec3(texture(material.specular, TexCoords));
    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;

    //return vec3(0.3, 0.0, 0.0);
    return ambient + diffuse + specular;
}

vec3 CalcSpotLight(vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(spotLight.position - fragPos);
    float diff = max(dot(normal, lightDir), 0.0);

    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

    float distance = length(spotLight.position - fragPos);
    float attenuation = 1.0 / (spotLight.constant + spotLight.linear * distance +
    spotLight.quadratic * (distance * distance));

    float theta = dot(lightDir, normalize(-spotLight.direction));
    float epsilon = spotLight.cutOff - spotLight.outerCutOff;
    float intensity = clamp((theta - spotLight.outerCutOff) / epsilon, 0.0, 1.0);

    vec3 ambient = spotLight.ambient * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse = spotLight.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = spotLight.specular * spec * vec3(texture(material.specular, TexCoords));
    ambient *= attenuation;
    diffuse *= attenuation * intensity;
    specular *= attenuation * intensity;

    return ambient + diffuse + specular;
}