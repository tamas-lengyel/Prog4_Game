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

        Dictionary<string, Brush> brushes = new Dictionary<string, Brush>();

        Pen Is = new Pen(Brushes.Black, 1);

        Brush BackgroundBrush { get { return GetBrush("snowy.png", true); } }

        Brush GetBrush(string fname, bool isTiled)
        {
            if (!brushes.ContainsKey(fname))
            {
                //ImageBrush ib= new ImageBrush(new BitmapImage(new Uri(xxxx)));
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = Assembly.LoadFrom("DungeonPenetrator").GetManifestResourceStream("DungeonPenetrator.Images." + fname);
                bmp.EndInit();
                ImageBrush ib = new ImageBrush(bmp);

                if (isTiled)
                {
                    ib.TileMode = TileMode.Tile;
                    ib.Viewport = new Rect(0, 0, GameModel.TileSize, GameModel.TileSize);
                    ib.ViewboxUnits = BrushMappingMode.Absolute;
                    // ib.ViewBox - ine img = many textures
                }

                brushes.Add(fname, ib);
            }
            return brushes[fname];
        }

        //internal static BitmapImage GetImage(string fileName, bool isTiled)
        //{
        //    BitmapImage bmp = new BitmapImage();
        //    bmp.BeginInit();
        //    bmp.StreamSource = Assembly.LoadFrom("DungeonPenetrator").GetManifestResourceStream("DungeonPenetrator.Images." + fileName);
        //    bmp.EndInit();
        //    ImageBrush ib = new ImageBrush(bmp);

        //    if (isTiled)
        //    {
        //        ib.TileMode = TileMode.Tile;
        //        ib.Viewport = new Rect(0, 0, GameModel.TileSize, GameModel.TileSize);
        //        ib.ViewboxUnits = BrushMappingMode.Absolute;
        //        // ib.ViewBox - ine img = many textures
        //    }
        //    return bmp;
        //}

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
                //Geometry g = new RectangleGeometry(new Rect(0, 0, model.GameWidth, model.GameHeight));
                GeometryDrawing box = new GeometryDrawing(Brushes.Green, Is, new RectangleGeometry(new Rect(0,
                           0, model.GameWidth, model.GameHeight)));
                //ImageDrawing background = new ImageDrawing(BackgroundBrush, new Rect(0, 0, model.GameWidth, model.GameHeight));
                //oldBackground = new GeometryDrawing(BackgroundBrush, null, g);

                oldBackground = box;
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
                GeometryGroup g = new GeometryGroup();
                foreach (var lava in model.Lavas)
                {
                    Geometry box = new RectangleGeometry(new Rect(lava.Cords.X * GameModel.TileSize,
                        lava.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                    g.Children.Add(box);
                }
                oldLavas = new GeometryDrawing(Brushes.Orange, Is, g);
            }
            return oldLavas;
        }

        private Drawing GetWaters()
        {
            if (oldWaters == null)
            {
                GeometryGroup g = new GeometryGroup();
                foreach (var water in model.Waters)
                {
                    Geometry box = new RectangleGeometry(new Rect(water.Cords.X * GameModel.TileSize,
                        water.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                    g.Children.Add(box);
                }
                oldWaters = new GeometryDrawing(Brushes.LightBlue, Is, g);
            }
            return oldWaters;
        }

        private Drawing GetWalls()
        {
            if (oldWalls == null)
            {
                GeometryGroup g = new GeometryGroup();
                foreach (var wall in model.Walls)
                {
                    Geometry box = new RectangleGeometry(new Rect(wall.Cords.X * GameModel.TileSize,
                        wall.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                    g.Children.Add(box);
                }
                oldWalls = new GeometryDrawing(Brushes.Gray, Is, g);
            }
            return oldWalls;
        }

        private Drawing GetShootingMonsters()
        {
            GeometryGroup g = new GeometryGroup();
            foreach (var enemy in model.ShootingMonsters)
            {
                if (oldShootingMonsters == null || !oldShootingMonstersPosition.Contains(enemy.Cords))
                {
                    Geometry box = new RectangleGeometry(new Rect(enemy.Cords.X * GameModel.TileSize,
                        enemy.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                    g.Children.Add(box);
                }
            }
            oldShootingMonsters = new GeometryDrawing(Brushes.Yellow, Is, g);
            return oldShootingMonsters;
        }

        private Drawing GetTrackingMonsters()
        {
            GeometryGroup g = new GeometryGroup();
            foreach (var enemy in model.TrackingMonsters)
            {
                if (oldTrackingMonsters == null || !oldTrackingMonstersPosition.Contains(enemy.Cords))
                {
                    Geometry box = new RectangleGeometry(new Rect(enemy.Cords.X * GameModel.TileSize,
                        enemy.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                    g.Children.Add(box);
                }
            }
            oldTrackingMonsters = new GeometryDrawing(Brushes.DarkRed, Is, g);
            return oldTrackingMonsters;
        }

        private Drawing GetPowerups()
        {
            if (oldPowerups == null)
            {
                DrawingGroup g = new DrawingGroup();
                foreach (var powerup in model.Powerups)
                {
                    FormattedText text;
                    GeometryDrawing box;
                    Geometry geo;
                    GeometryDrawing textGeo;
                    switch (powerup.Type)
                    {
                        case Model.Passive.PowerupType.Health:
                            box = new GeometryDrawing(Brushes.Purple, Is, new RectangleGeometry(new Rect(powerup.Cords.X * GameModel.TileSize,
                           powerup.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize)));
                            text = new FormattedText("H", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 30, Brushes.Black);
                            text.TextAlignment = TextAlignment.Center;
                            geo = text.BuildGeometry(new Point((powerup.Cords.X * GameModel.TileSize) + (GameModel.TileSize / 2), (powerup.Cords.Y * GameModel.TileSize) + (GameModel.TileSize / 3)));
                            textGeo = new GeometryDrawing(Brushes.Black, null, geo);
                            break;
                        case Model.Passive.PowerupType.Damage:
                            box = new GeometryDrawing(Brushes.Purple, Is, new RectangleGeometry(new Rect(powerup.Cords.X * GameModel.TileSize,
                           powerup.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize)));
                            text = new FormattedText("D", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 30, Brushes.Black);
                            text.TextAlignment = TextAlignment.Center;
                            geo = text.BuildGeometry(new Point((powerup.Cords.X * GameModel.TileSize) + (GameModel.TileSize / 2), (powerup.Cords.Y * GameModel.TileSize) + (GameModel.TileSize / 3)));
                            textGeo = new GeometryDrawing(Brushes.Black, null, geo);
                            break;
                        case Model.Passive.PowerupType.FiringSpeed:
                            box = new GeometryDrawing(Brushes.Purple, Is, new RectangleGeometry(new Rect(powerup.Cords.X * GameModel.TileSize,
                           powerup.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize)));
                            text = new FormattedText("R", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 30, Brushes.Black);
                            text.TextAlignment = TextAlignment.Center;
                            geo = text.BuildGeometry(new Point((powerup.Cords.X * GameModel.TileSize) + (GameModel.TileSize / 2), (powerup.Cords.Y * GameModel.TileSize) + (GameModel.TileSize / 3)));
                            textGeo = new GeometryDrawing(Brushes.Black, null, geo);
                            break;
                        default:
                            box = new GeometryDrawing(Brushes.Purple, Is, new RectangleGeometry(new Rect(powerup.Cords.X * GameModel.TileSize,
                         powerup.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize)));
                            text = new FormattedText("ERROR", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 30, Brushes.Black);
                            text.TextAlignment = TextAlignment.Center;
                            geo = text.BuildGeometry(new Point((powerup.Cords.X * GameModel.TileSize) + (GameModel.TileSize / 2), (powerup.Cords.Y * GameModel.TileSize) + (GameModel.TileSize / 3)));
                            textGeo = new GeometryDrawing(Brushes.Black, null, geo);
                            break;
                    }


                    g.Children.Add(box);
                    g.Children.Add(textGeo);
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
                    GeometryDrawing box = new GeometryDrawing(Brushes.Blue, Is, new RectangleGeometry(new Rect(enemy.Cords.X * GameModel.TileSize,
                           enemy.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize)));

                    FormattedText text = new FormattedText("F", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 30, Brushes.Black);
                    text.TextAlignment = TextAlignment.Center;
                    Geometry geo = text.BuildGeometry(new Point((enemy.Cords.X * GameModel.TileSize) + (GameModel.TileSize / 2), (enemy.Cords.Y * GameModel.TileSize) + (GameModel.TileSize / 3)));
                    GeometryDrawing textGeo = new GeometryDrawing(Brushes.Black, null, geo);

                    g.Children.Add(box);
                    g.Children.Add(textGeo);
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
            if (oldPlayer == null || oldPlayerPosition != model.MyPlayer.Cords)
            {
                Geometry g = new RectangleGeometry(new Rect(model.MyPlayer.Cords.X * GameModel.TileSize,
                    model.MyPlayer.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                oldPlayer = new GeometryDrawing(Brushes.Red, Is, g);
                oldPlayerPosition = model.MyPlayer.Cords;
            }
            return oldPlayer;
        }
    }
}
