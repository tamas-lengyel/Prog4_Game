// <copyright file="ManualSaveGameRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Repository
{
    using System.IO;
    using System.Reflection;
    using Model;
    using Newtonsoft.Json;

    /// <summary>
    /// Repository for manual saving.
    /// </summary>
    public class ManualSaveGameRepository : StorageRepository<GameModel>, ISaveGameRepository
    {
        private string filename;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualSaveGameRepository"/> class.
        /// </summary>
        /// <param name="filename">Filename the user writes in the input field.</param>
        public ManualSaveGameRepository(string filename)
            : base()
        {
            this.filename = filename;

            if (!File.Exists(filename))
            {
                File.Create(filename).Close();
            }
        }

        /// <summary>
        /// Gets a GameModel from a savegame.json file.
        /// </summary>
        /// <returns>A GameModel.</returns>
        public GameModel GetSaveGame()
        {
            if (File.ReadAllText(this.filename) == string.Empty)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<GameModel>(File.ReadAllText(this.filename));
        }

        /// <summary>
        /// Inserts a GameModel into the save file as json.
        /// </summary>
        /// <param name="entity">A GameModel type object.</param>
        public override void Insert(GameModel entity)
        {
            string json = JsonConvert.SerializeObject(entity, Formatting.Indented);
            File.WriteAllText(this.filename, json);
        }
    }
}