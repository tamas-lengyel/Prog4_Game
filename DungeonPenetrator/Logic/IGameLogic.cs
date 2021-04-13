﻿using Model;
using Model.Active;
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
        void MoveFlyingEnemy(FlyingEnemy flyingEnemy);
        void MoveRegularEnemy(ActiveGameObjects activeGameObjects);
        Projectile EnemyShoot(Point enemyLocation, Point playerLoc, int speed);
        void DisposeEnemy(ActiveGameObjects activeGameObject);

        void DamageActiveGameObject(ActiveGameObjects activeGameObjects, int damage);
        void DisposeBullet(Projectile projectile);
        void DropRandomCollectable();

    }
}
