using Logic;
using Model;
using Renderer;
using Repository;
using System;
using System.Threading;
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
                MouseLeftButtonDown += Left_MouseButtonDown;
            }
            MoveEnemies();
            ShootingEnemies();

            DispatcherTimer updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromMilliseconds(20);
            updateTimer.Tick += UpdateScreen;
            updateTimer.Start();

            //InvalidateVisual();
        }

        private void UpdateScreen(object sender, EventArgs e)
        {
            InvalidateVisual();
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
                DispatcherTimer moveOneEnemy = new DispatcherTimer();
                moveOneEnemy.Tick += delegate
                {
                    gameLogic.MoveRegularEnemy(item);
                };
                moveOneEnemy.Interval = TimeSpan.FromMilliseconds(rnd.Next(1000, 3000));
                moveOneEnemy.Start();
            }
        }

        private void ShootingEnemies()
        {
            foreach (var item in model.ShootingMonsters)
            {
                DispatcherTimer shootOneEnemy = new DispatcherTimer();
                shootOneEnemy.Tick += delegate
                {
                    Projectile enemyProjectile = gameLogic.EnemyShoot(item.Cords, rnd.Next(2, 4), item.Damage);

                    model.Projectiles.Add(enemyProjectile);
                    DispatcherTimer playerProjectileTimer = new DispatcherTimer();
                    playerProjectileTimer.Interval = TimeSpan.FromMilliseconds(20);
                    playerProjectileTimer.Tick += delegate
                    {
                        gameLogic.MoveProjectile(enemyProjectile);
                    };
                    playerProjectileTimer.Start();
                };
                shootOneEnemy.Interval = TimeSpan.FromMilliseconds(rnd.Next(3000, 5000));
                shootOneEnemy.Start();
            }
        }

        private void Left_MouseButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DispatcherTimer playerProjectileTimer = new DispatcherTimer();
            DispatcherTimer reloadTimer = new DispatcherTimer();
            if (!model.MyPlayer.IsReloading)
            {
                Point mousePos = e.GetPosition(this);
                model.MyPlayer.IsReloading = true;
                Projectile newProjectile = gameLogic.PlayerShoot(mousePos, 10);
                model.Projectiles.Add(newProjectile);

                playerProjectileTimer.Interval = TimeSpan.FromMilliseconds(15);
               
                playerProjectileTimer.Tick += new EventHandler((sender, e) => PlayerProjectileTimer_Tick(this, e, newProjectile));
                playerProjectileTimer.Start();

                reloadTimer.Tick += delegate
                {
                    model.MyPlayer.IsReloading = false;
                    reloadTimer.Stop();
                };
                reloadTimer.Interval = TimeSpan.FromMilliseconds(500 / model.MyPlayer.FiringSpeed);
                reloadTimer.Start();
            }
        }

        private void PlayerProjectileTimer_Tick(object sender, EventArgs e, Projectile projectile)
        {
            gameLogic.MoveProjectile(projectile);
        }

        private void Win_KeyDown(object sender, KeyEventArgs e)
        {
            DispatcherTimer moveOnce = new DispatcherTimer();
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

                    //case Key.Space:
                    //    DispatcherTimer playerProjectileTimer = new DispatcherTimer();
                    //    playerProjectileTimer.Stop();
                    //    DispatcherTimer reloadTimer = new DispatcherTimer();
                    //    reloadTimer.Stop();

                    //    if (!model.MyPlayer.IsReloading)
                    //    {
                    //        model.MyPlayer.IsReloading = true;
                    //        Point mousePos = Mouse.GetPosition(this);
                    //        Projectile newProjectile = gameLogic.PlayerShoot(mousePos, 5);
                    //        model.Projectiles.Add(newProjectile);

                    //        playerProjectileTimer.Interval = TimeSpan.FromMilliseconds(20);
                    //        /*playerProjectileTimer.Tick += delegate{
                    //            gameLogic.MoveProjectile(newProjectile);
                    //        };*/
                    //        playerProjectileTimer.Tick += new EventHandler((sender, e) => PlayerProjectileTimer_Tick(this, e, newProjectile));
                    //        playerProjectileTimer.Start();

                    //        reloadTimer.Tick += delegate
                    //        {
                    //            model.MyPlayer.IsReloading = false;
                    //            reloadTimer.Stop();
                    //        };
                    //        reloadTimer.Interval = TimeSpan.FromMilliseconds(500 / model.MyPlayer.FiringSpeed); //500
                    //        reloadTimer.Start();
                    //    }
                    //    break;
                    default:
                        break;
                }
            }
        }
    }
}