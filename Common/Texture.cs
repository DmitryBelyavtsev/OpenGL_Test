using OpenTK.Graphics.OpenGL;
using StbImageSharp;

public class Texture
{
    public readonly int Handle;

    public Texture(int glHandle)
    {
        Handle = glHandle;
    }

    public static Texture LoadFromFile(string path)
    {
        //создание объекта тектуры и получение дескриптора
        int handle = GL.GenTexture();

        //привязка текстуры
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, handle);

        //system.drawing начало координат текстуры в левом верхнем а для оpengl нужно
        //чтобы было в левом нижнем, приходиться переворачивать.
        StbImage.stbi_set_flip_vertically_on_load(1);

        using (var stream = File.OpenRead(path))
        {
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            //Генерация текстуры
            //   Тип генерируемой текстуры. Существуют различные типы текстур, но сейчас нам нужна только Texture2D
            //   Уровень детализации. Можно использовать для начала с меньшего мипмапа, но сейчас это не нужно, оставляем 0
            //   Целевой формат пикселей. Это формат, в котором OpenGL будет хранить наше изображение
            //   Ширина изображения
            //   Высота изображения
            //   Граница изображения. Всегда должна быть 0; это устаревший параметр, который Khronos так и не убрал
            //   Формат пикселей, объяснено выше. Так как мы загрузили пиксели как RGBA, используем PixelFormat.Rgba
            //   Тип данных пикселей
            //   И, наконец, сами пиксели
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        }
            

        //Фильтрация текстур, без них рендериться не будет.
        // Сначала устанавливаем фильтры min и mag. Они используются при уменьшении и увеличении текстуры соответственно
        // Здесь используем Linear для обоих. Это означает, что OpenGL будет смешивать пиксели, поэтому текстуры при сильном масштабировании будут выглядеть размытыми
        // Также можно использовать (среди других вариантов) Nearest, который просто берет ближайший пиксель, что делает текстуру пикселированной при сильном масштабировании
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        //Действия opengl при врапе, повторять текстуру s - x, t - y
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        //Генерация мипмапов
        // Мипмапы - это уменьшенные копии текстуры. Каждый уровень мипмапа в два раза меньше предыдущего
        // Сгенерированные мипмапы идут вплоть до одного пикселя
        // OpenGL будет автоматически переключаться между мипмапами, когда объект становится достаточно далеко
        // Это предотвращает муаровые эффекты, а также экономит пропускную способность текстур
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        return new Texture(handle);
    }

    // Активация текстуры
    // Можно привязать несколько текстур, если шейдеру нужно больше одной
    // Для этого используйте GL.ActiveTexture, чтобы установить, к какому слоту привязывается GL.BindTexture
    // Стандарт OpenGL требует как минимум 16 слотов, но может быть больше в зависимости от видеокарты
    public void Use(TextureUnit textureUnit)
    {
        GL.ActiveTexture(textureUnit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }
}
