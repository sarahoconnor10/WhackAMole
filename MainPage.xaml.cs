using Plugin.Maui.Audio;

namespace OConnor_Sarah_VS22;
/*
 * SOURCE LIST 
 * Audio Manager functionality - https://www.youtube.com/watch?v=oIYnEuZ9oew&ab_channel=GeraldVersluis 
 * start.mp3 - https://pixabay.com/sound-effects/message-incoming-132126/
 * hit1.mp3 - https://www.youtube.com/watch?v=8usQCG6WHzE&ab_channel=freesoundeffects
 * end.mp3 - https://youtu.be/EHLSwoSjr1I?si=udGN9zewt1re9U9E
 * hammer.svg & hammer2.svg - https://freesvg.org/roughly-drawn-hammer
 * mole.svg & moleflipped.png - https://freesvg.org/mole-in-a-hole
 * mole2.svg - https://freesvg.org/mole2
 
 */
public partial class MainPage : ContentPage
{
    private IAudioManager audioManager;
    private System.Timers.Timer GameTimer;
    private System.Timers.Timer MoleTimer;
    private Random random;

    /* Variables */
    private int scoreCounter = 0;
    private int countdown = 15;
    private int timerInterval = 1000;
    private int moleInterval = 1000;
    private int moleCountdown = 1;
    private int maxRows = 4;
    private int highscore;
    private bool gameRunning = false;
    private bool four_grid = true;
    private bool gridFiveDrawn = false;
    private bool gridFourDrawn = false;
    

