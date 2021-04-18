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

        Point oldBossPosition;
        List<Point> oldTrackingMonstersPosition = new List<Point>();
        List<Point> oldShootingMonstersPosition = new List<Point>();
        List<Point> oldFlyingMonstersPosition = new List<Point>();
        List<Point> oldProjectilePosition = new List<Point>();
        Point oldPlayerPosition;
        Point oldMousePosition;

        Dictionary<string, Brush> brushes = new Dictionary<string, Brush>();

        Pen Is = new Pen(Brushes.Black, 1);

        Brush BackgroundBrush { get { return GetBrush("bg3.png", false); } }
        Brush PlayerBrush { get { return GetBrush("characterblue100.png", false); } }

        Brush GetBrush(string fname, bool isTiled)
        {
            if (!brushes.ContainsKey(fname))
            {
                //ImageBrush ib = new ImageBrush(new BitmapImage(new Uri(xxxx)));
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = Assembly.LoadFrom("DungeonPenetrator").GetManifestResourceStream("DungeonPenetrator.Images." + fname);
                bmp.EndInit();
                ImageBrush ib = new ImageBrush(bmp);

                brushes.Add(fname, ib);
            }
            return brushes[fname];
        }

        internal static BitmapImage GetImage(string fileName)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = Assembly.LoadFrom("DungeonPenetrator").GetManifestResourceStream("DungeonPenetrator.Images." + fileName);
            bmp.EndInit();
            //bmp.Rotation = Rotation.Rotate90;
            return bmp;
        }

        public GameRenderer(IGameModel model)
        {
            this.model = model;
        }

        public Drawing BuildDrawing()
        {
            DrawingGroup dg = new DrawingGroup();
            dg.Children.Add(GetBackground());
            dg.Children.Add(GetLevelExit());
            dg.Children.Add(GetLavas());
            dg.Children.Add(GetWaters());
            dg.Children.Add(GetWalls());
            dg.Children.Add(GetShootingMonsters());
            dg.Children.Add(GetTrackingMonsters());
            dg.Children.Add(GetPowerups());
            dg.Children.Add(GetFlyingMonsters());
            //dg.Children.Add(GetBoss());
            dg.Children.Add(GetProjectiles());
            dg.Children.Add(GetPlayer());
            return dg;
        }

        private Drawing GetBackground()
        {
            if (oldBackground == null)
            {
                Geometry g = new RectangleGeometry(new Rect(0, 0, model.GameWidth, model.GameHeight));
                oldBackground = new GeometryDrawing(BackgroundBrush, null, g);
            }
            
            return oldBackground;
        }

        private Drawing GetLevelExit()
        {
            if (oldLevelExit == null)
            {
                DrawingGroup g = new DrawingGroup();
                GeometryDrawing box = new GeometryDrawing(Brushes.HotPink, Is, new RectangleGeometry(new Rect(model.LevelExit.X * GameModel.TileSize,
                           model.LevelExit.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize)));
                FormattedText text = new FormattedText("G", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 30, Brushes.Black);
                text.TextAlignment = TextAlignment.Center;
                Geometry geo = text.BuildGeometry(new Point((model.LevelExit.X * GameModel.TileSize) + (GameModel.TileSize / 2), (model.LevelExit.Y * GameModel.TileSize) + (GameModel.TileSize / 3)));
                GeometryDrawing textGeo = new GeometryDrawing(Brushes.Black, null, geo);
                g.Children.Add(box);
                g.Children.Add(textGeo);
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
                    //Geometry box = new RectangleGeometry(new Rect(lava.Cords.X * GameModel.TileSize,
                    //    lava.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
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
                    //Geometry box = new RectangleGeometry(new Rect(water.Cords.X * GameModel.TileSize,
                    //    water.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                    ImageDrawing drawing = new ImageDrawing(GetImage("water.png"), new Rect(water.Cords.X * GameModel.TileSize,
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
                    //Geometry box = new RectangleGeometry(new Rect(wall.Cords.X * GameModel.TileSize,
                    //    wall.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
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
            if (oldPowerups == null)
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
            if (oldBoss == null || oldBossPosition != model.Boss.Cords)
            {
                //Geometry g = new RectangleGeometry(new Rect(model.Boss.Cords.X * GameModel.TileSize, 
                //    model.Boss.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                //oldBoss = new GeometryDrawing(Brushes.Chocolate, Is, g);
                //oldBossPosition = model.Boss.Cords;
            }
            return oldBoss;
        }

        private Drawing GetProjectiles()
        {
            DrawingGroup g = new DrawingGroup();
            foreach (var projectile in model.Projectiles)
            {
                if (oldFlyingMonsters == null || !oldFlyingMonstersPosition.Contains(projectile.Cords))
                {
                    GeometryDrawing box = new GeometryDrawing(Brushes.Red, Is, new RectangleGeometry(new Rect(projectile.Cords.X,
                           projectile.Cords.Y, 10, 10)));
                    
                    g.Children.Add(box);
                }
            }
            oldProjectiles = g;
            return oldProjectiles;

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

                //Geometry g = new RectangleGeometry(new Rect(model.MyPlayer.Cords.X * GameModel.TileSize,
                //    model.MyPlayer.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));

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
