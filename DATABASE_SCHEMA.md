# Схема базы данных SQLite

## Таблицы

### 1. ConversationHistory
Хранит историю диалогов для контекста AI.

```sql
CREATE TABLE IF NOT EXISTS ConversationHistory (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    Role TEXT NOT NULL,                -- 'user' или 'assistant'
    Content TEXT NOT NULL,             -- Текст сообщения
    Timestamp TEXT NOT NULL            -- ISO 8601 format
);

CREATE INDEX IF NOT EXISTS idx_conversation_userid 
    ON ConversationHistory(UserId, Timestamp DESC);
```

**Пример данных:**
```
Id | UserId    | Role      | Content                      | Timestamp
---|-----------|-----------|------------------------------|------------------------
1  | 123456789 | user      | Привет, как дела?            | 2024-11-23T10:30:00Z
2  | 123456789 | assistant | Привет! Всё отлично, чем м...| 2024-11-23T10:30:05Z
3  | 123456789 | user      | Запусти печать cube.gcode    | 2024-11-23T10:31:00Z
4  | 123456789 | assistant | Запускаю печать...           | 2024-11-23T10:31:02Z
```

---

### 2. ActionLog
Логи всех выполненных действий (для аудита и отладки).

```sql
CREATE TABLE IF NOT EXISTS ActionLog (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    PluginName TEXT NOT NULL,          -- Имя плагина (KlipperPlugin, PCControlPlugin)
    FunctionName TEXT NOT NULL,        -- Имя функции (start_print, wake_pc)
    Parameters TEXT,                   -- JSON с параметрами
    Success INTEGER NOT NULL,          -- 1 = успех, 0 = ошибка
    Result TEXT,                       -- Результат или сообщение об ошибке
    Timestamp TEXT NOT NULL            -- ISO 8601 format
);

CREATE INDEX IF NOT EXISTS idx_actionlog_userid 
    ON ActionLog(UserId, Timestamp DESC);

CREATE INDEX IF NOT EXISTS idx_actionlog_plugin 
    ON ActionLog(PluginName, Timestamp DESC);
```

**Пример данных:**
```
Id | UserId    | PluginName    | FunctionName | Parameters              | Success | Result              | Timestamp
---|-----------|---------------|--------------|-------------------------|---------|---------------------|------------------------
1  | 123456789 | KlipperPlugin | start_print  | {"filename":"cube..."}  | 1       | Print started       | 2024-11-23T10:31:02Z
2  | 123456789 | KlipperPlugin | get_status   | {}                      | 1       | Status: printing    | 2024-11-23T10:35:00Z
3  | 987654321 | PCControlPlugin| wake_pc     | {}                      | 0       | Host unreachable    | 2024-11-23T11:00:00Z
```

---

### 3. UserSettings
Пользовательские настройки и предпочтения.

```sql
CREATE TABLE IF NOT EXISTS UserSettings (
    UserId INTEGER NOT NULL,
    SettingKey TEXT NOT NULL,          -- Ключ настройки
    SettingValue TEXT NOT NULL,        -- Значение (может быть JSON)
    UpdatedAt TEXT NOT NULL,           -- Дата последнего обновления
    PRIMARY KEY (UserId, SettingKey)
);
```

**Пример данных:**
```
UserId    | SettingKey           | SettingValue      | UpdatedAt
----------|----------------------|-------------------|------------------------
123456789 | preferred_language   | ru                | 2024-11-23T10:00:00Z
123456789 | notification_enabled | true              | 2024-11-23T10:00:00Z
123456789 | printer_name         | Voron 2.4         | 2024-11-23T10:00:00Z
987654321 | preferred_language   | en                | 2024-11-23T11:00:00Z
```

---

## SQL для инициализации

Полный SQL скрипт (используется в `SQLiteStorage.InitializeAsync()`):

```sql
-- Таблица истории диалогов
CREATE TABLE IF NOT EXISTS ConversationHistory (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    Role TEXT NOT NULL CHECK(Role IN ('user', 'assistant')),
    Content TEXT NOT NULL,
    Timestamp TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE INDEX IF NOT EXISTS idx_conversation_userid 
    ON ConversationHistory(UserId, Timestamp DESC);

-- Таблица логов действий
CREATE TABLE IF NOT EXISTS ActionLog (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    PluginName TEXT NOT NULL,
    FunctionName TEXT NOT NULL,
    Parameters TEXT,
    Success INTEGER NOT NULL CHECK(Success IN (0, 1)),
    Result TEXT,
    Timestamp TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE INDEX IF NOT EXISTS idx_actionlog_userid 
    ON ActionLog(UserId, Timestamp DESC);

CREATE INDEX IF NOT EXISTS idx_actionlog_plugin 
    ON ActionLog(PluginName, Timestamp DESC);

-- Таблица настроек пользователей
CREATE TABLE IF NOT EXISTS UserSettings (
    UserId INTEGER NOT NULL,
    SettingKey TEXT NOT NULL,
    SettingValue TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    PRIMARY KEY (UserId, SettingKey)
);
```

