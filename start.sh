#!/bin/bash
# Скрипт для быстрого запуска бота

echo "=== MyTgBot2 Launcher ==="
echo ""

# Проверка наличия .NET
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK не установлен!"
    echo "Установите .NET SDK 6.0 или выше:"
    echo "  wget https://dot.net/v1/dotnet-install.sh"
    echo "  chmod +x dotnet-install.sh"
    echo "  ./dotnet-install.sh --channel 6.0"
    exit 1
fi

echo "✓ .NET SDK найден: $(dotnet --version)"
echo ""

# Проверка конфигурации
if [ ! -f "secrets.txt" ]; then
    echo "❌ Файл secrets.txt не найден!"
    echo "Создайте файл secrets.txt и заполните необходимые данные."
    exit 1
fi

echo "✓ secrets.txt найден"
echo ""

# Проверка наличия токена
if ! grep -q "TELEGRAM_BOT_TOKEN=" secrets.txt; then
    echo "⚠️  TELEGRAM_BOT_TOKEN не найден в secrets.txt"
    echo "Добавьте: TELEGRAM_BOT_TOKEN=your_token"
fi

if ! grep -q "GEMINI_API_KEY=" secrets.txt; then
    echo "⚠️  GEMINI_API_KEY не найден в secrets.txt"
    echo "Добавьте: GEMINI_API_KEY=your_key"
fi

echo ""

# Восстановление зависимостей
echo "[1/3] Восстановление NuGet пакетов..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "❌ Ошибка при восстановлении пакетов!"
    exit 1
fi
echo ""

# Сборка проекта
echo "[2/3] Сборка проекта..."
dotnet build --configuration Release
if [ $? -ne 0 ]; then
    echo "❌ Ошибка при сборке проекта!"
    echo "Возможно не все методы реализованы."
    echo "См. DEVELOPMENT_GUIDE.md для инструкций."
    exit 1
fi
echo ""

# Запуск
echo "[3/3] Запуск бота..."
echo "------------------------------------------------------------"
dotnet run --configuration Release
