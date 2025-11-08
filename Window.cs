using OpenTK.Graphics.ES11;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

public class Window : GameWindow
{
    private Shader shader;

    private int vertexBufferObject;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
    {

    }

    /// <summary>
    /// Действия при загрузки окна
    /// </summary>
    protected override void OnLoad()
    {
        base.OnLoad();

        //Создание объекта шейдера
        shader = new("Shaders/default.vert", "Shaders/default.frag");

        //Цвет окна после очистки нужен для вызова GL.Clear в OnRenderFrame
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1f);


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

        //Прошлый кадр фронт буффер новый в бек буффере, меняем их местами чтобы увидеть результат
        SwapBuffers();
    }
}