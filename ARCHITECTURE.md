# Архитектура системы MyTgBot2

## Общая схема

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                             TELEGRAM USER                                    │
│                        (отправляет сообщения боту)                           │
└──────────────────────────────────┬──────────────────────────────────────────┘
                                   │
                                   │ Telegram Bot API
                                   │
┌──────────────────────────────────▼──────────────────────────────────────────┐
│                         TELEGRAM BOT HANDLER                                 │
│                      (Bot/TelegramBotHandler.cs)                             │
│                                                                               │
│  • Получение сообщений (long polling)                                        │
│  • Проверка авторизации (whitelist)                                          │
│  • Отправка ответов пользователю                                             │
└──────────────┬──────────────────────────────────────┬────────────────────────┘
               │                                      │
               │ 1. Получено сообщение                │ 5. Отправить ответ
               │                                      │
┌──────────────▼──────────────────────────────────────▼────────────────────────┐
│                            CORE LAYER                                         │
│                                                                               │
│  ┌────────────────────────┐  ┌──────────────────────┐                        │
│  │ ConfigurationLoader    │  │   Plugin Manager      │                        │
│  │                        │  │                       │                        │
│  │ • appsettings.json     │  │ • Регистрация плагинов│                        │
│  │ • secrets.txt          │  │ • Вызов функций       │                        │
│  └────────────────────────┘  └──────────▲────────────┘                        │
│                                         │                                     │
│                                         │ 4. Выполнить actions                │
└─────────────────────────────────────────┼─────────────────────────────────────┘
                 │                        │
                 │ 2. Обработать AI       │
                 │                        │
┌────────────────▼────────────────────────┼─────────────────────────────────────┐
│                        AI PROVIDER LAYER                                      │
│                                         │                                     │
│  ┌────────────────────┐   ┌────────────┴────────────┐                        │
│  │  Gemini Provider   │   │   NoAI Provider         │                        │
│  │                    │   │                         │                        │
│  │ • Google Gemini API│   │ • Прямые команды        │                        │
│  │ • Function calling │   │ • Fallback              │                        │
│  └────────────────────┘   └─────────────────────────┘                        │
│                                                                               │
│           3. Вернуть AIResponse (text + actions[])                           │
└───────────────────────────────────────────────────────────────────────────────┘
                                         │
                                         │
┌────────────────────────────────────────▼─────────────────────────────────────┐
│                           PLUGIN LAYER                                        │
│                                                                               │
│  ┌────────────────────────────────────┐  ┌──────────────────────────────────┐│
│  │      KLIPPER PLUGIN                │  │     PC CONTROL PLUGIN            ││
│  │                                    │  │                                  ││
│  │ • start_print(filename)            │  │ • wake_pc()                      ││
│  │ • pause_print()                    │  │ • shutdown_pc()                  ││
│  │ • resume_print()                   │  │ • execute_command(cmd)           ││
│  │ • cancel_print()                   │  │ • check_pc_status()              ││
│  │ • get_printer_status()             │  │ • start_steam()                  ││
│  │ • list_files()                     │  │ • get_process_list()             ││
│  │ • take_photo()                     │  │                                  ││
│  └──────────────┬─────────────────────┘  └──────────────┬───────────────────┘│
└─────────────────┼────────────────────────────────────────┼────────────────────┘
                  │                                        │
                  │ Moonraker API                          │ SSH / WOL
                  │                                        │
┌─────────────────▼────────────────────┐  ┌───────────────▼─────────────────────┐
│      KLIPPER / MOONRAKER             │  │         USER PC                     │
│                                      │  │                                     │
│  • http://localhost:7125             │  │  • SSH: 192.168.1.100:22            │
│  • API endpoints                     │  │  • WOL: AA:BB:CC:DD:EE:FF           │
│  • Webcam: http://localhost:8080     │  │                                     │
└──────────────────────────────────────┘  └─────────────────────────────────────┘


