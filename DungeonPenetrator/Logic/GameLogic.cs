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
        }

        public Projectile EnemyShoot(Point enemyLocation,Point playerLoc,int speed)
        {
            Projectile projectile = new Projectile(enemyLocation, playerLoc);
            projectile.Type = ProjectileType.Enemy;
            projectile.Speed = speed; 
            return projectile;
        }

        public void DropRandomCollectable()
        {
            List<Point> emptyTiles = GetEmptyTileSpaces();
            switch(rnd.Next(0, 3))
            {
                case 0:
                    gameModel.Powerup.Add(new Powerups { Cords = emptyTiles[rnd.Next(0,emptyTiles.Count())], Type = PowerupType.Health, ModifyRate = 5 });
                    break;
                case 1:
                    gameModel.Powerup.Add(new Powerups { Cords = emptyTiles[rnd.Next(0, emptyTiles.Count())], Type = PowerupType.Damage, ModifyRate = 5 });
                    break;
                case 2:
                    gameModel.Powerup.Add(new Powerups { Cords = emptyTiles[rnd.Next(0, emptyTiles.Count())], Type = PowerupType.FiringSpeed, ModifyRate = 5 });
                    break;
            }

        }
        private List<Point> GetEmptyTileSpaces()
        {
            List<Point> emptyTiles = new List<Point>();
            for (int y = 0; y < (int)(gameModel.GameHeight / gameModel.TileSize); y++)
            {
                for (int x = 0; x < (int)(gameModel.GameWidth / gameModel.TileSize); x++)
                {
                    Point current = new Point(x, y);
                    if (current != gameModel.MyPlayer.Cords &&
                        current != gameModel.LevelExit &&
                        current != gameModel.Boss.Cords &&
                        !(gameModel.Wall.Select(x => x.Cords).Contains(current)) &&
                        !(gameModel.Water.Select(x => x.Cords).Contains(current)) &&
                        !(gameModel.Lava.Select(x => x.Cords).Contains(current)) &&
                        !(gameModel.Powerup.Select(x => x.Cords).Contains(current)) &&
                        !(gameModel.ShootingMonster.Select(x => x.Cords).Contains(current)))
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
                if ((check.X < 0 || check.X >= gameModel.GameWidth/gameModel.TileSize) || (check.Y < 0 || check.Y >= gameModel.GameHeight / gameModel.TileSize))
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
                if ((check.X < 0 || check.X >= gameModel.GameWidth / gameModel.TileSize) || (check.Y < 0 || check.Y >= gameModel.GameHeight / gameModel.TileSize))
                {
                    rmList.Add(item);
                }
                else if (gameModel.GameAreaChar[(int)check.X, (int)check.Y] == 'W' ||
                        gameModel.GameAreaChar[(int)check.X, (int)check.Y] == 'G') // Fly tracking -> cant move through walls,goal
                {
                    rmList.Add(item);
                }
            }
            return neighbours;
        }

        public void DisposeEnemy(ActiveGameObjects activeGameObject)
        {
            switch (activeGameObject)
            {
                case FlyingEnemy:
                    gameModel.FlyingMonster.Remove((activeGameObject as FlyingEnemy));
                    break;
                case ShootingEnemy:
                    gameModel.ShootingMonster.Remove((activeGameObject as ShootingEnemy));
                    break;
                case TrackingEnemy:
                    gameModel.TrackingMonster.Remove((activeGameObject as TrackingEnemy));
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
            double newX = projectile.Cords.X  + projectile.direction.X;
            double newY = projectile.Cords.Y + projectile.direction.Y;
            if ((newX < 0 || newX >= gameModel.GameWidth) || (newY < 0 || newY >= gameModel.GameHeight))
            {
                gameModel.Projectiles.Remove(projectile);
                return;
            }
            gameModel.Projectiles.Find(x => x.Equals(projectile)).Cords = new Point(newX, newY);
        }
        public void MovePlayer(int dx,int dy)
        {
            int newX = (int)(gameModel.MyPlayer.Cords.X + dx);
            int newY = (int)(gameModel.MyPlayer.Cords.Y + dy);
            if (newX >= 0 && newY >= 0 && newX < gameModel.GameWidth && newY < gameModel.GameHeight
                && (gameModel.GameAreaChar[newX,newY] != 'W' ||
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
            Projectile projectile = new Projectile(gameModel.MyPlayer.Cords, mousePos);
            projectile.Type = ProjectileType.Player;
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
    }
}
