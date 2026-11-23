# Инструкция для джуна по реализации

## 🎯 Цель
Вам нужно реализовать все методы, помеченные `throw new NotImplementedException()`.
Это позволит боту начать работать.

## 📝 План работы

### Этап 1: Базовая конфигурация (1-2 часа)
**Приоритет: КРИТИЧНЫЙ**

**Файлы**: 
- `Core/Services/ConfigurationLoader.cs`

**Что делать**:
1. Реализовать `LoadConfiguration()` - читать файлы и парсить
2. Реализовать `ParseSecretsFile()` - парсинг `KEY=VALUE` формата
3. Добавить валидацию обязательных полей

**Как проверить**:
```csharp
var loader = new ConfigurationLoader();
loader.LoadConfiguration("appsettings.json", "secrets.txt");
Console.WriteLine(loader.Secrets.TelegramBotToken); // Должен вывести токен
```

---

### Этап 2: Storage / База данных (2-3 часа)
**Приоритет: КРИТИЧНЫЙ**

**Файлы**: 
- `Storage/Database/SQLiteStorage.cs`

**Что делать**:
1. `InitializeAsync()` - создать таблицы БД
2. `SaveMessageAsync()` - INSERT сообщение в историю
3. `GetConversationHistoryAsync()` - SELECT последние N сообщений
4. `SaveActionLogAsync()` - INSERT лог действия
5. `SaveUserSettingAsync()` - INSERT OR REPLACE настройку
6. `GetUserSettingAsync()` - SELECT настройку

**Используйте**:
- `Microsoft.Data.Sqlite` (NuGet)
- Параметризованные запросы для безопасности
- Правильный порядок колонок

**SQL шаблон**:
```sql
CREATE TABLE IF NOT EXISTS ConversationHistory (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    Role TEXT NOT NULL,
    Content TEXT NOT NULL,
    Timestamp TEXT NOT NULL
);
CREATE INDEX IF NOT EXISTS idx_conversation_userid ON ConversationHistory(UserId, Timestamp);
```

**Как проверить**:
```csharp
var storage = new SQLiteStorage("Data Source=test.db");
await storage.InitializeAsync();
await storage.SaveMessageAsync(123, new Message { Role = "user", Content = "test" });
var history = await storage.GetConversationHistoryAsync(123);
Console.WriteLine(history.Count); // Должно быть >= 1
```

---

### Этап 3: AI Provider - Gemini (3-4 часа)
**Приоритет: ВЫСОКИЙ**

**Файлы**: 
- `AI/Providers/GeminiProvider.cs`

**Что делать**:
1. Реализовать `ProcessMessageAsync()`
   - Собрать историю сообщений
   - Собрать список функций из плагинов
   - Отправить запрос к Gemini API
   - Распарсить ответ (текст + function calls)
   - Вернуть `AIResponse`

2. Реализовать `BuildGeminiRequest()`
   - Правильный JSON для Gemini API
   - Добавить system instruction
   - Добавить историю
   - Добавить список доступных функций

3. Реализовать `ParseGeminiResponse()`
   - Извлечь текст из ответа
   - Извлечь function calls
   - Преобразовать в `ActionRequest[]`

**API Gemini**:
```
POST https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={API_KEY}

Body:
{
  "contents": [
    {"role": "user", "parts": [{"text": "..."}]},
    {"role": "model", "parts": [{"text": "..."}]}
  ],
  "tools": [
    {
      "function_declarations": [
        {
          "name": "start_print",
          "description": "Запустить печать",
          "parameters": {
            "type": "object",
            "properties": {
              "filename": {"type": "string", "description": "Имя файла"}
            },
            "required": ["filename"]
          }
        }
      ]
    }
  ]
}
```

**Ответ от Gemini**:
```json
{
  "candidates": [
    {
      "content": {
        "parts": [
          {"text": "Запускаю печать..."},
          {
            "functionCall": {
              "name": "start_print",
              "args": {"filename": "cube.gcode"}
            }
          }
        ]
      }
    }
  ]
}
```

**Как проверить**:
```csharp
var provider = new GeminiProvider("your-api-key", "gemini-2.0-flash", pluginManager);
var response = await provider.ProcessMessageAsync(123, "Запусти печать cube.gcode", history);
Console.WriteLine(response.Text); // Текст ответа
Console.WriteLine(response.Actions.Count); // Кол-во действий
```

---

### Этап 4: Telegram Bot Handler (2-3 часа)
**Приоритет: ВЫСОКИЙ**

**Файлы**: 
- `Bot/TelegramBotHandler.cs`

**Что делать**:
1. `StartAsync()` - запустить long polling
2. `HandleUpdateAsync()` - обработать входящее сообщение
   - Извлечь userId и текст
   - Проверить авторизацию
   - Загрузить историю
   - Вызвать AI провайдер
   - Выполнить actions
   - Отправить ответ
   - Сохранить в историю
3. `SendMessageAsync()` - отправка с разбивкой на части
4. `IsUserAuthorized()` - проверка whitelist
5. `ExecuteActionsAsync()` - выполнить через PluginManager

**Используйте**:
- `Telegram.Bot` (NuGet)
- `ITelegramBotClient.ReceiveAsync()` для long polling
- `ITelegramBotClient.SendTextMessageAsync()` для отправки

**Как проверить**:
- Запустить бота
- Отправить сообщение в Telegram
- Проверить что бот ответил

---

### Этап 5: Plugin Manager (1-2 часа)
**Приоритет: СРЕДНИЙ**

**Файлы**: 
- `Core/Services/PluginManager.cs`

