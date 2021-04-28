// <copyright file="IGameLogic.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Logic
{
    using System.Windows;
    using Model;
    using Model.Active;
    using Model.Passive;

    /// <summary>
    /// Interface for GameLogic.
    /// </summary>
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

        void UpdatePlayerInSight();

        void BossPatternShoot(Point bossLocation, int speed, int damage);

        void RandomBossMovement(Point bossLocation);

        Projectile BossShoot(Point bossLocation, int speed, int damage);

        void Updater();

        void MoveProjectile(ref Projectile projectile);

        void CollectPowerup(Powerups powerups);

        void DamageActiveGameObject(ActiveGameObjects activeGameObjects, int damage);

        void DisposeBullet(Projectile projectile);

        void DropRandomCollectable();
    }
}