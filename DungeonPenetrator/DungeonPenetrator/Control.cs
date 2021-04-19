using Logic;
using Model;
using Renderer;
using Repository;
using System;
using System.Threading;
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

        System.Timers.Timer updateTimer;
        DispatcherTimer shootOnce;
        DispatcherTimer moveOneEnemy;
        DispatcherTimer moveOnce;
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
            MoveEnemies();
            shootOnce = new DispatcherTimer();
            updateTimer = new System.Timers.Timer();
            updateTimer.Elapsed += new ElapsedEventHandler(this.UpdateScreen);
            updateTimer.Interval = 20;
            updateTimer.AutoReset = true;
            updateTimer.Enabled = true;
            shootOnce.Interval = TimeSpan.FromMilliseconds(rnd.Next(3000, 5000));
            shootOnce.Tick += ShootingEnemies;
        }

        private void ShootingEnemies(object sender, EventArgs e)
        {
            foreach (var item in model.ShootingMonsters)
            {
                Projectile enemyProjectile = gameLogic.EnemyShoot(item.Cords, rnd.Next(2, 4), item.Damage);
                model.Projectiles.Add(enemyProjectile);
                enemyProjectile.Timer = new DispatcherTimer(DispatcherPriority.Send);
                enemyProjectile.Timer.Interval = TimeSpan.FromMilliseconds(20);
                enemyProjectile.Timer.Tick += delegate
                {
                    gameLogic.MoveProjectile(ref enemyProjectile);
                };
                enemyProjectile.Timer.Start();
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
            gameLogic.Updater();
            if (model.MyPlayer.Health==0)
            {
                //updateTimer.Stop();
            }
            if (model.LevelFinished)
            {
                
                /*updateTimer.Stop();
                moveOneEnemy.Stop();
                shootOnce.Stop();
                reloadTimer.Stop();
                moveOnce.Stop();
                foreach (var item in model.Projectiles)
                {
                    item.Timer.Stop();
                }
                Thread.Sleep(1000);
                model = null;
                model = new GameModel();
                loadigLogic = new LoadingLogic(model, saveGameRepo, highscoreRepo);
                model = loadigLogic.Play();
                gameLogic = new GameLogic(model);
                renderer = new GameRenderer(model);
                updateTimer.Start();*/
            }
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

        private void MoveEnemies()
        {
            foreach (var item in model.TrackingMonsters)
            {
                moveOneEnemy = new DispatcherTimer();
                moveOneEnemy.Tick += delegate
                {
                    gameLogic.MoveRegularEnemy(item);
                };
                moveOneEnemy.Interval = TimeSpan.FromMilliseconds(rnd.Next(1000, 3000));
                moveOneEnemy.Start();
            }

            foreach (var item in model.FlyingMonsters)
            {
                moveOneEnemy = new DispatcherTimer();
                moveOneEnemy.Tick += delegate
                {
                    gameLogic.MoveFlyingEnemy(item);
                };
                moveOneEnemy.Interval = TimeSpan.FromMilliseconds(rnd.Next(1000, 3000));
                moveOneEnemy.Start();
            }
        }

        private void Left_MouseButtonDown(object sender,MouseButtonEventArgs e)
        {
            DispatcherTimer reloadTimer = new DispatcherTimer();
            if (!model.MyPlayer.IsReloading)
            {
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
            moveOnce = new DispatcherTimer();
            if (canmove)
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
        }
    }
}