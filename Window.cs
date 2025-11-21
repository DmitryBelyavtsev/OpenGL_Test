using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;

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
            // Позиции          Нормали              Текстурные координаты
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
        };

        private readonly Vector3[] cubePositions =
        {
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(2.0f, 5.0f, -15.0f),
            new Vector3(-1.5f, -2.2f, -2.5f),
            new Vector3(-3.8f, -2.0f, -12.3f),
            new Vector3(2.4f, -0.4f, -3.5f),
            new Vector3(-1.7f, 3.0f, -7.5f),
            new Vector3(1.3f, -2.0f, -2.5f),
            new Vector3(1.5f, 2.0f, -2.5f),
            new Vector3(1.5f, 0.2f, -1.5f),
            new Vector3(-1.3f, 1.0f, -1.5f)
        };

        private readonly Vector3[] pointLightPositions =
        {
            new Vector3(0.7f, 0.2f, 2.0f),
            new Vector3(2.3f, -3.3f, -4.0f),
            new Vector3(-4.0f, 2.0f, -12.0f),
            new Vector3(0.0f, 0.0f, -3.0f)
        };

    private Texture diffuseMap;
    private Texture specularMap;

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
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            var normalLocation = lightingShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            var texCoordLocation = lightingShader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        }

        {
            vaoLamp = GL.GenVertexArray();
            GL.BindVertexArray(vaoLamp);

            var positionLocation = lampShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
        }

        diffuseMap = Texture.LoadFromFile("Resources/container2.png");
        specularMap = Texture.LoadFromFile("Resources/container2_specular.png");

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

        diffuseMap.Use(TextureUnit.Texture0);
        specularMap.Use(TextureUnit.Texture1);
        //Привязываем шейдер
        lightingShader.Use();

        //lightingShader.SetMatrix4("model", Matrix4.Identity);
        lightingShader.SetMatrix4("view", camera.GetViewMatrix());
        lightingShader.SetMatrix4("projection", camera.GetProjectionMatrix());

        lightingShader.SetVector3("viewPos", camera.Position);

        lightingShader.SetInt("material.diffuse", 0);
        lightingShader.SetInt("material.specular", 1);
        lightingShader.SetFloat("material.shininess", 32f);
        
        lightingShader.SetVector3("directionLight.direction", new Vector3(-.2f,-1f,-.3f));
        lightingShader.SetVector3("directionLight.ambient", new Vector3(.05f, .05f, .05f));
        lightingShader.SetVector3("directionLight.diffuse", new Vector3(.4f, .4f, .4f));
        lightingShader.SetVector3("directionLight.specular", new Vector3(.5f, .5f, .5f));

        for(int i = 0; i < pointLightPositions.Length; i++)
        {
            lightingShader.SetVector3($"pointLights[{i}].position", pointLightPositions[i]);
            lightingShader.SetVector3($"pointLights[{i}].ambient", new Vector3(.05f, .05f, .05f));
            lightingShader.SetVector3($"pointLights[{i}].diffuse", new Vector3(.8f, .8f, .8f));
            lightingShader.SetVector3($"pointLights[{i}].specular", new Vector3(1f, 1f, 1f));
            lightingShader.SetFloat($"pointLights[{i}].constant", 1f);
            lightingShader.SetFloat($"pointLights[{i}].linear", .09f);
            lightingShader.SetFloat($"pointLights[{i}].quadratic", .032f);
        }


        //Рисуем элементы
        for(int i = 0; i < cubePositions.Length; i++)
        {
            var model = Matrix4.Identity;
            model *= Matrix4.CreateTranslation(cubePositions[i]);
            float angle = 20f * i;
            model *= Matrix4.CreateFromAxisAngle(new Vector3(1f, .3f, .5f), angle);
            lightingShader.SetMatrix4("model", model);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
        

        GL.BindVertexArray(vaoLamp);

        lampShader.Use();

        lampShader.SetMatrix4("view", camera.GetViewMatrix());
        lampShader.SetMatrix4("projection", camera.GetProjectionMatrix());

        for(int i = 0; i < pointLightPositions.Length; i++)
        {
            var lampMatrix = Matrix4.CreateScale(.2f);
            lampMatrix *= Matrix4.CreateTranslation(pointLightPositions[i]);

            lampShader.SetMatrix4("model", lampMatrix);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

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