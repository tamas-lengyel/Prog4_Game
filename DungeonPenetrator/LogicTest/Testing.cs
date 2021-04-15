using Logic;
using Model;
using Model.Active;
using Model.Passive;
using Moq;
using NUnit.Framework;
using Repository;
using System;
using System.Collections.Generic;
using System.Windows;

namespace LogicTest
{
    [TestFixture]
    public class Testing
    {
        private static ILoadingLogic loadingLogicTest;
        private static IGameLogic gameLogicTest;
        private static Mock<IHighscoreRepository> highscoreRepoMock;
        private static Mock<ISaveGameRepository> saveGameRepositoryMock;
        private static Mock<GameModel> gameModelMock;

        [SetUp]
        public void Init()
        {
            gameModelMock = new Mock<GameModel>();
            highscoreRepoMock = new Mock<IHighscoreRepository>();
            saveGameRepositoryMock = new Mock<ISaveGameRepository>();

            gameModelMock.Object.MyPlayer = new Player { Cords = new Point(3, 9), Damage = 5, Health = 100 };
            gameModelMock.Object.LevelCounter = 0;
            gameModelMock.Object.GameAreaChar = new char[(int)(gameModelMock.Object.GameWidth / gameModelMock.Object.TileSize), (int)(gameModelMock.Object.GameHeight / gameModelMock.Object.TileSize)];
            for (int y = 0; y < gameModelMock.Object.GameAreaChar.GetLength(1); y++)
            {
                for (int x = 0; x < gameModelMock.Object.GameAreaChar.GetLength(0); x++)
                {
                    gameModelMock.Object.GameAreaChar[x, y] = 'E'; // Generates Empty Cell
                }
            }
            gameModelMock.Object.GameAreaChar[(int)gameModelMock.Object.MyPlayer.Cords.X, (int)gameModelMock.Object.MyPlayer.Cords.Y] = 'C'; // Sets Character->Player pos
            gameModelMock.Object.GameAreaChar[(int)gameModelMock.Object.LevelExit.X, (int)gameModelMock.Object.LevelExit.Y] = 'G';



            // Walls
            gameModelMock.Object.Walls = new List<WallProp>();
            gameModelMock.Object.Walls.Add(new WallProp { Cords = new Point(0, 1) });
            gameModelMock.Object.Walls.Add(new WallProp { Cords = new Point(6, 1) });
            gameModelMock.Object.Walls.Add(new WallProp { Cords = new Point(2, 3) });
            gameModelMock.Object.Walls.Add(new WallProp { Cords = new Point(5, 3) });
            gameModelMock.Object.Walls.Add(new WallProp { Cords = new Point(6, 3) });
            gameModelMock.Object.Walls.Add(new WallProp { Cords = new Point(0, 5) });
            gameModelMock.Object.Walls.Add(new WallProp { Cords = new Point(6, 5) });
            gameModelMock.Object.Walls.Add(new WallProp { Cords = new Point(1, 7) });
            gameModelMock.Object.Walls.Add(new WallProp { Cords = new Point(3, 7) });
            gameModelMock.Object.Walls.Add(new WallProp { Cords = new Point(5, 7) });
            gameModelMock.Object.Walls.Add(new WallProp { Cords = new Point(6, 7) });
            foreach (var item in gameModelMock.Object.Walls)
            {
                gameModelMock.Object.GameAreaChar[(int)item.Cords.X, (int)item.Cords.Y] = 'W';
            }
            gameModelMock.Object.Waters = new List<WaterProp>();
            gameModelMock.Object.Waters.Add(new WaterProp { Cords = new Point(0, 2) });
            gameModelMock.Object.Waters.Add(new WaterProp { Cords = new Point(1, 3) });
            gameModelMock.Object.Waters.Add(new WaterProp { Cords = new Point(3, 5) });
            foreach (var item in gameModelMock.Object.Waters)
            {
                gameModelMock.Object.GameAreaChar[(int)item.Cords.X, (int)item.Cords.Y] = 'P';
            }
            gameModelMock.Object.Lavas = new List<LavaProp>();
            gameModelMock.Object.Lavas.Add(new LavaProp { Cords = new Point(2, 1), Damage = 5 });
            gameModelMock.Object.Lavas.Add(new LavaProp { Cords = new Point(4, 1), Damage = 5 });
            gameModelMock.Object.Lavas.Add(new LavaProp { Cords = new Point(0, 3), Damage = 5 });
            foreach (var item in gameModelMock.Object.Lavas)
            {
                gameModelMock.Object.GameAreaChar[(int)item.Cords.X, (int)item.Cords.Y] = 'L';
            }
            gameModelMock.Object.ShootingMonsters = new List<ShootingEnemy>();
            gameModelMock.Object.ShootingMonsters.Add(new ShootingEnemy { Cords = new Point(1, 4), Damage = 5, Health = 40 });
            foreach (var item in gameModelMock.Object.ShootingMonsters)
            {
                gameModelMock.Object.GameAreaChar[(int)item.Cords.X, (int)item.Cords.Y] = 'S';
            }
            gameModelMock.Object.TrackingMonsters = new List<TrackingEnemy>();
            gameModelMock.Object.TrackingMonsters.Add(new TrackingEnemy { Cords = new Point(2, 5), Damage = 5, Health = 60 });
            foreach (var item in gameModelMock.Object.TrackingMonsters)
            {
                gameModelMock.Object.GameAreaChar[(int)item.Cords.X, (int)item.Cords.Y] = 'T';
            }
            gameModelMock.Object.FlyingMonsters = new List<FlyingEnemy>();
            gameModelMock.Object.FlyingMonsters.Add(new FlyingEnemy { Cords = new Point(4, 3), Damage = 10, Health = 30 });
            foreach (var item in gameModelMock.Object.FlyingMonsters)
            {
                gameModelMock.Object.GameAreaChar[(int)item.Cords.X, (int)item.Cords.Y] = 'F';
            }

            gameModelMock.Object.Powerups = new List<Powerups>();
            gameModelMock.Object.Powerups.Add(new Powerups { Cords = new Point(0, 2), Type = PowerupType.Health });
            gameModelMock.Object.Powerups.Add(new Powerups { Cords = new Point(2, 2), Type = PowerupType.Damage });
            gameModelMock.Object.Powerups.Add(new Powerups { Cords = new Point(5, 2), Type = PowerupType.FiringSpeed });
            foreach (var item in gameModelMock.Object.Powerups)
            {
                switch (item.Type)
                {
                    case PowerupType.Health:
                        gameModelMock.Object.GameAreaChar[(int)item.Cords.X, (int)item.Cords.Y] = 'H';
                        break;
                    case PowerupType.Damage:
                        gameModelMock.Object.GameAreaChar[(int)item.Cords.X, (int)item.Cords.Y] = 'D';
                        break;
                    case PowerupType.FiringSpeed:
                        gameModelMock.Object.GameAreaChar[(int)item.Cords.X, (int)item.Cords.Y] = 'R';
                        break;
                }
            }
            gameModelMock.Object.Projectiles = new List<Projectile>();

            gameLogicTest = new GameLogic(gameModelMock.Object);
            loadingLogicTest = new LoadingLogic(gameModelMock.Object, saveGameRepositoryMock.Object, highscoreRepoMock.Object);
        }
        [Test]
        public void MovePlayer()
        {
            Point starting = gameModelMock.Object.MyPlayer.Cords;
            Point moveToPossible = new Point(starting.X - 1, starting.Y - 1);
            gameLogicTest.MovePlayer(-1,-1);

            Assert.That(gameModelMock.Object.MyPlayer.Cords, Is.EqualTo(moveToPossible));
        }
        [Test]
        public void DamageActiveGameObject()
        {
            FlyingEnemy f = gameModelMock.Object.FlyingMonsters[0];
            int damagedHealth = gameModelMock.Object.FlyingMonsters[0].Health - 10;
            gameLogicTest.DamageActiveGameObject(f, 10);
            Assert.That(gameModelMock.Object.FlyingMonsters[0].Health, Is.EqualTo(damagedHealth));
        }
        [Test]
        public void DisposeBullet()
        {
            Projectile p = new Projectile(new Point(0,0), new Point(100,100));
            p.Damage = 5;
            gameModelMock.Object.Projectiles.Add(p);

            gameLogicTest.DisposeBullet(p);

            Assert.That(gameModelMock.Object.Projectiles, Does.Not.Contain(p));
        }
        [Test]
        public void DisposeEnemy()
        {
            ShootingEnemy s = new ShootingEnemy();
            gameModelMock.Object.ShootingMonsters.Add(s);
            gameLogicTest.DisposeEnemy(s);
            Assert.That(gameModelMock.Object.ShootingMonsters, Does.Not.Contain(s));
        }
        [Test]
        public void MoveFlyingEnemy()
        {
            FlyingEnemy f = gameModelMock.Object.FlyingMonsters[0];
            Point nextStep = new Point(4, 4);
            gameLogicTest.MoveFlyingEnemy(f);

            Assert.That(gameModelMock.Object.FlyingMonsters[0].Cords, Is.EqualTo(nextStep));
        }
        [Test]
        public void MoveRegularEnemy()
        {
            TrackingEnemy t = gameModelMock.Object.TrackingMonsters[0];
            Point nextStep = new Point(2, 6);
            gameLogicTest.MoveRegularEnemy(t);

            Assert.That(gameModelMock.Object.TrackingMonsters[0].Cords, Is.EqualTo(nextStep));
        }
        [Test]
        public void CollectHealthPotion()
        {
            gameModelMock.Object.MyPlayer.Health = 50;
            int entryHealth = 50;
            Powerups healthPotion = gameModelMock.Object.Powerups[0];
            gameLogicTest.CollectPowerup(healthPotion);
            Assert.That(gameModelMock.Object.MyPlayer.Health, Is.EqualTo(entryHealth + healthPotion.ModifyRate));
        }
        [Test]
        public void CollectDamagePowerup()
        {
            gameModelMock.Object.MyPlayer.Damage = 10;
            int entryDamage = 10;
            Powerups damagePotion = gameModelMock.Object.Powerups[1];
            gameLogicTest.CollectPowerup(damagePotion);
            Assert.That(gameModelMock.Object.MyPlayer.Damage, Is.EqualTo(entryDamage + damagePotion.ModifyRate));
        }
        [Test]
        public void CollectFiringSpeed()
        {
            gameModelMock.Object.MyPlayer.FiringSpeed = 10;
            int entryFiringSpeed = 10;
            Powerups firingSpeedPotion = gameModelMock.Object.Powerups[2];
            gameLogicTest.CollectPowerup(firingSpeedPotion);

            Assert.That(gameModelMock.Object.MyPlayer.FiringSpeed, Is.EqualTo(entryFiringSpeed + firingSpeedPotion.ModifyRate));
        }
        [Test]
        public void DropRandomCollectable()
        {
            int collectables = gameModelMock.Object.Powerups.Count;
            int expectedCollectableNumbers = collectables+1;
            gameLogicTest.DropRandomCollectable();
            Assert.That(gameModelMock.Object.Powerups.Count, Is.EqualTo(expectedCollectableNumbers));
        }
        [Test]
        public void PlayerShoot()
        {
            Point shootTo = new Point(200, 500);
            int shootingSpeed = 10;
            Point playercords = new Point(gameModelMock.Object.MyPlayer.Cords.X * gameModelMock.Object.TileSize, gameModelMock.Object.MyPlayer.Cords.Y * gameModelMock.Object.TileSize);
            Projectile expectedProjectile = new Projectile(playercords, shootTo);
            expectedProjectile.Type = ProjectileType.Player;
            expectedProjectile.Speed = shootingSpeed;
            Projectile shootProjectile = gameLogicTest.PlayerShoot(shootTo, shootingSpeed);
            Assert.That(shootProjectile,Is.EqualTo(expectedProjectile));
           
        }
    }
}
