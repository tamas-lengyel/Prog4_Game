using Model;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Logic
{
    public class WindowLogic : IWindowLogic
    {
        IGameModel gameModel;
        ISaveGameRepository saveGameRepository;
        IHighscoreRepository highscoreRepository;

        public WindowLogic(IGameModel gameModel, ISaveGameRepository saveGameRepository, IHighscoreRepository highscoreRepository)
        {
            this.gameModel = gameModel;
            this.saveGameRepository = saveGameRepository;
            this.highscoreRepository = highscoreRepository;
        }

        public void EndGame(string name)
        {
            Highscore h = new Highscore();
            h.Level = gameModel.LevelCounter;
            h.Name = name;
            highscoreRepository.Insert(h);
            saveGameRepository.Insert(default);
        }

        public void GenerateMap(IGameModel gm)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Highscore> GetHighscores()
        {
            return highscoreRepository.GetAll().OrderBy(x => x.Level);
        }

        public void NextLevel()
        {
            gameModel.LevelCounter++;
            gameModel.MyPlayer.Cords = new Point(
                (int)gameModel.GameWidth/gameModel.TileSize,
                (int)(gameModel.GameHeight/gameModel.TileSize)-1);
            gameModel.LevelFinished = false; //itt
            GenerateMap(gameModel);
        }

        public IGameModel Play()
        {
            gameModel = saveGameRepository.GetSaveGame();
            if (gameModel.Equals(default))
            {
                GenerateMap(gameModel);
                return gameModel;
            }
            return gameModel;
        }
    }
}
