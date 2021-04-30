using Model;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace Repository
{
    public class ManualSaveGameRepository : StorageRepository<GameModel>, ISaveGameRepository
    {
        private string filename;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualSaveGameRepository"/> class.
        /// </summary>
        public ManualSaveGameRepository(string filename)
            : base()
        {
            this.filename = filename;
            /*if (!Directory.Exists(filename))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\Saves\");
            }*/

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
            if (File.ReadAllText(filename) == string.Empty)
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