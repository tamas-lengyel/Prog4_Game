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
        public void EndGame(string name);
        public void GenerateMap(IGameModel gm);
        public IQueryable<Highscore> GetHighscores();
        public void NextLevel();
        public IGameModel Play();

    }
}
