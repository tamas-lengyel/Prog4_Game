// <copyright file="LoadingLogic.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Model;
    using Model.Active;
    using Model.Passive;
    using Repository;

    /// <summary>
    /// Defines the loading of a game state.
    /// </summary>
    public class LoadingLogic : ILoadingLogic
    {
        private Random rnd = new Random();
        private IGameModel gameModel;
        private ISaveGameRepository saveGameRepository;
        private IHighscoreRepository highscoreRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingLogic"/> class.
        /// </summary>
        /// <param name="gameModel">Defines a gamestate.</param>
        /// <param name="saveGameRepository">Defines the repository of the gamesaving.</param>
        /// <param name="highscoreRepository">Defines the repository of the highscore system.</param>
        public LoadingLogic(IGameModel gameModel, ISaveGameRepository saveGameRepository, IHighscoreRepository highscoreRepository)
        {
            this.gameModel = gameModel;
            this.saveGameRepository = saveGameRepository;
            this.highscoreRepository = highscoreRepository;
        }

        /// <inheritdoc/>
        public IQueryable<Highscore> GetHighscores()
        {
            return this.highscoreRepository.GetAll().OrderBy(x => x.Level);
        }

        /// <inheritdoc/>
        public void NextLevel()
        {
            this.GenerateMap();
        }

        /// <inheritdoc/>
        public IGameModel Play()
        {
            this.gameModel = this.saveGameRepository.GetSaveGame();
            if (this.gameModel == null)
            {
                this.GenerateMap();
                this.saveGameRepository.Insert(this.gameModel as GameModel);
                return this.gameModel;
            }

            return this.gameModel;
        }

        /// <inheritdoc/>
        public void EndGame(string name)
        {
            Highscore h = new Highscore();
            h.Level = this.gameModel.LevelCounter - 1;
            h.Name = name;
            this.highscoreRepository.Insert(h);
            this.saveGameRepository.Insert(default);
        }

        /// <inheritdoc/>
        public void GenerateMap()
        {
            // static starting values of a newly created map
            if (this.gameModel == null)
            {
                this.gameModel = new GameModel();
                this.gameModel.MyPlayer = new Player();
                this.gameModel.MyPlayer.FiringSpeed = 1;
                this.gameModel.MyPlayer.Damage = 20;
                this.gameModel.MyPlayer.Health = 100;
                this.gameModel.LevelCounter = 0; // Gets raised to one, must be zero
            }

            int rndBiome = this.rnd.Next(0, 3);
            switch (rndBiome)
            {
                case 0:
                    this.gameModel.BiomeType = Biome.Plains;
                    break;
                case 1:
                    this.gameModel.BiomeType = Biome.Desert;
                    break;
                case 2:
                    this.gameModel.BiomeType = Biome.Snowy;
                    break;
                default:
                    this.gameModel.BiomeType = Biome.Plains;
                    break;
            }

            this.gameModel.MyPlayer.IsReloading = false;
            this.gameModel.MyPlayer.BeingDamagedByLava = false;
            this.gameModel.GameIsPaused = false;
            this.gameModel.Projectiles = new List<Projectile>();
            this.gameModel.Powerups = new List<Powerups>();
            this.gameModel.FlyingMonsters = new List<FlyingEnemy>();
            this.gameModel.ShootingMonsters = new List<ShootingEnemy>();
            this.gameModel.TrackingMonsters = new List<TrackingEnemy>();
            this.gameModel.Lavas = new List<LavaProp>();
            this.gameModel.Walls = new List<WallProp>();
            this.gameModel.Waters = new List<WaterProp>();
            this.gameModel.GameAreaChar = new char[(int)(this.gameModel.GameWidth / GameModel.TileSize), (int)(this.gameModel.GameHeight / GameModel.TileSize)];
            this.gameModel.LevelCounter++;
            this.gameModel.MyPlayer.Cords = new Point(
                (int)(this.gameModel.GameWidth / GameModel.TileSize / 2),
                (int)(this.gameModel.GameHeight / GameModel.TileSize) - 1);
            this.gameModel.LevelFinished = false;

            this.GenerateInitializedEmptyMap();

            if (this.gameModel.LevelCounter % 10 == 0)
            {
                /*for (int y = 1; y <= gameModel.GameAreaChar.GetLength(1) - 2; y += 6)
                {
                    int[] EmptySpaceCords = GenerateEmptySpacesForRow();
                    for (int x = 0; x < gameModel.GameAreaChar.GetLength(0); x++)
                    {
                        if (!EmptySpaceCords.Contains(x))
                        {
                            int randomProp = rnd.Next(0, 200);
                            switch (randomProp)
                            {
                                case < 150:
                                    gameModel.GameAreaChar[x, y] = 'W'; // Generates Wall
                                    break;

                                case > 150 and < 180:
                                    gameModel.GameAreaChar[x, y] = 'P'; // Generates "Puddle" (Water)
                                    break;

                                case > 180 and < 200:
                                    gameModel.GameAreaChar[x, y] = 'L'; // Generates Lava
                                    break;
                            }
                        }
                    }
                }
                GenerateCollectables();*/
                this.gameModel.GameAreaChar[(int)((this.gameModel.GameWidth / GameModel.TileSize) / 2), (int)((this.gameModel.GameHeight / GameModel.TileSize) / 2)] = 'B';
            }
            else
            {
                this.GenerateProps();
                this.GenerateCollectables();
                this.GenerateBasicEnemies();
            }

            for (int y = 0; y < this.gameModel.GameAreaChar.GetLength(1); y++)
            {
                for (int x = 0; x < this.gameModel.GameAreaChar.GetLength(0); x++)
                {
                    // Some parts here shall not be hardcoded.
                    switch (this.gameModel.GameAreaChar[x, y])
                    {
                        case 'W':
                            this.gameModel.Walls.Add(new WallProp { Cords = new Point(x, y) });
                            break;

                        case 'L':
                            this.gameModel.Lavas.Add(new LavaProp { Cords = new Point(x, y), Damage = 15 });
                            break;

                        case 'P':
                            this.gameModel.Waters.Add(new WaterProp { Cords = new Point(x, y) });
                            break;

                        case 'H':
                            this.gameModel.Powerups.Add(new Powerups(new Point(x, y), PowerupType.Health));
                            break;

                        case 'D':
                            this.gameModel.Powerups.Add(new Powerups(new Point(x, y), PowerupType.Damage));
                            break;

                        case 'R':
                            this.gameModel.Powerups.Add(new Powerups(new Point(x, y), PowerupType.FiringSpeed));
                            break;

                        case 'F':
                            this.gameModel.FlyingMonsters.Add(new FlyingEnemy { Cords = new Point(x, y), Damage = 10, Health = 30 * ((int)(this.gameModel.LevelCounter / 10) + 1), });
                            break;

                        case 'T':
                            this.gameModel.TrackingMonsters.Add(new TrackingEnemy { Cords = new Point(x, y), Damage = 2, Health = 60 * ((int)(this.gameModel.LevelCounter / 10) + 1), CanAttack = true, });
                            break;

                        case 'S':
                            this.gameModel.ShootingMonsters.Add(new ShootingEnemy { Cords = new Point(x, y), Damage = 10, Health = 40 * ((int)(this.gameModel.LevelCounter / 10) + 1), });
                            break;

                        case 'B':
                            this.gameModel.Boss = new BossEnemy { Cords = new Point(x, y), Damage = 15, Health = 3000 * ((int)(this.gameModel.LevelCounter / 10)), PlayerInSight = false, };
                            break;
                    }
                }
            }
        }

        private void GenerateInitializedEmptyMap()
        {
            for (int y = 0; y < this.gameModel.GameAreaChar.GetLength(1); y++)
            {
                for (int x = 0; x < this.gameModel.GameAreaChar.GetLength(0); x++)
                {
                    this.gameModel.GameAreaChar[x, y] = 'E'; // Generates Empty Cell
                }
            }

            this.gameModel.GameAreaChar[(int)this.gameModel.MyPlayer.Cords.X, (int)this.gameModel.MyPlayer.Cords.Y] = 'C'; // Sets Character->Player pos
            this.gameModel.GameAreaChar[(int)this.gameModel.LevelExit.X, (int)this.gameModel.LevelExit.Y] = 'G'; // Sets Goal->LevelExit pos
        }

        private void GenerateBasicEnemies()
        {
            int rndObjectNum = this.rnd.Next(15, (int)((int)(this.gameModel.GameWidth / GameModel.TileSize) * (int)(this.gameModel.GameHeight / GameModel.TileSize) / (GameModel.TileSize / 4)));
            for (int i = 0; i < rndObjectNum; i++)
            {
                Tuple<int, int> rndCord = new Tuple<int, int>(this.rnd.Next(2, (int)(this.gameModel.GameWidth / GameModel.TileSize) - 2), this.rnd.Next(3, (int)(this.gameModel.GameHeight / GameModel.TileSize) - 3));
                if (!(rndCord.Item1 == this.gameModel.MyPlayer.Cords.X && rndCord.Item2 == this.gameModel.MyPlayer.Cords.Y)
                    && !(rndCord.Item1 == this.gameModel.LevelExit.X && rndCord.Item2 == this.gameModel.LevelExit.Y))
                {
                    int randomObject = this.rnd.Next(0, 100);
                    switch (randomObject)
                    {
                        case < 75:
                            this.gameModel.GameAreaChar[rndCord.Item1, rndCord.Item2] = 'T'; // Generates Tracking monster
                            break;

                        case > 75 and < 87:
                            this.gameModel.GameAreaChar[rndCord.Item1, rndCord.Item2] = 'F'; // Generates Flying monster
                            break;

                        case > 87:
                            this.gameModel.GameAreaChar[rndCord.Item1, rndCord.Item2] = 'S'; // Generates shooting monster
                            break;
                    }
                }
            }
        }

        private void GenerateCollectables()
        {
            int rndObjectNum = this.rnd.Next(0, (int)((int)(this.gameModel.GameWidth / GameModel.TileSize) * (int)(this.gameModel.GameHeight / GameModel.TileSize) / (GameModel.TileSize / 2)));
            for (int i = 0; i < rndObjectNum; i++)
            {
                Tuple<int, int> rndCord = new Tuple<int, int>(this.rnd.Next((int)(this.gameModel.GameWidth / GameModel.TileSize)), this.rnd.Next((int)(this.gameModel.GameHeight / GameModel.TileSize)));
                if (!(rndCord.Item1 == this.gameModel.MyPlayer.Cords.X && rndCord.Item2 == this.gameModel.MyPlayer.Cords.Y)
                    && !(rndCord.Item1 == this.gameModel.LevelExit.X && rndCord.Item2 == this.gameModel.LevelExit.Y))
                {
                    int randomObject = this.rnd.Next(0, 3);
                    switch (randomObject)
                    {
                        case 0:
                            this.gameModel.GameAreaChar[rndCord.Item1, rndCord.Item2] = 'H'; // Generates HP powerup
                            break;

                        case 1:
                            this.gameModel.GameAreaChar[rndCord.Item1, rndCord.Item2] = 'D'; // Generates Damage powerup
                            break;

                        case 2:
                            this.gameModel.GameAreaChar[rndCord.Item1, rndCord.Item2] = 'R'; // Generates "Reload speed" firing powerup
                            break;
                    }
                }
            }
        }

        private void GenerateProps()
        {
            for (int y = 1; y <= this.gameModel.GameAreaChar.GetLength(1) - 2; y += 2)
            {
                int[] emptySpaceCords = this.GenerateEmptySpacesForRow();
                for (int x = 0; x < this.gameModel.GameAreaChar.GetLength(0); x++)
                {
                    if (!emptySpaceCords.Contains(x))
                    {
                        int randomProp = this.rnd.Next(0, 200);
                        switch (randomProp)
                        {
                            case < 150:
                                this.gameModel.GameAreaChar[x, y] = 'W'; // Generates Wall
                                break;

                            case > 150 and < 180:
                                this.gameModel.GameAreaChar[x, y] = 'P'; // Generates "Puddle" (Water)
                                break;

                            case > 180 and < 200:
                                this.gameModel.GameAreaChar[x, y] = 'L'; // Generates Lava
                                break;
                        }
                    }
                }
            }
        }

        private int[] GenerateEmptySpacesForRow()
        {
            int[] emptySpaceCords = new int[this.rnd.Next(3, (int)(this.gameModel.GameWidth / GameModel.TileSize) - 1)];
            for (int i = 0; i < emptySpaceCords.Length; i++)
            {
                emptySpaceCords[i] = this.rnd.Next(0, (int)(this.gameModel.GameWidth / GameModel.TileSize));
            }

            return emptySpaceCords;
        }
    }
}