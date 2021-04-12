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
        public List<Point> GetEmptyTileSpaces()
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
        public void MoveEnemy()
        {
            throw new NotImplementedException();
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
        public void MovePlayer(int dx,int dy)
        {
            int newX = (int)(gameModel.MyPlayer.Cords.X + dx);
            int newY = (int)(gameModel.MyPlayer.Cords.Y + dy);
            if (newX >= 0 && newY >= 0 && newX < gameModel.GameWidth && newY < gameModel.GameHeight
                && gameModel.Wall.Where(x=>x.Cords.X==newX && x.Cords.Y == newY).FirstOrDefault() == null)
            {
                gameModel.MyPlayer.Cords = new Point(newX, newY);
                gameModel.GameAreaChar[(int)gameModel.MyPlayer.Cords.X, (int)gameModel.MyPlayer.Cords.Y] = 'C'; // Sets Character->Player pos
            }
        }

        public Projectile PlayerShoot(Point mousePos,int speed)
        {
            Projectile projectile = new Projectile(gameModel.MyPlayer.Cords, mousePos);
            projectile.Type = ProjectileType.Player;
            projectile.Speed = speed; 
            return projectile;
        }
    }
}
