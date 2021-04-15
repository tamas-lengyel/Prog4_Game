using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SaveGameRepository : StorageRepository<GameModel>, ISaveGameRepository
    {
        string filename;
        public SaveGameRepository() : base()
        {
            this.filename = "savegame.json";
            if (!Directory.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $@"\Saves\")))
            {
                Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"\Saves"));
            }
            if (!File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $@"\Saves\{filename}")))
            {
                File.Create(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $@"\Saves\{filename}")).Close();
            }
        }

        public GameModel GetSaveGame()
        {
            if (File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $@"\Saves\{filename}")) == string.Empty)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<GameModel>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $@"\Saves\{filename}")));
        }

        public override void Insert(GameModel entity)
        {
            string json = JsonConvert.SerializeObject(entity, Formatting.Indented);
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $@"\Saves\{filename}"), json);
        }
    }
}
