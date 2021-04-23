using Logic;
using Model;
using Renderer;
using Repository;
using System;
using System.Media;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
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
        DispatcherTimer moveOnce;
        DispatcherTimer reloadTimer;
        DispatcherTimer levelTimer;
        private Thread soundPlayThread;
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
            //MoveEnemies();
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
            levelTimer = new DispatcherTimer();
            levelTimer.Interval = TimeSpan.FromMilliseconds(200);
            levelTimer.Tick += LevelTimer_Tick;
            levelTimer.Start();
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
                //shootOnce.Start();
                //moveOnce.Start();
            }
        }

        private void ShootingEnemies(object sender, EventArgs e)
        {
            if (model.ShootingMonsters.Count != 0)
            {
                int rndEnemy = rnd.Next(0, model.ShootingMonsters.Count);
                Projectile enemyProjectile = gameLogic.EnemyShoot(model.ShootingMonsters[rndEnemy].Cords, rnd.Next(6, 8), model.ShootingMonsters[rndEnemy].Damage);
                /*soundPlayThread = new Thread(() => {
                    SoundPlayer sp = new System.Media.SoundPlayer(Assembly.LoadFrom("DungeonPenetrator").GetManifestResourceStream("DungeonPenetrator.Images.piu.wav"));
                    sp.PlaySync();
                });
                soundPlayThread.IsBackground = true;
                soundPlayThread.Start();*/

                MediaPlayer mp = new MediaPlayer();
                mp.Open(new Uri("pack://application:,,,/Images/piu.wav"));
                //mp.Open(Assembly.LoadFrom("DungeonPenetrator").GetManifestResourceStream("DungeonPenetrator.Images.piu.wav"); 
                mp.Play();


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
            moveOnce.Start();
            
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
            moveOnce.Stop();
        }
        
        private void Left_MouseButtonDown(object sender,MouseButtonEventArgs e)
        {
            if (!model.MyPlayer.IsReloading && !model.GameIsPaused)
            {
                reloadTimer = new DispatcherTimer();
                Point mousePos = new Point(e.GetPosition((IInputElement)sender).X, e.GetPosition((IInputElement)sender).Y);
                model.MyPlayer.IsReloading = true;
                gameLogic.PlayerShoot(mousePos, 10);


                
                //System.Media.SoundPlayer sound = new System.Media.SoundPlayer(Assembly.LoadFrom("DungeonPenetrator").GetManifestResourceStream("DungeonPenetrator.Images.piu.wav"));
                /*soundPlayThread = new Thread(() => {
                    new System.Media.SoundPlayer(Assembly.LoadFrom("DungeonPenetrator").GetManifestResourceStream("DungeonPenetrator.Images.piu2.wav")).Play();
                });
                soundPlayThread.IsBackground = true;
                soundPlayThread.Start();*/
                
              
                reloadTimer.Tick += delegate
                {
                    model.MyPlayer.IsReloading = false;
                    reloadTimer.Stop();
                };
                reloadTimer.Interval = TimeSpan.FromMilliseconds(500 / model.MyPlayer.FiringSpeed);
                reloadTimer.Start();
            }
        }

        private void StartSoundPlay()
        {
                var play= new Thread(playpiu);
                play.Name = "soundplay";
                play.IsBackground = true;
                play.Start();
        }
        private void playpiu()
        {
            
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
                    case Key.Space:
                        //foreach (var item in model.Projectiles)
                        //{
                        //    item.Timer.Stop();
                        //    item.Timer = null;
                        //}
                        //shootOnce.Stop();
                        //moveOnce.Stop();
                        //updateTimer.Stop();
                        //if (reloadTimer != null)
                        //{
                        //    reloadTimer.Stop();
                        //}
                        //gameLogic = null;
                        //renderer = null;
                        //loadigLogic.NextLevel();
                        //// gameLogic = new GameLogic(model);
                        //// renderer = new GameRenderer(model);
                        ///*shootOnce = new DispatcherTimer();
                        //moveOnce = new DispatcherTimer();
                        ////reloadTimer = new DispatcherTimer();
                        //shootOnce.Interval = TimeSpan.FromMilliseconds(rnd.Next(1000, 2000));
                        //shootOnce.Tick += ShootingEnemies;
                        //moveOnce.Interval = TimeSpan.FromMilliseconds(rnd.Next(200, 500));
                        //moveOnce.Tick += MoveEnemies;*/
                        ///*if (reloadTimer != null)
                        //    reloadTimer.Start();*/
                        //updateTimer.Start();
                        //gameLogic = new GameLogic(model);
                        //renderer = new GameRenderer(model);
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
                    //Window win = Window.GetWindow(this);
                    //win.MouseLeftButtonDown += Left_MouseButtonDown;
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
                    //Window win = Window.GetWindow(this);
                    //win.MouseLeftButtonDown -= Left_MouseButtonDown;
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