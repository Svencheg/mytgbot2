# Примеры использования MyTgBot2

## 📱 Диалоги с ботом

### Пример 1: Проверка статуса принтера

**Пользователь:**
```
Привет! Как там мой принтер?
```

**Бот (AI → Klipper Plugin):**
```
Привет! Твой принтер сейчас в состоянии "ready" (готов к работе). 
Температуры:
• Экструдер: 25.3°C (цель: 0°C)
• Стол: 24.1°C (цель: 0°C)
Последняя печать завершена.
```

---

### Пример 2: Запуск печати

**Пользователь:**
```
Запусти печать cube.gcode
```

**Бот (AI → Klipper Plugin):**
```
✓ Печать запущена!
Файл: cube.gcode
Статус: printing
Ожидаемое время: ~1ч 23м

Могу отправить тебе фото процесса когда захочешь 📷
```

---

### Пример 3: Мониторинг печати

**Пользователь:**
```
Покажи прогресс
```

**Бот (AI → Klipper Plugin):**
```
Печать в процессе:
━━━━━━━━━━━━━━━━━━━━ 65%

Файл: detailed_model.gcode
Слой: 157 / 240
Прошло: 42м 15с
Осталось: ~23м

Температуры:
• Экструдер: 215°C ✓
• Стол: 60°C ✓

Скорость: 100%
```

---

### Пример 4: Фото процесса

**Пользователь:**
```
Сделай фото
```

**Бот (AI → Klipper Plugin):**
```
📷 Фото с веб-камеры:
[изображение]

Всё выглядит хорошо! Печать идёт ровно.
```

---

### Пример 5: Пауза печати

**Пользователь:**
```
Поставь на паузу
```

**Бот (AI → Klipper Plugin):**
```
⏸️ Печать приостановлена.

Текущее состояние:
• Прогресс: 45%
• Слой: 108 / 240

Чтобы продолжить, напиши "продолжи печать"
```

---

### Пример 6: Список файлов

**Пользователь:**
```
Какие файлы доступны для печати?
```

**Бот (AI → Klipper Plugin):**
```
📂 Доступные файлы на принтере:

1. cube_20mm.gcode (234 KB)
2. benchy.gcode (1.2 MB)
3. vase.gcode (890 KB)
4. phone_stand.gcode (456 KB)
5. gear_set.gcode (2.1 MB)

Всего файлов: 5

Чтобы запустить, напиши: "запусти печать [имя файла]"
```

---

### Пример 7: Управление компьютером

**Пользователь:**
```
Включи мой комп
```

**Бот (AI → PC Control Plugin):**
```
⚡ Отправлен сигнал Wake-on-LAN...

Компьютер включается... (это может занять ~30 секунд)

Статус: ожидание загрузки
```

**Через 30 секунд:**
```
✓ Компьютер онлайн!
Имя: DESKTOP-PC
IP: 192.168.1.100
```

---

### Пример 8: Запуск программы на ПК

**Пользователь:**
```
Запусти Steam
```

**Бот (AI → PC Control Plugin):**
```
✓ Steam запущен!

Выполнено: start steam://

Хочешь запустить какую-то игру?
```

---

### Пример 9: Выключить компьютер

**Пользователь:**
```
Вырубай комп
```

**Бот (AI → PC Control Plugin):**
```
⚠️ Вы уверены что хотите выключить компьютер?

Отправьте "да" для подтверждения или "нет" для отмены.
```

**Пользователь:**
```
да
```

**Бот:**
```
✓ Команда выключения отправлена.

Компьютер завершает работу...
```

---

### Пример 10: Сложный сценарий

**Пользователь:**
```
Привет! Запусти печать benchy.gcode, а пока принтер греется - включи мой комп и запусти на нём Spotify
```

**Бот (AI → Multiple Plugins):**
```
Окей, выполняю:

1️⃣ Запуск печати benchy.gcode...
   ✓ Печать запускается
   ⏳ Нагрев: экструдер 25→215°C, стол 24→60°C

2️⃣ Включение компьютера...
   ✓ Wake-on-LAN отправлен
   ⏳ Ожидаю загрузку...

3️⃣ Запуск Spotify...
   ✓ Компьютер онлайн
   ✓ Spotify запущен

Всё готово! Принтер печатает, музыка играет 🎵
```

---

## 🎮 Команды без AI (прямые команды)

Если AI не доступен (нет Gemini API ключа), бот работает в режиме прямых команд:

```
/start_print benchy.gcode
/get_printer_status
/pause_print
/resume_print
/cancel_print
/list_files
/take_photo

/wake_pc
/shutdown_pc
/execute_command tasklist
/start_steam

/help
```

