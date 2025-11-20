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
uniform SpotLight;

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


//Functions//
void main()
{
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);

    vec3 result = CalcDirLight(norm, viewDir);
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