using System;
using Model;
using System.Windows.Media;
using System.Windows;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Reflection;

namespace Renderer
{
    public class GameRenderer
    {
        IGameModel model;
        Random rnd = new Random();

        Drawing oldBackground;
        Drawing oldLevelExit;
        Drawing oldPlayer;
        Drawing oldProjectiles;
        Drawing oldFlyingMonsters;
        Drawing oldShootingMonsters;
        Drawing oldTrackingMonsters;
        Drawing oldLavas;
        Drawing oldWaters;
        Drawing oldWalls;
        Drawing oldPowerups;
        Drawing oldBoss;

        Model.Ui.HealthBar hpBar = new Model.Ui.HealthBar();
        Drawing oldHpBar;
        int oldPlayerHealth;

        Model.Ui.LevelCounter lvlCounter = new Model.Ui.LevelCounter();
        Drawing oldLvlCounter;
        int oldModelLvlCounter;

        Drawing oldPauseScreen;

        Point oldBossPosition;
        List<Point> oldTrackingMonstersPosition = new List<Point>();
        List<Point> oldShootingMonstersPosition = new List<Point>();
        List<Point> oldFlyingMonstersPosition = new List<Point>();
        List<Point> oldProjectilePosition = new List<Point>();
        int powerupCount;
        Point oldPlayerPosition;
        Point oldMousePosition = new Point(0, 0);

        Pen Is = new Pen(Brushes.Black, 1);

        public GameRenderer(IGameModel model)
        {
            this.model = model;
        }

        internal static BitmapImage GetImage(string fileName)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = Assembly.LoadFrom("DungeonPenetrator").GetManifestResourceStream("DungeonPenetrator.Images." + fileName);
            bmp.EndInit();
            return bmp;
        }

        public Drawing BuildDrawing()
        {
            DrawingGroup dg = new DrawingGroup();
            dg.Children.Add(GetBackground());
            dg.Children.Add(GetLevelExit());
            dg.Children.Add(GetLavas());
            dg.Children.Add(GetWaters());
            dg.Children.Add(GetWalls());
            dg.Children.Add(GetTrackingMonsters());
            dg.Children.Add(GetPowerups());
            dg.Children.Add(GetFlyingMonsters());
            dg.Children.Add(GetProjectiles());
            dg.Children.Add(GetShootingMonsters());
            if (model.LevelCounter % 10 ==0)
            {
                dg.Children.Add(GetBoss());
            }
            dg.Children.Add(GetPlayer());
            dg.Children.Add(GetLevelCounter());
            dg.Children.Add(GetHpBar());
            dg.Children.Add(GetPauseScreen());
            
            return dg;
        }

        private Drawing GetPauseScreen()
        {
            if (!model.GameIsPaused)
            {
                /*ImageDrawing drawing = new ImageDrawing(GetImage("stopped.png"), new Rect(1000, 1000, model.GameWidth, model.GameHeight));
                oldPauseScreen = drawing;*/
                return new ImageDrawing();
            }
            if (model.GameIsPaused)
            {
                ImageDrawing drawing = new ImageDrawing(GetImage("stopped.png"), new Rect(0, 0, model.GameWidth, model.GameHeight));
                oldPauseScreen = drawing;
            }
           
            return oldPauseScreen;
        }

        private Drawing GetLevelCounter()
        {
            DrawingGroup group = new DrawingGroup();
            if (oldLvlCounter == null || model.LevelCounter != oldModelLvlCounter)
            {
                oldModelLvlCounter = model.LevelCounter;
                GeometryDrawing box = new GeometryDrawing(Brushes.Gray, Is, new RectangleGeometry(new Rect(lvlCounter.LvlCounterX, lvlCounter.LvlCounterY, lvlCounter.LvlCounterWidth, lvlCounter.LvlCounterHeight)));

                FormattedText text = new FormattedText(oldModelLvlCounter.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 30, Brushes.Black);
                text.TextAlignment = TextAlignment.Center;
                Geometry geo = text.BuildGeometry(new Point(lvlCounter.LvlCounterX + (lvlCounter.LvlCounterWidth / 2), lvlCounter.LvlCounterY + (lvlCounter.LvlCounterHeight / 4.2)));
                GeometryDrawing textGeo = new GeometryDrawing(Brushes.Black, null, geo);

                group.Children.Add(box);
                group.Children.Add(textGeo);

                oldLvlCounter = group;
            }
            return oldLvlCounter;
        }

