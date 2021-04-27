using Logic;
using Model;
using Renderer;
using Repository;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace DungeonPenetrator
{
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

        public Control()
        {
            Loaded += Control_Loaded;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            model = new GameModel();
            saveGameRepo = new SaveGameRepository();
            highscoreRepo = new HighscoreRepository();
            loadigLogic = new LoadingLogic(model, saveGameRepo, highscoreRepo);
            model = loadigLogic.Play();
            gameLogic = new GameLogic(model);
            renderer = new GameRenderer(model);
            Window win = Window.GetWindow(this);
            if (win != null)
            {
                win.KeyDown += Win_KeyDown;
                win.MouseLeftButtonDown += this.Left_MouseButtonDown;
                win.MouseMove += Win_MouseMove;
            }
            updateTimer = new System.Timers.Timer();
            updateTimer.Elapsed += new ElapsedEventHandler(this.UpdateScreen);
            updateTimer.Interval = 30;
            updateTimer.AutoReset = true;
            updateTimer.Enabled = true;
            shootOnce = new DispatcherTimer();
            shootOnce.Interval = TimeSpan.FromMilliseconds(rnd.Next(1000, 2000));
            shootOnce.Tick += ShootingEnemies;
            moveOnce = new DispatcherTimer();
            moveOnce.Interval = TimeSpan.FromMilliseconds(rnd.Next(200, 500));
            moveOnce.Tick += MoveEnemies;
            moveBossTimer = new DispatcherTimer();
            moveBossTimer.Interval = TimeSpan.FromMilliseconds(800);
            moveBossTimer.Tick += MoveBoss;
            levelTimer = new DispatcherTimer();
            levelTimer.Interval = TimeSpan.FromMilliseconds(200);
            levelTimer.Tick += LevelTimer_Tick;
            levelTimer.Start();
        }

        private void MoveBoss(object sender, EventArgs e)
        {
            if (model.Boss != null)
            {
                if (model.Boss.PlayerInSight)
                {
                    gameLogic.MoveRegularEnemy(model.Boss);
                }
                else
                {
                    gameLogic.RandomBossMovement(model.Boss.Cords);
                }
            }
            moveOnce.Stop();
        }

        private void LevelTimer_Tick(object sender, EventArgs e)
        {
            if (model.MyPlayer.Health == 0)
            {
                levelTimer.Stop();
                foreach (var item in model.Projectiles)
                {
                    item.Timer.Stop();
                    item.Timer = null;
                }
                shootOnce.Stop();
                moveOnce.Stop();
                updateTimer.Stop();
                if (reloadTimer != null)
                {
                    reloadTimer.Stop();
                }

                saveGameRepo.Insert(null);

                GameOverWindow window = new GameOverWindow(loadigLogic, saveGameRepo);
                window.Show();
                Window win = Window.GetWindow(this);
                win.Close();
            }
            if (model.LevelFinished)
            {
                levelTimer.Stop();
                foreach (var item in model.Projectiles)
                {
                    item.Timer.Stop();
                    item.Timer = null;
                }
                shootOnce.Stop();
                moveOnce.Stop();
                updateTimer.Stop();
                if (reloadTimer != null)
                {
                    reloadTimer.Stop();
                }
                gameLogic = null;
                renderer = null;
                loadigLogic.NextLevel();
                gameLogic = new GameLogic(model);
                renderer = new GameRenderer(model);
                saveGameRepo.Insert(model as GameModel);
                updateTimer.Start();
                levelTimer.Start();
            }
        }

        private void ShootingEnemies(object sender, EventArgs e)
        {
            if (model.ShootingMonsters.Count != 0)
            {
                int rndEnemy = rnd.Next(0, model.ShootingMonsters.Count);
                Projectile enemyProjectile = gameLogic.EnemyShoot(model.ShootingMonsters[rndEnemy].Cords, rnd.Next(6, 8), model.ShootingMonsters[rndEnemy].Damage);
                model.Projectiles.Add(enemyProjectile);
                enemyProjectile.Timer = new DispatcherTimer(DispatcherPriority.Send);
                enemyProjectile.Timer.Interval = TimeSpan.FromMilliseconds(20);
                enemyProjectile.Timer.Tick += delegate
                {
                    gameLogic.MoveProjectile(ref enemyProjectile);
                };
                enemyProjectile.Timer.Start();
            }
            if (model.Boss != null && !model.Boss.PlayerInSight)
            {
                int rndNum = rnd.Next(0, 100);
                if (rndNum > 60)
                {
                    gameLogic.BossPatternShoot(model.Boss.Cords, rnd.Next(4, 6), model.Boss.Damage);
                }
                else
                {
                    Projectile bossProjectile = gameLogic.BossShoot(model.Boss.Cords, rnd.Next(8, 10), model.Boss.Damage*2);
                    model.Projectiles.Add(bossProjectile);
                    bossProjectile.Timer = new DispatcherTimer(DispatcherPriority.Send);
                    bossProjectile.Timer.Interval = TimeSpan.FromMilliseconds(20);
                    bossProjectile.Timer.Tick += delegate
                    {
                        gameLogic.MoveProjectile(ref bossProjectile);
                    };
                    bossProjectile.Timer.Start();
                }
            }
            shootOnce.Stop();
        }

        private void Win_MouseMove(object sender, MouseEventArgs e)
        {
            model.mousePosition = e.GetPosition(this);
        }

        private void UpdateScreen(object sender, EventArgs e)
        {
            shootOnce.Start();
            moveOnce.Start();
            moveBossTimer.Start();
            if (model.Boss != null)
            {
                gameLogic.UpdatePlayerInSight();
            }
            gameLogic.Updater();
            try { this.Dispatcher.Invoke(() => this.InvalidateVisual()); } // update screen
            catch (Exception) { }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (renderer != null)
            {
                drawingContext.DrawDrawing(renderer.BuildDrawing());
            }
        }

        private void MoveEnemies(object sender, EventArgs e)
        {
            if (model.TrackingMonsters.Count != 0)
            {
                int rndTrackingEnemyInd = rnd.Next(0, model.TrackingMonsters.Count);
                gameLogic.MoveRegularEnemy(model.TrackingMonsters[rndTrackingEnemyInd]);
            }
            if (model.FlyingMonsters.Count != 0)
            {
                int rndFlyingEnemyInd = rnd.Next(0, model.FlyingMonsters.Count);
                gameLogic.MoveFlyingEnemy(model.FlyingMonsters[rndFlyingEnemyInd]);
            }
        }

        private void Left_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!model.MyPlayer.IsReloading && !model.GameIsPaused)
            {
                reloadTimer = new DispatcherTimer();
                Point mousePos = new Point(e.GetPosition((IInputElement)sender).X, e.GetPosition((IInputElement)sender).Y);
                model.MyPlayer.IsReloading = true;
                gameLogic.PlayerShoot(mousePos, 10);
                reloadTimer.Tick += delegate
                {
                    model.MyPlayer.IsReloading = false;
                    reloadTimer.Stop();
                };
                reloadTimer.Interval = TimeSpan.FromMilliseconds(500 / model.MyPlayer.FiringSpeed);
                reloadTimer.Start();
            }
        }

        private void Win_KeyDown(object sender, KeyEventArgs e)
        {
            if (canmove && !model.GameIsPaused)
            {
                switch (e.Key)
                {
                    case Key.A:
                        gameLogic.MovePlayer(-1, 0);
                        canmove = false;
                        moveOnce.Tick += delegate
                        {
                            canmove = true;
                            moveOnce.Stop();
                        };
                        moveOnce.Interval = TimeSpan.FromMilliseconds(100);
                        moveOnce.Start();
                        break;

                    case Key.D:
                        gameLogic.MovePlayer(1, 0);
                        canmove = false;
                        moveOnce.Tick += delegate
                        {
                            canmove = true;
                            moveOnce.Stop();
                        };
                        moveOnce.Interval = TimeSpan.FromMilliseconds(100);
                        moveOnce.Start();
                        break;

                    case Key.W:
                        gameLogic.MovePlayer(0, -1);
                        canmove = false;
                        moveOnce.Tick += delegate
                        {
                            canmove = true;
                            moveOnce.Stop();
                        };
                        moveOnce.Interval = TimeSpan.FromMilliseconds(100);
                        moveOnce.Start();
                        break;

                    case Key.S:
                        gameLogic.MovePlayer(0, 1);
                        canmove = false;
                        moveOnce.Tick += delegate
                        {
                            canmove = true;
                            moveOnce.Stop();
                        };
                        moveOnce.Interval = TimeSpan.FromMilliseconds(100);
                        moveOnce.Start();
                        break;

                    default:
                        break;
                }
            }
            if (e.Key == Key.Escape)
            {
                model.GameIsPaused = !model.GameIsPaused;
                if (!model.GameIsPaused)
                {
                    foreach (var item in model.Projectiles)
                    {
                        item.Timer.Start();
                    }
                    updateTimer.Start();
                    levelTimer.Start();
                    if (reloadTimer != null)
                    {
                        reloadTimer.Start();
                    }
                }
                else
                {
                    levelTimer.Stop();
                    foreach (var item in model.Projectiles)
                    {
                        item.Timer.Stop();
                    }
                    shootOnce.Stop();
                    moveOnce.Stop();
                    updateTimer.Stop();
                    if (reloadTimer != null)
                    {
                        reloadTimer.Stop();
                    }
                }
                InvalidateVisual();
            }
        }
    }
}