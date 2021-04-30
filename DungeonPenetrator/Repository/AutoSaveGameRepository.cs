// <copyright file="SaveGameRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Repository
{
    using System.IO;
    using System.Reflection;
    using Model;
    using Newtonsoft.Json;

    /// <summary>
    /// SaveGame repo, this manages the saves.
    /// </summary>
    public class AutoSaveGameRepository : StorageRepository<GameModel>, ISaveGameRepository
    {
        private string filename;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoSaveGameRepository"/> class.
        /// </summary>
        public AutoSaveGameRepository()
            : base()
        {
            this.filename = "autosavegame.json";
            if (!Directory.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Saves\"))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Saves\");
            }

            if (!File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Saves\{this.filename}"))
            {
                File.Create(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Saves\{this.filename}").Close();
            }
        }

        /// <summary>
        /// Gets a GameModel from a savegame.json file.
        /// </summary>
        /// <returns>A GameModel.</returns>
        public GameModel GetSaveGame()
        {
            if (File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Saves\{this.filename}") == string.Empty)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<GameModel>(File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Saves\{this.filename}"));
        }

        /// <summary>
        /// Inserts a GameModel into the save file as json.
        /// </summary>
        /// <param name="entity">A GameModel type object.</param>
        public override void Insert(GameModel entity)
        {
            string json = JsonConvert.SerializeObject(entity, Formatting.Indented);
            File.WriteAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Saves\{this.filename}", json);
        }
    }
}