        private Drawing GetHpBar()
        {
            if (oldHpBar == null || model.MyPlayer.Health != oldPlayerHealth)
            {
                oldPlayerHealth = model.MyPlayer.Health;
                Geometry g = new RectangleGeometry(new Rect(hpBar.HpBarX, hpBar.HpBarY, hpBar.HpWidth, oldPlayerHealth * 7));
                g.Transform = new RotateTransform(180, hpBar.HpBarX + hpBar.HpWidth/2, hpBar.HpBarY + hpBar.HpHeight/2);
                oldHpBar = new GeometryDrawing(Brushes.Red, Is, g);
            }
            return oldHpBar;
        }

        private Drawing GetBackground()
        {
            if (oldBackground == null)
            {
                int random = rnd.Next(0, 4);
                ImageDrawing drawing;
                switch (random)
                {
                    case 1:
                        drawing = new ImageDrawing(GetImage("bggreen.png"), new Rect(0, 0, model.GameWidth, model.GameHeight));
                        break;
                    case 2:
                        drawing = new ImageDrawing(GetImage("backgroundtan.png"), new Rect(0, 0, model.GameWidth, model.GameHeight));
                        break;
                    case 3:
                        drawing = new ImageDrawing(GetImage("backgroundsnowy.png"), new Rect(0, 0, model.GameWidth, model.GameHeight));
                        break;
                    default:
                        drawing = new ImageDrawing(GetImage("bggreen.png"), new Rect(0, 0, model.GameWidth, model.GameHeight));
                        break;
                }
                
                oldBackground = drawing;
            }

            return oldBackground;
        }

        private Drawing GetLevelExit()
        {
            DrawingGroup g = new DrawingGroup();
            if (oldLevelExit == null || ((model.ShootingMonsters.Count == 0 && model.TrackingMonsters.Count == 0 && model.FlyingMonsters.Count == 0)&& model.Boss==null) )
            {
                ImageDrawing drawing = new ImageDrawing(GetImage("goal.png"), new Rect(model.LevelExit.X * GameModel.TileSize,
                           model.LevelExit.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));

                g.Children.Add(drawing);
                oldLevelExit = g;
            }
            else
            {
                ImageDrawing drawing = new ImageDrawing(GetImage("goallocked.png"), new Rect(model.LevelExit.X * GameModel.TileSize,
                           model.LevelExit.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));

                g.Children.Add(drawing);
                oldLevelExit = g;
            }
            return oldLevelExit;
        }

