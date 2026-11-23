/*
 * TICKET: Base Plugin Class
 * 
 * ОПИСАНИЕ:
 * Базовый абстрактный класс для всех плагинов.
 * Предоставляет общий функционал (логирование, валидация параметров).
 * 
 * НАЗНАЧЕНИЕ:
 * - Упростить создание новых плагинов
 * - Предоставить общие методы (логирование, обработка ошибок)
 * - Обеспечить единообразие плагинов
 * 
 * КАК ИСПОЛЬЗОВАТЬ:
 * Новый плагин наследуется от BasePlugin и реализует только специфичные методы
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyTgBot2.Core.Interfaces;
using MyTgBot2.Core.Models;

namespace MyTgBot2.Plugins.Base
{
    public abstract class BasePlugin : IPlugin
    {
        // TODO для джуна:
        // 1. Реализовать базовые методы
        // 2. Добавить логирование действий
        // 3. Добавить валидацию параметров
        // 4. Обработку ошибок с try-catch
        
        public abstract string PluginName { get; }
        public abstract string Version { get; }
        public abstract string Description { get; }
        
        /// <summary>
        /// Получить список функций плагина (должен быть реализован в наследниках)
        /// </summary>
        public abstract List<FunctionDefinition> GetAvailableFunctions();
        
        /// <summary>
        /// Выполнить действие (должно быть реализовано в наследниках)
        /// </summary>
        public abstract Task<ActionResult> ExecuteActionAsync(ActionRequest request, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Проверка доступности (может быть переопределена)
        /// </summary>
        public virtual Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }
        
        /// <summary>
        /// Инициализация (может быть переопределена)
        /// </summary>
        public virtual Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            Log($"Инициализация плагина {PluginName}");
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Освобождение ресурсов (может быть переопределена)
        /// </summary>
        public virtual Task DisposeAsync()
        {
            Log($"Освобождение ресурсов плагина {PluginName}");
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Создать успешный результат
        /// </summary>
        protected ActionResult Success(string message, object data = null)
        {
            return new ActionResult
            {
                Success = true,
                Message = message,
                Data = data
            };
        }
        
        /// <summary>
        /// Создать результат с ошибкой
        /// </summary>
        protected ActionResult Error(string message, string errorCode = null, Exception ex = null)
        {
            return new ActionResult
            {
                Success = false,
                Message = message,
                ErrorMessage = ex?.Message ?? message,
                ErrorCode = errorCode
            };
        }
        
        /// <summary>
        /// Логирование (TODO: подключить нормальный логгер)
        /// </summary>
        protected void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{PluginName}] {message}");
        }
        
        /// <summary>
        /// Получить параметр из запроса с проверкой типа
        /// </summary>
        protected T GetParameter<T>(ActionRequest request, string parameterName, T defaultValue = default)
        {
            if (request.Parameters.TryGetValue(parameterName, out var value))
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    Log($"Ошибка преобразования параметра {parameterName} к типу {typeof(T).Name}");
                    return defaultValue;
                }
            }
            return defaultValue;
        }
    }
}
