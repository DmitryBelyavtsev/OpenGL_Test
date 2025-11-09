using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
public class Shader
{
    public readonly int Handle;

    private readonly Dictionary<string, int> uniformLocations;
    public Shader(string vertPath, string fragPath)
    {
        // Чтение исходного кода шейдеров
        string vertShaderSource = File.ReadAllText(vertPath);
        string fragShaderSource = File.ReadAllText(fragPath);

        // Создание объекта шейдера и передача дескриптора на него
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        int fragShader = GL.CreateShader(ShaderType.FragmentShader);

        //связывание объекта шейдера и исходного кода через дескриптор
        GL.ShaderSource(vertexShader, vertShaderSource);
        GL.ShaderSource(fragShader, fragShaderSource);

        //Компиляция шейдера через приватный метод класса
        CompileShader(vertexShader);
        CompileShader(fragShader);

        //Создание программы шейдеров
        Handle = GL.CreateProgram();

        //Добавление шейдера в программу шейдера
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragShader);

        //Линковка программы, соединяет шейдеры и программу, компилирует их
        LinkProgram(Handle);

        //Программа скомпилирована, шейдеры больше не нужны
        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragShader);

        //Полное удаление шейдеров из памяти (они внутри программы на этапе linkProgram)
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragShader);


        GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

        uniformLocations = new();

        for(var i = 0; i < numberOfUniforms; i ++)
        {
            var key = GL.GetActiveUniform(Handle, i, out _, out _);

            var location = GL.GetUniformLocation(Handle, key);

            uniformLocations.Add(key, location);
        }
    }

    /// <summary>
    /// Компилирует шейдер принимая дескриптор на объект шейдера
    /// </summary>
    /// <param name="Shader"></param>
    private static void CompileShader(int shader)
    {
        GL.CompileShader(shader);

        //Получает доступ к полю объекта шейдера CompileStatus выводит результат в result
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int result);

        //Результат компиляции int, проверка на истину
        if (result != (int)All.True)
        {
            //Получение лога шейдера
            var infoLog = GL.GetShaderInfoLog(shader);

            //Выдача ошибки в случае неудачи
            throw new Exception($"Error occurred whilst copiling Shader({shader}).\n\n{infoLog}");
        }
    }

    private static void LinkProgram(int program)
    {
        GL.LinkProgram(program);

        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int result);
        if (result != (int)All.True)
        {
            GL.GetProgramInfoLog(program);

            throw new Exception($"Error occurred whilst linking Program({program})");
        }
    }

    public void Use()
    {
        GL.UseProgram(Handle);
    }

    public int GetAttribLocation(string str)
    {
        return GL.GetAttribLocation(Handle, str);
    }

    public void SetInt(string name, int value)
    {
        int location = GL.GetUniformLocation(Handle, name);

        GL.Uniform1(location, value);
    }

    public void SetMatrix4(string name, Matrix4 data)
    {
        GL.UseProgram(Handle);

        GL.UniformMatrix4(uniformLocations[name],true, ref data);
    }
}