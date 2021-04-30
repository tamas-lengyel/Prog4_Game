// <copyright file="Control.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DungeonPenetrator
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Timers;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Logic;
    using Microsoft.Win32;
    using Model;
    using Renderer;
    using Repository;

    /// <summary>
    /// Control class.
    /// </summary>
    internal class Control : FrameworkElement
    {
        private IGameLogic gameLogic;
        private ILoadingLogic loadigLogic;
        private IGameModel model;
        private ISaveGameRepository saveGameRepo;
        private IHighscoreRepository highscoreRepo;
        private GameRenderer renderer;
        private Random rnd = new Random();
        private bool canmove = true;

        private System.Timers.Timer updateTimer;
        private DispatcherTimer shootOnce;
        private DispatcherTimer moveOnce;
        private DispatcherTimer reloadTimer;
        private DispatcherTimer levelTimer;
        private DispatcherTimer moveBossTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Control"/> class.
        /// </summary>
        public Control()
        {
            this.Loaded += this.Control_Loaded;
        }

        /// <summary>
        /// Draws the drawing froup from Renderer DLL.
        /// </summary>
        /// <param name="drawingContext">Context.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.renderer != null)
            {
                drawingContext.DrawDrawing(this.renderer.BuildDrawing());
            }
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            this.model = new GameModel();
            this.highscoreRepo = new HighscoreRepository();
            Window win = Window.GetWindow(this);
            if ((win as MainWindow).AutoOrManual)
            {
                this.saveGameRepo = new ManualSaveGameRepository((win as MainWindow).LoadFilePath);
            }
            else
            {
                this.saveGameRepo = new AutoSaveGameRepository();
            }

            this.loadigLogic = new LoadingLogic(this.model, this.saveGameRepo, this.highscoreRepo);
            this.model = this.loadigLogic.Play();
            this.gameLogic = new GameLogic(this.model);
            this.renderer = new GameRenderer(this.model);
            if (win != null)
            {
                win.KeyDown += this.Win_KeyDown;
                win.MouseLeftButtonDown += this.Left_MouseButtonDown;
                win.MouseMove += this.Win_MouseMove;
            }

            this.updateTimer = new System.Timers.Timer();
            this.updateTimer.Elapsed += new ElapsedEventHandler(this.UpdateScreen);
            this.updateTimer.Interval = 30;
            this.updateTimer.AutoReset = true;
            this.shootOnce = new DispatcherTimer();
            this.shootOnce.Interval = TimeSpan.FromMilliseconds(this.rnd.Next(1000, 2000));
            this.shootOnce.Tick += this.ShootingEnemies;
            this.moveOnce = new DispatcherTimer();
            this.moveOnce.Interval = TimeSpan.FromMilliseconds(this.rnd.Next(200, 500));
            this.moveOnce.Tick += this.MoveEnemies;
            this.moveBossTimer = new DispatcherTimer();
            this.moveBossTimer.Interval = TimeSpan.FromMilliseconds(800);
            this.moveBossTimer.Tick += this.MoveBoss;
            this.levelTimer = new DispatcherTimer();
            this.levelTimer.Interval = TimeSpan.FromMilliseconds(200);
            this.levelTimer.Tick += this.LevelTimer_Tick;

            if (!this.model.GameIsPaused)
            {
                this.updateTimer.Enabled = true;
                this.levelTimer.Start();
            }

            this.InvalidateVisual();
        }

        private void MoveBoss(object sender, EventArgs e)
        {
            if (this.model.Boss != null)
            {
                if (this.model.Boss.PlayerInSight)
                {
                    this.gameLogic.MoveRegularEnemy(this.model.Boss);
                }
                else
                {
                    this.gameLogic.RandomBossMovement(this.model.Boss.Cords);
                }
            }

            this.moveOnce.Stop();
        }

        private void LevelTimer_Tick(object sender, EventArgs e)
        {
            if (this.model.MyPlayer.Health == 0)
            {
                this.levelTimer.Stop();
                foreach (var item in this.model.Projectiles)
                {
                    item.Timer.Stop();
                    item.Timer = null;
                }

                this.shootOnce.Stop();
                this.moveOnce.Stop();
                this.updateTimer.Stop();
                if (this.reloadTimer != null)
                {
                    this.reloadTimer.Stop();
                }

                this.saveGameRepo.Insert(null);

                GameOverWindow window = new GameOverWindow(this.loadigLogic, this.saveGameRepo);
                window.Show();
                Window win = Window.GetWindow(this);
                win.Close();
            }

            if (this.model.LevelFinished)
            {
                this.levelTimer.Stop();
                foreach (var item in this.model.Projectiles)
                {
                    item.Timer.Stop();
                    item.Timer = null;
                }

                this.shootOnce.Stop();
                this.moveOnce.Stop();
                this.updateTimer.Stop();
                if (this.reloadTimer != null)
                {
                    this.reloadTimer.Stop();
                }

                this.gameLogic = null;
                this.renderer = null;
                this.loadigLogic.NextLevel();
                this.gameLogic = new GameLogic(this.model);
                this.renderer = new GameRenderer(this.model);
                this.saveGameRepo.Insert(this.model as GameModel);
                this.updateTimer.Start();
                this.levelTimer.Start();
            }
        }

        private void ShootingEnemies(object sender, EventArgs e)
        {
            if (this.model.ShootingMonsters.Count != 0)
            {
                int rndEnemy = this.rnd.Next(0, this.model.ShootingMonsters.Count);
                this.gameLogic.EnemyShoot(this.model.ShootingMonsters[rndEnemy].Cords, this.rnd.Next(6, 8), this.model.ShootingMonsters[rndEnemy].Damage);
            }

            if (this.model.Boss != null && !this.model.Boss.PlayerInSight)
            {
                int rndNum = this.rnd.Next(0, 100);
                if (rndNum > 60)
                {
                    this.gameLogic.BossPatternShoot(this.model.Boss.Cords, this.rnd.Next(4, 6), this.model.Boss.Damage);
                }
                else
                {
                    this.gameLogic.BossShoot(this.model.Boss.Cords, this.rnd.Next(8, 10), this.model.Boss.Damage * 2);
                }
            }

            this.shootOnce.Stop();
        }

        private void Win_MouseMove(object sender, MouseEventArgs e)
        {
            this.model.MousePosition = e.GetPosition(this);
        }

        private void UpdateScreen(object sender, EventArgs e)
        {
            this.shootOnce.Start();
            this.moveOnce.Start();
            this.moveBossTimer.Start();
            if (this.model.Boss != null)
            {
                this.gameLogic.UpdatePlayerInSight();
            }

            this.gameLogic.Updater();
            try
            {
                this.Dispatcher.Invoke(() => this.InvalidateVisual());
            } // update screen
            catch (Exception)
            {
            }
        }

        private void MoveEnemies(object sender, EventArgs e)
        {
            if (this.model.TrackingMonsters.Count != 0)
            {
                int rndTrackingEnemyInd = this.rnd.Next(0, this.model.TrackingMonsters.Count);
                this.gameLogic.MoveRegularEnemy(this.model.TrackingMonsters[rndTrackingEnemyInd]);
            }

            if (this.model.FlyingMonsters.Count != 0)
            {
                int rndFlyingEnemyInd = this.rnd.Next(0, this.model.FlyingMonsters.Count);
                this.gameLogic.MoveFlyingEnemy(this.model.FlyingMonsters[rndFlyingEnemyInd]);
            }
        }

        private void Left_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.model.MyPlayer.IsReloading && !this.model.GameIsPaused)
            {
                this.reloadTimer = new DispatcherTimer();
                Point mousePos = new Point(e.GetPosition((IInputElement)sender).X, e.GetPosition((IInputElement)sender).Y);
                this.model.MyPlayer.IsReloading = true;
                this.gameLogic.PlayerShoot(mousePos, 10);
                this.reloadTimer.Tick += delegate
                {
                    this.model.MyPlayer.IsReloading = false;
                    this.reloadTimer.Stop();
                };
                this.reloadTimer.Interval = TimeSpan.FromMilliseconds(500 / this.model.MyPlayer.FiringSpeed);
                this.reloadTimer.Start();
            }
        }

        private void Win_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.canmove && !this.model.GameIsPaused)
            {
                switch (e.Key)
                {
                    case Key.A:
                        this.gameLogic.MovePlayer(-1, 0);
                        this.canmove = false;
                        this.moveOnce.Tick += delegate
                        {
                            this.canmove = true;
                            this.moveOnce.Stop();
                        };
                        this.moveOnce.Interval = TimeSpan.FromMilliseconds(100);
                        this.moveOnce.Start();
                        break;

                    case Key.D:
                        this.gameLogic.MovePlayer(1, 0);
                        this.canmove = false;
                        this.moveOnce.Tick += delegate
                        {
                            this.canmove = true;
                            this.moveOnce.Stop();
                        };
                        this.moveOnce.Interval = TimeSpan.FromMilliseconds(100);
                        this.moveOnce.Start();
                        break;

                    case Key.W:
                        this.gameLogic.MovePlayer(0, -1);
                        this.canmove = false;
                        this.moveOnce.Tick += delegate
                        {
                            this.canmove = true;
                            this.moveOnce.Stop();
                        };
                        this.moveOnce.Interval = TimeSpan.FromMilliseconds(100);
                        this.moveOnce.Start();
                        break;

                    case Key.S:
                        this.gameLogic.MovePlayer(0, 1);
                        this.canmove = false;
                        this.moveOnce.Tick += delegate
                        {
                            this.canmove = true;
                            this.moveOnce.Stop();
                        };
                        this.moveOnce.Interval = TimeSpan.FromMilliseconds(100);
                        this.moveOnce.Start();
                        break;

                    default:
                        break;
                }
            }

            if (e.Key == Key.Return && this.model.GameIsPaused)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.OverwritePrompt = true;
                saveFileDialog.CreatePrompt = true;
                saveFileDialog.Filter = "Json File (*.json)|*.json";
                saveFileDialog.FileName = "*.json";
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Saves\";
                if (saveFileDialog.ShowDialog() == true)
                {
                    ManualSaveGameRepository msgr = new ManualSaveGameRepository(saveFileDialog.FileName);
                    AutoSaveGameRepository asgr = new AutoSaveGameRepository();
                    this.model.MyPlayer.IsReloading = false;
                    msgr.Insert(this.model as GameModel);
                    asgr.Insert(this.model as GameModel);
                }
            }

            if (e.Key == Key.Escape)
            {
                this.model.GameIsPaused = !this.model.GameIsPaused;
                if (!this.model.GameIsPaused)
                {
                    foreach (var item in this.model.Projectiles)
                    {
                        item.Timer.Start();
                        item.Timer.Tick += delegate
                        {
                            this.gameLogic.MoveProjectile(item);
                        };
                    }

                    this.updateTimer.Start();
                    this.levelTimer.Start();
                    if (this.reloadTimer != null)
                    {
                        this.reloadTimer.Start();
                    }
                }
                else
                {
                    this.levelTimer.Stop();
                    foreach (var item in this.model.Projectiles)
                    {
                        item.Timer.Stop();
                    }

                    this.shootOnce.Stop();
                    this.moveOnce.Stop();
                    this.updateTimer.Stop();
                    if (this.reloadTimer != null)
                    {
                        this.reloadTimer.Stop();
                    }
                }

                this.InvalidateVisual();
            }
        }
    }
}