using SplashKitSDK;

public class Program
{
    public static void Main()
    {
        Window gameWindow = new Window("Runner Escape", 1280, 720);
        GameController game = new GameController();

        while (!SplashKit.WindowCloseRequested(gameWindow))
        {
            SplashKit.ProcessEvents();
            SplashKit.ClearScreen(Color.White);

            game.Update();
            game.Draw(gameWindow);

            SplashKit.RefreshScreen(60);
        }
    }
}
