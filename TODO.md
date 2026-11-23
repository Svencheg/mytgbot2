# Список задач для реализации (TODO)

## ✅ Выполнено

- [x] Создание структуры проекта
- [x] Создание всех интерфейсов (IPlugin, IAIProvider, IStorage)
- [x] Создание моделей данных (CoreModels, ConfigurationModels)
- [x] Скелет всех классов с сигнатурами методов
- [x] Файл конфигурации (appsettings.json, secrets.txt)
- [x] .csproj с зависимостями
- [x] Program.cs с точкой входа
- [x] Документация (README, DEVELOPMENT_GUIDE, EXAMPLES, COMMANDS)
- [x] Вспомогательные скрипты (start.sh, test.sh, check-config.sh)

---

## 🔥 Критичные задачи (нужно сделать первым)

### Core/Services/ConfigurationLoader.cs
- [ ] `LoadConfiguration()` - чтение appsettings.json через System.Text.Json
- [ ] `ParseSecretsFile()` - парсинг KEY=VALUE формата
- [ ] Добавить валидацию обязательных полей
- [ ] Обработка ошибок при чтении файлов

**Приоритет:** 🔴 КРИТИЧНЫЙ  
**Сложность:** ⭐ Простая  
**Время:** 1-2 часа

---

### Storage/Database/SQLiteStorage.cs
- [ ] `InitializeAsync()` - создать таблицы БД (3 таблицы)
- [ ] `SaveMessageAsync()` - INSERT сообщение
- [ ] `GetConversationHistoryAsync()` - SELECT последние N сообщений с ORDER BY
- [ ] `SaveActionLogAsync()` - INSERT лог действия
- [ ] `SaveUserSettingAsync()` - INSERT OR REPLACE
- [ ] `GetUserSettingAsync()` - SELECT
- [ ] Добавить индексы для производительности
- [ ] Connection pooling и правильный Dispose

**Приоритет:** 🔴 КРИТИЧНЫЙ  
**Сложность:** ⭐⭐ Средняя  
**Время:** 2-3 часа

---

### Core/Services/PluginManager.cs
- [ ] `RegisterPlugin()` - добавить в список _plugins
- [ ] `GetAllFunctions()` - собрать функции со всех плагинов
- [ ] `ExecuteActionAsync()` - найти плагин по имени и вызвать метод
- [ ] `LoadDynamicPlugins()` - загрузить DLL плагины (опционально)
- [ ] Обработка ошибок если плагин не найден

**Приоритет:** 🔴 КРИТИЧНЫЙ  
**Сложность:** ⭐⭐ Средняя  
**Время:** 1-2 часа

---

## 🟠 Высокий приоритет

### AI/Providers/GeminiProvider.cs
- [ ] `ProcessMessageAsync()` - главный метод обработки
- [ ] `BuildGeminiRequest()` - создать JSON для Gemini API
  - Добавить system instruction
  - Добавить историю сообщений
  - Добавить function declarations
- [ ] `ParseGeminiResponse()` - распарсить ответ
  - Извлечь текст
  - Извлечь function calls
  - Преобразовать в ActionRequest[]
- [ ] Обработка ошибок API (rate limit, invalid key, etc)
- [ ] Fallback на NoAI если Gemini недоступен

**Приоритет:** 🟠 ВЫСОКИЙ  
**Сложность:** ⭐⭐⭐ Сложная  
**Время:** 3-4 часа

**Документация:** https://ai.google.dev/docs

---

### Bot/TelegramBotHandler.cs
- [ ] `StartAsync()` - запустить long polling
- [ ] `HandleUpdateAsync()` - обработать входящее сообщение
  - Извлечь userId и текст
  - Проверить авторизацию
  - Загрузить историю из Storage
  - Вызвать AIProvider
  - Выполнить actions через PluginManager
  - Отправить ответ
  - Сохранить в историю
- [ ] `SendMessageAsync()` - отправка с разбивкой на части (4096 символов)
- [ ] `IsUserAuthorized()` - проверка whitelist
- [ ] `ExecuteActionsAsync()` - выполнить массив действий
- [ ] Обработка команд /start, /help
- [ ] Обработка ошибок и отправка пользователю

**Приоритет:** 🟠 ВЫСОКИЙ  
**Сложность:** ⭐⭐⭐ Сложная  
**Время:** 2-3 часа

---

### Plugins/Klipper/KlipperPlugin.cs
- [ ] `GetAvailableFunctions()` - вернуть список функций
- [ ] `IsAvailableAsync()` - проверить доступность Moonraker
- [ ] Реализовать каждую функцию:
  - [ ] `StartPrintAsync()` - POST /printer/print/start
  - [ ] `PausePrintAsync()` - POST /printer/print/pause
  - [ ] `ResumePrintAsync()` - POST /printer/print/resume
  - [ ] `CancelPrintAsync()` - POST /printer/print/cancel
  - [ ] `GetPrinterStatusAsync()` - GET /printer/objects/query
  - [ ] `ListFilesAsync()` - GET /server/files/list
  - [ ] `TakePhotoAsync()` - GET с webcam URL
- [ ] Обработка ошибок API
- [ ] Форматирование красивых ответов

**Приоритет:** 🟠 ВЫСОКИЙ  
**Сложность:** ⭐⭐ Средняя  
**Время:** 2-3 часа

**Документация:** https://moonraker.readthedocs.io/en/latest/web_api/

---

## 🟡 Средний приоритет

