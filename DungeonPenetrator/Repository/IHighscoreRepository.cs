// <copyright file="IHighscoreRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Repository
{
    using System.Linq;
    using Model;

    /// <summary>
    /// Interface for Highscore repo.
    /// </summary>
    public interface IHighscoreRepository : IStorageRepository<Highscore>
    {
        /// <summary>
        /// Gets all Highscores from highscore.json file.
        /// </summary>
        /// <returns>A Highscore type Queryable.</returns>
        IQueryable<Highscore> GetAll();
    }
}