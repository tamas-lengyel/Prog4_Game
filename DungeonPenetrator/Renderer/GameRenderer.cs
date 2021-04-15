using System;
using Model;
using System.Windows.Media;
using System.Windows;
using System.Collections.Generic;
using System.Globalization;

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


        Pen Is = new Pen(Brushes.Black, 1);

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
                oldBackground = new GeometryDrawing(Brushes.DarkOliveGreen, null, g);
            }
            return oldBackground;
        }

        private Drawing GetLevelExit()
        {
            if (oldLevelExit == null)
            {
                Geometry g = new RectangleGeometry(new Rect(model.LevelExit.X * model.TileSize, 
                    model.LevelExit.Y * model.TileSize, model.TileSize, model.TileSize));
                oldLevelExit = new GeometryDrawing(Brushes.Pink, Is, g);
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
                    Geometry box = new RectangleGeometry(new Rect(lava.Cords.X * model.TileSize, 
                        lava.Cords.Y * model.TileSize, model.TileSize, model.TileSize));
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
                    Geometry box = new RectangleGeometry(new Rect(water.Cords.X * model.TileSize, 
                        water.Cords.Y * model.TileSize, model.TileSize, model.TileSize));
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
                    Geometry box = new RectangleGeometry(new Rect(wall.Cords.X * model.TileSize, 
                        wall.Cords.Y * model.TileSize, model.TileSize, model.TileSize));
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
                    Geometry box = new RectangleGeometry(new Rect(enemy.Cords.X * model.TileSize,
                        enemy.Cords.Y * model.TileSize, model.TileSize, model.TileSize));
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
                    Geometry box = new RectangleGeometry(new Rect(enemy.Cords.X * model.TileSize,
                        enemy.Cords.Y * model.TileSize, model.TileSize, model.TileSize));
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
                    GeometryDrawing box = new GeometryDrawing(Brushes.Purple, Is, new RectangleGeometry(new Rect(powerup.Cords.X * model.TileSize,
                           powerup.Cords.Y * model.TileSize, model.TileSize, model.TileSize)));

                    FormattedText text = new FormattedText("H", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 30, Brushes.Black);
                    text.TextAlignment = TextAlignment.Center;
                    Geometry geo = text.BuildGeometry(new Point((powerup.Cords.X * model.TileSize) + (model.TileSize / 2), (powerup.Cords.Y * model.TileSize) + (model.TileSize / 3)));
                    GeometryDrawing textGeo = new GeometryDrawing(Brushes.Black, null, geo);

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
                    //Geometry box = new RectangleGeometry(new Rect(enemy.Cords.X * model.TileSize,
                    //       enemy.Cords.Y * model.TileSize, model.TileSize, model.TileSize));

                    GeometryDrawing box = new GeometryDrawing(Brushes.Blue, Is, new RectangleGeometry(new Rect(enemy.Cords.X * model.TileSize,
                           enemy.Cords.Y * model.TileSize, model.TileSize, model.TileSize)));

                    FormattedText text = new FormattedText("F", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 30, Brushes.Black);
                    text.TextAlignment = TextAlignment.Center;
                    Geometry geo = text.BuildGeometry(new Point((enemy.Cords.X * model.TileSize) + (model.TileSize /2), (enemy.Cords.Y * model.TileSize) + (model.TileSize / 3)));
                    GeometryDrawing textGeo = new GeometryDrawing(Brushes.Black, null, geo);

                    //g.Children.Add(box);
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
                //Geometry g = new RectangleGeometry(new Rect(model.Boss.Cords.X * model.TileSize, 
                //    model.Boss.Cords.Y * model.TileSize, model.TileSize, model.TileSize));
                //oldBoss = new GeometryDrawing(Brushes.Chocolate, Is, g);
                //oldBossPosition = model.Boss.Cords;
            }
            return oldBoss;
        }

        private Drawing GetProjectiles()
        {
            GeometryGroup g = new GeometryGroup();
            foreach (var projectile in model.Projectiles)
            {
                if (oldProjectiles == null || !oldProjectilePosition.Contains(projectile.Cords))
                {
                    Geometry box = new RectangleGeometry(new Rect(projectile.Cords.X * model.TileSize,
                           projectile.Cords.Y * model.TileSize, model.TileSize/10, model.TileSize/10));
                    g.Children.Add(box);
                }
            }
            oldProjectiles = new GeometryDrawing(Brushes.White, Is, g);
            return oldProjectiles;
        }

        private Drawing GetPlayer()
        {
            if (oldPlayer == null || oldPlayerPosition != model.MyPlayer.Cords)
            {
                Geometry g = new RectangleGeometry(new Rect(model.MyPlayer.Cords.X * model.TileSize, 
                    model.MyPlayer.Cords.Y * model.TileSize, model.TileSize, model.TileSize));
                oldPlayer = new GeometryDrawing(Brushes.Red, Is, g);
                oldPlayerPosition = model.MyPlayer.Cords;
            }
            return oldPlayer;
        }
    }
}
