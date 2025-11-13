using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class Window : GameWindow
{
    private Shader shader;

    private int vertexBufferObject;
    private int vertexArrayObject;
    private int elementBufferObject;

    private double time;
    private Stopwatch timer;

    private Camera camera;

    private bool firstMove = true;

    private Vector2 lastPosition;

    //Вершины прямоугольника с текстурными координатами
    float[] vertices = {
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
};

    private int[] indices =
    {
        0, 1, 3,
        1, 2, 3
    };

    private Texture texture;
    private Texture texture1;

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

        GL.Enable(EnableCap.DepthTest);

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

        //Создание и запись буффера элементов
        elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
        

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

        

        //Получить максимальное количество аттрибутов вершин поддерживаемых видеокартой
        GL.GetInteger(GetPName.MaxVertexAttribs, out int maxAttributeCount);
        Console.WriteLine($"Maximum number of vertex attributes supported: {maxAttributeCount}");

        //Создание объекта шейдера
        shader = new("Shaders/default.vert", "Shaders/default.frag");
        //Использовать шейдерную программу
        shader.Use();

        //получить индекс переменной glsl, обозначить интерпритацию данных для opengl
        var vertexLocation = shader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        var textCoordLocation = shader.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray(textCoordLocation);
        GL.VertexAttribPointer(textCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

        //Загрузка текстур
        texture = Texture.LoadFromFile("Resources/container.jpg");
        texture.Use(TextureUnit.Texture0);
        texture1 = Texture.LoadFromFile("Resources/awesomeface.png");
        texture1.Use(TextureUnit.Texture1);

        shader.SetInt("texture0", 0);
        shader.SetInt("texture1", 1);

        camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

        //захват курсора
        CursorState = CursorState.Grabbed;
        
        timer = new();
        timer.Start();
    }

    /// <summary>
    /// Вызывается каждый кадр
    /// </summary>
    /// <param name="args"></param>
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        time += 16.0 * args.Time;

        //Заливает экран цветом из буффера настроенным из GL.ClearColor
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        texture.Use(TextureUnit.Texture0);
        texture1.Use(TextureUnit.Texture1);
        //Привязываем шейдер
        shader.Use();

        //позиции модели
        var model = Matrix4.Identity * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(time));
        //* Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(time));
        //* Matrix4.CreateRotationZ((float)MathHelper.DegreesToRadians(time));

        //Привязываем vao 
        GL.BindVertexArray(vertexArrayObject);

        shader.SetMatrix4("model", model);
        shader.SetMatrix4("view", camera.GetViewMatrix());
        shader.SetMatrix4("projection", camera.GetProjectionMatrix());

        //Рисуем элементы
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

        //Прошлый кадр фронт буффер новый в бек буффере, меняем их местами чтобы увидеть результат
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if(!IsFocused) return;

        var input = KeyboardState;

        if (input.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        const float cameraSpeed = 1.5f;
        const float sensitivity = .2f;

        if(input.IsKeyDown(Keys.W))
        {
            camera.Position += camera.Front * cameraSpeed * (float)args.Time;
        }

        if(input.IsKeyDown(Keys.S))
        {
            camera.Position -= camera.Front * cameraSpeed * (float)args.Time;
        }

        if(input.IsKeyDown(Keys.A))
        {
            camera.Position -= camera.Right * cameraSpeed * (float)args.Time;
        }

        if(input.IsKeyDown(Keys.D))
        {
            camera.Position += camera.Right * cameraSpeed * (float)args.Time;
        }

        if(input.IsKeyDown(Keys.Space))
        {
            camera.Position += camera.Up * cameraSpeed * (float)args.Time;
        }

        if(input.IsKeyDown(Keys.LeftShift))
        {
            camera.Position -= camera.Up * cameraSpeed * (float)args.Time;
        }

        var mouse = MouseState;

        if(firstMove)
        {
            lastPosition = new Vector2(mouse.X, mouse.Y);
            firstMove = false;
        }
        else
        {
            var deltaX = mouse.X - lastPosition.X;
            var deltaY = mouse.Y - lastPosition.Y;

            lastPosition = new(mouse.X, mouse.Y);

            camera.Yaw += deltaX * sensitivity;
            camera.Pitch -= deltaY * sensitivity;
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        camera.Fov -= e.OffsetY;
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        //Нужно вызывать для перерасчёта NDC 
        GL.Viewport(0, 0, Size.X, Size.Y);

        camera.AspectRatio = Size.X / (float)Size.Y;
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