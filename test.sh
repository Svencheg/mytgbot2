#!/bin/bash
# Тестирование отдельных компонентов проекта

echo "=== Component Test Suite ==="
echo ""

# Цвета для вывода
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

TESTS_PASSED=0
TESTS_FAILED=0

# Функция для запуска теста
run_test() {
    local test_name=$1
    local test_command=$2
    
    echo -n "Testing: $test_name... "
    
    if eval "$test_command" &>/dev/null; then
        echo -e "${GREEN}✓ PASS${NC}"
        TESTS_PASSED=$((TESTS_PASSED+1))
    else
        echo -e "${RED}✗ FAIL${NC}"
        TESTS_FAILED=$((TESTS_FAILED+1))
    fi
}

echo "Running component tests..."
echo ""

# Test 1: Проверка компиляции
echo "[1/5] Compilation Test"
run_test "Project builds" "dotnet build --no-restore -v quiet"
echo ""

# Test 2: Проверка Moonraker API
echo "[2/5] Moonraker API Test"
MOONRAKER_URL=$(grep -A1 "MoonrakerUrl" appsettings.json | tail -n1 | cut -d'"' -f4)
if [ ! -z "$MOONRAKER_URL" ]; then
    run_test "Moonraker /server/info" "curl -s -f '$MOONRAKER_URL/server/info'"
else
    echo -e "${YELLOW}  ⊘ SKIP (not configured)${NC}"
fi
echo ""

# Test 3: Проверка структуры БД (если SQLite реализован)
echo "[3/5] Database Test"
if [ -f "Data/botdata.db" ]; then
    run_test "SQLite database exists" "test -f Data/botdata.db"
else
    echo -e "${YELLOW}  ⊘ SKIP (database not created yet)${NC}"
fi
echo ""

# Test 4: Проверка secrets.txt
echo "[4/5] Configuration Test"
run_test "secrets.txt exists" "test -f secrets.txt"
run_test "TELEGRAM_BOT_TOKEN set" "grep -q 'TELEGRAM_BOT_TOKEN=..*' secrets.txt"
run_test "appsettings.json valid" "python3 -c 'import json; json.load(open(\"appsettings.json\"))' 2>/dev/null || jq . appsettings.json"
echo ""

# Test 5: Проверка зависимостей
echo "[5/5] Dependencies Test"
run_test "Telegram.Bot package" "dotnet list package | grep -q Telegram.Bot"
run_test "SQLite package" "dotnet list package | grep -q SQLite"
run_test "SSH.NET package" "dotnet list package | grep -q SSH.NET"
echo ""

# Итог
echo "============================================"
echo "Test Results:"
echo -e "  ${GREEN}✓ Passed: $TESTS_PASSED${NC}"
echo -e "  ${RED}✗ Failed: $TESTS_FAILED${NC}"
echo ""

if [ $TESTS_FAILED -eq 0 ]; then
    echo -e "${GREEN}✅ All tests passed!${NC}"
    exit 0
else
    echo -e "${RED}❌ Some tests failed.${NC}"
    exit 1
fi
