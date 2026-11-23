#!/bin/bash
# Скрипт для проверки конфигурации и зависимостей

echo "=== MyTgBot2 Configuration Checker ==="
echo ""

ERRORS=0
WARNINGS=0

# Проверка .NET
echo "[1/8] Проверка .NET SDK..."
if command -v dotnet &> /dev/null; then
    VERSION=$(dotnet --version)
    echo "  ✓ .NET SDK найден: $VERSION"
    MAJOR=$(echo $VERSION | cut -d. -f1)
    if [ "$MAJOR" -lt 6 ]; then
        echo "  ⚠️  Рекомендуется версия 6.0 или выше"
        WARNINGS=$((WARNINGS+1))
    fi
else
    echo "  ❌ .NET SDK не установлен!"
    ERRORS=$((ERRORS+1))
fi
echo ""

# Проверка secrets.txt
echo "[2/8] Проверка secrets.txt..."
if [ -f "secrets.txt" ]; then
    echo "  ✓ Файл найден"
    
    # Проверка обязательных полей
    if grep -q "TELEGRAM_BOT_TOKEN=..*" secrets.txt; then
        echo "  ✓ TELEGRAM_BOT_TOKEN настроен"
    else
        echo "  ❌ TELEGRAM_BOT_TOKEN не настроен!"
        ERRORS=$((ERRORS+1))
    fi
    
    if grep -q "GEMINI_API_KEY=..*" secrets.txt; then
        echo "  ✓ GEMINI_API_KEY настроен"
    else
        echo "  ⚠️  GEMINI_API_KEY не настроен (бот будет работать в NoAI режиме)"
        WARNINGS=$((WARNINGS+1))
    fi
    
    if grep -q "AUTHORIZED_USER_IDS=..*" secrets.txt; then
        echo "  ✓ AUTHORIZED_USER_IDS настроен"
    else
        echo "  ❌ AUTHORIZED_USER_IDS не настроен!"
        ERRORS=$((ERRORS+1))
    fi
else
    echo "  ❌ Файл secrets.txt не найден!"
    ERRORS=$((ERRORS+1))
fi
echo ""

# Проверка appsettings.json
echo "[3/8] Проверка appsettings.json..."
if [ -f "appsettings.json" ]; then
    echo "  ✓ Файл найден"
else
    echo "  ❌ Файл appsettings.json не найден!"
    ERRORS=$((ERRORS+1))
fi
echo ""

# Проверка структуры папок
echo "[4/8] Проверка структуры проекта..."
REQUIRED_DIRS=("Core" "AI" "Bot" "Plugins" "Storage" "Data" "Logs")
for dir in "${REQUIRED_DIRS[@]}"; do
    if [ -d "$dir" ]; then
        echo "  ✓ $dir/"
    else
        echo "  ❌ $dir/ не найдена!"
        ERRORS=$((ERRORS+1))
    fi
done
echo ""

# Проверка наличия ключевых файлов
echo "[5/8] Проверка ключевых файлов..."
KEY_FILES=(
    "Program.cs"
    "MyTgBot2.csproj"
    "Core/Interfaces/IPlugin.cs"
    "Core/Interfaces/IAIProvider.cs"
    "Bot/TelegramBotHandler.cs"
)
for file in "${KEY_FILES[@]}"; do
    if [ -f "$file" ]; then
        echo "  ✓ $file"
    else
        echo "  ❌ $file не найден!"
        ERRORS=$((ERRORS+1))
    fi
done
echo ""

# Проверка доступности Klipper (если настроен)
echo "[6/8] Проверка доступности Klipper/Moonraker..."
if grep -q "MoonrakerUrl" appsettings.json; then
    MOONRAKER_URL=$(grep -A1 "MoonrakerUrl" appsettings.json | tail -n1 | cut -d'"' -f4)
    if [ ! -z "$MOONRAKER_URL" ]; then
        if curl -s -o /dev/null -w "%{http_code}" "$MOONRAKER_URL/server/info" | grep -q "200"; then
            echo "  ✓ Moonraker доступен: $MOONRAKER_URL"
        else
            echo "  ⚠️  Moonraker недоступен: $MOONRAKER_URL"
            echo "     (Плагин Klipper не будет работать)"
            WARNINGS=$((WARNINGS+1))
        fi
    fi
else
    echo "  ⚠️  MoonrakerUrl не настроен"
    WARNINGS=$((WARNINGS+1))
fi
echo ""

# Проверка SSH для PC Control (опционально)
echo "[7/8] Проверка настроек PC Control..."
if grep -q "PC_SSH_HOST=..*" secrets.txt 2>/dev/null; then
    echo "  ✓ PC SSH настроен"
else
    echo "  ⚠️  PC SSH не настроен (плагин PCControl не будет работать)"
    WARNINGS=$((WARNINGS+1))
fi
echo ""

# Проверка прав на запись
echo "[8/8] Проверка прав доступа..."
if [ -w "Data" ]; then
    echo "  ✓ Запись в Data/ разрешена"
else
    echo "  ❌ Нет прав на запись в Data/!"
    ERRORS=$((ERRORS+1))
fi
if [ -w "Logs" ]; then
    echo "  ✓ Запись в Logs/ разрешена"
else
    echo "  ❌ Нет прав на запись в Logs/!"
    ERRORS=$((ERRORS+1))
fi
echo ""

# Итог
echo "============================================"
echo "Итого:"
echo "  ❌ Ошибок: $ERRORS"
echo "  ⚠️  Предупреждений: $WARNINGS"
echo ""

if [ $ERRORS -eq 0 ]; then
    echo "✅ Конфигурация в порядке! Можно запускать бота."
    echo "   Используйте: ./start.sh"
    exit 0
else
    echo "❌ Обнаружены ошибки! Исправьте их перед запуском."
    echo "   См. README.md для инструкций."
    exit 1
fi
