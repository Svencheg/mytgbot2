/*
 * TICKET: Configuration Loader Service
 * 
 * ОПИСАНИЕ:
 * Сервис для загрузки конфигурации из appsettings.json и secrets.txt
 * Парсит конфигурационные файлы и предоставляет доступ к настройкам
 * 
 * НАЗНАЧЕНИЕ:
 * - Загрузка appsettings.json в объект AppSettings
 * - Парсинг secrets.txt (формат KEY=VALUE)
 * - Валидация обязательных параметров
 * - Предоставление единой точки доступа к конфигурации
 * 
 * ЧТО ПРИНИМАЕТ:
 * - string configPath: путь к appsettings.json
 * - string secretsPath: путь к secrets.txt
 * 
 * ЧТО ВОЗВРАЩАЕТ:
 * - AppSettings: объект с настройками приложения
 * - Secrets: объект с секретными данными
 * 
 * ВАЖНО:
 * - secrets.txt не должен коммититься в git (добавить в .gitignore)
 * - Валидировать наличие обязательных ключей (Telegram token, Gemini key)
 */

using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using MyTgBot2.Core.Models;

namespace MyTgBot2.Core.Services
{
    public class ConfigurationLoader
    {
        // TODO для джуна:
        // 1. Загрузить appsettings.json через File.ReadAllText()
        // 2. Десериализовать JSON в AppSettings через JsonSerializer.Deserialize<AppSettings>()
        // 3. Загрузить secrets.txt построчно
        // 4. Парсить каждую строку как KEY=VALUE (игнорируя комментарии #)
        // 5. Создать объект Secrets и заполнить его
        // 6. Валидировать обязательные параметры
        
        public AppSettings AppSettings { get; private set; }
        public Secrets Secrets { get; private set; }
        
        /// <summary>
        /// Загрузить конфигурацию из файлов
        /// </summary>
        public void LoadConfiguration(string appSettingsPath = "appsettings.json", string secretsPath = "secrets.txt")
        {
            // TODO: Реализация
            // 1. Проверить существование файлов
            // 2. Загрузить и распарсить appsettings.json
            // 3. Загрузить и распарсить secrets.txt
            // 4. Валидировать обязательные параметры
            // 5. Если ошибка - выбросить исключение с понятным сообщением
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Парсинг secrets.txt (формат KEY=VALUE)
        /// </summary>
        private Secrets ParseSecretsFile(string filePath)
        {
            // TODO: Реализация
            // 1. Прочитать все строки из файла
            // 2. Игнорировать пустые строки и строки начинающиеся с #
            // 3. Разделить каждую строку по '=' на key и value
            // 4. Заполнить объект Secrets
            // 5. Для AUTHORIZED_USER_IDS разделить по запятой и конвертировать в long
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Валидация обязательных параметров
        /// </summary>
        private void ValidateConfiguration()
        {
            // TODO: Реализация
            // Проверить что заполнены:
            // - TELEGRAM_BOT_TOKEN
            // - GEMINI_API_KEY
            // - AUTHORIZED_USER_IDS (хотя бы один ID)
            // Если что-то не заполнено - выбросить исключение
            throw new NotImplementedException();
        }
    }
}
