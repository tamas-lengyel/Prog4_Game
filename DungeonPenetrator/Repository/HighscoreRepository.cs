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
    public class HighscoreRepository : StorageRepository<Highscore>, IHighscoreRepository
    {
        string filename;
        public HighscoreRepository() : base()
        {
            this.filename = "highscores.json";
            if (!Directory.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $@"\Saves\")))
            {
                Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"\Saves\"));
            }
            if (!File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $@"\Saves\{filename}")))
            {
                File.Create(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $@"\Saves\{filename}")).Close();
            }
        }

        public IQueryable<Highscore> GetAll()
        {
            if (File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $@"\Saves\{filename}")) == string.Empty)
            {
                return new List<Highscore>().AsQueryable();
            }
            return JsonConvert.DeserializeObject<List<Highscore>>(File.ReadAllText(
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $@"\Saves\{filename}"))).AsQueryable();
        }

        public override void Insert(Highscore entity)
        {
            List<Highscore> list = GetAll().ToList();
            list.Add(entity);
            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $@"\Saves\{filename}"), json);
        }
    }
}