### AI/Adapters/AIProviderFactory.cs
- [ ] `CreateProviderAsync()` - создать провайдер по конфигурации
- [ ] Поддержка разных провайдеров (Gemini, NoAI, Custom)
- [ ] Fallback логика

**Приоритет:** 🟡 СРЕДНИ��  
**Сложность:** ⭐ Простая  
**Время:** 30 минут

---

### AI/Providers/NoAIProvider.cs
- [ ] `ProcessMessageAsync()` - парсить команды напрямую
- [ ] Парсинг формата `/command param1 param2`
- [ ] Создание ActionRequest без AI

**Приоритет:** 🟡 СРЕДНИЙ  
**Сложность:** ⭐ Простая  
**Время:** 1 час

---

## 🟢 Низкий приоритет (можно сделать позже)

### Plugins/PCControl/PCControlPlugin.cs
- [ ] `WakePCAsync()` - отправить magic packet
- [ ] `ShutdownPCAsync()` - выключить через SSH
- [ ] `RestartPCAsync()` - перезагрузить
- [ ] `ExecuteCommandAsync()` - выполнить команду через SSH
- [ ] `CheckPCStatusAsync()` - проверить доступность (ping)
- [ ] `GetProcessListAsync()` - список процессов
- [ ] `StartSteamAsync()` - запустить Steam
- [ ] `StartApplicationAsync()` - запустить программу

**Приоритет:** 🟢 НИЗКИЙ  
**Сложность:** ⭐⭐ Средняя  
**Время:** 2-3 часа

**Библиотека:** SSH.NET

---

## 🔵 Дополнительные улучшения (опционально)

### Расширенная функциональность
- [ ] Динамическая загрузка плагинов из DLL
- [ ] Web интерфейс для управления
- [ ] Поддержка нескольких принтеров
- [ ] Графики температур
- [ ] Таймлапс видео
- [ ] Уведомления о завершении печати (Push)
- [ ] Поддержка inline кнопок Telegram
- [ ] Локализация (мультиязычность)
- [ ] Метрики и статистика печати

### Интеграции
- [ ] Home Assistant / Zigbee
- [ ] Text-to-Speech / Speech-to-Text
- [ ] OctoPrint support (альтернатива Moonraker)
- [ ] Prusa Link support
- [ ] Discord bot
- [ ] Web hooks

### Безопасность
- [ ] Rate limiting
- [ ] Роли пользователей (admin, viewer)
- [ ] Двухфакторная аутентификация
- [ ] Шифрование secrets
- [ ] Audit log

### Мониторинг и отладка
- [ ] Prometheus метрики
- [ ] Structured logging (Serilog)
- [ ] Health checks endpoint
- [ ] Performance profiling

---

## 📊 Прогресс по компонентам

```
Core Layer:        [████████░░] 80% (только реализация методов)
AI Layer:          [██░░░░░░░░] 20% (нужна реализация Gemini)
Bot Layer:         [██░░░░░░░░] 20% (нужна реализация Handler)
Storage Layer:     [███░░░░░░░] 30% (нужна реализация SQLite)
Plugins - Klipper: [███░░░░░░░] 30% (нужны все методы)
Plugins - PC:      [█░░░░░░░░░] 10% (optional)
Documentation:     [██████████] 100% ✓
Infrastructure:    [██████████] 100% ✓
```

**Общий прогресс:** `~40%`

---

## 🎯 Roadmap

### Фаза 1: MVP (Minimum Viable Product) - 1-2 дня
- ✅ Структура проекта
- ⏳ ConfigurationLoader
- ⏳ SQLiteStorage (базовая реализация)
- ⏳ PluginManager
- ⏳ KlipperPlugin (хотя бы get_status и start_print)
- ⏳ GeminiProvider (базовая реализация)
- ⏳ TelegramBotHandler
- ⏳ Запуск и первый тест

**Цель:** Бот может получать сообщения, обрабатывать через Gemini, выполнять базовые команды Klipper

---

### Фаза 2: Full Featured - 2-3 дня
- Все функции KlipperPlugin
- Улучшенная обработка ошибок
- Красивое форматирование ответов
- История диалогов работает правильно
- Логирование всех действий

**Цель:** Полнофункциональный бот для управления принтером

---

### Фаза 3: PC Control - 1-2 дня
- PCControlPlugin реализация
- Wake-on-LAN
- SSH команды
- Интеграция со Steam

**Цель:** Управление компьютером через бота

---

### Фаза 4: Polish & Extend - ongoing
- Новые плагины
- UI улучшения
- Performance оптимизация
- Новые фичи

---

## 📝 Заметки для разработки

### Порядок разработки (рекомендуется):
1. ConfigurationLoader → тесты
2. SQLiteStorage → тесты
3. PluginManager → тесты
4. KlipperPlugin (только get_status) → тесты с реальным принтером
5. NoAIProvider → тесты (легче чем Gemini)
6. TelegramBotHandler (базовая версия) → тесты с ботом
7. Всё собрать вместе → протестировать end-to-end
8. GeminiProvider → интегрировать
9. Остальные функции KlipperPlugin
10. PCControlPlugin (опционально)

### Советы:
- Коммитьте после каждой задачи
- Пишите Console.WriteLine() для отладки
- Тестируйте каждый компонент отдельно
- Используйте Postman для тестирования API
- Не бойтесь гуглить примеры кода

### Если застряли:
1. Прочитайте документацию API
2. Поищите примеры на GitHub
3. Используйте ChatGPT для помощи с кодом
4. Упростите задачу - начните с минимального варианта

---

**Удачи в разработке! 🚀**

Начинайте с Фазы 1 и двигайтесь поэтапно.
