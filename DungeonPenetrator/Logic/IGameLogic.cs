using Model;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Logic
{
    public interface IGameLogic
    {
        // Playerwise
        void MovePlayer(int dx, int dy);
        Projectile PlayerShoot(Point mousePos, int speed);

        // Enemywise
        void MoveEnemy();
        Projectile EnemyShoot(Point enemyLocation, Point playerLoc, int speed);
        void DisposeEnemy(ActiveGameObjects activeGameObject);

        List<Point> GetEmptyTileSpaces();
        void DropRandomCollectable();

    }
}
