using Model;
using Model.Active;
using Model.Passive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Logic
{
    public class GameLogic : IGameLogic
    {
        IGameModel gameModel;
        Random rnd = new Random();
        public GameLogic(IGameModel gameModel)
        {
            this.gameModel = gameModel;
            gameModel.FlyingTrackingPath = BreadthFirstSearch(GetFlyingEnemyNeighbours);
            gameModel.BasicTrackingPath = BreadthFirstSearch(GetRegularEnemyNeighbours);
            InitailzeTimers();
        }
        private void InitailzeTimers()
        {
            gameModel.LavaTickTimer = new DispatcherTimer();
            gameModel.LavaTickTimer.Interval = TimeSpan.FromMilliseconds(300);
            gameModel.LavaTickTimer.Tick += delegate
            {
                gameModel.MyPlayer.BeingDamagedByLava = false;
                gameModel.LavaTickTimer.Stop();
            };
            gameModel.EnemyHitTickTimer = new DispatcherTimer();
            gameModel.EnemyHitTickTimer.Interval = TimeSpan.FromMilliseconds(300);
            gameModel.EnemyHitTickTimer.Tick += delegate
            {
                foreach (var item in gameModel.TrackingMonsters)
                {
                    item.CanAttack = true;
                }
                gameModel.EnemyHitTickTimer.Stop();
            };
        }
        public void UpdatePlayerInSight()
        {
            if (gameModel.LevelCounter%10==0)
            {
                double x = (gameModel.MyPlayer.Cords.X*GameModel.TileSize) - (gameModel.Boss.Cords.X*GameModel.TileSize);
                double y = gameModel.MyPlayer.Cords.Y * GameModel.TileSize - (gameModel.Boss.Cords.Y * GameModel.TileSize);
                double magnetude = (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
                if (magnetude<200)
                {
                    gameModel.Boss.PlayerInSight = true;
                }
                else
                {
                    gameModel.Boss.PlayerInSight = false;
                }
            }
        }
        public void RandomBossMovement(Point bossLocation)
        {
            List<Point> possibleMoves = GetRegularEnemyNeighbours(bossLocation);
            gameModel.Boss.Cords = possibleMoves[rnd.Next(0, possibleMoves.Count())];
        }
        public void BossPatternShoot(Point bossLocation, int speed, int damage)
        {
            foreach (var item in BossEnemy.ShootingPattern)
            {
                Point enemLocationCord = new Point((bossLocation.X * GameModel.TileSize) + GameModel.TileSize / 2, (bossLocation.Y * GameModel.TileSize) + GameModel.TileSize / 2);
                Point patternLocationCord = new Point(((bossLocation.X * GameModel.TileSize) + GameModel.TileSize / 2) +(item.X*GameModel.TileSize), ((bossLocation.Y * GameModel.TileSize) + GameModel.TileSize / 2) + (item.Y * GameModel.TileSize));
                Projectile projectile = new Projectile(enemLocationCord, patternLocationCord);
                projectile.Type = ProjectileType.Enemy;
                projectile.Damage = damage;
                projectile.Speed = speed;
                projectile.Timer = new DispatcherTimer(DispatcherPriority.Send);
                projectile.Timer.Interval = TimeSpan.FromMilliseconds(20);
                projectile.Timer.Tick += new EventHandler((sender, e) => ProjectileTick(this, e, ref projectile));
                projectile.Timer.Start();
                gameModel.Projectiles.Add(projectile);
            }
        }
        public Projectile EnemyShoot(Point enemyLocation, int speed, int damage) 
        {
            Point enemLocationCord = new Point((enemyLocation.X * GameModel.TileSize) + GameModel.TileSize / 2, (enemyLocation.Y * GameModel.TileSize) + GameModel.TileSize / 2);
            Point playerLocationCord = new Point((gameModel.MyPlayer.Cords.X * GameModel.TileSize) + GameModel.TileSize / 2, (gameModel.MyPlayer.Cords.Y * GameModel.TileSize) + GameModel.TileSize / 2);
            Projectile projectile = new Projectile(enemLocationCord, playerLocationCord);
            if (gameModel.LevelCounter % 10 == 0)
            {
                projectile.Type = ProjectileType.Boss;
            }
            else
            {
                projectile.Type = ProjectileType.Enemy;
            }
            projectile.Damage = damage;
            projectile.Speed = speed; 
            return projectile;
        }
        public void CollectPowerup(Powerups powerups)
        {
            switch (powerups.Type)
            {
                case PowerupType.Health:
                    if (gameModel.MyPlayer.Health+powerups.ModifyRate<100) // If player's maxhealth will be modified this should not be hardcoded like this
                    {
                        gameModel.MyPlayer.Health += (int)powerups.ModifyRate;
                        break;
                    }
                    gameModel.MyPlayer.Health = 100;
                    break;
                case PowerupType.Damage:
                    gameModel.MyPlayer.Damage += (int)powerups.ModifyRate;
                    break;
                case PowerupType.FiringSpeed: // max value should be 50 needs param
                    if (gameModel.MyPlayer.FiringSpeed<50)
                    {
                        gameModel.MyPlayer.FiringSpeed += powerups.ModifyRate;
                    }
                    break;
                default:
                    break;
            }
        }
        public void DropRandomCollectable()
        {
            List<Point> emptyTiles = GetEmptyTileSpaces();
            switch(rnd.Next(0, 3))
            {
                case 0:
                    gameModel.Powerups.Add(new Powerups (emptyTiles[rnd.Next(0,emptyTiles.Count())],PowerupType.Health ));
                    break;
                case 1:
                    gameModel.Powerups.Add(new Powerups (emptyTiles[rnd.Next(0, emptyTiles.Count())],PowerupType.Damage));
                    break;
                case 2:
                    gameModel.Powerups.Add(new Powerups (emptyTiles[rnd.Next(0, emptyTiles.Count())],PowerupType.FiringSpeed));
                    break;
            }
        }
        private List<Point> GetEmptyTileSpaces()
        {
            List<Point> emptyTiles = new List<Point>();
            for (int y = 0; y < (int)(gameModel.GameHeight / GameModel.TileSize); y++)
            {
                for (int x = 0; x < (int)(gameModel.GameWidth / GameModel.TileSize); x++)
                {
                    Point current = new Point(x, y);
                    if (current != gameModel.MyPlayer.Cords &&
                        current != gameModel.LevelExit &&
                        (gameModel.Boss == default ||  current != gameModel.Boss.Cords) &&
                        (gameModel.Walls == default || !(gameModel.Walls.Select(x => x.Cords).Contains(current))) &&
                        (gameModel.Waters == default || !(gameModel.Waters.Select(x => x.Cords).Contains(current))) &&
                        (gameModel.Lavas == default || !(gameModel.Lavas.Select(x => x.Cords).Contains(current))) &&
                        (gameModel.Powerups == default || !(gameModel.Powerups.Select(x => x.Cords).Contains(current))) &&
                        (gameModel.ShootingMonsters == default || !(gameModel.ShootingMonsters.Select(x => x.Cords).Contains(current))))
                    {
                        emptyTiles.Add(current);
                    }
                }
            }
            return emptyTiles;
        }
        private Dictionary<Point,Point> BreadthFirstSearch(Func<Point, List<Point>> getNeighbours)
        {
            List<Point> frontiner = new List<Point>();
            frontiner.Add(gameModel.MyPlayer.Cords);
            Dictionary<Point, Point> path = new Dictionary<Point, Point>();
            path.Add(gameModel.MyPlayer.Cords, new Point(0, 0)); // player's cord
            while (frontiner.Count>0)
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
                Point check = new Point(current.X + item.X, current.Y + item.Y);
                if ((check.X < 0 || check.X >= (int)(gameModel.GameWidth/GameModel.TileSize)) || (check.Y < 0 || check.Y >= (int)(gameModel.GameHeight / GameModel.TileSize)))
                {
                    rmList.Add(item);
                }
                else if(gameModel.GameAreaChar[(int)check.X, (int)check.Y] == 'W' ||
                        gameModel.GameAreaChar[(int)check.X, (int)check.Y] == 'P' ||
                        gameModel.GameAreaChar[(int)check.X, (int)check.Y] == 'G') // Basic tracking -> cant move through walls,water,goal
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
                Point check = new Point(current.X + item.X, current.Y + item.Y);
                if ((check.X < 0 || check.X >= (int)(gameModel.GameWidth / GameModel.TileSize)) || (check.Y < 0 || check.Y >= (int)(gameModel.GameHeight / GameModel.TileSize)))
                {
                    rmList.Add(item);
                }
                else if (gameModel.GameAreaChar[(int)check.X, (int)check.Y] == 'W' ||
                        gameModel.GameAreaChar[(int)check.X, (int)check.Y] == 'G') // Fly tracking -> cant move through walls,goal
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

        public void DisposeEnemy(ActiveGameObjects activeGameObject)
        {
            switch (activeGameObject)
            {
                case FlyingEnemy:
                    gameModel.FlyingMonsters.Remove((activeGameObject as FlyingEnemy));
                    break;
                case ShootingEnemy:
                    gameModel.ShootingMonsters.Remove((activeGameObject as ShootingEnemy));
                    break;
                case TrackingEnemy:
                    gameModel.TrackingMonsters.Remove((activeGameObject as TrackingEnemy));
                    break;
                case BossEnemy:
                    gameModel.Boss = null;
                    break;
                default:
                    break;
            }
        }
        public void DisposeBullet(Projectile projectile)
        {
            gameModel.Projectiles.Remove(projectile);
        }
        public void MoveProjectile(ref Projectile projectile)
        {
            double x = projectile.direction.X;
            double y = projectile.direction.Y;
            /*double magnetude = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            double ux = x / magnetude;
            double uy = y / magnetude;
            double newX = projectile.Cords.X + (ux*projectile.Speed);
            double newY = projectile.Cords.Y + (uy*projectile.Speed);*/


            double newX = projectile.Cords.X + (projectile.direction.X * projectile.Speed);
            double newY = projectile.Cords.Y + (projectile.direction.Y * projectile.Speed);
            if ((newX < 0 || newX >= gameModel.GameWidth) || (newY < 0 || newY >= gameModel.GameHeight))
            {
                projectile.Timer.Stop();
                gameModel.Projectiles.Remove(projectile);
                projectile = null;
                return;
            }
            //gameModel.Projectiles.Find(x => x.Equals(projectile)).Cords = new Point(newX, newY);
            projectile.Cords = new Point(newX, newY);
        }
        public void MovePlayer(int dx,int dy)
        {
            int newX = (int)(gameModel.MyPlayer.Cords.X + dx);
            int newY = (int)(gameModel.MyPlayer.Cords.Y + dy);
            if (newX >= 0 && newY >= 0 && newX < gameModel.GameWidth/GameModel.TileSize && newY < gameModel.GameHeight/GameModel.TileSize
                && (gameModel.GameAreaChar[newX,newY] != 'W' &&
                gameModel.GameAreaChar[newX, newY] != 'P' ))
            {
                gameModel.MyPlayer.Cords = new Point(newX, newY);
                gameModel.GameAreaChar[(int)gameModel.MyPlayer.Cords.X, (int)gameModel.MyPlayer.Cords.Y] = 'C'; // Sets Character->Player pos
                gameModel.FlyingTrackingPath=BreadthFirstSearch(GetFlyingEnemyNeighbours);
                gameModel.BasicTrackingPath = BreadthFirstSearch(GetRegularEnemyNeighbours);
            }
        }

        public void PlayerShoot(Point mousePos,int speed)
        {
            Point playerLocationCord = new Point((gameModel.MyPlayer.Cords.X * GameModel.TileSize) + GameModel.TileSize/2, (gameModel.MyPlayer.Cords.Y * GameModel.TileSize) + GameModel.TileSize / 2);
            Projectile projectile = new Projectile(playerLocationCord, mousePos);
            projectile.Type = ProjectileType.Player;
            projectile.Damage = gameModel.MyPlayer.Damage;
            projectile.Speed = speed;

            projectile.Timer = new DispatcherTimer(DispatcherPriority.Send);
            projectile.Timer.Interval = TimeSpan.FromMilliseconds(20);
            projectile.Timer.Tick += new EventHandler((sender, e) => ProjectileTick(this,e,ref projectile));
            Thread soundPlayThread = new Thread(() => {
                new System.Media.SoundPlayer(Assembly.LoadFrom("DungeonPenetrator").GetManifestResourceStream("DungeonPenetrator.Images.piu2.wav")).Play();
            });
            soundPlayThread.IsBackground = true;
            soundPlayThread.Start();
            projectile.Timer.Start();
            gameModel.Projectiles.Add(projectile);
            //return projectile;
        }

        private void ProjectileTick(object sender, EventArgs e,ref Projectile projectile)
        {

            MoveProjectile(ref projectile);
        }

        public void MoveFlyingEnemy(FlyingEnemy flyingEnemy)
        {
            flyingEnemy.Cords =
                new Point(flyingEnemy.Cords.X + gameModel.FlyingTrackingPath[flyingEnemy.Cords].X,
                flyingEnemy.Cords.Y + gameModel.FlyingTrackingPath[flyingEnemy.Cords].Y);
        }

        public void MoveRegularEnemy(ActiveGameObjects activeGameObjects)
        {
            switch (activeGameObjects)
            {
                case TrackingEnemy:
                    activeGameObjects.Cords =
                        new Point(activeGameObjects.Cords.X + gameModel.BasicTrackingPath[activeGameObjects.Cords].X,
                        activeGameObjects.Cords.Y + gameModel.BasicTrackingPath[activeGameObjects.Cords].Y);
                    break;
                case BossEnemy:
                    activeGameObjects.Cords =
                        new Point(activeGameObjects.Cords.X + gameModel.BasicTrackingPath[activeGameObjects.Cords].X,
                        activeGameObjects.Cords.Y + gameModel.BasicTrackingPath[activeGameObjects.Cords].Y);
                    break;
                default:
                    break;
            }
        }
        public void DamageActiveGameObject(ActiveGameObjects activeGameObjects,int damage)
        {
            switch (activeGameObjects)
            {
                case BossEnemy:
                    if (activeGameObjects.Health-damage<=0)
                    {
                        gameModel.Boss.Health = 0;
                        break;
                    }
                    gameModel.Boss.Health  -= damage;
                    break;
                case FlyingEnemy:
                    if (activeGameObjects.Health - damage <= 0)
                    {
                        gameModel.FlyingMonsters.Find(x=>x==activeGameObjects).Health = 0;
                        break;
                    }
                    gameModel.FlyingMonsters.Find(x => x == activeGameObjects).Health -= damage;
                    break;
                case TrackingEnemy:
                    if (activeGameObjects.Health - damage <= 0)
                    {
                        gameModel.TrackingMonsters.Find(x => x == activeGameObjects).Health = 0;
                        break;
                    }
                    gameModel.TrackingMonsters.Find(x => x == activeGameObjects).Health -= damage;
                    break;
                case ShootingEnemy:
                    if (activeGameObjects.Health - damage <= 0)
                    {
                        gameModel.ShootingMonsters.Find(x => x == activeGameObjects).Health = 0;
                        break;
                    }
                    gameModel.ShootingMonsters.Find(x => x == activeGameObjects).Health -= damage;
                    break;
                case Player:
                    if (activeGameObjects.Health - damage <= 0)
                    {
                        gameModel.MyPlayer.Health = 0;
                        Thread soundPlayThread = new Thread(() => {
                            new System.Media.SoundPlayer(Assembly.LoadFrom("DungeonPenetrator").GetManifestResourceStream("DungeonPenetrator.Images.auh.wav")).Play();
                        });
                        soundPlayThread.IsBackground = true;
                        soundPlayThread.Start();
                        break;
                    }
                    gameModel.MyPlayer.Health -= damage;
                    break;
                default:
                    break;
            }
        }

        private void ManageIntersectsForPlayer()
        {
            List<GameObjects> rmlist = new List<GameObjects>();
            var lavas = gameModel.Lavas;
            try
            {
            foreach (var item in lavas)
            {
                if (gameModel.MyPlayer.IsCollision(item))
                {
                    if (!gameModel.MyPlayer.BeingDamagedByLava)
                    {
                        DamageActiveGameObject(gameModel.MyPlayer, item.Damage);
                        gameModel.MyPlayer.BeingDamagedByLava = true;
                        gameModel.LavaTickTimer.Start();
                    }
                        
                }
            }
            var flyings = gameModel.FlyingMonsters;
            foreach (var item in flyings)
            {
                if (gameModel.MyPlayer.IsCollision(item))
                {
                    DamageActiveGameObject(gameModel.MyPlayer, item.Damage);
                    rmlist.Add(item);
                }
            }
            var trackings = gameModel.TrackingMonsters;
            foreach (var item in trackings)
            {
                if (gameModel.MyPlayer.IsCollision(item))
                {
                    if (item.CanAttack)
                    {
                        DamageActiveGameObject(gameModel.MyPlayer, item.Damage);
                        item.CanAttack = false;
                        gameModel.EnemyHitTickTimer.Start();
                    }
                }
            }
            var enemyProjectiles = gameModel.Projectiles.Where(x => x.Type.Equals(ProjectileType.Enemy) || x.Type.Equals(ProjectileType.Boss));
            foreach (var item in enemyProjectiles)
            {
                if (gameModel.MyPlayer.IsCollision(item))
                {
                    DamageActiveGameObject(gameModel.MyPlayer, item.Damage);
                    rmlist.Add(item);
                }
            }
            var powerups = gameModel.Powerups;
            foreach (var item in powerups)
            {
                if (gameModel.MyPlayer.IsCollision(item))
                {
                    CollectPowerup(item);
                    rmlist.Add(item);
                }
            }
            foreach (var item in rmlist)
            {
                switch (item)
                {
                    case FlyingEnemy:
                        gameModel.FlyingMonsters.Remove(item as FlyingEnemy);
                        break;
                    case Powerups:
                        gameModel.Powerups.Remove(item as Powerups);
                        break;
                    case Projectile:
                        gameModel.Projectiles.Remove(item as Projectile);
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
        public void Updater()
        {
            if (gameModel.ShootingMonsters.Count == 0 && gameModel.TrackingMonsters.Count == 0 && gameModel.FlyingMonsters.Count == 0 && gameModel.MyPlayer.Cords==gameModel.LevelExit)
            {
                gameModel.LevelFinished = true;
            }

            ManageIntersectsForPlayer();
            ManageProjectileIntersects();
        }
        private void ManageProjectileIntersects()
        {
            List<ActiveGameObjects> rmGameObjectList = new List<ActiveGameObjects>();
            List<Projectile> rmprojlist = new List<Projectile>();
            var projectiles = gameModel.Projectiles;
            try
            {
                foreach (var item in projectiles)
                {
                    foreach (var walls in gameModel.Walls)
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
                    gameModel.Projectiles.Remove(item);
                }
                projectiles = gameModel.Projectiles.Where(x => x.Type.Equals(ProjectileType.Player)).ToList();

                foreach (var item in projectiles)
                {
                    foreach (var flying in gameModel.FlyingMonsters)
                    {
                        if (flying.IsCollision(item))
                        {
                            DamageActiveGameObject(flying, item.Damage);
                            if (flying.Health == 0)
                            {
                                rmGameObjectList.Add(flying);
                            }
                            rmprojlist.Add(item);
                        }
                    }
                    foreach (var shooting in gameModel.ShootingMonsters)
                    {
                        if (shooting.IsCollision(item))
                        {
                            DamageActiveGameObject(shooting, item.Damage);
                            if (shooting.Health == 0)
                            {
                                rmGameObjectList.Add(shooting);
                            }
                            rmprojlist.Add(item);
                        }
                    }
                    foreach (var tracking in gameModel.TrackingMonsters)
                    {
                        if (tracking.IsCollision(item))
                        {
                            DamageActiveGameObject(tracking, item.Damage);
                            if (tracking.Health == 0)
                            {
                                rmGameObjectList.Add(tracking);
                            }
                            rmprojlist.Add(item);
                        }
                    }
                    if (gameModel.Boss!=null)
                    {
                        if (gameModel.Boss.IsCollision(item))
                        {
                            DamageActiveGameObject(gameModel.Boss, item.Damage);
                            if (gameModel.Boss.Health == 0)
                            {
                                rmGameObjectList.Add(gameModel.Boss);
                            }
                            rmprojlist.Add(item);
                        }
                    }
                }
                foreach (var item in rmprojlist)
                {
                    item.Timer.Stop();
                    gameModel.Projectiles.Remove(item);
                }
                foreach (var item in rmGameObjectList)
                {
                    DisposeEnemy(item);
                }
            }
            catch
            {

            }
        }

    }
}

