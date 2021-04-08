using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Repository
{
    public class HighscoreRepository : StorageRepository<Highscore>, IHighscoreRepository<Highscore>
    {
        string filename;
        public HighscoreRepository() : base()
        {
            this.filename = "highscores.json";
            if (!File.Exists(Directory.GetParent(Assembly.GetExecutingAssembly()
                .Location).Parent.Parent.Parent.Parent + @"\Repository" + $@"\Saves\{filename}"))
            {
                File.Create(Directory.GetParent(Assembly.GetExecutingAssembly()
                    .Location).Parent.Parent.Parent.Parent + @"\Repository" + $@"\Saves\{filename}");
            }
        }

        public IQueryable<Highscore> GetAll()
        {
            if (File.ReadAllText(Directory.GetParent(Assembly.GetExecutingAssembly().Location)
                .Parent.Parent.Parent.Parent + @"\Repository" + $@"\Saves\{filename}") == string.Empty)
            {
                return new List<Highscore>().AsQueryable();
            }
            return JsonConvert.DeserializeObject<List<Highscore>>(File.ReadAllText(Directory
                .GetParent(Assembly.GetExecutingAssembly()
                .Location).Parent.Parent.Parent.Parent + @"\Repository" + $@"\Saves\{filename}")).AsQueryable();
        }

        public override void Insert(Highscore entity)
        {
            List<Highscore> list = GetAll().ToList();
            list.Add(entity);
            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(Directory.GetParent(Assembly.GetExecutingAssembly()
                .Location).Parent.Parent.Parent.Parent + @"\Repository" + $@"\Saves\{filename}", json);
        }
    }
}
