# 1. Game Description and Learning Outcomes

## Game Description
"Runner Escape" is a fast-paced, 2D side-scrolling game where the player must avoid obstacles by jumping or sliding to continue progressing. The goal is to survive as long as possible, collecting points by avoiding obstacles. The game features a player character that can run, jump, and slide, while various obstacles (spikes and fireballs) appear and move toward the player. The game ends when the player collides with an obstacle, and the final score is displayed. The player can restart the game by pressing the Enter key.

## Learning Outcomes
By building this game, you will achieve the following learning outcomes:
- **Object-Oriented Programming**: You'll implement and manage game objects (e.g., player, obstacles) using classes and objects.
- **Animation**: You'll learn how to animate the player character and obstacles using sprite frames.
- **User Input Handling**: You will handle user inputs such as key presses (space for jumping, left shift for sliding).
- **Collision Detection**: You'll learn how to detect collisions between objects, leading to game over events.
- **Basic Physics**: The game uses basic physics concepts such as gravity, velocity, and acceleration. The player's jump follows a velocity model where the player rises with an initial jump force and falls back down due to gravity, simulating gravity in a simple way.

These learning outcomes will help you get hands-on experience with fundamental game development concepts, including real-time interaction, animation, and basic game logic.

## What This Tutorial Covers
In this tutorial, you will learn how to implement a simple side-scrolling game using SplashKit. The tutorial will be broken down into manageable steps to guide you through the key components:

- **Step 1**: Setting up the Player and Animating the Player
  - Creating the player character class
  - Implementing basic animation for running
- **Step 2**: Handling Player Input
  - Responding to user input for jumping and sliding
- **Step 3**: Adding Obstacles
  - Creating obstacle classes (spikes and fireballs)
  - Spawning obstacles at regular intervals
- **Step 4**: Collision Detection
  - Detecting when the player collides with obstacles
  - Implementing game-over conditions
- **Step 5**: Implementing a Score System
  - Keeping track of the player's score
  - Displaying the score on the screen

## 2. Prerequisites

Before starting, make sure you have the following:

