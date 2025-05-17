using SplashKitSDK;

public enum PlayerState
{
  Running,
  Jumping,
  Sliding
}

public class Player
{
  private Bitmap[] _runFrames;
  private int _frame = 0;
  private double _animationSpeed = 100; // milliseconds per frame
  private SplashKitSDK.Timer _animTimer;

  private float _x, _y;
  private float _velocityY = 0;
  public float X => _x;
  public float Y => _y;
  public Bitmap CurrentBitmap => _runFrames[_frame];

  private PlayerState _state = PlayerState.Running;

  private const float Gravity = 0.6f;
  private const float JumpForce = -15f;
  private const float GroundY = 650 - 365; // 650 (ground) - 365 (image height)
  private bool IsGrounded => _y >= GroundY;

  public Player(float x, float y)
  {
    _x = x;
    _y = GroundY;

    _runFrames = new Bitmap[12];
    for (int i = 0; i < 12; i++)
    {
      _runFrames[i] = SplashKit.LoadBitmap($"player_run_{i}", $"Assets/player_run_{i}.png");
    }

    _animTimer = SplashKit.CreateTimer("player_anim");
    SplashKit.StartTimer(_animTimer);
  }

  public void HandleInput()
  {
    if (IsGrounded)
    {
      if (SplashKit.KeyTyped(KeyCode.SpaceKey))
      {
        _velocityY = JumpForce;
        _state = PlayerState.Jumping;
      }
      else if (SplashKit.KeyDown(KeyCode.LeftShiftKey))
      {
        _state = PlayerState.Sliding;
        _y = GroundY + 60;
      }
      else
      {
        _state = PlayerState.Running;
        _y = GroundY;
      }
    }
  }

  public void Update()
  {
    if (_state == PlayerState.Jumping)
    {
      _velocityY += Gravity;
      _y += _velocityY;

      if (_y >= GroundY)
      {
        _y = GroundY;
        _velocityY = 0;
        _state = PlayerState.Running;
      }
    }

    if (_state == PlayerState.Running)
    {
      if (SplashKit.TimerTicks(_animTimer) > _animationSpeed)
      {
        _frame = (_frame + 1) % _runFrames.Length;
        SplashKit.ResetTimer(_animTimer);
      }
    }
  }

  public void Draw()
  {
    SplashKit.DrawBitmap(_runFrames[_frame], _x, _y);
  }
}
