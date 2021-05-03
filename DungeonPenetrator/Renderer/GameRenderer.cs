// <copyright file="GameRenderer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

[assembly: System.CLSCompliant(false)]

namespace Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Model;

    /// <summary>
    /// GameRenderer class.
    /// </summary>
    public class GameRenderer
    {
        private IGameModel model;

        private Drawing oldBackground;
        private Drawing oldLevelExit;
        private Drawing oldPlayer;
        private Drawing oldProjectiles;
        private Drawing oldFlyingMonsters;
        private Drawing oldShootingMonsters;
        private Drawing oldTrackingMonsters;
        private Drawing oldLavas;
        private Drawing oldWaters;
        private Drawing oldWalls;
        private Drawing oldPowerups;
        private Drawing oldBoss;

        private Model.Ui.HealthBar hpBar = new Model.Ui.HealthBar();
        private Drawing oldHpBar;
        private int oldPlayerHealth;

        private Model.Ui.LevelCounter lvlCounter = new Model.Ui.LevelCounter();
        private Drawing oldLvlCounter;
        private int oldModelLvlCounter;

        private Drawing oldPauseScreen;

        private Point oldBossPosition;
        private List<Point> oldTrackingMonstersPosition = new List<Point>();
        private List<Point> oldShootingMonstersPosition = new List<Point>();
        private List<Point> oldFlyingMonstersPosition = new List<Point>();
        private List<Point> oldProjectilePosition = new List<Point>();
        private int powerupCount;
        private Point oldPlayerPosition;
        private Point oldMousePosition = new Point(0, 0);

        private Pen iss = new Pen(Brushes.Black, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="GameRenderer"/> class.
        /// </summary>
        /// <param name="model">GameModel.</param>
        public GameRenderer(IGameModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// Groups all the things that needs to be drawn out on the screen.
        /// </summary>
        /// <returns>A drwing group with all things that needs to be drawn out on the screen.</returns>
        public Drawing BuildDrawing()
        {
            DrawingGroup dg = new DrawingGroup();
            dg.Children.Add(this.GetBackground());
            dg.Children.Add(this.GetLevelExit());
            dg.Children.Add(this.GetLavas());
            dg.Children.Add(this.GetWaters());
            dg.Children.Add(this.GetWalls());
            dg.Children.Add(this.GetTrackingMonsters());
            dg.Children.Add(this.GetPowerups());
            dg.Children.Add(this.GetFlyingMonsters());
            dg.Children.Add(this.GetProjectiles());
            dg.Children.Add(this.GetShootingMonsters());
            if (this.model.LevelCounter % 10 == 0)
            {
                dg.Children.Add(this.GetBoss());
            }

            dg.Children.Add(this.GetPlayer());
            dg.Children.Add(this.GetLevelCounter());
            dg.Children.Add(this.GetHpBar());
            dg.Children.Add(this.GetPauseScreen());

            return dg;
        }

        /// <summary>
        /// Loads the image, based on the parameter filename.
        /// </summary>
        /// <param name="fileName">filename.</param>
        /// <returns>A bitmap image.</returns>
        internal static BitmapImage GetImage(string fileName)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = Assembly.LoadFrom("DungeonPenetrator").GetManifestResourceStream("DungeonPenetrator.Images." + fileName);
            bmp.EndInit();
            return bmp;
        }

        /// <summary>
        /// Draws out a Pause screen.
        /// </summary>
        /// <returns>A drawing.</returns>
        private Drawing GetPauseScreen()
        {
            if (!this.model.GameIsPaused)
            {
                /*ImageDrawing drawing = new ImageDrawing(GetImage("stopped.png"), new Rect(1000, 1000, model.GameWidth, model.GameHeight));
                oldPauseScreen = drawing;*/
                return new ImageDrawing();
            }

            if (this.model.GameIsPaused)
            {
                ImageDrawing drawing = new ImageDrawing(GetImage("stopped2.png"), new Rect(0, 0, this.model.GameWidth, this.model.GameHeight));
                this.oldPauseScreen = drawing;
            }

            return this.oldPauseScreen;
        }

        /// <summary>
        /// Draws out a level counter.
        /// </summary>
        /// <returns>a drawing.</returns>
        private Drawing GetLevelCounter()
        {
            DrawingGroup group = new DrawingGroup();
            if (this.oldLvlCounter == null || this.model.LevelCounter != this.oldModelLvlCounter)
            {
                this.oldModelLvlCounter = this.model.LevelCounter;
                GeometryDrawing box = new GeometryDrawing(Brushes.Gray, this.iss, new RectangleGeometry(new Rect(this.lvlCounter.LvlCounterX, this.lvlCounter.LvlCounterY, this.lvlCounter.LvlCounterWidth, this.lvlCounter.LvlCounterHeight)));

                FormattedText text = new FormattedText(this.oldModelLvlCounter.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 30, Brushes.Black, 1);
                text.TextAlignment = TextAlignment.Center;
                Geometry geo = text.BuildGeometry(new Point(this.lvlCounter.LvlCounterX + (this.lvlCounter.LvlCounterWidth / 2), this.lvlCounter.LvlCounterY + (this.lvlCounter.LvlCounterHeight / 3)));
                GeometryDrawing textGeo = new GeometryDrawing(Brushes.Black, null, geo);

                group.Children.Add(box);
                group.Children.Add(textGeo);

                this.oldLvlCounter = group;
            }

            return this.oldLvlCounter;
        }

        /// <summary>
        /// Draws out the player's Hp bar.
        /// </summary>
        /// <returns>a drawing.</returns>
        private Drawing GetHpBar()
        {
            if (this.oldHpBar == null || this.model.MyPlayer.Health != this.oldPlayerHealth)
            {
                this.oldPlayerHealth = this.model.MyPlayer.Health;
                Geometry g = new RectangleGeometry(new Rect(this.hpBar.HpBarX, this.hpBar.HpBarY, this.hpBar.HpWidth, this.oldPlayerHealth * 7));
                g.Transform = new RotateTransform(180, this.hpBar.HpBarX + (this.hpBar.HpWidth / 2), this.hpBar.HpBarY + (this.hpBar.HpHeight / 2));
                this.oldHpBar = new GeometryDrawing(Brushes.Red, this.iss, g);
            }

            return this.oldHpBar;
        }

        /// <summary>
        /// Draws out the background.
        /// </summary>
        /// <returns>a drawing.</returns>
        private Drawing GetBackground()
        {
            if (this.oldBackground == null)
            {
                ImageDrawing drawing;
                switch (this.model.BiomeType)
                {
                    case Biome.Plains:
                        drawing = new ImageDrawing(GetImage("bggreen.png"), new Rect(0, 0, this.model.GameWidth, this.model.GameHeight));
                        break;

                    case Biome.Desert:
                        drawing = new ImageDrawing(GetImage("backgroundtan.png"), new Rect(0, 0, this.model.GameWidth, this.model.GameHeight));
                        break;

                    case Biome.Snowy:
                        drawing = new ImageDrawing(GetImage("backgroundsnowy.png"), new Rect(0, 0, this.model.GameWidth, this.model.GameHeight));
                        break;

                    default:
                        drawing = new ImageDrawing(GetImage("bggreen.png"), new Rect(0, 0, this.model.GameWidth, this.model.GameHeight));
                        break;
                }

                this.oldBackground = drawing;
            }

            return this.oldBackground;
        }

        /// <summary>
        /// Draws out the level exit.
        /// </summary>
        /// <returns>a drawing.</returns>
        private Drawing GetLevelExit()
        {
            DrawingGroup g = new DrawingGroup();
            if (this.oldLevelExit == null || ((this.model.ShootingMonsters.Count == 0 && this.model.TrackingMonsters.Count == 0 && this.model.FlyingMonsters.Count == 0) && this.model.Boss == null))
            {
                ImageDrawing drawing = new ImageDrawing(GetImage("goal.png"), new Rect(this.model.LevelExit.X * GameModel.TileSize, this.model.LevelExit.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));

                g.Children.Add(drawing);
                this.oldLevelExit = g;
            }
            else
            {
                ImageDrawing drawing = new ImageDrawing(GetImage("goallocked.png"), new Rect(this.model.LevelExit.X * GameModel.TileSize, this.model.LevelExit.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));

                g.Children.Add(drawing);
                this.oldLevelExit = g;
            }

            return this.oldLevelExit;
        }

        /// <summary>
        /// Draws out the lavas.
        /// </summary>
        /// <returns>a drawing.</returns>
        private Drawing GetLavas()
        {
            if (this.oldLavas == null)
            {
                DrawingGroup g = new DrawingGroup();
                foreach (var lava in this.model.Lavas)
                {
                    ImageDrawing drawing = new ImageDrawing(GetImage("lava.png"), new Rect(lava.Cords.X * GameModel.TileSize, lava.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                    g.Children.Add(drawing);
                }

                this.oldLavas = g;
            }

            return this.oldLavas;
        }

        /// <summary>
        /// Draws out the waters.
        /// </summary>
        /// <returns>a drawing.</returns>
        private Drawing GetWaters()
        {
            if (this.oldWaters == null)
            {
                DrawingGroup g = new DrawingGroup();
                foreach (var water in this.model.Waters)
                {
                    ImageDrawing drawing = new ImageDrawing(GetImage("water2.png"), new Rect(water.Cords.X * GameModel.TileSize, water.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                    g.Children.Add(drawing);
                }

                this.oldWaters = g;
            }

            return this.oldWaters;
        }

        /// <summary>
        /// Draws out the rocks.
        /// </summary>
        /// <returns>a drawing.</returns>
        private Drawing GetWalls()
        {
            if (this.oldWalls == null)
            {
                DrawingGroup g = new DrawingGroup();
                foreach (var wall in this.model.Walls)
                {
                    ImageDrawing drawing = new ImageDrawing(GetImage("rock100.png"), new Rect(wall.Cords.X * GameModel.TileSize, wall.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                    g.Children.Add(drawing);
                }

                this.oldWalls = g;
            }

            return this.oldWalls;
        }

        /// <summary>
        /// Draws out the shooting enemies.
        /// </summary>
        /// <returns>a drawing.</returns>
        private Drawing GetShootingMonsters()
        {
            DrawingGroup g = new DrawingGroup();
            foreach (var enemy in this.model.ShootingMonsters)
            {
                if (this.oldShootingMonsters == null || !this.oldShootingMonstersPosition.Contains(enemy.Cords))
                {
                    Point p = new Point((this.model.MyPlayer.Cords.X * GameModel.TileSize) - (enemy.Cords.X * GameModel.TileSize), (this.model.MyPlayer.Cords.Y * GameModel.TileSize) - (enemy.Cords.Y * GameModel.TileSize));
                    double rotation = Math.Atan2(p.Y, p.X) * 180 / Math.PI;
                    BitmapImage bmp = GetImage("ct100big.png");
                    TransformedBitmap tb = new TransformedBitmap();
                    tb.BeginInit();
                    tb.Source = bmp;
                    tb.Transform = new RotateTransform(90);
                    tb.EndInit();

                    ImageDrawing drawing = new ImageDrawing(tb, new Rect(enemy.Cords.X * GameModel.TileSize, enemy.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));

                    RotateTransform rotate = new RotateTransform(rotation, (enemy.Cords.X * GameModel.TileSize) + (GameModel.TileSize / 2), (enemy.Cords.Y * GameModel.TileSize) + (GameModel.TileSize / 2));

                    g.Children.Add(new DrawingGroup() { Children = { drawing }, Transform = rotate });
                }
            }

            this.oldShootingMonsters = g;
            return this.oldShootingMonsters;
        }

        /// <summary>
        /// Draws out the tracking enemies.
        /// </summary>
        /// <returns>a drawing.</returns>
        private Drawing GetTrackingMonsters()
        {
            DrawingGroup g = new DrawingGroup();
            foreach (var enemy in this.model.TrackingMonsters)
            {
                if (this.oldTrackingMonsters == null || !this.oldTrackingMonstersPosition.Contains(enemy.Cords))
                {
                    BitmapImage bmp = GetImage("hoodtracker100100.png");
                    TransformedBitmap tb = new TransformedBitmap();
                    tb.BeginInit();
                    tb.Source = bmp;
                    switch (this.model.BasicTrackingPath[enemy.Cords])
                    {
                        case { } point when point == new Point(1, 0):
                            tb.Transform = new RotateTransform(270);
                            break;

                        case { } point when point == new Point(0, 1):
                            tb.Transform = new RotateTransform(0);
                            break;

                        case { } point when point == new Point(-1, 0):
                            tb.Transform = new RotateTransform(90);
                            break;

                        case { } point when point == new Point(0, -1):
                            tb.Transform = new RotateTransform(180);
                            break;

                        default:
                            break;
                    }

                    tb.EndInit();

                    ImageDrawing drawing = new ImageDrawing(tb, new Rect(enemy.Cords.X * GameModel.TileSize, enemy.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));

                    g.Children.Add(drawing);
                }
            }

            this.oldTrackingMonsters = g;
            return this.oldTrackingMonsters;
        }

        /// <summary>
        /// Draws out all the powerups.
        /// </summary>
        /// <returns>a drawing.</returns>
        private Drawing GetPowerups()
        {
            if (this.oldPowerups == null || this.powerupCount != this.model.Powerups.Count)
            {
                DrawingGroup g = new DrawingGroup();
                foreach (var powerup in this.model.Powerups)
                {
                    ImageDrawing drawing;
                    switch (powerup.Type)
                    {
                        case Model.Passive.PowerupType.Health:
                            drawing = new ImageDrawing(GetImage("redpotion.png"), new Rect(powerup.Cords.X * GameModel.TileSize, powerup.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                            break;

                        case Model.Passive.PowerupType.Damage:
                            drawing = new ImageDrawing(GetImage("bluepotion.png"), new Rect(powerup.Cords.X * GameModel.TileSize, powerup.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                            break;

                        case Model.Passive.PowerupType.FiringSpeed:
                            drawing = new ImageDrawing(GetImage("yellowpotion.png"), new Rect(powerup.Cords.X * GameModel.TileSize, powerup.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                            break;

                        default:
                            drawing = new ImageDrawing(GetImage("error-icon-32.png"), new Rect(powerup.Cords.X * GameModel.TileSize, powerup.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));
                            break;
                    }

                    g.Children.Add(drawing);
                }

                if (this.powerupCount != this.model.Powerups.Count)
                {
                    this.powerupCount = this.model.Powerups.Count;
                }

                this.oldPowerups = g;
            }

            return this.oldPowerups;
        }

        /// <summary>
        /// Draws out all the flying enemies.
        /// </summary>
        /// <returns>a drawing.</returns>
        private Drawing GetFlyingMonsters()
        {
            DrawingGroup g = new DrawingGroup();
            foreach (var enemy in this.model.FlyingMonsters)
            {
                if (this.oldFlyingMonsters == null || !this.oldFlyingMonstersPosition.Contains(enemy.Cords))
                {
                    BitmapImage bmp = GetImage("missle100.png");
                    TransformedBitmap tb = new TransformedBitmap();
                    tb.BeginInit();
                    tb.Source = bmp;
                    switch (this.model.FlyingTrackingPath[enemy.Cords])
                    {
                        case { } point when point == new Point(1, 0):
                            tb.Transform = new RotateTransform(90);
                            break;

                        case { } point when point == new Point(0, 1):
                            tb.Transform = new RotateTransform(180);
                            break;

                        case { } point when point == new Point(-1, 0):
                            tb.Transform = new RotateTransform(270);
                            break;

                        case { } point when point == new Point(0, -1):
                            tb.Transform = new RotateTransform(0);
                            break;

                        default:
                            break;
                    }

                    tb.EndInit();

                    ImageDrawing drawing = new ImageDrawing(tb, new Rect(enemy.Cords.X * GameModel.TileSize, enemy.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));

                    g.Children.Add(drawing);
                }
            }

            this.oldFlyingMonsters = g;
            return this.oldFlyingMonsters;
        }

        /// <summary>
        /// Draws out the boss.
        /// </summary>
        /// <returns>a drawing.</returns>
        private Drawing GetBoss()
        {
            DrawingGroup g = new DrawingGroup();
            if (this.oldBoss == null || (this.model.Boss != null && this.oldBossPosition != this.model.Boss.Cords))
            {
                ImageDrawing drawing = new ImageDrawing(GetImage("hoodghost2.png"), new Rect((this.model.Boss.Cords.X * GameModel.TileSize) - ((GameModel.TileSize * 3) / 2), (this.model.Boss.Cords.Y * GameModel.TileSize) - ((GameModel.TileSize * 3) / 2), GameModel.TileSize * 3, GameModel.TileSize * 3));

                g.Children.Add(drawing);

                this.oldBoss = g;
                this.oldBossPosition = this.model.Boss.Cords;
            }

            if (this.model.Boss == null)
            {
                return g;
            }

            return this.oldBoss;
        }

        /// <summary>
        /// Draws out all the projectiles.
        /// </summary>
        /// <returns>a drawing.</returns>
        private Drawing GetProjectiles()
        {
            DrawingGroup g = new DrawingGroup();
            try
            {
                ImageDrawing drawing;
                foreach (var projectile in this.model.Projectiles)
                {
                    if (this.oldProjectiles == null || !this.oldProjectilePosition.Contains(projectile.Cords))
                    {
                        switch (projectile.Type)
                        {
                            case ProjectileType.Enemy:
                                drawing = new ImageDrawing(GetImage("bullet3.png"), new Rect(projectile.Cords.X, projectile.Cords.Y, 10, 10));
                                break;

                            case ProjectileType.Player:
                                drawing = new ImageDrawing(GetImage("bullet3.png"), new Rect(projectile.Cords.X, projectile.Cords.Y, 10, 10));
                                break;

                            case ProjectileType.Boss:
                                drawing = new ImageDrawing(GetImage("bossbullet.png"), new Rect(projectile.Cords.X, projectile.Cords.Y, 40, 40));
                                break;

                            default:
                                drawing = new ImageDrawing(GetImage("bullet3.png"), new Rect(projectile.Cords.X, projectile.Cords.Y, 10, 10));
                                break;
                        }

                        g.Children.Add(drawing);
                    }
                }

                this.oldProjectiles = g;
                return this.oldProjectiles;
            }
            catch
            {
                return g;
            }
        }

        /// <summary>
        /// Draws out the player.
        /// </summary>
        /// <returns>a drawing.</returns>
        private Drawing GetPlayer()
        {
            DrawingGroup g = new DrawingGroup();
            if (this.oldPlayer == null || this.oldPlayerPosition != this.model.MyPlayer.Cords || this.model.MousePosition != this.oldMousePosition)
            {
                Point p = new Point(this.model.MousePosition.X - (this.model.MyPlayer.Cords.X * GameModel.TileSize), this.model.MousePosition.Y - (this.model.MyPlayer.Cords.Y * GameModel.TileSize));
                double rotation = Math.Atan2(p.Y, p.X) * 180 / Math.PI;

                BitmapImage bmp = GetImage("100.png");
                TransformedBitmap tb = new TransformedBitmap();
                tb.BeginInit();
                tb.Source = bmp;
                tb.Transform = new RotateTransform(90);
                tb.EndInit();

                ImageDrawing drawing = new ImageDrawing(tb, new Rect(this.model.MyPlayer.Cords.X * GameModel.TileSize, this.model.MyPlayer.Cords.Y * GameModel.TileSize, GameModel.TileSize, GameModel.TileSize));

                RotateTransform rotate = new RotateTransform(rotation, (this.model.MyPlayer.Cords.X * GameModel.TileSize) + (GameModel.TileSize / 2), (this.model.MyPlayer.Cords.Y * GameModel.TileSize) + (GameModel.TileSize / 2));

                g.Children.Add(new DrawingGroup() { Children = { drawing }, Transform = rotate });

                this.oldPlayer = g;
                this.oldPlayerPosition = this.model.MyPlayer.Cords;
            }

            return this.oldPlayer;
        }
    }
}