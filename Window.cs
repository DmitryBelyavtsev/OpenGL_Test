using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System.Diagnostics;

public class Window : GameWindow
{
    private Shader lampShader;
    private Shader lightingShader;

    private int vertexBufferObject;
    private int vaoModel;
    private int vaoLamp;
    private int elementBufferObject;

    private double time;

    private Camera camera;

    private bool firstMove = true;

    private Vector2 lastPosition;

    private readonly float[] vertices =
        {
             // Позиция          Нормаль
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, // Передняя грань
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, // Задняя грань
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, // Левая грань
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f, // Правая грань
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, // Нижняя грань
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, // Верхняя грань
             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
        };

    private Texture texture;
    private Texture texture1;

    private readonly Vector3 lightPosition = new Vector3(1.2f, 1f, 2f);

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
        GL.ClearColor(0f, 0f, 0f, 1f);

        GL.Enable(EnableCap.DepthTest);

        //Создание буффера объектов
        vertexBufferObject = GL.GenBuffer();

        // привязка в vao буффер объекта буффера вершин, vbo это ArrayBuffer, скачать что за буффер
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);

        // Запись данных о вершинах треугольника в vbo
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        //Создание объекта шейдера
        lightingShader = new("Shaders/default.vert", "Shaders/lighting.frag");
        lampShader = new("Shaders/default.vert", "Shaders/default.frag");

        //Создание VAO
        {
            vaoModel = GL.GenVertexArray();
            GL.BindVertexArray(vaoModel);

            var positionLocation = lightingShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            var normalLocation = lightingShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        }

        {
            vaoLamp = GL.GenVertexArray();
            GL.BindVertexArray(vaoLamp);

            var positionLocation = lampShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        }

        camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

        //захват курсора
        CursorState = CursorState.Grabbed;
        
    }

    /// <summary>
    /// Вызывается каждый кадр
    /// </summary>
    /// <param name="args"></param>
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        //Заливает экран цветом из буффера настроенным из GL.ClearColor
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.BindVertexArray(vaoModel);

        //Привязываем шейдер
        lightingShader.Use();

        lightingShader.SetMatrix4("model", Matrix4.Identity);
        lightingShader.SetMatrix4("view", camera.GetViewMatrix());
        lightingShader.SetMatrix4("projection", camera.GetProjectionMatrix());

        lightingShader.SetVector3("viewPos", camera.Position);

        lightingShader.SetVector3("material.ambient", new Vector3(1f, .5f, .31f));
        lightingShader.SetVector3("material.diffuse", new Vector3(1f, .5f, .31f));
        lightingShader.SetVector3("material.specular", new Vector3(.5f, .5f, .5f));
        lightingShader.SetFloat("material.shininess", 32f);

        Vector3 lightColor;
        float time = DateTime.Now.Second + DateTime.Now.Millisecond / 1000f;
        lightColor.X = (float)Math.Sin(time * 2.0f);
        lightColor.Y = (float)Math.Sin(time * 0.7f);
        lightColor.Z = (float)Math.Sin(time * 1.3f);

        Vector3 ambientColor = lightColor * new Vector3(.2f);
        Vector3 diffuseColor = lightColor * new Vector3(.5f);

        lightingShader.SetVector3("light.position", lightPosition);
        lightingShader.SetVector3("light.ambient", ambientColor);
        lightingShader.SetVector3("light.diffuse", diffuseColor);
        lightingShader.SetVector3("light.specular", new Vector3(1f, 1f, 1f));

        //Рисуем элементы
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

        GL.BindVertexArray(vaoLamp);

        lampShader.Use();

        var lampMatrix = Matrix4.CreateScale(.2f);

        lampMatrix *= Matrix4.CreateTranslation(lightPosition);

        lampShader.SetMatrix4("model", lampMatrix);
        lampShader.SetMatrix4("view", camera.GetViewMatrix());
        lampShader.SetMatrix4("projection", camera.GetProjectionMatrix());

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

        if (input.IsKeyDown(Keys.W))
        {
            camera.Position += camera.Front * cameraSpeed * (float)args.Time;
        }
        
        if(input.IsKeyDown(Keys.LeftShift) & input.IsKeyDown(Keys.W))
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

        if(input.IsKeyDown(Keys.LeftControl))
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
        GL.DeleteVertexArray(vaoLamp);
        GL.DeleteVertexArray(vaoModel);

        GL.DeleteProgram(lampShader.Handle);

        base.OnUnload();
    }
}