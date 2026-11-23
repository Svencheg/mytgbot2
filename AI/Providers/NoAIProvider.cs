/*
 * TICKET: No-AI Provider (Fallback)
 * 
 * ОПИСАНИЕ:
 * Простой провайдер без использования AI.
 * Используется как fallback когда AI недоступен или отключен.
 * Поддерживает только прямые команды через текст.
 * 
 * НАЗНАЧЕНИЕ:
 * - Работа бота в режиме "без AI" (только прямые команды)
 * - Fallback при недоступности основного AI провайдера
 * - Парсинг простых команд из текста
 * 
 * ЧТО ПРИНИМАЕТ:
 * - string userMessage: текст сообщения
 * - List<FunctionDefinition> functions: доступные команды
 * 
 * ЧТО ВОЗВРАЩАЕТ:
 * - AIResponse с распознанной командой или сообщением об ошибке
 * 
 * ПРИМЕРЫ КОМАНД:
 * "/start_print model.gcode" -> ActionRequest { FunctionName = "start_print", Parameters = { file: "model.gcode" } }
 * "/get_status" -> ActionRequest { FunctionName = "get_status" }
 * "/wake_pc" -> ActionRequest { FunctionName = "wake_pc" }
 * "Привет" -> AIResponse { UserMessage = "Используйте команды: /start_print, /get_status, /wake_pc" }
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyTgBot2.Core.Interfaces;
using MyTgBot2.Core.Models;

namespace MyTgBot2.AI.Providers
{
    public class NoAIProvider : IAIProvider
    {
        // TODO для джуна:
        // 1. Парсить текст сообщения
        // 2. Если начинается с "/" - это команда
        // 3. Извлечь имя команды и параметры
        // 4. Найти соответствующую FunctionDefinition
        // 5. Создать ActionRequest
        // 6. Если команда не распознана - вернуть список доступных команд
        
        public string ProviderName => "NoAI";
        
        public Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
        {
            // Всегда доступен
            return Task.FromResult(true);
        }
        
        public Task<AIResponse> ProcessMessageAsync(
            string userMessage,
            List<Message> conversationHistory,
            List<FunctionDefinition> availableFunctions,
            CancellationToken cancellationToken = default)
        {
            // TODO: Реализация
            // 1. Проверить начинается ли сообщение с "/"
            // 2. Если да - распарсить команду:
            //    Пример: "/start_print model.gcode bed_temp=60"
            //    -> functionName = "start_print"
            //    -> parameters = { file: "model.gcode", bed_temp: "60" }
            // 3. Найти FunctionDefinition по имени
            // 4. Создать ActionRequest
            // 5. Вернуть AIResponse с ActionRequest и текстом "Выполняю команду..."
            // 6. Если команда не найдена - вернуть список доступных команд
            throw new NotImplementedException();
        }
        
        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
        
        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Парсинг команды из текста
        /// Формат: /command_name param1=value1 param2=value2
        /// </summary>
        private (string commandName, Dictionary<string, object> parameters) ParseCommand(string text)
        {
            // TODO: Реализация
            // 1. Удалить "/" в начале
            // 2. Разделить по пробелам
            // 3. Первое слово - имя команды
            // 4. Остальные - параметры в формате key=value или просто значения
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Создать сообщение со списком доступных команд
        /// </summary>
        private string GenerateHelpMessage(List<FunctionDefinition> functions)
        {
            // TODO: Реализация
            // Пример вывода:
            // "Доступные команды:\n"
            // "/start_print [file] - Запустить печать\n"
            // "/get_status - Получить статус принтера\n"
            // ...
            throw new NotImplementedException();
        }
    }
}
