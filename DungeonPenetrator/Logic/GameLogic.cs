using Model;
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

        public List<Point> GetEmptyTileSpaces()
        {
            throw new NotImplementedException();
        }

        public void MoveEnemy()
        {
            throw new NotImplementedException();
        }

        public void MovePlayer(int dx,int dy)
        {
            int newX = (int)(gameModel.MyPlayer.Cords.X + dx);
            int newY = (int)(gameModel.MyPlayer.Cords.Y + dy);
            if (newX >= 0 && newY >= 0 && newX < gameModel.GameWidth && newY < gameModel.GameHeight
                && gameModel.Wall.Where(x=>x.Cords.X==newX && x.Cords.Y == newY).FirstOrDefault() == null)
            {
                gameModel.MyPlayer.Cords = new Point(newX, newY);
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
