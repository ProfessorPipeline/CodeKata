using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MyDotNetApp
{
    class Program
    {
        private static readonly string DataFilePath = "data.json";
        private static Dictionary<string, string> _data = new();

        static void Main(string[] args)
        {
            LoadData();

            if (args.Length == 0)
            {
                Console.WriteLine("No command provided.");
                return;
            }

            switch (args[0])
            {
                case "add" when args.Length == 3:
                    AddData(args[1], args[2]);
                    break;

                case "get" when args.Length == 2:
                    GetData(args[1]);
                    break;

                default:
                    Console.WriteLine("Invalid command.");
                    break;
            }
        }

        private static void AddData(string key, string value)
        {
            _data[key] = value;
            SaveData();
            Console.WriteLine($"Added: {key} = {value}");
        }

        private static void GetData(string key)
        {
            if (_data.TryGetValue(key, out var value))
            {
                Console.WriteLine($"Value: {value}");
            }
            else
            {
                Console.WriteLine("Key not found.");
            }
        }

        private static void LoadData()
        {
            if (File.Exists(DataFilePath))
            {
                var json = File.ReadAllText(DataFilePath);
                _data = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            }
        }

        private static void SaveData()
        {
            var json = JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(DataFilePath, json);
        }
    }
}
