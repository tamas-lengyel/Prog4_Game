// <copyright file="HighscoreRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Repository
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Model;
    using Newtonsoft.Json;

    /// <summary>
    /// Highscore repo, this manages the highscores.
    /// </summary>
    public class HighscoreRepository : StorageRepository<Highscore>, IHighscoreRepository
    {
        private string filename;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighscoreRepository"/> class.
        /// </summary>
        public HighscoreRepository()
            : base()
        {
            this.filename = "highscores.json";
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
        /// Gets the Highscores as a Queryable.
        /// </summary>
        /// <returns>A Highscore type Queryable.</returns>
        public IQueryable<Highscore> GetAll()
        {
            if (File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Saves\{this.filename}") == string.Empty)
            {
                return new List<Highscore>().AsQueryable();
            }

            return JsonConvert.DeserializeObject<List<Highscore>>(File.ReadAllText(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Saves\{this.filename}")).AsQueryable();
        }

        /// <summary>
        /// Inserts a highscore into a file as json.
        /// </summary>
        /// <param name="entity">Highscore type object.</param>
        public override void Insert(Highscore entity)
        {
            List<Highscore> list = this.GetAll().ToList();
            list.Add(entity);
            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Saves\{this.filename}", json);
        }
    }
}