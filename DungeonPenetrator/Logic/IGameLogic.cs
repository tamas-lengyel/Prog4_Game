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
        Projectile PlayerShoot(Point mousePos, int speed);

        // Enemywise
        void MoveFlyingEnemy(FlyingEnemy flyingEnemy);
        void MoveRegularEnemy(ActiveGameObjects activeGameObjects);
        public Projectile EnemyShoot(Point enemyLocation, int speed, int damage);
        void DisposeEnemy(ActiveGameObjects activeGameObject);

        public void MoveProjectile(Projectile projectile);
        void CollectPowerup(Powerups powerups);
        void DamageActiveGameObject(ActiveGameObjects activeGameObjects, int damage);
        void DisposeBullet(Projectile projectile);
        void DropRandomCollectable();

    }
}
