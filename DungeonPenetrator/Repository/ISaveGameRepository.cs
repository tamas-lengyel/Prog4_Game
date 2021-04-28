// <copyright file="ISaveGameRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Repository
{
    using Model;

    /// <summary>
    /// Interface for SaveGame repo.
    /// </summary>
    public interface ISaveGameRepository : IStorageRepository<GameModel>
    {
        /// <summary>
        /// Gets a GameModel from a savegame.json file.
        /// </summary>
        /// <returns>A GameModel.</returns>
        GameModel GetSaveGame();
    }
}