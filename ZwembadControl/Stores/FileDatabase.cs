using System.Text.Json;

namespace ZwembadControl.Stores
{
    public interface IFileDatabase<T>
    {
        void Save(T item);
        List<T> GetAll();
        T GetById(Func<T, bool> predicate);
        void Delete(Func<T, bool> predicate);
    }

    public class FileDatabase<T> : IFileDatabase<T>
    {
        private readonly string _filePath;
        private static readonly object _lock = new object();

        public FileDatabase(string filePath)
        {
            _filePath = filePath;
            EnsureFileExists();
        }

        public void Save(T item)
        {
            lock (_lock)
            {
                List<T> items = GetAll();
                items.Add(item);
                File.WriteAllText(_filePath, JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        public List<T> GetAll()
        {
            lock (_lock)
            {
                string json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
        }

        public T GetById(Func<T, bool> predicate)
        {
            lock (_lock)
            {
                return GetAll().FirstOrDefault(predicate);
            }
        }

        public void Delete(Func<T, bool> predicate)
        {
            lock (_lock)
            {
                List<T> items = GetAll();
                items.RemoveAll(new Predicate<T>(predicate));
                File.WriteAllText(_filePath, JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        private void EnsureFileExists()
        {
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }
    }
}