### 2.1. Software Requirements
- **SplashKit SDK**: Install SplashKit from [SplashKit's website](https://splashkit.io/).
- **C# IDE**: Use Visual Studio code with the .NET SDK installed.

### 2.2. Programming Knowledge
- Basic understanding of **C#** (classes, methods, variables).
- Familiarity with **Object-Oriented Programming (OOP)** concepts.

### 2.3. Game Development Concepts
- Basic knowledge of **animation**, **user input handling**, and **collision detection**.

### 2.4. Assets
- Sprites for the player and obstacles (e.g., spikes, fireballs) in `.png` format.

## 3. Project Setup

### 3.1. Create a New C# Console Application

Open your preferred IDE and create a new C# Console Application:

- **Visual Studio Code**:
  1. Open a terminal
  2. Run the following commands:

     ```bash
     dotnet new console -n RunnerEscape
     cd RunnerEscape
     ```

## 4. Creating the Game Window and Player

### 4.1. Set Up the Game Window

Let’s start by creating the main game loop. This is where the game window is created and continuously updated.

Create a file named `Program.cs` and add the following code:

```csharp
using SplashKitSDK;

public class Program
{
    public static void Main()
    {
        // Create the main game window (width: 1280, height: 720)
        Window gameWindow = new Window("Runner Escape", 1280, 720);

        // Initialize the game controller (we'll define this class later )
        GameController game = new GameController();

        // The main game loop - runs until the window is closed
        while (!SplashKit.WindowCloseRequested(gameWindow))
        {
            // Handle user input (like key presses)
            SplashKit.ProcessEvents();

            // Clear the screen to white before drawing each frame
            SplashKit.ClearScreen(Color.White);
            game.Update();
            game.Draw(gameWindow);

            // Refresh the screen at 60 fps
            SplashKit.RefreshScreen(60);
        }
    }
}

 **Note:** The `GameController` class is referenced in the code but has not been implemented yet.  
Don’t worry — we will create it in the next step. It will handle the core game logic including:
Player control
Obstacle management
Score tracking

This structure is common in real-time games:

**Initialization**: Creating the window and setting up the game.
**Game Loop**: Keeps running until the player closes the game.
**Input → Update → Draw → Refresh**

### 4.2 Creating the Player

The `Player` class represents the main character that the user controls. The player can **run**, **jump**, and **slide**. This class will manage the player's movement, animation, physics, and rendering.

Create a new file in your project folder named: Player.cs

This file will contain the player logic and behavior.

---

### 4.2. Implement the Player Class

Paste the following code into `Player.cs`. The code is annotated with meaningful comments to help you understand key logic areas.

```csharp
using SplashKitSDK;

public enum PlayerState
{
    Running,
    Jumping,
    Sliding
}

public class Player
{
    private Bitmap[] _runFrames; // Animation frames for running
    private int _frame = 0;
    private double _animationSpeed = 100; // Milliseconds per frame
    private SplashKitSDK.Timer _animTimer;

    private float _x, _y;
    private float _velocityY = 0;

    // Properties for easy access
    public float X => _x;
    public float Y => _y;
    public Bitmap CurrentBitmap => _runFrames[_frame];

    private PlayerState _state = PlayerState.Running;

    // Constants for physics and positioning
    private const float Gravity = 0.6f;
    private const float JumpForce = -15f;
    private const float GroundY = 650 - 365; // Adjusted for image height
    private bool IsGrounded => _y >= GroundY;

    public Player(float x, float y)
    {
        _x = x;
        _y = GroundY;

        // Load player run animation frames from Assets/
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
                _velocityY = JumpForce; // Start jumping upward
                _state = PlayerState.Jumping;
            }
            else if (SplashKit.KeyDown(KeyCode.LeftShiftKey))
            {
                _state = PlayerState.Sliding;
                _y = GroundY + 60; // Lower the character during slide
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
                // Reset when player lands back on ground
                _y = GroundY;
                _velocityY = 0;
                _state = PlayerState.Running;
            }
        }

        // Run animation only if player is running
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

# 5. Step 2: Handling Player Input

In this step, we'll extend the `Player` class to handle user input for jumping and sliding actions. The input handling will listen for specific keys and change the player state accordingly.

## 5.1. Player Input Handling

We already have a `HandleInput()` method in the `Player` class that detects:

- Spacebar (`SpaceKey`) to jump
- Left Shift (`LeftShiftKey`) to slide

Now, let’s update the `GameController` class to call this method inside the game loop.

## 5.2. Create the GameController Class

Create a new file named `GameController.cs`. This class will manage the main game logic including:

- Player instance
- Handling input
- Updating game objects
- Drawing to the screen

Here’s a basic implementation:

```csharp
using SplashKitSDK;

public class GameController
{
    private Player _player;

    public GameController()
    {
        // Initialize player at position (100, 0)
        _player = new Player(100, 0);
    }

    public void Update()
    {
        // Handle player input
        _player.HandleInput();

        // Update player position and animation
        _player.Update();
    }

    public void Draw(Window window)
    {
        // Draw the player on the game window
        _player.Draw();
    }
}

# 6. Step 3: Adding Obstacles

In this step, we will create obstacle classes for spikes and fireballs, spawn them at intervals, and make them move toward the player.

## 6.1. Create an Obstacle Base Class

First, create a new file named `Obstacle.cs`. This base class will define common properties and methods for all obstacles:

```csharp
using SplashKitSDK;

public abstract class Obstacle
{
    protected Bitmap _bitmap;
    protected float _x, _y;
    protected float _speed;

    public float X => _x;
    public float Y => _y;
    public Bitmap Bitmap => _bitmap;

    public Obstacle(string bitmapName, string bitmapPath, float startX, float startY, float speed)
    {
        _bitmap = SplashKit.LoadBitmap(bitmapName, bitmapPath);
        _x = startX;
        _y = startY;
        _speed = speed;
    }

    // Move the obstacle left across the screen
    public virtual void Update()
    {
        _x -= _speed;
    }

    // Draw the obstacle on the window
    public virtual void Draw()
    {
        SplashKit.DrawBitmap(_bitmap, _x, _y);
    }

    // Get collision rectangle for collision detection
    public virtual Rectangle CollisionRectangle()
    {
        return new Rectangle((int)_x, (int)_y, _bitmap.Width, _bitmap.Height);
    }
}

## 6.3. Create the Animated SkyObstacle (Fireball)

In this step, we'll create a new obstacle type called `SkyObstacle`. This obstacle represents a **fireball** that animates by cycling through multiple frames.

### SkyObstacle.cs

Create a file named `SkyObstacle.cs` and add the following code:

```csharp
using SplashKitSDK;
using System;

public class SkyObstacle : Obstacle
{
    private Bitmap[] _fireballFrames;
    private int _frame = 0;
    private double _animationSpeed = 100; // milliseconds per frame
    private SplashKitSDK.Timer _animTimer;
    private Random _random;

    // Return the current animation frame bitmap
    public override Bitmap CurrentBitmap => _fireballFrames[_frame];

    public SkyObstacle(float x, float y, float speed) : base(x, y, speed)
    {
        _fireballFrames = new Bitmap[5];
        _random = new Random(); // Initialize random generator

        // Load all fireball animation frames from the Assets folder
        for (int i = 0; i < 5; i++)
        {
            _fireballFrames[i] = SplashKit.LoadBitmap($"fireball_{i}", $"Assets/fireball_animation_{i}.png");
        }

        // Create and start the timer for animation frame switching
        _animTimer = SplashKit.CreateTimer("fireball_anim");
        SplashKit.StartTimer(_animTimer);
    }

    public override void Update()
    {
        // Move the fireball left towards the player
        _x -= _speed;

        // Animate the fireball by cycling through frames
        if (SplashKit.TimerTicks(_animTimer) > _animationSpeed)
        {
            _frame = (_frame + 1) % _firebal# 1. Game Description and Learning Outcomes

## Game Description
"Runner Escape" is a fast-paced, 2D side-scrolling game where the player must avoid obstacles by jumping or sliding to continue progressing. The goal is to survive as long as possible, collecting points by avoiding obstacles. The game features a player character that can run, jump, and slide, while various obstacles (spikes and fireballs) appear and move toward the player. The game ends when the player collides with an obstacle, and the final score is displayed. The player can restart the game by pressing the Enter key.

## Learning Outcomes
By building this game, you will achieve the following learning outcomes:
- **Object-Oriented Programming**: You'll implement and manage game objects (e.g., player, obstacles) using classes and objects.
- **Animation**: You'll learn how to animate the player character and obstacles using sprite frames.
- **User Input Handling**: You will handle user inputs such as key presses (space for jumping, left shift for sliding).
- **Collision Detection**: You'll learn how to detect collisions between objects, leading to game over events.
- **Basic Physics**: The game uses basic physics concepts such as gravity, velocity, and acceleration. The player's jump follows a velocity model where the player rises with an initial jump force and falls back down due to gravity, simulating gravity in a simple way.

These learning outcomes will help you get hands-on experience with fundamental game development concepts, including real-time interaction, animation, and basic game logic.

## What This Tutorial Covers
In this tutorial, you will learn how to implement a simple side-scrolling game using SplashKit. The tutorial will be broken down into manageable steps to guide you through the key components:

- **Step 1**: Setting up the Player and Animating the Player
  - Creating the player character class
  - Implementing basic animation for running
- **Step 2**: Handling Player Input
  - Responding to user input for jumping and sliding
- **Step 3**: Adding Obstacles
  - Creating obstacle classes (spikes and fireballs)
  - Spawning obstacles at regular intervals
- **Step 4**: Collision Detection
  - Detecting when the player collides with obstacles
  - Implementing game-over conditions
- **Step 5**: Implementing a Score System
  - Keeping track of the player's score
  - Displaying the score on the screen

## 2. Prerequisites

Before starting, make sure you have the following:

### 2.1. Software Requirements
- **SplashKit SDK**: Install SplashKit from [SplashKit's website](https://splashkit.io/).
- **C# IDE**: Use Visual Studio code with the .NET SDK installed.

### 2.2. Programming Knowledge
- Basic understanding of **C#** (classes, methods, variables).
- Familiarity with **Object-Oriented Programming (OOP)** concepts.

### 2.3. Game Development Concepts
- Basic knowledge of **animation**, **user input handling**, and **collision detection**.

### 2.4. Assets
- Sprites for the player and obstacles (e.g., spikes, fireballs) in `.png` format.

## 3. Project Setup

### 3.1. Create a New C# Console Application

Open your preferred IDE and create a new C# Console Application:

- **Visual Studio Code**:
  1. Open a terminal
  2. Run the following commands:

     ```bash
     dotnet new console -n RunnerEscape
     cd RunnerEscape
     ```

## 4. Creating the Game Window and Player

### 4.1. Set Up the Game Window

Let’s start by creating the main game loop. This is where the game window is created and continuously updated.

Create a file named `Program.cs` and add the following code:

```csharp
using SplashKitSDK;

public class Program
{
    public static void Main()
    {
        // Create the main game window (width: 1280, height: 720)
        Window gameWindow = new Window("Runner Escape", 1280, 720);

        // Initialize the game controller (we'll define this class later )
        GameController game = new GameController();

        // The main game loop - runs until the window is closed
        while (!SplashKit.WindowCloseRequested(gameWindow))
        {
            // Handle user input (like key presses)
            SplashKit.ProcessEvents();

            // Clear the screen to white before drawing each frame
            SplashKit.ClearScreen(Color.White);
            game.Update();
            game.Draw(gameWindow);

            // Refresh the screen at 60 fps
            SplashKit.RefreshScreen(60);
        }
    }
}
```
 **Note:** The `GameController` class is referenced in the code but has not been implemented yet.  
Don’t worry — we will create it in the next step. It will handle the core game logic including:
Player control
Obstacle management
Score tracking

This structure is common in real-time games:

**Initialization**: Creating the window and setting up the game.
**Game Loop**: Keeps running until the player closes the game.
**Input → Update → Draw → Refresh**

### 4.2 Creating the Player

The `Player` class represents the main character that the user controls. The player can **run**, **jump**, and **slide**. This class will manage the player's movement, animation, physics, and rendering.

Create a new file in your project folder named: Player.cs

This file will contain the player logic and behavior.

---

### 4.2. Implement the Player Class

Paste the following code into `Player.cs`. The code is annotated with meaningful comments to help you understand key logic areas.

```csharp
using SplashKitSDK;

public enum PlayerState
{
    Running,
    Jumping,
    Sliding
}

public class Player
{
    private Bitmap[] _runFrames; // Animation frames for running
    private int _frame = 0;
    private double _animationSpeed = 100; // Milliseconds per frame
    private SplashKitSDK.Timer _animTimer;

    private float _x, _y;
    private float _velocityY = 0;

    // Properties for easy access
    public float X => _x;
    public float Y => _y;
    public Bitmap CurrentBitmap => _runFrames[_frame];

    private PlayerState _state = PlayerState.Running;

    // Constants for physics and positioning
    private const float Gravity = 0.6f;
    private const float JumpForce = -15f;
    private const float GroundY = 650 - 365; // Adjusted for image height
    private bool IsGrounded => _y >= GroundY;

    public Player(float x, float y)
    {
        _x = x;
        _y = GroundY;

        // Load player run animation frames from Assets/
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
                _velocityY = JumpForce; // Start jumping upward
                _state = PlayerState.Jumping;
            }
            else if (SplashKit.KeyDown(KeyCode.LeftShiftKey))
            {
                _state = PlayerState.Sliding;
                _y = GroundY + 60; // Lower the character during slide
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
                // Reset when player lands back on ground
                _y = GroundY;
                _velocityY = 0;
                _state = PlayerState.Running;
            }
        }

        // Run animation only if player is running
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
```
# 5. Step 2: Handling Player Input

In this step, we'll extend the `Player` class to handle user input for jumping and sliding actions. The input handling will listen for specific keys and change the player state accordingly.

## 5.1. Player Input Handling

We already have a `HandleInput()` method in the `Player` class that detects:

- Spacebar (`SpaceKey`) to jump
- Left Shift (`LeftShiftKey`) to slide

Now, let’s update the `GameController` class to call this method inside the game loop.

## 5.2. Create the GameController Class

Create a new file named `GameController.cs`. This class will manage the main game logic including:

- Player instance
- Handling input
- Updating game objects
- Drawing to the screen

Here’s a basic implementation:

```csharp
using SplashKitSDK;

public class GameController
{
    private Player _player;

    public GameController()
    {
        // Initialize player at position (100, 0)
        _player = new Player(100, 0);
    }

    public void Update()
    {
        // Handle player input
        _player.HandleInput();

        // Update player position and animation
        _player.Update();
    }

    public void Draw(Window window)
    {
        // Draw the player on the game window
        _player.Draw();
    }
}
```
# 6. Step 3: Adding Obstacles

In this step, we will create obstacle classes for spikes and fireballs, spawn them at intervals, and make them move toward the player.

## 6.1. Create an Obstacle Base Class

First, create a new file named `Obstacle.cs`. This base class will define common properties and methods for all obstacles:

```csharp
using SplashKitSDK;

public abstract class Obstacle
{
    protected Bitmap _bitmap;
    protected float _x, _y;
    protected float _speed;

    public float X => _x;
    public float Y => _y;
    public Bitmap Bitmap => _bitmap;

    public Obstacle(string bitmapName, string bitmapPath, float startX, float startY, float speed)
    {
        _bitmap = SplashKit.LoadBitmap(bitmapName, bitmapPath);
        _x = startX;
        _y = startY;
        _speed = speed;
    }

    // Move the obstacle left across the screen
    public virtual void Update()
    {
        _x -= _speed;
    }

    // Draw the obstacle on the window
    public virtual void Draw()
    {
        SplashKit.DrawBitmap(_bitmap, _x, _y);
    }

    // Get collision rectangle for collision detection
    public virtual Rectangle CollisionRectangle()
    {
        return new Rectangle((int)_x, (int)_y, _bitmap.Width, _bitmap.Height);
    }
}
```
## 6.3. Create the Animated SkyObstacle (Fireball)

In this step, we'll create a new obstacle type called `SkyObstacle`. This obstacle represents a **fireball** that animates by cycling through multiple frames.

### SkyObstacle.cs

Create a file named `SkyObstacle.cs` and add the following code:

```csharp
using SplashKitSDK;
using System;

public class SkyObstacle : Obstacle
{
    private Bitmap[] _fireballFrames;
    private int _frame = 0;
    private double _animationSpeed = 100; // milliseconds per frame
    private SplashKitSDK.Timer _animTimer;
    private Random _random;

    // Return the current animation frame bitmap
    public override Bitmap CurrentBitmap => _fireballFrames[_frame];

    public SkyObstacle(float x, float y, float speed) : base(x, y, speed)
    {
        _fireballFrames = new Bitmap[5];
        _random = new Random(); // Initialize random generator

        // Load all fireball animation frames from the Assets folder
        for (int i = 0; i < 5; i++)
        {
            _fireballFrames[i] = SplashKit.LoadBitmap($"fireball_{i}", $"Assets/fireball_animation_{i}.png");
        }

        // Create and start the timer for animation frame switching
        _animTimer = SplashKit.CreateTimer("fireball_anim");
        SplashKit.StartTimer(_animTimer);
    }

    public override void Update()
    {
        // Move the fireball left towards the player
        _x -= _speed;

        // Animate the fireball by cycling through frames
        if (SplashKit.TimerTicks(_animTimer) > _animationSpeed)
        {
            _frame = (_frame + 1) % _fireballFrames.Length; // Loop animation frames
            SplashKit.ResetTimer(_animTimer);
        }
    }

    public override void Draw()
    {
        // Draw the current frame of the fireball animation
        SplashKit.DrawBitmap(_fireballFrames[_frame], _x, _y);
    }
}
```
### Explanation

- We load multiple fireball images (`fireball_animation_0.png` to `fireball_animation_4.png`) into an array to create an animation.
- The `_animTimer` tracks time to switch frames at regular intervals (`_animationSpeed`).
- The `Update` method moves the fireball towards the player and cycles through the animation frames.
- The `Draw` method renders the current animation frame.

This animated fireball obstacle will add visual interest and challenge to your game!


### 6.4 GroundObstacle Class

This class creates ground-based obstacles using spike images.

```csharp
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
```
#### Explanation

- The `GroundObstacle` class loads multiple spike images to add variety.
- A random spike is selected when the obstacle is created to keep gameplay visually interesting.
- The `Update` method moves the obstacle leftwards toward the player.
- The `Draw` method renders the selected spike image at its current position.



## 7. Game Logic with GameController

Create a new file named `GameController.cs` and add the following code:

```csharp
using SplashKitSDK;
using System;
using System.Collections.Generic;

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
    private int _spawnInterval = 2500; // milliseconds
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

            // Increase score every second
            if (SplashKit.TimerTicks(_scoreTimer) >= 1000)
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
        // Spawn obstacle every few seconds
        if (SplashKit.TimerTicks(_spawnTimer) > _spawnInterval)
        {
            if (_random.Next(0, 2) == 0)
            {
                _obstacles.Add(new GroundObstacle(1280, 650 - 80, _obstacleSpeed));
            }
            else
            {
                _obstacles.Add(new SkyObstacle(1280, _random.Next(150, 250), _obstacleSpeed));
            }

            SplashKit.ResetTimer(_spawnTimer);
        }

        // Update and remove off-screen obstacles
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
```

lFrames.Length; // Loop animation frames
            SplashKit.ResetTimer(_animTimer);
        }
    }

    public override void Draw()
    {
        // Draw the current frame of the fireball animation
        SplashKit.DrawBitmap(_fireballFrames[_frame], _x, _y);
    }
}

### Explanation

- We load multiple fireball images (`fireball_animation_0.png` to `fireball_animation_4.png`) into an array to create an animation.
- The `_animTimer` tracks time to switch frames at regular intervals (`_animationSpeed`).
- The `Update` method moves the fireball towards the player and cycles through the animation frames.
- The `Draw` method renders the current animation frame.

This animated fireball obstacle will add visual interest and challenge to your game!


### 6.4 GroundObstacle Class

This class creates ground-based obstacles using spike images.

```csharp
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

#### Explanation

- The `GroundObstacle` class loads multiple spike images to add variety.
- A random spike is selected when the obstacle is created to keep gameplay visually interesting.
- The `Update` method moves the obstacle leftwards toward the player.
- The `Draw` method renders the selected spike image at its current position.



## 7. Game Logic with GameController

Create a new file named `GameController.cs` and add the following code:

```csharp
using SplashKitSDK;
using System;
using System.Collections.Generic;

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
    private int _spawnInterval = 2500; // milliseconds
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

            // Increase score every second
            if (SplashKit.TimerTicks(_scoreTimer) >= 1000)
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
        // Spawn obstacle every few seconds
        if (SplashKit.TimerTicks(_spawnTimer) > _spawnInterval)
        {
            if (_random.Next(0, 2) == 0)
            {
                _obstacles.Add(new GroundObstacle(1280, 650 - 80, _obstacleSpeed));
            }
            else
            {
                _obstacles.Add(new SkyObstacle(1280, _random.Next(150, 250), _obstacleSpeed));
            }

            SplashKit.ResetTimer(_spawnTimer);
        }

        // Update and remove off-screen obstacles
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
```

# Runner Escape - Game Summary

## Overview
**Runner Escape** is a 2D side-scrolling endless runner game developed in C# using the SplashKit SDK. The player controls a running character who must avoid oncoming obstacles by jumping or sliding. The game continues until the player collides with an obstacle, with the goal being to survive as long as possible.

---

## Core Features

### Gameplay Mechanics
- **Endless Runner:** Player keeps running automatically.
- **Jumping:** Press `Spacebar` to jump.
- **Sliding:** Press `Left Shift` to slide under obstacles.
- **Obstacle Spawning:** Spikes and fireballs spawn at intervals and move toward the player.

### Collision Detection
- Collision between the player and an obstacle ends the game.

### Scoring System
- Score increases based on how long the player survives.

### Animation
- Running animation uses multiple frames for smooth motion.
- Player state (idle, jumping, sliding) reflected with appropriate sprites.

### Restart System
- Press `Enter` after game over to restart the game.

---

## Technical Stack

- **Language:** C#
- **Framework:** SplashKit SDK
- **Paradigm:** Object-Oriented Programming (OOP)
- **Game Loop:** Real-time input, update, and render cycles

---

## Learning Outcomes

- Understanding of game development structure (setup, loop, update, render)
- Use of OOP to structure game entities (Player, Obstacles, GameController)
- Real-time input handling and response
- Basic 2D physics and collision detection
- Score management and display
- Efficient sprite animation

---

## Future Scope 

###  Gameplay Enhancements
- **Power-ups:** Add collectible items that grant temporary abilities (e.g., invincibility).
- **Multiple Levels or Themes:** Introduce new environments as player progresses.
- **Difficulty Scaling:** Increase game speed or obstacle frequency over time.

###  Graphics and Audio
- **Sound Effects & Music:** Add background music and sound effects for actions.
- **Enhanced Animations:** Add smoother transitions and additional character poses.
- **Background Scrolling:** Implement parallax background for visual depth.

###  Save & Leaderboard
- **High Score Saving:** Store the highest score using local storage or files.
- **Online Leaderboards:** Submit scores to a server and display global rankings.

### Portability
- **Touch Controls:** Adapt for mobile platforms with touch-based input.
- **Cross-Platform Build:** Explore support for Mac, Windows, and web (via WebAssembly or game engines).

---

## Conclusion
**Runner Escape** is a fun, fast-paced game that demonstrates core principles of 2D game development in C#. It's ideal for learning about animation, user input, collision handling, and game design patterns in a hands-on way. With strong extensibility, it provides an excellent base for more complex projects.
