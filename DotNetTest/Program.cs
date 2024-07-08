using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MyDotNetApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<MyApp>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .Build();

            await host.RunAsync();
        }
    }

    public class MyApp : IHostedService
    {
        private readonly ILogger<MyApp> _logger;
        private readonly string _dataFilePath = "data.json";
        private Dictionary<string, string> _data;

        public MyApp(ILogger<MyApp> logger)
        {
            _logger = logger;
            LoadData();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Application started.");
            HandleArguments(Environment.GetCommandLineArgs());
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Application stopped.");
            return Task.CompletedTask;
        }

        private void HandleArguments(string[] args)
        {
            if (args.Length > 1)
            {
                var command = args[1];
                if (command == "add" && args.Length == 4)
                {
                    var key = args[2];
                    var value = args[3];
                    _data[key] = value;
                    SaveData();
                    _logger.LogInformation($"Added: {key} = {value}");
                }
                else if (command == "get" && args.Length == 3)
                {
                    var key = args[2];
                    if (_data.TryGetValue(key, out var value))
                    {
                        _logger.LogInformation($"Value: {value}");
                    }
                    else
                    {
                        _logger.LogInformation("Key not found.");
                    }
                }
                else
                {
                    _logger.LogInformation("Invalid command.");
                }
            }
            else
            {
                _logger.LogInformation("No command provided.");
            }
        }

        private void LoadData()
        {
            if (File.Exists(_dataFilePath))
            {
                var json = File.ReadAllText(_dataFilePath);
                _data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }
            else
            {
                _data = new Dictionary<string, string>();
            }
        }

        private void SaveData()
        {
            var json = JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_dataFilePath, json);
        }
    }
}
