using SplashKitSDK;
using System;

public class GroundObstacle : Obstacle
{
    private Bitmap[] _spikes;
    private int _selectedSpike;
    private Random _random;
    public override Bitmap CurrentBitmap => _spikes[_selectedSpike]; // GroundObstacle

    public GroundObstacle(float x, float y, float speed) : base(x, y, speed)
    {
        _spikes = new Bitmap[4];
        _random = new Random(); // Initialize the random generator

        // Load four different spike images
        for (int i = 0; i < 4; i++)
        {
            _spikes[i] = SplashKit.LoadBitmap($"spike_{i}", $"Assets/spike_{i}.png");
        }

        // Randomly select one of the spikes
        _selectedSpike = _random.Next(0, 4); // Randomly select an index between 0 and 3
    }

    public override void Update()
    {
        _x -= _speed; // Move the obstacle towards the player
    }

    public override void Draw()
    {
        SplashKit.DrawBitmap(_spikes[_selectedSpike], _x, _y); // Draw the selected spike
    }
}
