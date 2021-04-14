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
            gameModelMock.Object.Wall = new List<WallProp>();
            gameModelMock.Object.Wall.Add(new WallProp { Cords = new Point(0, 1) });
            gameModelMock.Object.Wall.Add(new WallProp { Cords = new Point(6, 1) });
            gameModelMock.Object.Wall.Add(new WallProp { Cords = new Point(2, 3) });
            gameModelMock.Object.Wall.Add(new WallProp { Cords = new Point(5, 3) });
            gameModelMock.Object.Wall.Add(new WallProp { Cords = new Point(6, 3) });
            gameModelMock.Object.Wall.Add(new WallProp { Cords = new Point(0, 5) });
            gameModelMock.Object.Wall.Add(new WallProp { Cords = new Point(6, 5) });
            gameModelMock.Object.Wall.Add(new WallProp { Cords = new Point(1, 7) });
            gameModelMock.Object.Wall.Add(new WallProp { Cords = new Point(3, 7) });
            gameModelMock.Object.Wall.Add(new WallProp { Cords = new Point(5, 7) });
            gameModelMock.Object.Wall.Add(new WallProp { Cords = new Point(6, 7) });
            foreach (var item in gameModelMock.Object.Wall)
            {
                gameModelMock.Object.GameAreaChar[(int)item.Cords.X, (int)item.Cords.Y] = 'W';
            }
            gameModelMock.Object.Water = new List<WaterProp>();
            gameModelMock.Object.Water.Add(new WaterProp { Cords = new Point(0, 2) });
            gameModelMock.Object.Water.Add(new WaterProp { Cords = new Point(1, 3) });
            gameModelMock.Object.Water.Add(new WaterProp { Cords = new Point(3, 5) });
            foreach (var item in gameModelMock.Object.Water)
            {
                gameModelMock.Object.GameAreaChar[(int)item.Cords.X, (int)item.Cords.Y] = 'P';
            }
            gameModelMock.Object.Lava = new List<LavaProp>();
            gameModelMock.Object.Lava.Add(new LavaProp { Cords = new Point(2, 1), Damage = 5 });
            gameModelMock.Object.Lava.Add(new LavaProp { Cords = new Point(4, 1), Damage = 5 });
            gameModelMock.Object.Lava.Add(new LavaProp { Cords = new Point(0, 3), Damage = 5 });
            foreach (var item in gameModelMock.Object.Lava)
            {
                gameModelMock.Object.GameAreaChar[(int)item.Cords.X, (int)item.Cords.Y] = 'L';
            }
            gameModelMock.Object.ShootingMonster = new List<ShootingEnemy>();
            gameModelMock.Object.ShootingMonster.Add(new ShootingEnemy { Cords = new Point(1, 4), Damage = 5, Health = 40 });
            foreach (var item in gameModelMock.Object.ShootingMonster)
            {
                gameModelMock.Object.GameAreaChar[(int)item.Cords.X, (int)item.Cords.Y] = 'S';
            }
            gameModelMock.Object.TrackingMonster = new List<TrackingEnemy>();
            gameModelMock.Object.TrackingMonster.Add(new TrackingEnemy { Cords = new Point(2, 5), Damage = 5, Health = 60 });
            foreach (var item in gameModelMock.Object.TrackingMonster)
            {
                gameModelMock.Object.GameAreaChar[(int)item.Cords.X, (int)item.Cords.Y] = 'T';
            }
            gameModelMock.Object.FlyingMonster = new List<FlyingEnemy>();
            gameModelMock.Object.FlyingMonster.Add(new FlyingEnemy { Cords = new Point(4, 3), Damage = 10, Health = 30 });
            foreach (var item in gameModelMock.Object.FlyingMonster)
            {
                gameModelMock.Object.GameAreaChar[(int)item.Cords.X, (int)item.Cords.Y] = 'F';
            }

            gameModelMock.Object.Powerup = new List<Powerups>();
            gameModelMock.Object.Powerup.Add(new Powerups { Cords = new Point(0, 2), Type = PowerupType.Health });
            foreach (var item in gameModelMock.Object.Powerup)
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
            FlyingEnemy f = gameModelMock.Object.FlyingMonster[0];
            int damagedHealth = gameModelMock.Object.FlyingMonster[0].Health - 10;
            gameLogicTest.DamageActiveGameObject(f, 10);
            Assert.That(gameModelMock.Object.FlyingMonster[0].Health, Is.EqualTo(damagedHealth));
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
            gameModelMock.Object.ShootingMonster.Add(s);
            gameLogicTest.DisposeEnemy(s);
            Assert.That(gameModelMock.Object.ShootingMonster, Does.Not.Contain(s));
        }
        [Test]
        public void MoveFlyingEnemy()
        {
            FlyingEnemy f = gameModelMock.Object.FlyingMonster[0];
            Point nextStep = new Point(4, 4);
            gameLogicTest.MoveFlyingEnemy(f);

            Assert.That(gameModelMock.Object.FlyingMonster[0].Cords, Is.EqualTo(nextStep));
        }
        [Test]
        public void CollectHealthPotion()
        {
            gameModelMock.Object.MyPlayer.Health = 50;
            int entryHealth = 50;
            Powerups healthPotion = gameModelMock.Object.Powerup[0];
            gameLogicTest.CollectPowerup(healthPotion);
            Assert.That(gameModelMock.Object.MyPlayer.Health, Is.EqualTo(entryHealth + healthPotion.ModifyRate));
        }
    }
}
