using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public interface IWindowLogic
    {
        void EndGame(string name);
        void GenerateMap();
        IQueryable<Highscore> GetHighscores();
        void NextLevel();
        IGameModel Play();

    }
}
