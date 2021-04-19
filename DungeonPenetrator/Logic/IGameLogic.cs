using Model;
using Model.Active;
using Model.Passive;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Logic
{
    public interface IGameLogic
    {
        // Playerwise
        void MovePlayer(int dx, int dy);
        void PlayerShoot(Point mousePos, int speed);

        // Enemywise
        void MoveFlyingEnemy(FlyingEnemy flyingEnemy);
        void MoveRegularEnemy(ActiveGameObjects activeGameObjects);
        Projectile EnemyShoot(Point enemyLocation, int speed, int damage);
        void DisposeEnemy(ActiveGameObjects activeGameObject);

        void Updater();
        void MoveProjectile(ref Projectile projectile);
        void CollectPowerup(Powerups powerups);
        void DamageActiveGameObject(ActiveGameObjects activeGameObjects, int damage);
        void DisposeBullet(Projectile projectile);
        void DropRandomCollectable();

    }
}