        private Drawing GetLavas()
        {
            if (oldLavas == null)
            {
                DrawingGroup g = new DrawingGroup();
                foreach (var lava in model.Lavas)
                {
                    ImageDrawing drawing = new ImageDrawing(GetImage("lava.png"), new Rect(lava.Cords.X * GameModel.TileSize,
                        lava.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                    g.Children.Add(drawing);
                }
                oldLavas = g;
            }
            return oldLavas;
        }

        private Drawing GetWaters()
        {
            if (oldWaters == null)
            {
                DrawingGroup g = new DrawingGroup();
                foreach (var water in model.Waters)
                {
                    ImageDrawing drawing = new ImageDrawing(GetImage("water2.png"), new Rect(water.Cords.X * GameModel.TileSize,
                        water.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                    g.Children.Add(drawing);
                }
                oldWaters = g;
            }
            return oldWaters;
        }

        private Drawing GetWalls()
        {
            if (oldWalls == null)
            {
                DrawingGroup g = new DrawingGroup();
                foreach (var wall in model.Walls)
                {
                    ImageDrawing drawing = new ImageDrawing(GetImage("rock100.png"), new Rect(wall.Cords.X * GameModel.TileSize,
                        wall.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                    g.Children.Add(drawing);
                }
                oldWalls = g;
            }
            return oldWalls;
        }

        private Drawing GetShootingMonsters()
        {
            DrawingGroup g = new DrawingGroup();
            foreach (var enemy in model.ShootingMonsters)
            {
                if (oldShootingMonsters == null || !oldShootingMonstersPosition.Contains(enemy.Cords))
                {
                    Point p = new Point((model.MyPlayer.Cords.X * GameModel.TileSize) - (enemy.Cords.X * GameModel.TileSize), (model.MyPlayer.Cords.Y * GameModel.TileSize) - (enemy.Cords.Y * GameModel.TileSize));
                    double rotation = Math.Atan2(p.Y, p.X) * 180 / Math.PI;
                    BitmapImage bmp = GetImage("ct100big.png");
                    TransformedBitmap tb = new TransformedBitmap();
                    tb.BeginInit();
                    tb.Source = bmp;
                    tb.Transform = new RotateTransform(90);
                    tb.EndInit();

                    ImageDrawing drawing = new ImageDrawing(tb, new Rect(enemy.Cords.X * GameModel.TileSize,
                        enemy.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));

                    RotateTransform rotate = new RotateTransform(rotation, (enemy.Cords.X * GameModel.TileSize) + GameModel.TileSize/2 , (enemy.Cords.Y * GameModel.TileSize) + GameModel.TileSize / 2);

                    g.Children.Add(new DrawingGroup() { Children = { drawing }, Transform = rotate });
                }
            }
            oldShootingMonsters = g;
            return oldShootingMonsters;
        }

        private Drawing GetTrackingMonsters()
        {
            DrawingGroup g = new DrawingGroup();
            foreach (var enemy in model.TrackingMonsters)
            {
                if (oldTrackingMonsters == null || !oldTrackingMonstersPosition.Contains(enemy.Cords))
                {
                    BitmapImage bmp = GetImage("hoodtracker100100.png");
                    TransformedBitmap tb = new TransformedBitmap();
                    tb.BeginInit();
                    tb.Source = bmp;
                    switch (model.BasicTrackingPath[enemy.Cords])
                    {
                        case { } Point when Point == new Point(1, 0):
                            tb.Transform = new RotateTransform(270);
                            break;
                        case { } Point when Point == new Point(0, 1):
                            tb.Transform = new RotateTransform(0);
                            break;
                        case { } Point when Point == new Point(-1, 0):
                            tb.Transform = new RotateTransform(90);
                            break;
                        case { } Point when Point == new Point(0, -1):
                            tb.Transform = new RotateTransform(180);
                            break;
                        default:
                            break;
                    }
                    tb.EndInit();

                    ImageDrawing drawing = new ImageDrawing(tb, new Rect(enemy.Cords.X * GameModel.TileSize,
                        enemy.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));

                    g.Children.Add(drawing);
                }
            }
            oldTrackingMonsters = g;
            return oldTrackingMonsters;
        }

        private Drawing GetPowerups()
        {
            if (oldPowerups == null || powerupCount!=model.Powerups.Count)
            {
                DrawingGroup g = new DrawingGroup();
                foreach (var powerup in model.Powerups)
                {
                    ImageDrawing drawing;
                    switch (powerup.Type)
                    {
                        case Model.Passive.PowerupType.Health:
                            drawing = new ImageDrawing(GetImage("redpotion.png"), new Rect(powerup.Cords.X * GameModel.TileSize,
                                powerup.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                            break;
                        case Model.Passive.PowerupType.Damage:
                            drawing = new ImageDrawing(GetImage("bluepotion.png"), new Rect(powerup.Cords.X * GameModel.TileSize,
                                powerup.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                            break;
                        case Model.Passive.PowerupType.FiringSpeed:
                            drawing = new ImageDrawing(GetImage("yellowpotion.png"), new Rect(powerup.Cords.X * GameModel.TileSize,
                                powerup.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                            break;
                        default:
                            drawing = new ImageDrawing(GetImage("error-icon-32.png"), new Rect(powerup.Cords.X * GameModel.TileSize,
                                powerup.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                            break;       
                    }
                    g.Children.Add(drawing);
                }
                if (powerupCount != model.Powerups.Count)
                {
                    powerupCount = model.Powerups.Count;
                }
                oldPowerups = g;
            }
            return oldPowerups;
        }

        private Drawing GetFlyingMonsters()
        {
            DrawingGroup g = new DrawingGroup();
            foreach (var enemy in model.FlyingMonsters)
            {
                if (oldFlyingMonsters == null || !oldFlyingMonstersPosition.Contains(enemy.Cords))
                {
                    BitmapImage bmp = GetImage("missle100.png");
                    TransformedBitmap tb = new TransformedBitmap();
                    tb.BeginInit();
                    tb.Source = bmp;
                    switch (model.FlyingTrackingPath[enemy.Cords])
                    {
                        case { } Point when Point == new Point(1, 0):
                            tb.Transform = new RotateTransform(90);
                            break;
                        case { } Point when Point == new Point(0, 1):
                            tb.Transform = new RotateTransform(180);
                            break;
                        case { } Point when Point == new Point(-1, 0):
                            tb.Transform = new RotateTransform(270);
                            break;
                        case { } Point when Point == new Point(0, -1):
                            tb.Transform = new RotateTransform(0);
                            break;
                        default:
                            break;
                    }
                    tb.EndInit();

                    ImageDrawing drawing = new ImageDrawing(tb, new Rect(enemy.Cords.X * GameModel.TileSize,
                        enemy.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));

                    g.Children.Add(drawing);
                }
            }
            oldFlyingMonsters = g;
            return oldFlyingMonsters;
        }

        private Drawing GetBoss()
        {
            DrawingGroup g = new DrawingGroup();
            if (oldBoss == null || model.Boss != null && oldBossPosition != model.Boss.Cords)
            {
                    ImageDrawing drawing = new ImageDrawing(GetImage("hoodghost.png"), new Rect((model.Boss.Cords.X * GameModel.TileSize) - ((GameModel.TileSize*3) / 2),
                        (model.Boss.Cords.Y * GameModel.TileSize) - ((GameModel.TileSize * 3) / 2), GameModel.TileSize*3, GameModel.TileSize*3));

                    g.Children.Add(drawing);

                    oldBoss = g;
                    oldBossPosition = model.Boss.Cords;
            }
            if (model.Boss==null)
            {
                return g;
            }
            return oldBoss;
        }

        private Drawing GetProjectiles()
        {
            DrawingGroup g = new DrawingGroup();
            try
            {
                ImageDrawing drawing;
                foreach (var projectile in model.Projectiles)
                {
                    if (oldProjectiles == null || !oldProjectilePosition.Contains(projectile.Cords))
                    {
                        switch (projectile.Type)
                        {
                            case ProjectileType.Enemy:
                                drawing = new ImageDrawing(GetImage("bullet3.png"), new Rect(projectile.Cords.X,
                               projectile.Cords.Y, 10, 10));
                                break;
                            case ProjectileType.Player:
                                drawing = new ImageDrawing(GetImage("bullet3.png"), new Rect(projectile.Cords.X,
                               projectile.Cords.Y, 10, 10));
                                break;
                            case ProjectileType.Boss:
                                drawing = new ImageDrawing(GetImage("bossbullet.png"), new Rect(projectile.Cords.X,
                               projectile.Cords.Y, 40, 40));
                                break;
                            default:
                                drawing = new ImageDrawing(GetImage("bullet3.png"), new Rect(projectile.Cords.X,
                               projectile.Cords.Y, 10, 10));
                                break;
                        }
                        

                        g.Children.Add(drawing);
                    }
                }
                oldProjectiles = g;
                return oldProjectiles;
            }
            catch
            {
                return g;
            }
        }

        private Drawing GetPlayer()
        {
            DrawingGroup g = new DrawingGroup();
            if (oldPlayer == null || oldPlayerPosition != model.MyPlayer.Cords || model.mousePosition != oldMousePosition)
            {
                
                Point p = new Point(model.mousePosition.X - (model.MyPlayer.Cords.X * GameModel.TileSize), model.mousePosition.Y - (model.MyPlayer.Cords.Y * GameModel.TileSize));
                double rotation = Math.Atan2(p.Y, p.X) * 180 / Math.PI;

                BitmapImage bmp = GetImage("100.png");
                TransformedBitmap tb = new TransformedBitmap();
                tb.BeginInit();
                tb.Source = bmp;
                tb.Transform = new RotateTransform(90);
                tb.EndInit();

                ImageDrawing drawing = new ImageDrawing(tb, new Rect(model.MyPlayer.Cords.X * GameModel.TileSize,
                    model.MyPlayer.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));

                RotateTransform rotate = new RotateTransform(rotation, (model.MyPlayer.Cords.X * GameModel.TileSize) + GameModel.TileSize / 2, (model.MyPlayer.Cords.Y * GameModel.TileSize) + GameModel.TileSize / 2);

                g.Children.Add(new DrawingGroup() { Children = { drawing }, Transform = rotate });

                oldPlayer = g;
                oldPlayerPosition = model.MyPlayer.Cords;
            }
            return oldPlayer;
        }
    }
}
