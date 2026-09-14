using Raylib_cs;

enum GameStatus
{
    Start,
    Playing,
    Continue,
    End
}
public class Program {

    public Program() { }
    public static void Main(string[] args)
    {
        const int heightWindow = 450, widthWindow = 800;

        Raylib.InitWindow(widthWindow, heightWindow, "Pong");
        Raylib.SetTargetFPS(60);

        const int racketHeight = 100, racketWidth = 30, leftRacketX = 40, rightRacketX = widthWindow - racketWidth - leftRacketX, ballRadius = 30;
        int leftRacketY = 10;
        int rightRacketY = 10;
        int ballX = 400;
        int ballY = 225;
        bool ballIsDown = true;
        bool ballIsRight = true;
        int ballSpeed = 2;
        int leftPlayerScore = 0;
        int rightPlayerScore = 0;
        GameStatus gameState = GameStatus.Start;

        while (!Raylib.WindowShouldClose())
        {
            HandleInput();
            Update();
            Draw();
        }

        void HandleInput()
        {
            switch(gameState){
                case GameStatus.Start:
                    if (Raylib.IsKeyPressed(KeyboardKey.I))
                        gameState = GameStatus.Playing;
                    break;
                case GameStatus.Playing:
                    if (Raylib.IsKeyDown(KeyboardKey.Up) && rightRacketY > 0)
                        rightRacketY -= 10;
                    if (Raylib.IsKeyDown(KeyboardKey.Down) && rightRacketY < heightWindow - racketHeight - 1)
                        rightRacketY += 10;

                    if (Raylib.IsKeyDown(KeyboardKey.W) && leftRacketY > 0)
                        leftRacketY -= 10;
                    if (Raylib.IsKeyDown(KeyboardKey.S) && leftRacketY < heightWindow - racketHeight - 1)
                        leftRacketY += 10;
                    break;
                case GameStatus.Continue:
                    if (Raylib.IsKeyPressed(KeyboardKey.C))
                    {
                        ballX = 400;
                        ballY = 225;
                        gameState = GameStatus.Playing;
                    }
                    if (Raylib.IsKeyPressed(KeyboardKey.E))
                        gameState = GameStatus.End;
                    break;
                case GameStatus.End:
                default:
                    break;
            }
        }

        void MoveBall()
        {
            if (ballIsDown)
                ballY += ballSpeed;
            else ballY -= ballSpeed;

            if (ballIsRight)
                ballX += ballSpeed;
            else ballX -= ballSpeed;

            if (ballY <= 1 + ballRadius || ballY >= heightWindow - ballRadius - 1)
                ballIsDown = !ballIsDown;

            //if (ballX <= 1 + ballRadius || ballX >= widthWindow - ballRadius - 1)
            //    ballIsRight = !ballIsRight;

            if (ballX <= leftRacketX + racketWidth + 1)
            {
                if (ballY >= leftRacketY + 1 && ballY <= leftRacketY + racketHeight)
                    ballIsRight = !ballIsRight;
                else if (ballX <= 1)
                {
                    rightPlayerScore++;
                    gameState = GameStatus.Continue;
                }
            }

            if (ballX >= rightRacketX + 1)
            {
                if (ballY >= rightRacketY + 1 && ballY <= rightRacketY + racketHeight)
                    ballIsRight = !ballIsRight;
                else if (ballX >= widthWindow - 1)
                {
                    leftPlayerScore++;
                    gameState = GameStatus.Continue;
                }
            }
        }

        void Update()
        {
            switch (gameState)
            {
                case GameStatus.Playing:
                    MoveBall();
                    break;
                default:
                    break;
            }
        }

        void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            switch (gameState)
            {
                case GameStatus.Start:
                        Raylib.DrawText("Presiona I para inciar", widthWindow / 4, heightWindow / 2, 42, Color.White);
                    break;
                case GameStatus.Playing:
                        Raylib.DrawCircle(ballX, ballY, ballRadius, Color.White);
                        Raylib.DrawRectangle(leftRacketX, leftRacketY, racketWidth, racketHeight, Color.White);
                        Raylib.DrawRectangle(rightRacketX, rightRacketY, racketWidth, racketHeight, Color.White);
                        Raylib.DrawText("Jugador 1: " + leftPlayerScore.ToString(), leftRacketX, 5, 24, Color.White);
                        Raylib.DrawText("Jugador 2: " + rightPlayerScore.ToString(), rightRacketX, 5, 24, Color.White);
                    break;
                case GameStatus.Continue:
                    Raylib.DrawText("Presiona C para continuar", widthWindow / 5, heightWindow / 2, 42, Color.White);
                    Raylib.DrawText("Presiona E para Terminar", widthWindow / 4, (heightWindow / 2) + 60, 42, Color.White);
                    break;
                case GameStatus.End:
                    if (rightPlayerScore == leftPlayerScore)
                        Raylib.DrawText("Empate, " + rightPlayerScore.ToString() + " puntos", 0, heightWindow / 2, 36, Color.White);
                    else if (rightPlayerScore > leftPlayerScore)
                        Raylib.DrawText("Gana el jugador 2 con " + rightPlayerScore.ToString() + " puntos", 0, heightWindow / 2, 36, Color.White);
                    else
                        Raylib.DrawText("Gana el jugador 1 con " + leftPlayerScore.ToString() + " puntos", 0, heightWindow / 2, 36, Color.White);
                    break;
                default:
                    break;
            }
                    Raylib.EndDrawing();
        }
        Raylib.CloseWindow();
    }
}