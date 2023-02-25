using System;
using System.IO;
using UnityEngine;

namespace SandSimulation
{
    [Serializable]
    public class JsonCellDatabaseProvider : ICellDatabaseProvider
    {
        public const string AssetsRelativeFolder = "Settings";
        public const string FileName = "CellDatabase.json";
        
        public static readonly string AssetsDirectory = Path.Combine("Assets", AssetsRelativeFolder);
        public static readonly string AssetsFilePath = Path.Combine(AssetsDirectory, FileName);
        public static readonly string FileSystemPath =
            Path.Combine(Application.dataPath, AssetsRelativeFolder, FileName);
        
        private CellDatabase _database;
        public CellDatabase Database => GetDatabase();

        public void SaveChanges()
        {
            Save(_database);
        }
        
        public void Dispose()
        {
            _database?.Dispose();
            _database = null;
        }
        
        private void Load()
        {
            Dispose();
            
            if (!File.Exists(FileSystemPath))
            {
                _database = new CellDatabase();
                Save(_database);
                return;
            }

            var databaseJson = File.ReadAllText(FileSystemPath);
            _database = JsonUtility.FromJson<CellDatabase>(databaseJson);
            _database.Initialize();
            DisposableManager.TrackDisposable(this);
        }

        private void Save(CellDatabase database)
        {
            var json = JsonUtility.ToJson(database);
            File.WriteAllText(FileSystemPath, json);
        }
        
        private CellDatabase GetDatabase()
        {
            if (_database == null) Load();
            return _database;
        }
    }
}
