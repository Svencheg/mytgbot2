/*
 * TICKET: AI Provider Interface
 * 
 * ОПИСАНИЕ:
 * Базовый интерфейс для всех AI провайдеров (Gemini, кастомные AI, локальные модели).
 * Обеспечивает единый контракт для работы с различными AI сервисами.
 * 
 * НАЗНАЧЕНИЕ:
 * - Позволяет легко заменять AI провайдеров без изменения основного кода
 * - Поддерживает как простые текстовые запросы, так и structured function calling
 * - Обеспечивает расширяемость для добавления новых AI моделей
 * 
 * ЧТО ПРИНИМАЕТ:
 * - string userMessage: текст запроса пользователя
 * - List<Message> conversationHistory: история диалога для контекста
 * - List<FunctionDefinition> availableFunctions: список доступных команд для AI
 * - CancellationToken: для отмены долгих операций
 * 
 * ЧТО ВОЗВРАЩАЕТ:
 * - AIResponse: объект с текстовым ответом пользователю и списком действий для выполнения
 * 
 * ПРИМЕРЫ ИСПОЛЬЗОВАНИЯ:
 * 1. Простой запрос: "Какая погода?" -> AIResponse { UserMessage = "Я бот для управления принтером" }
 * 2. Команда: "Запусти печать" -> AIResponse { Actions = [StartPrintAction], UserMessage = "Запускаю печать..." }
 * 3. Статус: "Как там принтер?" -> AIResponse { Actions = [GetStatusAction], UserMessage = "Проверяю статус..." }
 */

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyTgBot2.Core.Models;

namespace MyTgBot2.Core.Interfaces
{
    public interface IAIProvider
    {
        /// <summary>
        /// Уникальное имя провайдера (например, "Gemini", "CustomAI", "NoAI")
        /// </summary>
        string ProviderName { get; }
        
        /// <summary>
        /// Доступен ли провайдер для использования (проверка ключей API, соединения)
        /// </summary>
        Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Основной метод обработки запроса пользователя
        /// </summary>
        /// <param name="userMessage">Текст сообщения от пользователя</param>
        /// <param name="conversationHistory">История предыдущих сообщений</param>
        /// <param name="availableFunctions">Список команд, которые AI может вызывать</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Ответ AI с текстом для пользователя и действиями для выполнения</returns>
        Task<AIResponse> ProcessMessageAsync(
            string userMessage,
            List<Message> conversationHistory,
            List<FunctionDefinition> availableFunctions,
            CancellationToken cancellationToken = default
        );
        
        /// <summary>
        /// Инициализация провайдера (загрузка конфигурации, установка соединения)
        /// </summary>
        Task InitializeAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Освобождение ресурсов при завершении работы
        /// </summary>
        Task DisposeAsync();
    }
}
