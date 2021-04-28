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
        /// <summary>
        /// Defines the game state changes when the gameplay is over.
        /// </summary>
        /// <param name="name">Defines the name for saving purposes.</param>
        void EndGame(string name);

        /// <summary>
        /// Generates a map whether it's moving between levels or if it's the first generation.
        /// </summary>
        void GenerateMap();

        /// <summary>
        /// Gets all the saved highscores.
        /// </summary>
        /// <returns>A pre defined highscore entity.</returns>
        IQueryable<Highscore> GetHighscores();

        /// <summary>
        /// Updates the game state to the next level.
        /// </summary>
        void NextLevel();

        /// <summary>
        /// Loads in a gamestate either from a saved or newly generated one.
        /// </summary>
        /// <returns>The game state.</returns>
        IGameModel Play();
    }
}