**Что делать**:
1. `RegisterPlugin()` - добавить плагин в список
2. `GetAllFunctions()` - собрать функции со всех плагинов
3. `ExecuteActionAsync()` - найти плагин и вызвать метод
4. `LoadDynamicPlugins()` - загрузить DLL плагины (опционально)

**Как проверить**:
```csharp
var manager = new PluginManager("/plugins");
manager.RegisterPlugin(new KlipperPlugin(...));
var functions = manager.GetAllFunctions();
Console.WriteLine(functions.Count); // Должны быть функции
```

---

### Этап 6: Klipper Plugin (2-3 часа)
**Приоритет: СРЕДНИЙ**

**Файлы**: 
- `Plugins/Klipper/KlipperPlugin.cs`

**Что делать**:
1. `GetAvailableFunctions()` - вернуть список команд
2. `ExecuteActionAsync()` - switch по командам
3. Реализовать каждую команду:
   - `StartPrintAsync()` - POST к Moonraker API
   - `GetPrinterStatusAsync()` - GET статуса
   - `TakePhotoAsync()` - GET фото с камеры

**Moonraker API примеры**:
```bash
# Получить статус
GET http://localhost:7125/printer/objects/query?print_stats&toolhead

# Запустить печать
POST http://localhost:7125/printer/print/start?filename=cube.gcode

# Пауза
POST http://localhost:7125/printer/print/pause

# Статус сервера
GET http://localhost:7125/server/info
```

**Как проверить**:
```csharp
var plugin = new KlipperPlugin("http://localhost:7125", "http://localhost:8080/?action=snapshot");
await plugin.InitializeAsync();
var isAvailable = await plugin.IsAvailableAsync(); // true если Klipper работает
var request = new ActionRequest { FunctionName = "get_printer_status", Parameters = new() };
var result = await plugin.ExecuteActionAsync(request);
Console.WriteLine(result.Message); // Статус принтера
```

---

### Этап 7: PC Control Plugin (2-3 часа)
**Приоритет: НИЗКИЙ** (можно пропустить на начальном этапе)

**Файлы**: 
- `Plugins/PCControl/PCControlPlugin.cs`

**Что делать**:
1. `WakePCAsync()` - отправить magic packet
2. `ExecuteCommandAsync()` - выполнить через SSH
3. `CheckPCStatusAsync()` - ping

**SSH библиотека**: `SSH.NET`
```csharp
using Renci.SshNet;

var client = new SshClient("192.168.1.100", "admin", "password");
client.Connect();
var result = client.RunCommand("tasklist");
Console.WriteLine(result.Result);
client.Disconnect();
```

**Wake-on-LAN**:
```csharp
byte[] magicPacket = new byte[102];
// 6 байт 0xFF
for (int i = 0; i < 6; i++) magicPacket[i] = 0xFF;
// 16 повторений MAC адреса
byte[] mac = { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF };
for (int i = 1; i <= 16; i++)
{
    Array.Copy(mac, 0, magicPacket, i * 6, 6);
}
// Отправить UDP broadcast на порт 9
var udp = new UdpClient();
udp.Send(magicPacket, magicPacket.Length, "192.168.1.255", 9);
```

---

### Этап 8: NoAI Provider (опционально, 1 час)
**Приоритет: НИЗКИЙ**

**Файлы**: 
- `AI/Providers/NoAIProvider.cs`

**Что делать**:
1. Парсить сообщение как команду: `/start_print cube.gcode`
2. Создать `ActionRequest` напрямую

---

## 🔥 С чего начать СЕЙЧАС

Начните с этого порядка:

### День 1
1. ✅ ConfigurationLoader - загрузка конфигурации
2. ✅ SQLiteStorage - инициализация БД и основные методы
3. ✅ Проверить что БД работает (создать тестовый файл)

### День 2
4. ✅ PluginManager - регистрация и управление
5. ✅ KlipperPlugin - функции и выполнение (хотя бы `get_printer_status`)
6. ✅ Проверить что плагин работает (curl к Moonraker)

### День 3
7. ✅ GeminiProvider - интеграция с API
8. ✅ Проверить что Gemini отвечает и возвращает function calls

### День 4
9. ✅ TelegramBotHandler - обработка сообщений
10. ✅ Собрать все вместе в Program.cs
11. ✅ ЗАПУСТИТЬ И ПРОТЕСТИРОВАТЬ!

---

## 📚 Полезные ссылки

- [Gemini API Docs](https://ai.google.dev/docs)
- [Telegram Bot API](https://core.telegram.org/bots/api)
- [Moonraker API](https://moonraker.readthedocs.io/en/latest/web_api/)
- [SSH.NET Docs](https://github.com/sshnet/SSH.NET)
- [SQLite C# Tutorial](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/)

---

## ⚠️ Важные замечания

1. **Безопасность**:
   - ВСЕГДА использовать параметризованные SQL запросы
   - НЕ коммитить `secrets.txt`
   - Проверять авторизацию пользователей

2. **Обработка ошибок**:
   - Оборачивать HTTP запросы в try-catch
   - Логировать все ошибки
   - Возвращать понятные сообщения пользователю

3. **Тестирование**:
   - Тестировать каждый компонент отдельно
   - Использовать postman/curl для проверки API
   - Проверять логи в консоли

4. **Оптимизация**:
   - Переиспользовать HttpClient (не создавать каждый раз новый)
   - Кешировать конфигурацию
   - Использовать async/await правильно

---

## 🎓 Советы

- Начинайте с простого - реализуйте минимум для работы
- Используйте Console.WriteLine() для отладки
- Не пытайтесь сделать всё идеально сразу
- Коммитьте код часто (small commits)
- Если застряли - пишите в чат или ищите примеры в интернете

**Удачи!** 💪
