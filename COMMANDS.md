# Быстрый справочник по командам

## 🚀 Запуск

```bash
# Проверка конфигурации
./check-config.sh

# Запуск бота
./start.sh

# Запуск вручную
dotnet run

# Запуск тестов
./test.sh
```

---

## 📦 Сборка

```bash
# Восстановить зависимости
dotnet restore

# Сборка Debug
dotnet build

# Сборка Release
dotnet build -c Release

# Publish (создать standalone версию)
dotnet publish -c Release -o ./publish
```

---

## 🔧 Разработка

```bash
# Добавить NuGet пакет
dotnet add package PackageName

# Удалить NuGet пакет
dotnet remove package PackageName

# Показать установленные пакеты
dotnet list package

# Очистить сборку
dotnet clean
```

---

## 🗄️ База данных

```bash
# Просмотр таблиц SQLite
sqlite3 Data/botdata.db ".tables"

# Просмотр данных
sqlite3 Data/botdata.db "SELECT * FROM ConversationHistory LIMIT 10;"

# Схема таблицы
sqlite3 Data/botdata.db ".schema ConversationHistory"

# Удалить базу (пересоздать)
rm Data/botdata.db
```

---

## 📝 Логи

```bash
# Последние логи
tail -f Logs/log-*.txt

# Поиск ошибок
grep "ERROR" Logs/log-*.txt

# Очистить логи
rm Logs/*.txt
```

---

## 🌐 Тестирование API

### Moonraker (Klipper)

```bash
# Статус сервера
curl http://localhost:7125/server/info

# Статус принтера
curl http://localhost:7125/printer/objects/query?print_stats

# Список файлов
curl http://localhost:7125/server/files/list?root=gcodes

# Запустить печать
curl -X POST http://localhost:7125/printer/print/start?filename=cube.gcode

# Пауза
curl -X POST http://localhost:7125/printer/print/pause

# Возобновить
curl -X POST http://localhost:7125/printer/print/resume

# Отменить
curl -X POST http://localhost:7125/printer/print/cancel
```

### Gemini API

```bash
# Простой запрос (замените YOUR_API_KEY)
curl -X POST \
  "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=YOUR_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "contents": [
      {
        "parts": [
          {"text": "Привет, как дела?"}
        ]
      }
    ]
  }'
```

---

## 🔐 Безопасность

```bash
# Проверить что secrets.txt в .gitignore
grep secrets.txt .gitignore

# Убедиться что secrets.txt не в git
git status | grep secrets.txt

# Права доступа к файлу
chmod 600 secrets.txt  # Только владелец может читать/писать
```

---

## 📊 Мониторинг

```bash
# Запущен ли бот
ps aux | grep dotnet | grep MyTgBot2

# Использование CPU/RAM
top -p $(pgrep -f MyTgBot2)

# Размер базы данных
du -h Data/botdata.db

# Размер логов
du -sh Logs/
```

---

## 🐛 Отладка

```bash
# Запуск с подробными логами
dotnet run --verbosity detailed

# Проверка компиляции без запуска
dotnet build --no-restore

# Список всех TODO в коде
grep -r "TODO" --include="*.cs" .

# Список NotImplementedException
grep -r "NotImplementedException" --include="*.cs" .
```

---

## 🔄 Git (если используется)

```bash
# Инициализация репозитория
git init

# Добавить все файлы (кроме .gitignore)
git add .

# Коммит
git commit -m "Initial commit"

# Проверить что secrets.txt не добавлен
git status | grep secrets.txt  # Не должно быть вывода
```

---

## 🎯 Быстрые тесты

### Test 1: Проверка конфигурации
```bash
dotnet run --project . --no-build 2>&1 | head -20
```

### Test 2: Проверка Klipper
```bash
curl -s http://localhost:7125/server/info | jq .
```

### Test 3: Проверка Telegram token
```bash
TOKEN=$(grep TELEGRAM_BOT_TOKEN secrets.txt | cut -d= -f2)
curl -s "https://api.telegram.org/bot$TOKEN/getMe" | jq .
```

---

## 📱 Telegram команды для бота

Когда бот запустится, можно отправить:

```
/start - Начать работу
/help - Список команд
Привет - Простое сообщение
Запусти печать cube.gcode - Команда с action
Как дела у принтера? - Естественный запрос
Включи компьютер - Команда для PC Control
```

---

## 🛠️ Troubleshooting

### Проблема: Ошибка компиляции
```bash
# Очистить и пересобрать
dotnet clean
rm -rf bin obj
dotnet restore
dotnet build
```

### Проблема: Бот не отвечает
```bash
# Проверить токен
grep TELEGRAM_BOT_TOKEN secrets.txt

# Проверить ID пользователя
grep AUTHORIZED_USER_IDS secrets.txt

# Узнать свой Telegram ID
# Отправить сообщение @userinfobot в Telegram
```

### Проблема: Moonraker недоступен
```bash
# Проверить что Klipper запущен
systemctl status klipper

# Проверить Moonraker
systemctl status moonraker

# Проверить доступность
curl http://localhost:7125/server/info
```

---

## 📚 Полезные ссылки

- [.NET CLI Reference](https://learn.microsoft.com/en-us/dotnet/core/tools/)
- [SQLite Browser](https://sqlitebrowser.org/)
- [Postman](https://www.postman.com/) - для тестирования API
- [Telegram Bot API](https://core.telegram.org/bots/api)

---

Сохраните этот файл для быстрого доступа к командам!
