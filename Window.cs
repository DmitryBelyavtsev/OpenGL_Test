using OpenTK.Windowing.Desktop;

public class Window : GameWindow
{
    private Shader shader;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
    {

    }

    protected override void OnLoad()
    {
        base.OnLoad();

        shader = new("Shaders/default.vert", "Shaders/default.frag");
    }
}