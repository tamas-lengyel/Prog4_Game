using Model;
using Model.Active;
using Model.Passive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        }

        public Projectile EnemyShoot(Point enemyLocation, int speed, int damage) 
        {
            Point enemLocationCord = new Point((enemyLocation.X * GameModel.TileSize) + GameModel.TileSize / 2, (enemyLocation.Y * GameModel.TileSize) + GameModel.TileSize / 2);
            Point playerLocationCord = new Point((gameModel.MyPlayer.Cords.X * GameModel.TileSize) + GameModel.TileSize / 2, (gameModel.MyPlayer.Cords.Y * GameModel.TileSize) + GameModel.TileSize / 2);
            Projectile projectile = new Projectile(enemLocationCord, playerLocationCord);
            projectile.Type = ProjectileType.Enemy;
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
                        gameModel.MyPlayer.Health += powerups.ModifyRate;
                        break;
                    }
                    gameModel.MyPlayer.Health = 100;
                    break;
                case PowerupType.Damage:
                    gameModel.MyPlayer.Damage += powerups.ModifyRate;
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
                    gameModel.Powerups.Add(new Powerups { Cords = emptyTiles[rnd.Next(0,emptyTiles.Count())], Type = PowerupType.Health });
                    break;
                case 1:
                    gameModel.Powerups.Add(new Powerups { Cords = emptyTiles[rnd.Next(0, emptyTiles.Count())], Type = PowerupType.Damage});
                    break;
                case 2:
                    gameModel.Powerups.Add(new Powerups { Cords = emptyTiles[rnd.Next(0, emptyTiles.Count())], Type = PowerupType.FiringSpeed});
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
        public void MoveProjectile(Projectile projectile)
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
                gameModel.Projectiles.Remove(projectile);
                projectile = null;
                return;
            }
            gameModel.Projectiles.Find(x => x.Equals(projectile)).Cords = new Point(newX, newY);
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

        public Projectile PlayerShoot(Point mousePos,int speed)
        {
            Point playerLocationCord = new Point((gameModel.MyPlayer.Cords.X * GameModel.TileSize) + GameModel.TileSize/2, (gameModel.MyPlayer.Cords.Y * GameModel.TileSize) + GameModel.TileSize / 2);
            Projectile projectile = new Projectile(playerLocationCord, mousePos);
            projectile.Type = ProjectileType.Player;
            projectile.Damage = gameModel.MyPlayer.Damage;
            projectile.Speed = speed; 
            return projectile;
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
                        new Point(activeGameObjects.Cords.X + gameModel.FlyingTrackingPath[activeGameObjects.Cords].X,
                        activeGameObjects.Cords.Y + gameModel.FlyingTrackingPath[activeGameObjects.Cords].Y);
                    break;
                case BossEnemy:
                    activeGameObjects.Cords =
                        new Point(activeGameObjects.Cords.X + gameModel.FlyingTrackingPath[activeGameObjects.Cords].X,
                        activeGameObjects.Cords.Y + gameModel.FlyingTrackingPath[activeGameObjects.Cords].Y);
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
                        gameModel.Boss = null;
                        break;
                    }
                    gameModel.Boss.Health  -= damage;
                    break;
                case FlyingEnemy:
                    if (activeGameObjects.Health - damage <= 0)
                    {
                        gameModel.FlyingMonsters.Find(x=>x==activeGameObjects).Health = 0;
                        DisposeEnemy(activeGameObjects);
                        break;
                    }
                    gameModel.FlyingMonsters.Find(x => x == activeGameObjects).Health -= damage;
                    break;
                case TrackingEnemy:
                    if (activeGameObjects.Health - damage <= 0)
                    {
                        gameModel.TrackingMonsters.Find(x => x == activeGameObjects).Health = 0;
                        DisposeEnemy(activeGameObjects);
                        break;
                    }
                    gameModel.TrackingMonsters.Find(x => x == activeGameObjects).Health -= damage;
                    break;
                case ShootingEnemy:
                    if (activeGameObjects.Health - damage <= 0)
                    {
                        gameModel.ShootingMonsters.Find(x => x == activeGameObjects).Health = 0;
                        DisposeEnemy(activeGameObjects);
                        break;
                    }
                    gameModel.ShootingMonsters.Find(x => x == activeGameObjects).Health -= damage;
                    break;
                case Player:
                    if (activeGameObjects.Health - damage <= 0)
                    {
                        gameModel.MyPlayer.Health = 0;
                        break;
                    }
                    gameModel.MyPlayer.Health -= damage;
                    break;
                default:
                    break;
            }
        }
    }
}

