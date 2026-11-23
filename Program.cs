/*
 * PROGRAM.CS - Точка входа приложения
 * 
 * ОПИСАНИЕ:
 * Главный файл программы. Инициализирует все компоненты и запускает бота.
 * 
 * АРХИТЕКТУРА ЗАПУСКА:
 * 1. Загрузить конфигурацию (appsettings.json + secrets.txt)
 * 2. Инициализировать Storage (SQLite)
 * 3. Загрузить плагины через PluginManager
 * 4. Создать AI провайдер через Factory
 * 5. Создать и запустить TelegramBotHandler
 * 6. Работать до Ctrl+C или ошибки
 * 7. Корректно завершить работу (dispose всех ресурсов)
 * 
 * ОБРАБОТКА ОШИБОК:
 * - Логировать все ошибки
 * - При критических ошибках завершать работу
 * - Graceful shutdown при Ctrl+C
 */

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MyTgBot2.Core.Services;
using MyTgBot2.Core.Interfaces;
using MyTgBot2.AI.Adapters;
using MyTgBot2.AI.Providers;
using MyTgBot2.Bot;
using MyTgBot2.Storage.Database;
using MyTgBot2.Plugins.Klipper;
using MyTgBot2.Plugins.PCControl;

namespace MyTgBot2
{
    class Program
    {
        // TODO для джуна:
        // 1. Загрузить конфигурацию через ConfigurationLoader
        // 2. Создать экземпляры всех сервисов
        // 3. Инициализировать Storage
        // 4. Загрузить плагины
        // 5. Создать AI провайдер
        // 6. Создать и запустить TelegramBotHandler
        // 7. Ожидать завершения (CancellationToken с Ctrl+C)
        // 8. Корректно завершить работу
        
        private static CancellationTokenSource _cancellationTokenSource;
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== MyTgBot2 - Telegram Bot для управления 3D принтером и компьютером ===");
            Console.WriteLine($"Запуск: {DateTime.Now}");
            Console.WriteLine();
            
            try
            {
                // Настройка CancellationToken для Ctrl+C
                _cancellationTokenSource = new CancellationTokenSource();
                Console.CancelKeyPress += (sender, e) =>
                {
                    Console.WriteLine("\nПолучен сигнал завершения. Останавливаю бота...");
                    e.Cancel = true;
                    _cancellationTokenSource.Cancel();
                };
                
                // TODO: Реализация инициализации
                // 1. Загрузить конфигурацию
                Console.WriteLine("[1/6] Загрузка конфигурации...");
                var configLoader = new ConfigurationLoader();
                configLoader.LoadConfiguration("appsettings.json", "secrets.txt");
                var appSettings = configLoader.AppSettings;
                var secrets = configLoader.Secrets;
                Console.WriteLine($"  ✓ Конфигурация загружена. Режим: {appSettings.Application.Environment}");
                
                // 2. Инициализировать Storage
                Console.WriteLine("[2/6] Инициализация базы данных...");
                IStorage storage = new SQLiteStorage(appSettings.Database.ConnectionString);
                await storage.InitializeAsync(_cancellationTokenSource.Token);
                Console.WriteLine("  ✓ База данных готова");
                
                // 3. Загрузить плагины
                Console.WriteLine("[3/6] Загрузка плагинов...");
                var pluginManager = new PluginManager(appSettings.Plugins.PluginsDirectory);
                
                // Регистрация встроенных плагинов
                // TODO: Позже добавить динамическую загрузку из DLL
                var klipperPlugin = new KlipperPlugin(
                    appSettings.Klipper.MoonrakerUrl,
                    appSettings.Klipper.WebcamUrl
                );
                await klipperPlugin.InitializeAsync(_cancellationTokenSource.Token);
                
                var pcPlugin = new PCControlPlugin(
                    secrets.PcSshHost,
                    secrets.PcSshPort,
                    secrets.PcSshUsername,
                    secrets.PcSshPassword,
                    secrets.PcSshKeyPath,
                    secrets.PcMacAddress,
                    secrets.PcBroadcastIp
                );
                await pcPlugin.InitializeAsync(_cancellationTokenSource.Token);
                
                // TODO: Добавить плагины в PluginManager
                Console.WriteLine($"  ✓ Загружено плагинов: 2 (Klipper, PCControl)");
                
                // 4. Создать AI провайдер
                Console.WriteLine("[4/6] Инициализация AI провайдера...");
                var aiFactory = new AIProviderFactory(appSettings, secrets);
                IAIProvider aiProvider = await aiFactory.CreateProviderAsync();
                Console.WriteLine($"  ✓ AI провайдер: {aiProvider.ProviderName}");
                
                // 5. Создать Telegram бот
                Console.WriteLine("[5/6] Создание Telegram бота...");
                var bot = new TelegramBotHandler(
                    secrets.TelegramBotToken,
                    aiProvider,
                    pluginManager,
                    storage,
                    secrets.AuthorizedUserIds,
                    appSettings.Bot.MessageChunkSize
                );
                Console.WriteLine("  ✓ Telegram бот создан");
                
                // 6. Запустить бота
                Console.WriteLine("[6/6] Запуск бота...");
                await bot.StartAsync(_cancellationTokenSource.Token);
                Console.WriteLine("  ✓ Бот запущен и готов к работе!");
                Console.WriteLine();
                Console.WriteLine("Бот работает. Нажмите Ctrl+C для остановки.");
                
                // Ожидание завершения
                await Task.Delay(Timeout.Infinite, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\nБот остановлен пользователем.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ КРИТИЧЕСКАЯ ОШИБКА: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Environment.ExitCode = 1;
            }
            finally
            {
                // TODO: Корректное завершение
                // - Закрыть соединения с БД
                // - Выгрузить плагины
                // - Dispose AI провайдера
                // - Dispose бота
                Console.WriteLine("\nЗавершение работы...");
                Console.WriteLine($"Остановлен: {DateTime.Now}");
            }
        }
    }
}
