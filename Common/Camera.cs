

using OpenTK.Mathematics;

public class Camera
{
    private Vector3 front = -Vector3.UnitZ;

    private Vector3 up = Vector3.UnitY;

    private Vector3 right = Vector3.UnitX;

    //Вращение вокруг X (радианы)
    private float pitch; 
    
    //Вращение вокруг Y (радианы)
    private float yaw = - MathHelper.PiOver2; //без этого камеры была бы повернута на 90 градусов вправо

    //Угол обзора
    private float fov = MathHelper.PiOver2;

    //Позиция камеры
    public Vector3 Position { get; set; }

    public float AspectRatio { private get; set; }

    public Vector3 Front => front;

    public Vector3 Up => up;

    public Vector3 Right => right;

    public float Pitch
    {
        get => MathHelper.RadiansToDegrees(pitch);
        set
        {
            var angle = MathHelper.Clamp(value, -89f, 89f);
            pitch = MathHelper.DegreesToRadians(angle);
            UpdateVectors();
        }
    }

    public float Yaw
    {
        get => MathHelper.RadiansToDegrees(yaw);
        set
        {
            yaw = MathHelper.DegreesToRadians(value);
            UpdateVectors();
        }
    }

    public float Fov
    {
        get => MathHelper.RadiansToDegrees(fov);
        set
        {
            var angle = MathHelper.Clamp(value, 1f, 90f);
            fov = MathHelper.DegreesToRadians(angle);
        }
    }

    public Camera(Vector3 position, float aspectRation)
    {
        Position = position;
        AspectRatio = aspectRation;
    }

    public Matrix4 GetViewMatrix() => Matrix4.LookAt(Position, Position + front, up);

    public Matrix4 GetProjectionMatrix() => Matrix4.CreatePerspectiveFieldOfView(fov, AspectRatio, 0.01f, 100f);

    private void UpdateVectors()
    {
        front.X = MathF.Cos(pitch) * MathF.Cos(yaw);
        front.Y = MathF.Sin(pitch);
        front.Z = MathF.Cos(pitch) * MathF.Sin(yaw);

        front = Vector3.Normalize(front);

        right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
        up = Vector3.Normalize(Vector3.Cross(right, front));
    }

}
