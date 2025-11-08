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
}