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

        /// <summary>
        /// Moves the player to new cordinates if possible.
        /// </summary>
        /// <param name="dx">Defines the displacement on the X axis.</param>
        /// <param name="dy">Defines the displacement on the Y axis.</param>
        void MovePlayer(int dx, int dy);

        /// <summary>
        /// Shoots a projectile torwards the mouse's position.
        /// </summary>
        /// <param name="mousePos">Defines the position of the mouse.</param>
        /// <param name="speed">Defines the speed of the bullet.</param>
        void PlayerShoot(Point mousePos, int speed);

        // Enemywise

        /// <summary>
        /// Moves a flying enemy due to it's moving pattern.
        /// </summary>
        /// <param name="flyingEnemy">Defines the flying enemy which we want to move.</param>
        void MoveFlyingEnemy(FlyingEnemy flyingEnemy);

        /// <summary>
        /// Moves an active enemy due to the regular moving pattern.
        /// </summary>
        /// <param name="activeGameObjects">Defines the flying enemy which we want to move.</param>
        void MoveRegularEnemy(ActiveGameObjects activeGameObjects);

        /// <summary>
        /// Creates a new projectile torwards the players position for basic enemy projectiles.
        /// </summary>
        /// <param name="enemyLocation">Defines the starting location of the new projectile.</param>
        /// <param name="speed">Defines the speed of the new projectile.</param>
        /// <param name="damage">Defines the damage of the projectile.</param>
        /// <returns>A projectile which is setup to be moved.</returns>
        Projectile EnemyShoot(Point enemyLocation, int speed, int damage);

        /// <summary>
        /// Updates the boss's sight whether the player is in range.
        /// </summary>
        void UpdatePlayerInSight();

        /// <summary>
        /// Creates bullets due to the boss's shooting pattern.
        /// </summary>
        /// <param name="bossLocation">Defines the starting point for the projectiles.</param>
        /// <param name="speed">Defines the speed of the projectile.</param>
        /// <param name="damage">Defines the damage of the projectile.</param>
        void BossPatternShoot(Point bossLocation, int speed, int damage);

        /// <summary>
        /// Randomly moves the boss according to the regular moving pattern.
        /// </summary>
        /// <param name="bossLocation">Defines the location of the boss.</param>
        void RandomBossMovement(Point bossLocation);

        /// <summary>
        /// Creates a new projectile torwards the players position for basic boss type projectiles.
        /// </summary>
        /// <param name="bossLocation">Defines the starting location of the new projectile.</param>
        /// <param name="speed">Defines the speed of the new projectile.</param>
        /// <param name="damage">Defines the damage of the projectile.</param>
        /// <returns>A projectile which is setup to be moved.</returns>
        Projectile BossShoot(Point bossLocation, int speed, int damage);

        /// <summary>
        /// The main updater of the game checks basic intersects and game states.
        /// </summary>
        void Updater();

        /// <summary>
        /// Moves a projectile torwards it's direction.
        /// </summary>
        /// <param name="projectile">Defines the projectile which should be moved.</param>
        void MoveProjectile(ref Projectile projectile);

        /// <summary>
        /// Defines the behavior of powerups when the player collects them.
        /// </summary>
        /// <param name="powerups">Defines the powerup which is currently picked up.</param>
        void CollectPowerup(Powerups powerups);

        /// <summary>
        /// Defines basic damaging of the game objects.
        /// </summary>
        /// <param name="activeGameObjects">Defines the objects which is taking daming.</param>
        /// <param name="damage">Defines the damage which is taken from the damaged object.</param>
        void DamageActiveGameObject(ActiveGameObjects activeGameObjects, int damage);

        /// <summary>
        /// Drops a randomly generated collectable on an empty spot of the map.
        /// </summary>
        void DropRandomCollectable();
    }
}