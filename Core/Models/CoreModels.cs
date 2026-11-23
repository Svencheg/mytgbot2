/*
 * TICKET: Core Data Models
 * 
 * ОПИСАНИЕ:
 * Основные модели данных, используемые во всей системе.
 * Эти классы представляют структуру данных, передаваемых между компонентами.
 */

using System;
using System.Collections.Generic;

namespace MyTgBot2.Core.Models
{
    // ========== MESSAGE MODELS ==========
    
    /// <summary>
    /// Сообщение в диалоге (от пользователя или от AI)
    /// Используется для хранения истории и передачи контекста
    /// </summary>
    public class Message
    {
        /// <summary>
        /// ID записи в БД (для истории)
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Telegram ID пользователя
        /// </summary>
        public long UserId { get; set; }
        
        /// <summary>
        /// Роль отправителя: "user", "assistant", "system"
        /// </summary>
        public string Role { get; set; }
        
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// Временная метка
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// Дополнительные метаданные (опционально)
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }
    }
    
    // ========== AI RESPONSE MODELS ==========
    
    /// <summary>
    /// Ответ от AI провайдера
    /// Содержит текст для пользователя и список действий для выполнения
    /// </summary>
    public class AIResponse
    {
        /// <summary>
        /// Текстовое сообщение для отправки пользователю
        /// </summary>
        public string UserMessage { get; set; }
        
        /// <summary>
        /// Список действий, которые нужно выполнить (может быть пустым)
        /// </summary>
        public List<ActionRequest> Actions { get; set; } = new List<ActionRequest>();
        
        /// <summary>
        /// Успешно ли обработан запрос
        /// </summary>
        public bool Success { get; set; } = true;
        
        /// <summary>
        /// Сообщение об ошибке (если Success = false)
        /// </summary>
        public string ErrorMessage { get; set; }
    }
    
    // ========== ACTION MODELS ==========
    
    /// <summary>
    /// Запрос на выполнение действия через плагин
    /// </summary>
    public class ActionRequest
    {
        /// <summary>
        /// Имя плагина, который должен выполнить действие
        /// Например: "KlipperPlugin", "PCControlPlugin"
        /// </summary>
        public string PluginName { get; set; }
        
        /// <summary>
        /// Имя функции/команды для выполнения
        /// Например: "start_print", "wake_pc", "get_status"
        /// </summary>
        public string FunctionName { get; set; }
        
        /// <summary>
        /// Параметры команды в виде словаря
        /// Например: { "file": "model.gcode", "bed_temp": "60" }
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        
        /// <summary>
        /// ID запроса для отслеживания
        /// </summary>
        public string RequestId { get; set; } = Guid.NewGuid().ToString();
    }
    
    /// <summary>
    /// Результат выполнения действия плагином
    /// </summary>
    public class ActionResult
    {
        /// <summary>
        /// Успешно ли выполнено действие
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Сообщение о результате (для пользователя)
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// Данные, возвращённые действием (могут быть любого типа)
        /// Например: статус принтера, путь к фот��, список процессов
        /// </summary>
        public object Data { get; set; }
        
        /// <summary>
        /// Сообщение об ошибке (если Success = false)
        /// </summary>
        public string ErrorMessage { get; set; }
        
        /// <summary>
        /// Код ошибки (опционально)
        /// </summary>
        public string ErrorCode { get; set; }
    }
    
    // ========== FUNCTION DEFINITION (для AI) ==========
    
    /// <summary>
    /// Описание функции/команды для AI (Function Calling)
    /// AI использует это для понимания, какие команды доступны
    /// </summary>
    public class FunctionDefinition
    {
        /// <summary>
        /// Имя плагина
        /// </summary>
        public string PluginName { get; set; }
        
        /// <summary>
        /// Имя функции (должно быть уникальным в рамках плагина)
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Описание функции для AI (что она делает)
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Список параметров функции
        /// </summary>
        public List<FunctionParameter> Parameters { get; set; } = new List<FunctionParameter>();
    }
    
    /// <summary>
    /// Параметр функции
    /// </summary>
    public class FunctionParameter
    {
        /// <summary>
        /// Имя параметра
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Тип параметра: "string", "integer", "boolean", "number"
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Описание параметра для AI
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Обязателен ли параметр
        /// </summary>
        public bool Required { get; set; }
        
        /// <summary>
        /// Значение по умолчанию (опционально)
        /// </summary>
        public object DefaultValue { get; set; }
    }
    
    // ========== LOGGING MODELS ==========
    
    /// <summary>
    /// Запись лога действия системы
    /// Используется для отслеживания всех команд и действий
    /// </summary>
    public class ActionLog
    {
        /// <summary>
        /// ID записи в БД
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Telegram ID пользователя, инициировавшего действие
        /// </summary>
        public long UserId { get; set; }
        
        /// <summary>
        /// Временная метка
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// Имя плагина
        /// </summary>
        public string PluginName { get; set; }
        
        /// <summary>
        /// Имя выполненной функции
        /// </summary>
        public string FunctionName { get; set; }
        
        /// <summary>
        /// Параметры в JSON формате
        /// </summary>
        public string Parameters { get; set; }
        
        /// <summary>
        /// Успешно ли выполнено
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Сообщение о результате
        /// </summary>
        public string ResultMessage { get; set; }
        
        /// <summary>
        /// Сообщение об ошибке (если есть)
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