┌───────────────────────────────────────────────────────────────────────────────┐
│                           STORAGE LAYER                                       │
│                                                                               │
│  ┌────────────────────────────────────────────────────────────────────────┐  │
│  │                      SQLite Database                                   │  │
│  │                   (Data/botdata.db)                                    │  │
│  │                                                                        │  │
│  │  ┌───────────────────────┐  ┌──────────────┐  ┌──────────────────┐   │  │
│  │  │ ConversationHistory   │  │  ActionLog   │  │  UserSettings    │   │  │
│  │  │                       │  │              │  │                  │   │  │
│  │  │ • UserId              │  │ • UserId     │  │ • UserId         │   │  │
│  │  │ • Role                │  │ • PluginName │  │ • SettingKey     │   │  │
│  │  │ • Content             │  │ • FunctionName│ │ • SettingValue   │   │  │
│  │  │ • Timestamp           │  │ • Success    │  │ • UpdatedAt      │   │  │
│  │  └───────────────────────┘  └──────────────┘  └──────────────────┘   │  │
│  └────────────────────────────────────────────────────────────────────────┘  │
└───────────────────────────────────────────────────────────────────────────────┘
```

---

## Поток данных (детальный)

### Сценарий: Пользователь просит запустить печать

```
[1] ПОЛЬЗОВАТЕЛЬ
    │
    │  "Запусти печать cube.gcode"
    │
    ▼
[2] TELEGRAM → TelegramBotHandler.HandleUpdateAsync()
    │
    │  • Извлечь userId, messageText
    │  • Проверить авторизацию (IsUserAuthorized)
    │
    ▼
[3] STORAGE → SQLiteStorage.GetConversationHistoryAsync()
    │
    │  • Загрузить последние 50 сообщений
    │  • Вернуть List<Message>
    │
    ▼
[4] AI PROVIDER → GeminiProvider.ProcessMessageAsync()
    │
    │  • Собрать историю
    │  • Собрать доступные функции из плагинов
    │  • Отправить запрос к Gemini API
    │  • Получить ответ: { text: "...", functionCalls: [...] }
    │  • Парсить в AIResponse
    │
    ▼
[5] PLUGIN MANAGER → PluginManager.ExecuteActionAsync()
    │
    │  • Найти плагин: KlipperPlugin
    │  • Вызвать метод: start_print(filename: "cube.gcode")
    │
    ▼
[6] KLIPPER PLUGIN → KlipperPlugin.StartPrintAsync()
    │
    │  • POST http://localhost:7125/printer/print/start?filename=cube.gcode
    │  • Получить ответ от Moonraker
    │  • Вернуть ActionResult
    │
    ▼
[7] STORAGE → SQLiteStorage.SaveActionLogAsync()
    │
    │  • Сохранить лог: success=true, result="Print started"
    │
    ▼
[8] TELEGRAM → TelegramBotHandler.SendMessageAsync()
    │
    │  • Отправить ответ: "✓ Печать запущена!"
    │
    ▼
[9] STORAGE → SQLiteStorage.SaveMessageAsync() × 2
    │
    │  • Сохранить сообщение пользователя
    │  • Сохранить ответ бота
    │
    ▼
[10] ПОЛЬЗОВАТЕЛЬ получает ответ
```

---

## Классы и зависимости

### Core Layer
```
IPlugin (interface)
  ↑
  ├── BasePlugin (abstract)
  │     ↑
  │     ├── KlipperPlugin
  │     └── PCControlPlugin
  │
IAIProvider (interface)
  ↑
  ├── GeminiProvider
  └── NoAIProvider
  │
IStorage (interface)
  ↑
  └── SQLiteStorage
  │
PluginManager
  ├── depends on: List<IPlugin>
  │
ConfigurationLoader
  ├── returns: BotConfiguration, Secrets
```

### Bot Layer
```
TelegramBotHandler
  ├── depends on: ITelegramBotClient
  ├── depends on: IAIProvider
  ├── depends on: IStorage
  ├── depends on: PluginManager
  └── depends on: BotConfiguration
```

### Models
```
Message
  ├── Role: string
  ├── Content: string
  └── Timestamp: DateTime

AIResponse
  ├── Text: string
  ├── Actions: ActionRequest[]
  └── RequiresConfirmation: bool

ActionRequest
  ├── PluginName: string
  ├── FunctionName: string
  └── Parameters: Dictionary<string, object>

ActionResult
  ├── Success: bool
  ├── Message: string
  ├── Data: object?
  └── ErrorMessage: string?
```

---

## Компонентная архитектура

```
┌─────────────────────────────────────────────────────────────────┐
│                        PRESENTATION LAYER                       │
│                      (Telegram Bot Handler)                     │
│                                                                 │
│  Responsibilities:                                              │
│  • Получение и отправка сообщений                               │
│  • Авторизация пользователей                                    │
│  • Обработка команд (/start, /help)                             │
└────────────────────────┬────────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────────┐
│                       BUSINESS LOGIC LAYER                      │
│                     (Core: Managers, Loaders)                   │
│                                                                 │
│  Responsibilities:                                              │
│  • Маршрутизация запросов                                       │
│  • Управление плагинами                                         │
│  • Конфигурация приложения                                      │
└────────────────┬───────────────────┬────────────────────────────┘
                 │                   │
