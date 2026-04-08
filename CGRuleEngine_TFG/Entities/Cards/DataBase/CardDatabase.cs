using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace CGRuleEngine_TFG.Entities.Cards.DataBase
{
    public class CardDatabase
    {
        private readonly Dictionary<string, CardTemplate> _templates = new Dictionary<string, CardTemplate>();
        private readonly string _sourcePath;

        public int Count => _templates.Count;

        // Constructor that takes the path to the directory containing card templates
        public CardDatabase(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"The path provided doesnt exist.");
            }

            _sourcePath = path;
            LoadAllTemplates();
        }

        // Method to load all card templates from the specified directory
        private void LoadAllTemplates()
        {
            _templates.Clear();

            string[] files = Directory.GetFiles(_sourcePath, "*.json", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                try
                {
                    string jsonContent = File.ReadAllText(file);

                    // Deserialize the JSON content into a CardTemplate object
                    CardTemplate? template = JsonConvert.DeserializeObject<CardTemplate>(jsonContent);

                    if (template != null && !string.IsNullOrEmpty(template.Id))
                    {
                        if (_templates.ContainsKey(template.Id))
                        {
                            throw new InvalidOperationException($"ERROR: ID duplicado ({template.Id}) en el archivo {file}");
                        }

                        _templates.Add(template.Id, template);
                    }
                }
                catch (Exception ex)
                {
                    // Log the error and continue loading other templates
                    Console.WriteLine($"Error al cargar el archivo {file}: {ex.Message}");
                }
            }
        }

        // Method to retrieve a card template by its ID
        public CardTemplate GetTemplate(string cardId)
        {
            if (_templates.TryGetValue(cardId, out CardTemplate? template))
            {
                return template;
            }
            throw new KeyNotFoundException($"No existe ninguna carta con el ID: {cardId}");
        }

        // Method to check if a card template with the specified ID exists in the database
        public bool Contains(string cardId)
        {
            return _templates.ContainsKey(cardId);
        }

        // Method to reload all card templates from the source directory, useful for development when templates are frequently updated
        public void Reload()
        {
            LoadAllTemplates();
        }

        // Method to retrieve all card templates in the database
        public IEnumerable<CardTemplate> GetAllTemplates()
        {
            return _templates.Values;
        }

        // Method to filter card templates based on a provided predicate, allowing for flexible querying of the database
        public List<CardTemplate> FilterBy(Func<CardTemplate, bool> predicate)
        {
            return _templates.Values.Where(predicate).ToList();
        }
    }
}