---

## 🔧 Примеры для разработчиков

### Добавление кастомного плагина

```csharp
// Plugins/Weather/WeatherPlugin.cs

public class WeatherPlugin : BasePlugin
{
    public override string PluginName => "WeatherPlugin";
    
    public override List<FunctionDefinition> GetAvailableFunctions()
    {
        return new List<FunctionDefinition>
        {
            new FunctionDefinition
            {
                Name = "get_weather",
                Description = "Получить погоду в городе",
                Parameters = new List<FunctionParameter>
                {
                    new FunctionParameter
                    {
                        Name = "city",
                        Type = "string",
                        Description = "Название города",
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
        if (request.FunctionName == "get_weather")
        {
            var city = GetParameter<string>(request, "city");
            
            // TODO: Вызвать API погоды
            var weather = await FetchWeatherAsync(city, cancellationToken);
            
            return Success($"Погода в {city}: {weather.Temperature}°C, {weather.Description}");
        }
        
        return Error("Unknown function");
    }
}
```

**Использование:**

**Пользователь:**
```
Какая погода в Москве?
```

**Бот (AI → Weather Plugin):**
```
Погода в Москве:
🌡️ Температура: -5°C
☁️ Облачно с прояснениями
💨 Ветер: 3 м/с
💧 Влажность: 78%
```

---

### Пример расширения функциональности Klipper

```csharp
// Добавить в KlipperPlugin.cs

new FunctionDefinition
{
    Name = "set_bed_temperature",
    Description = "Установить температуру стола",
    Parameters = new List<FunctionParameter>
    {
        new FunctionParameter
        {
            Name = "temperature",
            Type = "integer",
            Description = "Целевая температура в градусах Цельсия",
            Required = true
        }
    }
}

// В ExecuteActionAsync:
case "set_bed_temperature":
    var temp = GetParameter<int>(request, "temperature");
    await SetBedTemperatureAsync(temp, cancellationToken);
    return Success($"Температура стола установлена: {temp}°C");
```

**Использование:**

**Пользователь:**
```
Нагрей стол до 70 градусов
```

**Бот:**
```
✓ Температура стола установлена: 70°C

Текущая: 24°C → Цель: 70°C
Ожидаемое время нагрева: ~5 минут
```

---

## 🧪 Тестовые сценарии

### Сценарий 1: Первый запуск
1. Запустить бота
2. Отправить `/start` в Telegram
3. Проверить что бот ответил приветствием

### Сценарий 2: Проверка авторизации
1. Отправить сообщение с неавторизованного аккаунта
2. Бот должен ответить: "Доступ запрещён"

### Сценарий 3: Работа с историей
1. Отправить: "Привет, меня зовут Алекс"
2. Отправить: "Как меня зовут?"
3. Бот должен ответить: "Алекс" (используя контекст)

### Сценарий 4: Обработка ошибок
1. Отправить: "Запусти печать несуществующий_файл.gcode"
2. Бот должен корректно обработать ошибку и сообщить что файл не найден

---

## 📊 Формат данных

### Структура сообщения в БД

```json
{
  "id": 1,
  "userId": 123456789,
  "role": "user",
  "content": "Запусти печать cube.gcode",
  "timestamp": "2024-11-23T10:30:00Z"
}
```

### Структура ActionRequest

```json
{
  "pluginName": "KlipperPlugin",
  "functionName": "start_print",
  "parameters": {
    "filename": "cube.gcode"
  },
  "requestId": "req-20241123-103015-abc123"
}
```

### Структура AIResponse

```json
{
  "text": "Запускаю печать файла cube.gcode...",
  "actions": [
    {
      "pluginName": "KlipperPlugin",
      "functionName": "start_print",
      "parameters": {
        "filename": "cube.gcode"
      }
    }
  ],
  "confidence": 0.95,
  "requiresConfirmation": false
}
```

### Структура ActionResult

```json
{
  "success": true,
  "message": "Печать успешно запущена",
  "data": {
    "filename": "cube.gcode",
    "estimatedTime": "1h 23m",
    "status": "printing"
  },
  "errorMessage": null,
  "errorCode": null
}
```

---

## 🎨 Форматирование ответов

Бот поддерживает Markdown форматирование:

```markdown
*жирный текст*
_курсив_
`код`
[ссылка](https://example.com)

• Маркированный список
1. Нумерованный список

━━━━━━━━━━ прогресс бары
✓ ✗ ⚠️ 📷 🎵 ⚡ галочки и иконки
```

---

Это живые примеры - адаптируйте под свои нужды!