    public MainPage(IAudioManager audioManager)
	{
		InitializeComponent();
        this.audioManager = audioManager;
        /* Upon start - initialises random object, sets up timers, initialises high score, draws the 4x4 grid */
        random = new Random(); 
        SetGameTimer();
        SetMoleTimer();
        InitialiseHighScore();
        DrawFourGrid();
    }
    private void InitialiseHighScore()
    {
        /* Initialise the highscore in memory and changes the label */
        highscore = Preferences.Default.Get("High_Score", 0);
        Highscore_Lbl.Text = "HIGH SCORE: " + highscore;
    }
    private async void StartBtn_Clicked(object sender, EventArgs e)
    {
        /* changes button text */
        StartBtn.Text = "Play Again";

        /* starts the game if not already running */
        if (!gameRunning)
        {
            var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("start.mp3"));
            player.Play();
            StartGame();
        }
        /* if game running - calls CheckIfSure */
        else
            CheckIfSure();
    }
    private void StartGame()
    {
        /* stops any existing timers, calls MoveTheMole depending on which grid is active, starts the timers */
        MoleTimer.Stop();
        GameTimer.Stop();
        scoreCounter = 0;
        moleCountdown = 1;
        Score_Lbl.Text = scoreCounter.ToString();
        countdown = 15;
        Timer_Lbl.Text = $" 00:{countdown}";
        gameRunning = true;
        if (four_grid)
        {
            mole_image4.IsVisible = true;
            MoveTheMole(mole_image4);
        }
        else
        {
            mole_image5.IsVisible = true;
            MoveTheMole(mole_image5);
        }
        GameTimer.Start();
        MoleTimer.Start();
    }
    private async void ImageTapped(object sender, EventArgs e)
    {
        if (!gameRunning)
            return;
        /* play sound when image is hit, increase the score, and move the mole */
        var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("hit1.mp3"));
        player.Play();
        IncreaseScore();
        MoleTimer.Stop();
        moleCountdown = 1;
        MoveTheMole((Image)sender);
    }
    private async void MoveTheMole(Image img)
    {
        /* Uses random value to move the mole to a new spot in the grid */
        
        int r, c;
        r = random.Next(0, maxRows);
        c = random.Next(0, maxRows);
        img.SetValue(Grid.RowProperty, r);
        img.SetValue(Grid.ColumnProperty, c);

        /* animates the mole upon appearance */
        img.Opacity = 0;
        img.Scale = 0;
        img.FadeTo(1, 200);
        await img.ScaleTo(1, 200);

        /* resets mole timer */
        MoleTimer.Stop();
        moleCountdown = 1;
        MoleTimer.Start();
    }
    private void IncreaseScore()
    {
        /* Increments the score counter */
        ++scoreCounter;
        Score_Lbl.Text = " " + scoreCounter.ToString();
    }
    private void EndGame()
    {
        /* stops timers, hides images, resets timer/score, declares high score if beaten */
        if (scoreCounter > highscore)
        {
            highscore = scoreCounter;
            Highscore_Lbl.Text = "HIGH SCORE: " + highscore;
            Preferences.Default.Set("High_Score", highscore);
        }
        GameTimer.Stop();
        MoleTimer.Stop();
        scoreCounter = 0;
        moleCountdown = 1;
        Score_Lbl.Text = scoreCounter.ToString();
        countdown = 15;
        Timer_Lbl.Text = $" 00:{countdown}";
        mole_image4.IsVisible = false;
        mole_image5.IsVisible = false;
        gameRunning = false;
    }
    private void PauseGame()
    {
        /* stops timers, hides mole */
        if (!gameRunning)
            return;
        GameTimer.Stop();
        MoleTimer.Stop();
        mole_image4.IsVisible = false;
        mole_image5.IsVisible = false;
        gameRunning = false;
    }
    private void ContinueGame()
    {
        /* starts timers, unhides mole */

        if (gameRunning)
            return;
        if(four_grid)
            mole_image4.IsVisible = true;
        else
            mole_image5.IsVisible = true;
        gameRunning = true;
        GameTimer.Start();
        MoleTimer.Start();
    }
    private void SwitchBtn_Clicked(object sender, EventArgs e)
    {
        if (!gameRunning)
        {
            if (four_grid)
            {
                //IF 4 GRID IS TRUE - SWITCH TO 5 GRID
                maxRows = 5;
                Grid4.IsVisible = false;
                Grid4.IsEnabled = false;
                four_grid = false;
                HardMode();
                SwitchBtn.Text = "Easy Mode";
                SwitchBtn.BackgroundColor = Color.FromRgb(255, 255, 255);
                SwitchBtn.TextColor = Color.FromRgb(0, 0, 0);
                DrawFiveGrid();
                Grid5.IsEnabled = true;
                Grid5.IsVisible = true;
            }
            else
            {
                //IF 5 GRID IS TRUE - SWITCH TO 4 GRID
                Grid5.IsEnabled = false;
                Grid5.IsVisible = false;
                Grid4.IsVisible = true;
                Grid4.IsEnabled = true;
                four_grid = true;
                maxRows = 4;
                EasyMode();
                SwitchBtn.Text = "HARD MODE";
                SwitchBtn.BackgroundColor = Color.FromRgb(0, 0, 0);
                SwitchBtn.TextColor = Color.FromRgb(255, 0, 0);
            }
        }
    }
    private void DrawFourGrid()
    {
        /* if frame has not already been drawn - create frame for grid */
        if (gridFourDrawn)
            return;

        for (int i = 0; i < maxRows; i++)
            Grid4.AddRowDefinition(new RowDefinition());

        for (int i = 0; i < maxRows; i++)
            Grid4.AddColumnDefinition(new ColumnDefinition());

        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxRows; col++)
            {
                /* if/else to target every second box in grid */
                if (row % 2 == 0 && col % 2 == 0)
                {
                    Frame styledFrame = new Frame
                    {
                        BackgroundColor = Color.FromRgb(97, 219, 26),
                        CornerRadius = 30,
                        HasShadow = false, 
                        Padding = new Thickness(5),
                        BorderColor = Color.FromRgb(97, 219, 26)
                    };
                    Grid4.Add(styledFrame, row, col);
                }
                else if (!(row % 2 == 0) && !(col % 2 == 0))
                {
                    Frame styledFrame = new Frame
                    {
                        BackgroundColor = Color.FromRgb(97, 219, 26), 
                        CornerRadius = 30, 
                        HasShadow = false,
                        Padding = new Thickness(5),
                        BorderColor = Color.FromRgb(97, 219, 26)
                    };
                    Grid4.Add(styledFrame, row, col);
                }
                else
                {
                    Frame styledFrame = new Frame
                    {
                        BackgroundColor = Color.FromRgb(31, 184, 11),
                        CornerRadius = 30,
                        HasShadow = false,
                        Padding = new Thickness(5),
                        BorderColor = Color.FromRgb(31, 184, 11)
                    };
                    Grid4.Add(styledFrame, row, col);
                }//else
            }//inner loop - col
        }//outer loop - row
        gridFourDrawn = true;
    }
    private void DrawFiveGrid()
    {
        /* if frame has not already been drawn - create frame for grid */
        if (gridFiveDrawn)
            return;
        for (int i = 0; i < maxRows; i++)
            Grid5.AddRowDefinition(new RowDefinition());

        for (int i = 0; i < maxRows; i++)
            Grid5.AddColumnDefinition(new ColumnDefinition());

        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxRows; col++)
            {
                /* if/else to target every second box in grid */
                if (row % 2 == 0 && col % 2 == 0)
                {
                    Frame styledFrame = new Frame
                    {
                        BackgroundColor = Color.FromRgb(97, 219, 26),
                        CornerRadius = 25,
                        HasShadow = false,
                        Padding = new Thickness(5),
                        BorderColor = Color.FromRgb(97, 219, 26)
                    };
                    Grid5.Add(styledFrame, row, col);
                }
                else if (!(row % 2 == 0) && !(col % 2 == 0))
                {
                    Frame styledFrame = new Frame
                    {
                        BackgroundColor = Color.FromRgb(97, 219, 26),
                        CornerRadius = 25,
                        HasShadow = false,
                        Padding = new Thickness(5),
                        BorderColor = Color.FromRgb(97, 219, 26)
                    };
                    Grid5.Add(styledFrame, row, col);
                }
                else
                {
                    Frame styledFrame = new Frame
                    {
                        BackgroundColor = Color.FromRgb(31, 184, 11),
                        CornerRadius = 25,
                        HasShadow = false,
                        Padding = new Thickness(5),
                        BorderColor = Color.FromRgb(31, 184, 11)
                    };
                    Grid5.Add(styledFrame, row, col);
                }
            }//inner loop - col
        }//outer loop - row
        gridFiveDrawn = true;
    }
    private void HardMode()
    {
        /* if hard mode is chosen - mole moves faster, resets mole timer */
        moleInterval = 300;
        moleCountdown = 1;
        SetMoleTimer();
    }
    private void EasyMode()
    {
        /* if easy mode is chosen - mole moves slower, resets mole timer */
        moleInterval = 1000;
        moleCountdown = 1;
        SetMoleTimer();
    }
    private async void CheckIfSure()
    {
        /* pauses the game, prompts user to confirm they would like to restart the game */
        PauseGame();
        bool answer = await DisplayAlert("Alert", "Are you sure you want to restart?", "Yes", "Cancel");
        if (answer)
            EndGame();
        else
            ContinueGame();
    }

   
    /* TIMERS */
    private void SetGameTimer()
    {
        GameTimer = new System.Timers.Timer
        {
                Interval = timerInterval
        };
        GameTimer.Elapsed += Game_Timer_Elapsed;
    }
    private void Game_Timer_Elapsed(object sender, EventArgs e)
    {
        Dispatcher.Dispatch(
            () =>
            {
                GameTimerFunction();
            }
            );
    }
    private async void GameTimerFunction()
    {
        if (countdown > 0)
        {
            --countdown;
            if (countdown < 10)
                Timer_Lbl.Text = $" 00:0{countdown}";
            else
                Timer_Lbl.Text = $" 00:{countdown}";
        }
        else
        {
            EndGame();
            var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("end.mp3"));
            player.Play();
        }
    }
    private void SetMoleTimer()
    {

        MoleTimer = new System.Timers.Timer
        {
            Interval = moleInterval
        }; 
       MoleTimer.Elapsed += Timer_Mole_Elapsed;
    }

    private void Timer_Mole_Elapsed(object sender, EventArgs e)
    {
        Dispatcher.Dispatch(
            () =>
            {
                TimerMoleFunction();
            }
            );
    }
    private void TimerMoleFunction()
    {
        if(moleCountdown > 0)
        {
            moleCountdown--;
        }
        else if(moleCountdown == 0)
        {
            if(four_grid)
                MoveTheMole(mole_image4);
            else
                MoveTheMole(mole_image5);
            moleCountdown = 1;
        }
    }

}//public class MainPage: ContentPage

