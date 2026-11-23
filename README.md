# MyTgBot2 - Telegram Bot для управления 3D принтером и компьютером

Telegram бот на C# для управления 3D принтером (Klipper) и компьютером через AI ассистента (Gemini).

---

## 📋 Оглавление

1. [Возможности](#возможности)
2. [Архитектура](#архитектура)
3. [Установка](#установка)
4. [Конфигурация](#конфигурация)
5. [Запуск](#запуск)
6. [Разработка плагинов](#разработка-плагинов)
7. [Структура проекта](#структура-проекта)
8. [API документация](#api-документация)

---

## 🚀 Возможности

### Управление 3D принтером (Klipper)
- Запуск/остановка/пауза печати
- Получение статуса принтера (температуры, прогресс)
- Фотографирование процесса печати
- Список доступных файлов для печати

### Управление компьютером
- Включение компьютера (Wake-on-LAN)
- Выключение/перезагрузка
- Выполнение команд в CMD/PowerShell
- Запуск программ (Steam, игры)
- Получение списка процессов

### AI ассистент
- Естественное общение на русском/английском
- Контекстная память диалогов
- Автоматическое определение команд
- Поддержка Gemini API и кастомных AI
- Режим работы без AI (прямые команды)

### Расширяемость
- Динамическая загрузка плагинов (DLL)
- Простое добавление новых команд
- Готов для умного дома, TTS/STT и других интеграций

---

## 🏗 Архитектура

```
┌─────────────┐
│  Telegram   │  Пользователь отправляет сообщение
│    User     │
└──────┬──────┘
       │
       ▼
┌─────────────────────────────────────────┐
│      TelegramBotHandler                 │  Получает сообщение
│  - Проверка авторизации (whitelist)     │
│  - Загрузка истории диалога             │
└──────┬──────────────────────────────────┘
       │
       ▼
┌─────────────────────────────────────────┐
│         AI Provider                     │  Обрабатывает запрос
│  - Gemini API (function calling)        │
│  - NoAI Provider (прямые команды)       │
│  - Custom AI Provider                   │
│                                         │
│  Возвращает:                            │
│  ├─ Текст для пользователя              │
│  └─ Список действий (ActionRequest[])   │
└──────┬──────────────────────────────────┘
       │
       ▼
┌─────────────────────────────────────────┐
│       Plugin Manager                    │  Выполняет команды
│  - Маршрутизация к нужному плагину      │
│  - Выполнение действий                  │
└──────┬──────────────────────────────────┘
       │
       ├──────────────┬──────────────┬─────────────┐
       ▼              ▼              ▼             ▼
┌──────────┐  ┌──────────┐  ┌───────────┐  ┌────────────┐
│ Klipper  │  │    PC    │  │   Smart   │  │   Custom   │
│  Plugin  │  │ Control  │  │   Home    │  │   Plugin   │
│          │  │  Plugin  │  │  (future) │  │  (future)  │
└──────────┘  └──────────┘  └───────────┘  └────────────┘
       │
       ▼
┌─────────────────────────────────────────┐
│          Storage (SQLite)               │  Сохраняет данные
│  - История диалогов (контекст для AI)   │
│  - Логи всех действий                   │
│  - Настройки пользователей              │
└─────────────────────────────────────────┘
```

### Поток обработки сообщения

1. **Пользователь**: "Запусти печать model.gcode"
2. **TelegramBotHandler**: Проверяет авторизацию, загружает историю
3. **AI Provider**: Анализирует запрос → `{ text: "Запускаю печать...", actions: [start_print] }`
4. **Plugin Manager**: Находит KlipperPlugin → выполняет `start_print`
5. **KlipperPlugin**: Отправляет команду к Moonraker API
6. **TelegramBotHandler**: Отправляет ответ пользователю
7. **Storage**: Сохраняет диалог и лог действия

---

## 💿 Установка

### Требования

- **ОС**: Debian/Ubuntu (или Windows/macOS для разработки)
- **.NET SDK**: версия 6.0 или выше
- **3D принтер**: Klipper с Moonraker API
- **Компьютер для управления**: Windows с SSH сервером (OpenSSH) и Wake-on-LAN

### Шаги установки

1. **Клонировать/скопировать проект**
```bash
cd /root/termux/files/home/mytgbot2
```

2. **Установить .NET SDK 6.0** (если не установлен)
```bash
# Debian/Ubuntu
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 6.0

# Добавить в PATH
export PATH="$PATH:$HOME/.dotnet"
```

3. **Восстановить зависимости**
```bash
dotnet restore
```

4. **Настроить конфигурацию** (см. раздел Конфигурация)

5. **Скомпилировать проект**
```bash
dotnet build --configuration Release
```

---

## ⚙️ Конфигурация

### 1. secrets.txt (обязательно!)

Скопируйте `secrets.txt` и заполните:

```bash
nano secrets.txt
```

```
# Telegram Bot Token от @BotFather
TELEGRAM_BOT_TOKEN=123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11

# Gemini API Key (получить на https://aistudio.google.com/app/apikey)
GEMINI_API_KEY=AIzaSyABC123defGHI456jklMNO789pqrSTU

# Whitelist пользователей (Telegram ID через запятую)
# Узнать свой ID: @userinfobot в Telegram
AUTHORIZED_USER_IDS=123456789,987654321

# SSH для управления компьютером
PC_SSH_HOST=192.168.1.100
PC_SSH_PORT=22
PC_SSH_USERNAME=admin
PC_SSH_PASSWORD=your_password
PC_SSH_KEY_PATH=

# Wake-on-LAN (MAC адрес компьютера)
PC_MAC_ADDRESS=AA:BB:CC:DD:EE:FF
PC_BROADCAST_IP=192.168.1.255
```

**ВАЖНО**: Добавьте `secrets.txt` в `.gitignore` (уже добавлен)!

### 2. appsettings.json (опционально)

Настройки уже заданы по умолчанию. Можно изменить:

- `Klipper.MoonrakerUrl` - URL Moonraker API
- `Klipper.WebcamUrl` - URL веб-камеры
- `AI.Gemini.Model` - модель Gemini (gemini-3.0-flash)
- `Logging` - настройки ��огирования

---

## 🚀 Запуск

### Вариант 1: Запуск вручную (для тестирования)

```bash
cd /root/termux/files/home/mytgbot2
dotnet run
```

Вывод:
```
=== MyTgBot2 - Telegram Bot ===
[1/6] Загрузка конфигурации...
  ✓ Конфигурация загружена
[2/6] Инициализация базы данных...
  ✓ База данных готова
[3/6] Загрузка плагинов...
  ✓ Загружено плагинов: 2
[4/6] Инициализация AI провайдера...
  ✓ AI провайдер: Gemini
[5/6] Создание Telegram бота...
  ✓ Telegram бот создан
[6/6] Запуск бота...
  ✓ Бот запущен!

Бот работает. Нажмите Ctrl+C для остановки.
```

### Вариант 2: Запуск как сервис (для постоянной работы)

#### Для systemd (Debian/Ubuntu)

1. Создать unit файл:
```bash
sudo nano /etc/systemd/system/mytgbot2.service
```

2. Вставить:
```ini
[Unit]
Description=MyTgBot2 Telegram Bot
After=network.target

[Service]
Type=simple
User=root
WorkingDirectory=/root/termux/files/home/mytgbot2
ExecStart=/root/.dotnet/dotnet run --project /root/termux/files/home/mytgbot2/MyTgBot2.csproj
Restart=always
RestartSec=10

[Install]
WantedBy=multi-user.target
```

3. Включить и запустить:
```bash
sudo systemctl daemon-reload
sudo systemctl enable mytgbot2
sudo systemctl start mytgbot2
```

4. Проверить статус:
```bash
sudo systemctl status mytgbot2
```

5. Просмотр логов:
```bash
sudo journalctl -u mytgbot2 -f
```

---

## 🔌 Разработка плагинов

### Создание нового плагина

Плагины расширяют функциональность бота. Создать новый плагин легко:

#### Шаг 1: Создать класс плагина

```csharp
// Plugins/SmartHome/SmartHomePlugin.cs

using MyTgBot2.Core.Models;
using MyTgBot2.Plugins.Base;

namespace MyTgBot2.Plugins.SmartHome
{
    public class SmartHomePlugin : BasePlugin
    {
        public override string PluginName => "SmartHomePlugin";
        public override string Version => "1.0.0";
        public override string Description => "Управление умным домом";
        
        public override List<FunctionDefinition> GetAvailableFunctions()
        {
            return new List<FunctionDefinition>
            {
                new FunctionDefinition
                {
                    PluginName = "SmartHomePlugin",
                    Name = "turn_on_light",
                    Description = "Включить свет в комнате",
                    Parameters = new List<FunctionParameter>
                    {
                        new FunctionParameter
                        {
                            Name = "room",
                            Type = "string",
                            Description = "Название комнаты (bedroom, kitchen, living_room)",
                            Required = true
                        }
                    }
                }
            };
        }
        
        public override async Task<ActionResult> ExecuteActionAsync(
            ActionRequest request, 
            CancellationToken cancellationToken = default)
        {
            switch (request.FunctionName)
            {
                case "turn_on_light":
                    var room = GetParameter<string>(request, "room");
                    // TODO: Вызов API умного дома
                    return Success($"Свет в комнате {room} включен");
                    
                default:
                    return Error("Неизвестная команда");
            }
        }
    }
}
```

#### Шаг 2: Зарегистрировать в Program.cs

```csharp
// Program.cs

var smartHomePlugin = new SmartHomePlugin();
await smartHomePlugin.InitializeAsync(_cancellationTokenSource.Token);
// Добавить в PluginManager
```

#### Шаг 3: Перекомпилировать и запустить

```bash
dotnet build
dotnet run
```

Теперь AI может использовать команды плагина!

### Динамическая загрузка плагинов (DLL)

Плагины можно компилировать отдельно и загружать динамически:

1. Создать отдельный проект плагина
2. Скомпилировать в DLL
3. Положить DLL в папку `Plugins/`
4. PluginManager автоматически загрузит его при старте

---

## 📁 Структура проекта

```
mytgbot2/
│
├── Core/                          # Ядро системы
│   ├── Interfaces/                # Интерфейсы (IPlugin, IAIProvider, IStorage)
│   │   ├── IPlugin.cs
│   │   ├── IAIProvider.cs
│   │   └── IStorage.cs
│   ├── Models/                    # Модели данных
│   │   ├── CoreModels.cs          # Message, AIResponse, ActionRequest, etc.
│   │   └── ConfigurationModels.cs # AppSettings, Secrets
│   └── Services/                  # Сервисы
│       ├── PluginManager.cs       # Управление плагинами
│       └── ConfigurationLoader.cs # Загрузка конфигурации
│
├── AI/                            # AI провайдеры
│   ├── Providers/
│   │   ├── GeminiProvider.cs      # Интеграция с Gemini API
│   │   └── NoAIProvider.cs        # Fallback без AI
│   └── Adapters/
│       └── AIProviderFactory.cs   # Фабрика провайдеров
│
├── Bot/                           # Telegram бот
│   └── TelegramBotHandler.cs      # Обработка сообщений
│
├── Plugins/                       # Плагины
│   ├── Base/
│   │   └── BasePlugin.cs          # Базовый класс для плагинов
│   ├── Klipper/
│   │   └── KlipperPlugin.cs       # Управление 3D принтером
│   └── PCControl/
│       └── PCControlPlugin.cs     # Управление компьютером
│
├── Storage/                       # Хранилище данных
│   └── Database/
│       └── SQLiteStorage.cs       # SQLite реализация
│
├── Data/                          # База данных (генерируется автоматически)
│   └── botdata.db
│
├── Logs/                          # Логи (генерируется автоматически)
│   └── log-*.txt
│
├── Program.cs                     # Точка входа
├── MyTgBot2.csproj               # Файл проекта
├── appsettings.json              # Конфигурация
├── secrets.txt                    # Секреты (НЕ КОММИТИТЬ!)
├── .gitignore
└── README.md                      # Эта инструкция
```

---

## 📚 API документация

### Интерфейс IPlugin

Все плагины должны реализовывать:

```csharp
public interface IPlugin
{
    string PluginName { get; }
    string Version { get; }
    string Description { get; }
    
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
    List<FunctionDefinition> GetAvailableFunctions();
    Task<ActionResult> ExecuteActionAsync(ActionRequest request, CancellationToken cancellationToken);
    Task InitializeAsync(CancellationToken cancellationToken);
    Task DisposeAsync();
}
```

### Класс ActionRequest

```csharp
public class ActionRequest
{
    public string PluginName { get; set; }      // Имя плагина
    public string FunctionName { get; set; }    // Имя команды
    public Dictionary<string, object> Parameters { get; set; } // Параметры
    public string RequestId { get; set; }       // ID для отслеживания
}
```

### Класс ActionResult

```csharp
public class ActionResult
{
    public bool Success { get; set; }           // Успех/ошибка
    public string Message { get; set; }         // Сообщение для пользователя
    public object Data { get; set; }            // Данные результата
    public string ErrorMessage { get; set; }    // Текст ошибки
    public string ErrorCode { get; set; }       // Код ошибки
}
```

---

## 🎯 Примеры использования

### Пример 1: Управление принтером

**Пользователь**: "Как там мой принтер?"

**AI → Plugin**: `get_printer_status()`

**Ответ**: "Принтер печатает model.gcode, прогресс 45%, сопло 215°C, стол 60°C"

### Пример 2: Запуск печати

**Пользователь**: "Запусти печать cube.gcode"

**AI → Plugin**: `start_print(filename="cube.gcode")`

**Ответ**: "Печать запущена! Файл: cube.gcode"

### Пример 3: Управление компьютером

**Пользователь**: "Включи мой комп и запусти Steam"

**AI → Plugins**: 
1. `wake_pc()`
2. `start_steam()`

**Ответ**: "Компьютер включается... Steam запущен!"

### Пример 4: Режим без AI

**Пользователь**: `/start_print cube.gcode`

**NoAI → Plugin**: `start_print(filename="cube.gcode")`

**Ответ**: "Печать запущена!"

---

## 🛠 Разработка

### Запуск в debug режиме

```bash
dotnet run --configuration Debug
```

### Запуск тестов (TODO)

```bash
dotnet test
```

### Сборка release версии

```bash
dotnet publish -c Release -o ./publish
```

---

## 🐛 Troubleshooting

### Проблема: Бот не отвечает

**Решение**:
1. Проверить `TELEGRAM_BOT_TOKEN` в `secrets.txt`
2. Проверить что ваш Telegram ID в `AUTHORIZED_USER_IDS`
3. Проверить логи: `tail -f Logs/log-*.txt`

### Проблема: Gemini API ошибка

**Решение**:
1. Проверить `GEMINI_API_KEY`
2. Проверить квоту API на https://aistudio.google.com
3. Бот автоматически переключится на NoAI режим

### Проблема: Принтер недоступен

**Решение**:
1. Проверить `Klipper.MoonrakerUrl` в `appsettings.json`
2. Проверить доступность: `curl http://localhost:7125/server/info`
3. Проверить что Klipper запущен

### Проблема: Wake-on-LAN не работает

**Решение**:
1. Включить WOL в BIOS компьютера
2. Проверить `PC_MAC_ADDRESS` в `secrets.txt`
3. Проверить что компьютер в той же локальной сети

---

## 📝 TODO / Планы развития

- [ ] Реализация всех методов (см. `throw new NotImplementedException()`)
- [ ] Динамическая загрузка плагинов из DLL
- [ ] Web интерфейс для управления
- [ ] Поддержка нескольких принтеров
- [ ] Интеграция с умным домом (Zigbee, Home Assistant)
- [ ] TTS/STT для голосового управления
- [ ] Telegram inline кнопки для быстрых команд
- [ ] Графики температур и статистика печати
- [ ] Уведомления о завершении печати
- [ ] Multi-language support

---

## 📄 Лицензия

MIT License - используйте свободно

---

## 👤 Автор

Создано для управления 3D принтером и компьютером через Telegram бота с AI.

**Контакты**: укажите ваши контакты если хотите

---

## 🙏 Благодарности

- Telegram Bot API
- Google Gemini API
- Klipper/Moonraker
- SSH.NET library

---

**Удачи в разработке!** 🚀
