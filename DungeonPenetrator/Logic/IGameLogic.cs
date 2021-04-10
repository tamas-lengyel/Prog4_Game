using Model;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Logic
{
    public interface IGameLogic
    {
        // Playerwise
        public void MovePlayer(int dx, int dy);
        public Projectile PlayerShoot(Point mousePos, int speed);

        // Enemywise
        public void MoveEnemy();
        public Projectile EnemyShoot(Point enemyLocation, Point playerLoc, int speed);

        public List<Point> GetEmptyTileSpaces();

    }
}
