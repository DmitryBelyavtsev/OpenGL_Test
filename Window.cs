using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class Window : GameWindow
{
    private Shader shader;

    private int vertexBufferObject;
    private int vertexArrayObject;

    //Вершины треугольника
    private float[] vertices =
    {
        -.5f, -.5f, 0f,
        .5f, -.5f, 0f,
        0f, .5f, 0f
    };

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
    {

    }

    /// <summary>
    /// Действия при загрузки окна
    /// </summary>
    protected override void OnLoad()
    {
        base.OnLoad();

        //Цвет окна после очистки нужен для вызова GL.Clear в OnRenderFrame
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1f);

        //Создание буффера объектов
        vertexBufferObject = GL.GenBuffer();

        // привязка в vao буффер объекта буффера вершин, vbo это ArrayBuffer, скачать что за буффер
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

        // Запись данных о вершинах треугольника в vbo
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        // Особенность данного буффера в том что драйвер opengl, 
        // не знает как интерпретировать данные,
        // поэтому нужно через объект VAO скачать как их интерпретировать.

        //Создание VAO
        vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayObject);

        // Эта функция имеет две задачи: рассказать OpenGL о формате данных, а также связать текущий array buffer с VAO.
        // Это означает, что после этого вызова мы настроили этот атрибут для получения данных из текущего array buffer и интерпретации их указанным способом.
        // Аргументы:
        //   Location входной переменной в шейдере. Строка layout(location = 0) в вершинном шейдере явно устанавливает его в 0.
        //   Сколько элементов будет отправлено в переменную. В этом случае 3 float'а на каждую вершину.
        //   Тип данных элементов, в этом случае float.
        //   Должны ли данные быть преобразованы в нормализованные координаты устройства. В этом случае false, потому что это уже сделано.
        //   Stride; это сколько байтов между последним элементом одной вершины и первым элементом следующей. 3 * sizeof(float) в этом случае.
        //   Offset; это сколько байтов нужно пропустить, чтобы найти первый элемент первой вершины. 0 на данный момент.
        // Stride и Offset пока просто упомянуты, но когда мы дойдем до текстурных координат, они будут показаны более подробно.

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

        //Включение переменной в шейдере
        GL.EnableVertexAttribArray(0);

        //Создание объекта шейдера
        shader = new("Shaders/default.vert", "Shaders/default.frag");
        
        //Использовать шейдерную программу
        shader.Use();
    }

    /// <summary>
    /// Вызывается каждый кадр
    /// </summary>
    /// <param name="args"></param>
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        //Заливает экран цветом из буффера настроенным из GL.ClearColor
        GL.Clear(ClearBufferMask.ColorBufferBit);

        //Привязываем шейдер
        shader.Use();

        //Привязываем vao 
        GL.BindVertexArray(vertexArrayObject);

        //Рисуем три вершины треугольника
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

        //Прошлый кадр фронт буффер новый в бек буффере, меняем их местами чтобы увидеть результат
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        var input = KeyboardState;

        if (input.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }

    protected override void OnUnload()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);

        GL.DeleteBuffer(vertexBufferObject);
        GL.DeleteVertexArray(vertexArrayObject);

        GL.DeleteProgram(shader.Handle);

        base.OnUnload();
    }
}