---

## Примеры запросов

### Получить последние 50 сообщений пользователя
```sql
SELECT Role, Content, Timestamp
FROM ConversationHistory
WHERE UserId = ?
ORDER BY Timestamp DESC
LIMIT 50;
```

### Сохранить сообщение
```sql
INSERT INTO ConversationHistory (UserId, Role, Content, Timestamp)
VALUES (?, ?, ?, datetime('now'));
```

### Получить статистику действий за день
```sql
SELECT 
    PluginName,
    FunctionName,
    COUNT(*) as Count,
    SUM(Success) as SuccessCount,
    COUNT(*) - SUM(Success) as FailCount
FROM ActionLog
WHERE UserId = ?
  AND date(Timestamp) = date('now')
GROUP BY PluginName, FunctionName;
```

### Получить последние ошибки
```sql
SELECT 
    PluginName,
    FunctionName,
    Result,
    Timestamp
FROM ActionLog
WHERE Success = 0
ORDER BY Timestamp DESC
LIMIT 20;
```

### Сохранить настройку пользователя
```sql
INSERT OR REPLACE INTO UserSettings (UserId, SettingKey, SettingValue, UpdatedAt)
VALUES (?, ?, ?, datetime('now'));
```

### Получить настройку
```sql
SELECT SettingValue
FROM UserSettings
WHERE UserId = ? AND SettingKey = ?;
```

---

## Размер и производительность

### Оценка размера:
- **ConversationHistory**: ~100 байт на сообщение
  - 1000 сообщений ≈ 100 KB
  - 10000 сообщений ≈ 1 MB
  
- **ActionLog**: ~150 байт на действие
  - 1000 действий ≈ 150 KB
  
- **UserSettings**: ~50 байт на настройку
  - 100 настроек ≈ 5 KB

### Оптимизация:
- Индексы созданы для быстрого поиска по UserId и Timestamp
- Можно добавить периодическую очистку старых записей (> 30 дней)
- SQLite хорошо работает до нескольких GB данных

---

## Миграции (для будущих изменений)

### Добавление колонки
```sql
ALTER TABLE ConversationHistory 
ADD COLUMN MessageType TEXT DEFAULT 'text';
```

### Создание новой таблицы для принтеров
```sql
CREATE TABLE IF NOT EXISTS Printers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    Name TEXT NOT NULL,
    MoonrakerUrl TEXT NOT NULL,
    WebcamUrl TEXT,
    IsActive INTEGER DEFAULT 1,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
);
```

---

## Backup и восстановление

### Создать backup
```bash
# Создать копию базы
cp Data/botdata.db Data/botdata.db.backup

# Или экспортировать в SQL
sqlite3 Data/botdata.db .dump > backup.sql
```

### Восстановить из backup
```bash
# Из файла
cp Data/botdata.db.backup Data/botdata.db

# Из SQL
sqlite3 Data/botdata.db < backup.sql
```

---

## Инструменты для работы с БД

### SQLite Browser (GUI)
```bash
# Установка (Ubuntu/Debian)
sudo apt-get install sqlitebrowser

# Запуск
sqlitebrowser Data/botdata.db
```

### Командная строка
```bash
# Открыть базу
sqlite3 Data/botdata.db

# Команды SQLite CLI:
.tables                    # Список таблиц
.schema ConversationHistory# Схема таблицы
.mode column              # Красивый вывод
.headers on               # Показывать заголовки
SELECT * FROM ConversationHistory LIMIT 10;
.quit                     # Выход
```

---

## Connection String в appsettings.json

```json
{
  "Database": {
    "ConnectionString": "Data Source=Data/botdata.db;Cache=Shared"
  }
}
```

**Параметры:**
- `Data Source=...` - путь к файлу БД
- `Cache=Shared` - разделяемый кэш (для многопоточности)
- `Mode=ReadWrite` - режим (можно добавить)

---

## Пример использования в коде

```csharp
using Microsoft.Data.Sqlite;

public class SQLiteStorage : IStorage
{
    private readonly string _connectionString;
    
    public SQLiteStorage(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        
        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS ConversationHistory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                Role TEXT NOT NULL,
                Content TEXT NOT NULL,
                Timestamp TEXT NOT NULL
            );
        ";
        
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
    
    public async Task SaveMessageAsync(long userId, Message message, CancellationToken cancellationToken)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        
        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO ConversationHistory (UserId, Role, Content, Timestamp)
            VALUES ($userId, $role, $content, $timestamp)
        ";
        command.Parameters.AddWithValue("$userId", userId);
        command.Parameters.AddWithValue("$role", message.Role);
        command.Parameters.AddWithValue("$content", message.Content);
        command.Parameters.AddWithValue("$timestamp", DateTime.UtcNow.ToString("O"));
        
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
```

---

Эта схема покрывает все потребности проекта и готова к использованию!
