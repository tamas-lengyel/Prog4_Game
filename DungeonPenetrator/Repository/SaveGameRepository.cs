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
            if (!File.Exists(Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent.Parent.Parent.Parent + @"\Repository" + $@"\Saves\{filename}"))
            {
                File.Create(Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent.Parent.Parent.Parent + @"\Repository" + $@"\Saves\{filename}");
            }
        }

        public GameModel GetSaveGame()
        {
            if (File.ReadAllText(Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent.Parent.Parent.Parent + @"\Repository" + $@"\Saves\{filename}") == string.Empty)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<GameModel>(File.ReadAllText(Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent.Parent.Parent.Parent + @"\Repository" + $@"\Saves\{filename}"));
        }

        public override void Insert(GameModel entity)
        {
            string json = JsonConvert.SerializeObject(entity, Formatting.Indented);
            File.WriteAllText(Directory.GetParent(Assembly.GetExecutingAssembly()
                .Location).Parent.Parent.Parent.Parent + @"\Repository" + $@"\Saves\{filename}", json);
        }
    }
}
