// <copyright file="GameLogic.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using Model;
    using Model.Active;
    using Model.Passive;

    /// <summary>
    /// Contains methods for the maing logic of the game.
    /// </summary>
    public class GameLogic : IGameLogic
    {
        private IGameModel gameModel;
        private Random rnd = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="GameLogic"/> class.
        /// Sets up the dependency injection to the game's model.
        /// </summary>
        /// <param name="gameModel">Model of the game's elements.</param>
        public GameLogic(IGameModel gameModel)
        {
            this.gameModel = gameModel;
            gameModel.FlyingTrackingPath = this.BreadthFirstSearch(this.GetFlyingEnemyNeighbours);
            gameModel.BasicTrackingPath = this.BreadthFirstSearch(this.GetRegularEnemyNeighbours);
            this.InitailzeTimers();
        }

        /// <inheritdoc/>
        public void Updater()
        {
            if (this.gameModel.ShootingMonsters.Count == 0 && this.gameModel.TrackingMonsters.Count == 0 && this.gameModel.FlyingMonsters.Count == 0 && this.gameModel.Boss == null && this.gameModel.MyPlayer.Cords == this.gameModel.LevelExit)
            {
                this.gameModel.LevelFinished = true;
            }

            this.ManageIntersectsForPlayer();
            this.ManageProjectileIntersects();
        }

        /// <inheritdoc/>
        public void UpdatePlayerInSight()
        {
            if (this.gameModel.LevelCounter % 10 == 0)
            {
                double x = (this.gameModel.MyPlayer.Cords.X * GameModel.TileSize) - (this.gameModel.Boss.Cords.X * GameModel.TileSize);
                double y = (this.gameModel.MyPlayer.Cords.Y * GameModel.TileSize) - (this.gameModel.Boss.Cords.Y * GameModel.TileSize);
                double magnetude = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                if (magnetude < 200)
                {
                    this.gameModel.Boss.PlayerInSight = true;
                }
                else
                {
                    this.gameModel.Boss.PlayerInSight = false;
                }
            }
        }

        /// <inheritdoc/>
        public void RandomBossMovement(Point bossLocation)
        {
            List<Point> possibleMoves = this.GetRegularEnemyNeighbours(bossLocation);
            this.gameModel.Boss.Cords = possibleMoves[this.rnd.Next(0, possibleMoves.Count())];
        }

        /// <inheritdoc/>
        public void BossPatternShoot(Point bossLocation, int speed, int damage)
        {
            foreach (var item in BossEnemy.ShootingPattern)
            {
                Point enemLocationCord = new Point(bossLocation.X * GameModel.TileSize, bossLocation.Y * GameModel.TileSize);
                Point patternLocationCord = new Point((bossLocation.X * GameModel.TileSize) + (item.X * GameModel.TileSize), (bossLocation.Y * GameModel.TileSize) + (item.Y * GameModel.TileSize));
                Projectile projectile = new Projectile(enemLocationCord, patternLocationCord);
                projectile.Type = ProjectileType.Enemy;
                projectile.Damage = damage;
                projectile.Speed = speed;
                projectile.Timer = new DispatcherTimer(DispatcherPriority.Send);
                projectile.Timer.Interval = TimeSpan.FromMilliseconds(20);
                projectile.Timer.Tick += new EventHandler((sender, e) => this.ProjectileTick(this, e, ref projectile));
                projectile.Timer.Start();
                this.gameModel.Projectiles.Add(projectile);
            }
        }

        /// <inheritdoc/>
        public Projectile EnemyShoot(Point enemyLocation, int speed, int damage)
        {
            Point enemLocationCord = new Point((enemyLocation.X * GameModel.TileSize) + (GameModel.TileSize / 2), (enemyLocation.Y * GameModel.TileSize) + (GameModel.TileSize / 2));
            Point playerLocationCord = new Point((this.gameModel.MyPlayer.Cords.X * GameModel.TileSize) + (GameModel.TileSize / 2), (this.gameModel.MyPlayer.Cords.Y * GameModel.TileSize) + (GameModel.TileSize / 2));
            Projectile projectile = new Projectile(enemLocationCord, playerLocationCord);
            projectile.Type = ProjectileType.Enemy;
            projectile.Damage = damage;
            projectile.Speed = speed;
            return projectile;
        }

        /// <inheritdoc/>
        public Projectile BossShoot(Point bossLocation, int speed, int damage)
        {
            Point enemLocationCord = new Point(bossLocation.X * GameModel.TileSize, bossLocation.Y * GameModel.TileSize);
            Point playerLocationCord = new Point((this.gameModel.MyPlayer.Cords.X * GameModel.TileSize) + (GameModel.TileSize / 2), (this.gameModel.MyPlayer.Cords.Y * GameModel.TileSize) + (GameModel.TileSize / 2));
            Projectile projectile = new Projectile(enemLocationCord, playerLocationCord);
            projectile.Type = ProjectileType.Boss;
            projectile.Damage = damage;
            projectile.Speed = speed;
            return projectile;
        }

        /// <inheritdoc/>
        public void CollectPowerup(Powerups powerups)
        {
            switch (powerups.Type)
            {
                // If player's maxhealth will be modified this should not be hardcoded like this
                case PowerupType.Health:
                    if (this.gameModel.MyPlayer.Health + powerups.ModifyRate < 100)
                    {
                        this.gameModel.MyPlayer.Health += (int)powerups.ModifyRate;
                        break;
                    }

                    this.gameModel.MyPlayer.Health = 100;
                    break;

                case PowerupType.Damage:
                    this.gameModel.MyPlayer.Damage += (int)powerups.ModifyRate;
                    break;

                case PowerupType.FiringSpeed: // max value should be 50 needs param
                    if (this.gameModel.MyPlayer.FiringSpeed < 50)
                    {
                        this.gameModel.MyPlayer.FiringSpeed += powerups.ModifyRate;
                    }

                    break;

                default:
                    break;
            }
        }

        /// <inheritdoc/>
        public void DropRandomCollectable()
        {
            List<Point> emptyTiles = this.GetEmptyTileSpaces();
            switch (this.rnd.Next(0, 3))
            {
                case 0:
                    this.gameModel.Powerups.Add(new Powerups(emptyTiles[this.rnd.Next(0, emptyTiles.Count())], PowerupType.Health));
                    break;

                case 1:
                    this.gameModel.Powerups.Add(new Powerups(emptyTiles[this.rnd.Next(0, emptyTiles.Count())], PowerupType.Damage));
                    break;

                case 2:
                    this.gameModel.Powerups.Add(new Powerups(emptyTiles[this.rnd.Next(0, emptyTiles.Count())], PowerupType.FiringSpeed));
                    break;
            }
        }

        /// <inheritdoc/>
        public void MoveProjectile(ref Projectile projectile)
        {
            double x = projectile.Direction.X;
            double y = projectile.Direction.Y;
            /*double magnetude = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            double ux = x / magnetude;
            double uy = y / magnetude;
            double newX = projectile.Cords.X + (ux*projectile.Speed);
            double newY = projectile.Cords.Y + (uy*projectile.Speed);*/

            double newX = projectile.Cords.X + (projectile.Direction.X * projectile.Speed);
            double newY = projectile.Cords.Y + (projectile.Direction.Y * projectile.Speed);
            if ((newX < 0 || newX >= this.gameModel.GameWidth) || (newY < 0 || newY >= this.gameModel.GameHeight))
            {
                projectile.Timer.Stop();
                this.gameModel.Projectiles.Remove(projectile);
                projectile = null;
                return;
            }

            projectile.Cords = new Point(newX, newY);
        }

        /// <inheritdoc/>
        public void MovePlayer(int dx, int dy)
        {
            int newX = (int)(this.gameModel.MyPlayer.Cords.X + dx);
            int newY = (int)(this.gameModel.MyPlayer.Cords.Y + dy);
            if (newX >= 0 && newY >= 0 && newX < this.gameModel.GameWidth / GameModel.TileSize && newY < this.gameModel.GameHeight / GameModel.TileSize
                && (this.gameModel.GameAreaChar[newX, newY] != 'W' &&
                this.gameModel.GameAreaChar[newX, newY] != 'P'))
            {
                this.gameModel.MyPlayer.Cords = new Point(newX, newY);
                this.gameModel.GameAreaChar[(int)this.gameModel.MyPlayer.Cords.X, (int)this.gameModel.MyPlayer.Cords.Y] = 'C'; // Sets Character->Player pos
                this.gameModel.FlyingTrackingPath = this.BreadthFirstSearch(this.GetFlyingEnemyNeighbours);
                this.gameModel.BasicTrackingPath = this.BreadthFirstSearch(this.GetRegularEnemyNeighbours);
            }
        }

        /// <inheritdoc/>
        public void PlayerShoot(Point mousePos, int speed)
        {
            Point playerLocationCord = new Point((this.gameModel.MyPlayer.Cords.X * GameModel.TileSize) + (GameModel.TileSize / 2), (this.gameModel.MyPlayer.Cords.Y * GameModel.TileSize) + (GameModel.TileSize / 2));
            Projectile projectile = new Projectile(playerLocationCord, mousePos);
            projectile.Type = ProjectileType.Player;
            projectile.Damage = this.gameModel.MyPlayer.Damage;
            projectile.Speed = speed;

            projectile.Timer = new DispatcherTimer(DispatcherPriority.Send);
            projectile.Timer.Interval = TimeSpan.FromMilliseconds(20);
            projectile.Timer.Tick += new EventHandler((sender, e) => this.ProjectileTick(this, e, ref projectile));
            Thread soundPlayThread = new Thread(() =>
            {
                new System.Media.SoundPlayer(Assembly.LoadFrom("DungeonPenetrator").GetManifestResourceStream("DungeonPenetrator.Images.piu2.wav")).Play();
            });
            soundPlayThread.IsBackground = true;
            soundPlayThread.Start();
            projectile.Timer.Start();
            this.gameModel.Projectiles.Add(projectile);
        }

        /// <inheritdoc/>
        public void MoveFlyingEnemy(FlyingEnemy flyingEnemy)
        {
            flyingEnemy.Cords = new Point(flyingEnemy.Cords.X + this.gameModel.FlyingTrackingPath[flyingEnemy.Cords].X, flyingEnemy.Cords.Y + this.gameModel.FlyingTrackingPath[flyingEnemy.Cords].Y);
        }

        /// <inheritdoc/>
        public void MoveRegularEnemy(ActiveGameObjects activeGameObjects)
        {
            switch (activeGameObjects)
            {
                case TrackingEnemy:
                    activeGameObjects.Cords =
                        new Point(
                            activeGameObjects.Cords.X + this.gameModel.BasicTrackingPath[activeGameObjects.Cords].X,
                            activeGameObjects.Cords.Y + this.gameModel.BasicTrackingPath[activeGameObjects.Cords].Y);
                    break;

                case BossEnemy:
                    activeGameObjects.Cords =
                        new Point(
                            activeGameObjects.Cords.X + this.gameModel.BasicTrackingPath[activeGameObjects.Cords].X,
                            activeGameObjects.Cords.Y + this.gameModel.BasicTrackingPath[activeGameObjects.Cords].Y);
                    break;

                default:
                    break;
            }
        }

        /// <inheritdoc/>
        public void DamageActiveGameObject(ActiveGameObjects activeGameObjects, int damage)
        {
            switch (activeGameObjects)
            {
                case BossEnemy:
                    if (activeGameObjects.Health - damage <= 0)
                    {
                        this.gameModel.Boss.Health = 0;
                        break;
                    }

                    this.gameModel.Boss.Health -= damage;
                    break;

                case FlyingEnemy:
                    if (activeGameObjects.Health - damage <= 0)
                    {
                        this.gameModel.FlyingMonsters.Find(x => x == activeGameObjects).Health = 0;
                        break;
                    }

                    this.gameModel.FlyingMonsters.Find(x => x == activeGameObjects).Health -= damage;
                    break;

                case TrackingEnemy:
                    if (activeGameObjects.Health - damage <= 0)
                    {
                        this.gameModel.TrackingMonsters.Find(x => x == activeGameObjects).Health = 0;
                        break;
                    }

                    this.gameModel.TrackingMonsters.Find(x => x == activeGameObjects).Health -= damage;
                    break;

                case ShootingEnemy:
                    if (activeGameObjects.Health - damage <= 0)
                    {
                        this.gameModel.ShootingMonsters.Find(x => x == activeGameObjects).Health = 0;
                        break;
                    }

                    this.gameModel.ShootingMonsters.Find(x => x == activeGameObjects).Health -= damage;
                    break;

                case Player:
                    if (activeGameObjects.Health - damage <= 0)
                    {
                        this.gameModel.MyPlayer.Health = 0;
                        Thread soundPlayThread = new Thread(() =>
                        {
                            new System.Media.SoundPlayer(Assembly.LoadFrom("DungeonPenetrator").GetManifestResourceStream("DungeonPenetrator.Images.auh.wav")).Play();
                        });
                        soundPlayThread.IsBackground = true;
                        soundPlayThread.Start();
                        break;
                    }

                    this.gameModel.MyPlayer.Health -= damage;
                    break;

                default:
                    break;
            }
        }

        private void ManageIntersectsForPlayer()
        {
            List<GameObjects> rmlist = new List<GameObjects>();
            var lavas = this.gameModel.Lavas;
            try
            {
                foreach (var item in lavas)
                {
                    if (this.gameModel.MyPlayer.IsCollision(item))
                    {
                        if (!this.gameModel.MyPlayer.BeingDamagedByLava)
                        {
                            this.DamageActiveGameObject(this.gameModel.MyPlayer, item.Damage);
                            this.gameModel.MyPlayer.BeingDamagedByLava = true;
                            this.gameModel.LavaTickTimer.Start();
                        }
                    }
                }

                var flyings = this.gameModel.FlyingMonsters;
                foreach (var item in flyings)
                {
                    if (this.gameModel.MyPlayer.IsCollision(item))
                    {
                        this.DamageActiveGameObject(this.gameModel.MyPlayer, item.Damage);
                        rmlist.Add(item);
                    }
                }

                var trackings = this.gameModel.TrackingMonsters;
                foreach (var item in trackings)
                {
                    if (this.gameModel.MyPlayer.IsCollision(item))
                    {
                        if (item.CanAttack)
                        {
                            this.DamageActiveGameObject(this.gameModel.MyPlayer, item.Damage);
                            item.CanAttack = false;
                            this.gameModel.EnemyHitTickTimer.Start();
                        }
                    }
                }

                var enemyProjectiles = this.gameModel.Projectiles.Where(x => x.Type.Equals(ProjectileType.Enemy) || x.Type.Equals(ProjectileType.Boss));
                foreach (var item in enemyProjectiles)
                {
                    if (this.gameModel.MyPlayer.IsCollision(item))
                    {
                        this.DamageActiveGameObject(this.gameModel.MyPlayer, item.Damage);
                        rmlist.Add(item);
                    }
                }

                var powerups = this.gameModel.Powerups;
                foreach (var item in powerups)
                {
                    if (this.gameModel.MyPlayer.IsCollision(item))
                    {
                        if (item.Type.Equals(PowerupType.Health) && this.gameModel.MyPlayer.Health < 100)
                        {
                            this.CollectPowerup(item);
                            rmlist.Add(item);
                        }
                        else if (!item.Type.Equals(PowerupType.Health))
                        {
                            this.CollectPowerup(item);
                            rmlist.Add(item);
                        }
                    }
                }

                if (this.gameModel.Boss != null)
                {
                    if (this.gameModel.MyPlayer.IsCollision(this.gameModel.Boss))
                    {
                        if (!this.gameModel.MyPlayer.BeingDamagedByLava)
                        {
                            this.DamageActiveGameObject(this.gameModel.MyPlayer, this.gameModel.Boss.Damage);
                            this.gameModel.MyPlayer.BeingDamagedByLava = true;
                            this.gameModel.LavaTickTimer.Start();
                        }
                    }
                }

                foreach (var item in rmlist)
                {
                    switch (item)
                    {
                        case FlyingEnemy:
                            this.gameModel.FlyingMonsters.Remove(item as FlyingEnemy);
                            break;

                        case Powerups:
                            this.gameModel.Powerups.Remove(item as Powerups);
                            break;

                        case Projectile:
                            this.gameModel.Projectiles.Remove(item as Projectile);
                            break;

                        default:
                            break;
                    }
                }
            }
            catch
            {
            }
        }

        private void ManageProjectileIntersects()
        {
            List<ActiveGameObjects> rmGameObjectList = new List<ActiveGameObjects>();
            List<Projectile> rmprojlist = new List<Projectile>();
            var projectiles = this.gameModel.Projectiles;
            try
            {
                foreach (var item in projectiles)
                {
                    foreach (var walls in this.gameModel.Walls)
                    {
                        if (walls.IsCollision(item))
                        {
                            rmprojlist.Add(item);
                        }
                    }
                }

                foreach (var item in rmprojlist)
                {
                    item.Timer.Stop();
                    this.gameModel.Projectiles.Remove(item);
                }

                projectiles = this.gameModel.Projectiles.Where(x => x.Type.Equals(ProjectileType.Player)).ToList();

                foreach (var item in projectiles)
                {
                    foreach (var flying in this.gameModel.FlyingMonsters)
                    {
                        if (flying.IsCollision(item))
                        {
                            this.DamageActiveGameObject(flying, item.Damage);
                            if (flying.Health == 0)
                            {
                                rmGameObjectList.Add(flying);
                            }

                            rmprojlist.Add(item);
                        }
                    }

                    foreach (var shooting in this.gameModel.ShootingMonsters)
                    {
                        if (shooting.IsCollision(item))
                        {
                            this.DamageActiveGameObject(shooting, item.Damage);
                            if (shooting.Health == 0)
                            {
                                rmGameObjectList.Add(shooting);
                            }

                            rmprojlist.Add(item);
                        }
                    }

                    foreach (var tracking in this.gameModel.TrackingMonsters)
                    {
                        if (tracking.IsCollision(item))
                        {
                            this.DamageActiveGameObject(tracking, item.Damage);
                            if (tracking.Health == 0)
                            {
                                rmGameObjectList.Add(tracking);
                            }

                            rmprojlist.Add(item);
                        }
                    }

                    if (this.gameModel.Boss != null)
                    {
                        if (this.gameModel.Boss.IsCollision(item))
                        {
                            this.DamageActiveGameObject(this.gameModel.Boss, item.Damage);
                            if (this.gameModel.Boss.Health == 0)
                            {
                                rmGameObjectList.Add(this.gameModel.Boss);
                            }

                            rmprojlist.Add(item);
                        }
                    }
                }

                foreach (var item in rmprojlist)
                {
                    item.Timer.Stop();
                    this.gameModel.Projectiles.Remove(item);
                }

                foreach (var item in rmGameObjectList)
                {
                    this.DisposeEnemy(item);
                }
            }
            catch
            {
            }
        }

        private void DisposeBullet(Projectile projectile)
        {
            this.gameModel.Projectiles.Remove(projectile);
        }

        private void InitailzeTimers()
        {
            this.gameModel.LavaTickTimer = new DispatcherTimer();
            this.gameModel.LavaTickTimer.Interval = TimeSpan.FromMilliseconds(300);
            this.gameModel.LavaTickTimer.Tick += delegate
            {
                this.gameModel.MyPlayer.BeingDamagedByLava = false;
                this.gameModel.LavaTickTimer.Stop();
            };
            this.gameModel.EnemyHitTickTimer = new DispatcherTimer();
            this.gameModel.EnemyHitTickTimer.Interval = TimeSpan.FromMilliseconds(300);
            this.gameModel.EnemyHitTickTimer.Tick += delegate
            {
                foreach (var item in this.gameModel.TrackingMonsters)
                {
                    item.CanAttack = true;
                }

                this.gameModel.EnemyHitTickTimer.Stop();
            };
        }

        private List<Point> GetEmptyTileSpaces()
        {
            List<Point> emptyTiles = new List<Point>();
            for (int y = 0; y < (int)(this.gameModel.GameHeight / GameModel.TileSize); y++)
            {
                for (int x = 0; x < (int)(this.gameModel.GameWidth / GameModel.TileSize); x++)
                {
                    Point current = new Point(x, y);
                    if (current != this.gameModel.MyPlayer.Cords &&
                        current != this.gameModel.LevelExit &&
                        (this.gameModel.Boss == default || current != this.gameModel.Boss.Cords) &&
                        (this.gameModel.Walls == default || !this.gameModel.Walls.Select(x => x.Cords).Contains(current)) &&
                        (this.gameModel.Waters == default || !this.gameModel.Waters.Select(x => x.Cords).Contains(current)) &&
                        (this.gameModel.Lavas == default || !this.gameModel.Lavas.Select(x => x.Cords).Contains(current)) &&
                        (this.gameModel.Powerups == default || !this.gameModel.Powerups.Select(x => x.Cords).Contains(current)) &&
                        (this.gameModel.ShootingMonsters == default || !this.gameModel.ShootingMonsters.Select(x => x.Cords).Contains(current)))
                    {
                        emptyTiles.Add(current);
                    }
                }
            }

            return emptyTiles;
        }

        private Dictionary<Point, Point> BreadthFirstSearch(Func<Point, List<Point>> getNeighbours)
        {
            List<Point> frontiner = new List<Point>();
            frontiner.Add(this.gameModel.MyPlayer.Cords);
            Dictionary<Point, Point> path = new Dictionary<Point, Point>();
            path.Add(this.gameModel.MyPlayer.Cords, new Point(0, 0)); // player's cord
            while (frontiner.Count > 0)
            {
                Point current = frontiner[0];
                frontiner.RemoveAt(0);
                var neighbours = getNeighbours(current);
                foreach (var next in neighbours)
                {
                    if (!path.ContainsKey(next))
                    {
                        frontiner.Add(next);
                        path[next] = new Point(current.X - next.X, current.Y - next.Y); // generate direction vector to the next tile
                    }
                }
            }

            return path;
        }

        private List<Point> GetRegularEnemyNeighbours(Point current)
        {
            List<Point> neighbours = new List<Point>();
            neighbours.Add(new Point(1, 0));
            neighbours.Add(new Point(-1, 0));
            neighbours.Add(new Point(0, 1));
            neighbours.Add(new Point(0, -1));
            List<Point> rmList = new List<Point>();
            foreach (var item in neighbours)
            {
                // Basic tracking -> cant move through walls,water,goal
                Point check = new Point(current.X + item.X, current.Y + item.Y);
                if ((check.X < 0 || check.X >= (int)(this.gameModel.GameWidth / GameModel.TileSize)) || (check.Y < 0 || check.Y >= (int)(this.gameModel.GameHeight / GameModel.TileSize)))
                {
                    rmList.Add(item);
                }
                else if (this.gameModel.GameAreaChar[(int)check.X, (int)check.Y] == 'W' ||
                        this.gameModel.GameAreaChar[(int)check.X, (int)check.Y] == 'P' ||
                        this.gameModel.GameAreaChar[(int)check.X, (int)check.Y] == 'G')
                {
                    rmList.Add(item);
                }
            }

            foreach (var item in rmList)
            {
                neighbours.Remove(item);
            }

            for (int i = 0; i < neighbours.Count; i++)
            {
                neighbours[i] = new Point(current.X + neighbours[i].X, current.Y + neighbours[i].Y);
            }

            return neighbours;
        }

        private List<Point> GetFlyingEnemyNeighbours(Point current)
        {
            List<Point> neighbours = new List<Point>();
            neighbours.Add(new Point(1, 0));
            neighbours.Add(new Point(-1, 0));
            neighbours.Add(new Point(0, 1));
            neighbours.Add(new Point(0, -1));
            List<Point> rmList = new List<Point>();
            foreach (var item in neighbours)
            {
                // Fly tracking -> cant move through walls,goal
                Point check = new Point(current.X + item.X, current.Y + item.Y);
                if ((check.X < 0 || check.X >= (int)(this.gameModel.GameWidth / GameModel.TileSize)) || (check.Y < 0 || check.Y >= (int)(this.gameModel.GameHeight / GameModel.TileSize)))
                {
                    rmList.Add(item);
                }
                else if (this.gameModel.GameAreaChar[(int)check.X, (int)check.Y] == 'W' ||
                        this.gameModel.GameAreaChar[(int)check.X, (int)check.Y] == 'G')
                {
                    rmList.Add(item);
                }
            }

            foreach (var item in rmList)
            {
                neighbours.Remove(item);
            }

            for (int i = 0; i < neighbours.Count; i++)
            {
                neighbours[i] = new Point(current.X + neighbours[i].X, current.Y + neighbours[i].Y);
            }

            return neighbours;
        }

        private void DisposeEnemy(ActiveGameObjects activeGameObject)
        {
            switch (activeGameObject)
            {
                case FlyingEnemy:
                    this.gameModel.FlyingMonsters.Remove(activeGameObject as FlyingEnemy);
                    break;

                case ShootingEnemy:
                    this.gameModel.ShootingMonsters.Remove(activeGameObject as ShootingEnemy);
                    break;

                case TrackingEnemy:
                    this.gameModel.TrackingMonsters.Remove(activeGameObject as TrackingEnemy);
                    break;

                case BossEnemy:
                    this.gameModel.Boss = null;
                    break;

                default:
                    break;
            }
        }

        private void ProjectileTick(object sender, EventArgs e, ref Projectile projectile)
        {
            this.MoveProjectile(ref projectile);
        }
    }
}