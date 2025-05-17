using SplashKitSDK;
using System;

public class GameController
{
  private bool _gameStarted = false;
  private bool _gameOver = false;

  private Player _player;
  private Random _random;
  private int _score = 0;
  private SplashKitSDK.Timer _scoreTimer;

  private List<Obstacle> _obstacles;
  private SplashKitSDK.Timer _spawnTimer;
  private int _spawnInterval = 2500; // in ms
  private float _obstacleSpeed = 9f;

  public GameController()
  {
    _player = new Player(100, 500);
    _random = new Random();
    _obstacles = new List<Obstacle>();

    _scoreTimer = SplashKit.CreateTimer("score_timer");
    SplashKit.StartTimer(_scoreTimer);


    _spawnTimer = SplashKit.CreateTimer("spawn_timer");
    SplashKit.StartTimer(_spawnTimer);
  }

  public void Update()
  {
    if (!_gameStarted)
    {
      if (SplashKit.KeyTyped(KeyCode.ReturnKey))
        StartGame();
    }
    else
    {
      _player.HandleInput();
      _player.Update();
      UpdateObstacles();
      CheckCollisions();
      if (SplashKit.TimerTicks(_scoreTimer) >= 1000) // every 1 second
      {
        _score += 1;
        SplashKit.ResetTimer(_scoreTimer);
      }

    }
  }

  public void Draw(Window gamewindow)
  {
    if (!_gameStarted)
    {
      SplashKit.DrawText("RUNNER ESCAPE", Color.Black, "Arial", 24, gamewindow.Width / 2 - 60, gamewindow.Height / 2);

      if (_gameOver)
      {
        SplashKit.DrawText("Game Over!", Color.Red, "Arial", 24, gamewindow.Width / 2 - 60, gamewindow.Height / 2 + 20);
        SplashKit.DrawText($"Final Score: {_score}", Color.Black, "Arial", 24, gamewindow.Width / 2 - 60, gamewindow.Height / 2 + 30);
        SplashKit.DrawText("Press ENTER to restart", Color.Gray, "Arial", 24, gamewindow.Width / 2 - 60, gamewindow.Height / 2 + 50);
      }
      else
      {
        SplashKit.DrawText("Press ENTER to start", Color.Gray, "Arial", 24, gamewindow.Width / 2 - 60, gamewindow.Height / 2 + 20);
      }
    }
    else
    {
      _player.Draw();
      SplashKit.DrawText($"Score: {_score}", Color.Black, "Arial", 20, 20, 20);

      foreach (Obstacle obs in _obstacles)
        obs.Draw();
    }
  }

  private void UpdateObstacles()
  {
    // Spawn new obstacle every interval
    if (SplashKit.TimerTicks(_spawnTimer) > _spawnInterval)
    {
      if (_random.Next(0, 2) == 0)
      {
        _obstacles.Add(new GroundObstacle(1280, 650 - 80, _obstacleSpeed)); // y = bottom - spike height
      }
      else
      {
        _obstacles.Add(new SkyObstacle(1280, _random.Next(150, 250), _obstacleSpeed));
      }

      SplashKit.ResetTimer(_spawnTimer);
    }

    // Update and clean up obstacles
    for (int i = _obstacles.Count - 1; i >= 0; i--)
    {
      _obstacles[i].Update();

      if (_obstacles[i].X + 100 < 0)
        _obstacles.RemoveAt(i);
    }
  }

  private void CheckCollisions()
  {
    foreach (Obstacle obs in _obstacles)
    {
      if (SplashKit.BitmapCollision(
          _player.CurrentBitmap, _player.X, _player.Y,
          obs.CurrentBitmap, obs.X, obs.Y))
      {
        // Handle collision 
        GameOver();
        return;
      }
    }

  }
  private void GameOver()
  {
    _gameStarted = false;
    _gameOver = true;
  }
  private void StartGame()
  {
    _gameStarted = true;
    _gameOver = false;
    _player = new Player(100, 500);
    _obstacles.Clear();
    SplashKit.ResetTimer(_spawnTimer);

    SplashKit.ResetTimer(_scoreTimer);
  }
}
