using SplashKitSDK;

public abstract class Obstacle
{
    protected float _x, _y;
    protected float _speed;
    public float X => _x;
public float Y => _y;
public abstract Bitmap CurrentBitmap { get; }

    public Obstacle(float x, float y, float speed)
    {
        _x = x;
        _y = y;
        _speed = speed;
    }

    public abstract void Update();
    public abstract void Draw();
}


