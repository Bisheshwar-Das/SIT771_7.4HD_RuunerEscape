using SplashKitSDK;
using System;

public class SkyObstacle : Obstacle
{
    private Bitmap[] _fireballFrames;
    private int _frame = 0;
    private double _animationSpeed = 100; // milliseconds per frame
    private SplashKitSDK.Timer _animTimer;
    private Random _random;
public override Bitmap CurrentBitmap => _fireballFrames[_frame]; // SkyObstacle
    public SkyObstacle(float x, float y, float speed) : base(x, y, speed)
    {
        _fireballFrames = new Bitmap[5];
        _random = new Random(); // Initialize the random generator

        // Load fireball images for animation
        for (int i = 0; i < 5; i++)
        {
            _fireballFrames[i] = SplashKit.LoadBitmap($"fireball_{i}", $"Assets/fireball_animation_{i}.png");
        }

        _animTimer = SplashKit.CreateTimer("fireball_anim");
        SplashKit.StartTimer(_animTimer);
    }

    public override void Update()
    {
        _x -= _speed; // Move the obstacle towards the player

        // Update the animation for the fireball
        if (SplashKit.TimerTicks(_animTimer) > _animationSpeed)
        {
            _frame = (_frame + 1) % _fireballFrames.Length; // Loop through frames
            SplashKit.ResetTimer(_animTimer);
        }
    }

    public override void Draw()
    {
        SplashKit.DrawBitmap(_fireballFrames[_frame], _x, _y); // Draw the fireball animation
    }
}