┌────────────────▼─────────┐  ┌──────▼─────────────────────────────┐
│      AI LAYER            │  │      PLUGIN LAYER                  │
│                          │  │                                    │
│  • Natural Language      │  │  • Domain-specific actions         │
│    Understanding         │  │  • Integration with external       │
│  • Function calling      │  │    systems                         │
│  • Context awareness     │  │                                    │
└──────────────────────────┘  └────────────────────────────────────┘
                 │                   │
                 └─────────┬─────────┘
                           │
┌──────────────────────────▼─────────────────────────────────────┐
│                       DATA ACCESS LAYER                        │
│                        (Storage / SQLite)                      │
│                                                                │
│  Responsibilities:                                             │
│  • Персистентность данных                                      │
│  • История диалогов                                            │
│  • Логи действий                                               │
│  • Настройки пользователей                                     │
└────────────────────────────────────────────────────────────────┘
```

---

## Паттерны проектирования

### 1. Strategy Pattern
```csharp
// Выбор AI провайдера в runtime
IAIProvider provider = config.AIProvider switch
{
    "Gemini" => new GeminiProvider(...),
    "NoAI" => new NoAIProvider(...),
    _ => new NoAIProvider(...)
};
```

### 2. Plugin Architecture
```csharp
// Динамическое добавление функциональности
pluginManager.RegisterPlugin(new KlipperPlugin(...));
pluginManager.RegisterPlugin(new PCControlPlugin(...));
pluginManager.RegisterPlugin(new WeatherPlugin(...)); // future
```

### 3. Factory Pattern
```csharp
// AIProviderFactory создаёт нужный провайдер
var provider = await AIProviderFactory.CreateProviderAsync(config, pluginManager);
```

### 4. Repository Pattern
```csharp
// Абстракция над хранилищем данных
IStorage storage = new SQLiteStorage(connectionString);
await storage.SaveMessageAsync(userId, message);
```

### 5. Dependency Injection
```csharp
// Program.cs
var botHandler = new TelegramBotHandler(
    client,
    aiProvider,
    storage,
    pluginManager,
    config,
    logger
);
```

---

## Масштабируемость

### Горизонтальное расширение (добавление функций):
```
1. Создать новый плагин:
   Plugins/Weather/WeatherPlugin.cs

2. Зарегистрировать в Program.cs:
   pluginManager.RegisterPlugin(new WeatherPlugin(...));

3. Готово! AI автоматически узнает о новых функциях.
```

### Вертикальное расширение (замена компонентов):
```
1. Заменить AI провайдер:
   GeminiProvider → OpenAIProvider
   
2. Заменить Storage:
   SQLiteStorage → PostgreSQLStorage
   
3. Заменить мессенджер:
   TelegramBotHandler → DiscordBotHandler
```

---

## Безопасность

```
┌─────────────────────────────────────────┐
│          Уровни защиты                  │
├─────────────────────────────────────────┤
│ 1. Telegram whitelist (UserId)          │
│    ↓                                    │
│ 2. Rate limiting (опционально)          │
│    ↓                                    │
│ 3. Validation параметров                │
│    ↓                                    │
│ 4. Confirmation для опасных действий    │
│    ↓                                    │
│ 5. Audit log (ActionLog table)          │
└─────────────────────────────────────────┘
```

---

## Производительность

### Асинхронность:
- Все IO операции: `async/await`
- HTTP запросы: `HttpClient` (reused)
- Database: параметризованные запросы

### Кэширование:
- Конфигурация: загружается 1 раз
- Плагины: регистрируются 1 раз
- История: загружается по запросу (limit 50)

### Оптимизация БД:
- Индексы на UserId + Timestamp
- Connection pooling (SQLite `Cache=Shared`)

---

## Мониторинг

```
┌──────────────────────────────────────┐
│          Logging (Serilog)           │
├──────────────────────────────────────┤
│ • Console sink (для разработки)      │
│ • File sink (Logs/log-*.txt)         │
│ • Structured logging                 │
└──────────────────────────────────────┘
              │
              ▼
┌──────────────────────────────────────┐
│       Database (ActionLog)           │
├──────────────────────────────────────┤
│ • Все выполненные действия           │
│ • Success/Fail статус                │
│ • Параметры и результаты             │
└──────────────────────────────────────┘
```

---

Эта архитектура обеспечивает:
✅ Гибкость
✅ Расширяемость
✅ Тестируемость
✅ Поддерживаемость
✅ Производительность
