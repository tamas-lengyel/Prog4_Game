// <copyright file="ILoadingLogic.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Logic
{
    using System.Linq;
    using Model;

    /// <summary>
    /// Interface for LoadingLogic.
    /// </summary>
    public interface ILoadingLogic
    {
        void EndGame(string name);

        void GenerateMap();

        IQueryable<Highscore> GetHighscores();

        void NextLevel();

        IGameModel Play();
    }
}