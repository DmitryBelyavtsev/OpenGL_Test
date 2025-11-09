using OpenTK.Graphics.OpenGL;
public class Shader
{
    public readonly int Handle;

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
}