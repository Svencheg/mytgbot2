/*
 * TICKET: Storage Interface
 * 
 * ОПИСАНИЕ:
 * Интерфейс для работы с хранилищем данных (SQLite, JSON, MongoDB в будущем).
 * Обеспечивает сохранение истории диалогов, логов команд и настроек пользователей.
 * 
 * НАЗНАЧЕНИЕ:
 * - Единый интерфейс для работы с различными типами хранилищ
 * - Сохранение истории для контекста AI
 * - Логирование всех действий системы
 * - Хранение настроек пользователей
 * 
 * ЧТО ПРИНИМАЕТ:
 * - long userId: Telegram ID пользователя
 * - Message: объект сообщения для сохранения
 * - LogEntry: запись лога для сохранения
 * - int limit: количество записей для извлечения
 * 
 * ЧТО ВОЗВРАЩАЕТ:
 * - List<Message>: история сообщений
 * - bool: успех операции
 * - Dictionary<string, string>: настройки пользователя
 * 
 * СТРУКТУРА ДАННЫХ:
 * 1. ConversationHistory: userId, timestamp, role (user/assistant), content
 * 2. ActionLogs: userId, timestamp, actionName, parameters, result, success
 * 3. UserSettings: userId, settingKey, settingValue
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyTgBot2.Core.Models;

namespace MyTgBot2.Core.Interfaces
{
    public interface IStorage
    {
        // ===== CONVERSATION HISTORY =====
        
        /// <summary>
        /// Сохранить сообщение в историю диалога
        /// </summary>
        Task<bool> SaveMessageAsync(long userId, Message message, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Получить последние N сообщений из истории для контекста AI
        /// </summary>
        Task<List<Message>> GetConversationHistoryAsync(long userId, int limit = 20, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Очистить историю диалога пользователя
        /// </summary>
        Task<bool> ClearConversationHistoryAsync(long userId, CancellationToken cancellationToken = default);
        
        // ===== ACTION LOGS =====
        
        /// <summary>
        /// Сохранить лог выполненного действия
        /// </summary>
        Task<bool> SaveActionLogAsync(ActionLog log, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Получить логи действий за период
        /// </summary>
        Task<List<ActionLog>> GetActionLogsAsync(long userId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
        
        // ===== USER SETTINGS =====
        
        /// <summary>
        /// Сохранить настройку пользователя
        /// </summary>
        Task<bool> SaveUserSettingAsync(long userId, string key, string value, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Получить настройку пользователя
        /// </summary>
        Task<string> GetUserSettingAsync(long userId, string key, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Получить все настройки пользователя
        /// </summary>
        Task<Dictionary<string, string>> GetAllUserSettingsAsync(long userId, CancellationToken cancellationToken = default);
        
        // ===== INITIALIZATION =====
        
        /// <summary>
        /// Инициализация хранилища (создание таблиц, миграции)
        /// </summary>
        Task InitializeAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        Task DisposeAsync();
    }
}
