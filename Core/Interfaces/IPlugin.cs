/*
 * TICKET: Plugin Interface
 * 
 * ОПИСАНИЕ:
 * Базовый интерфейс для всех плагинов системы.
 * Плагины расширяют функциональность бота (управление принтером, компьютером, умным домом).
 * 
 * НАЗНАЧЕНИЕ:
 * - Обеспечивает единый контракт для всех плагинов
 * - Позволяет динамически загружать новые плагины из DLL без перекомпиляции
 * - Предоставляет AI список доступных команд через FunctionDefinitions
 * 
 * АРХИТЕКТУРА ПЛАГИНА:
 * 1. Плагин регистрируется при старте или загружается динамически
 * 2. Плагин предоставляет список FunctionDefinition (описание команд для AI)
 * 3. AI вызывает команду через ExecuteActionAsync()
 * 4. Плагин выполняет действие и возвращает результат
 * 
 * ЧТО ПРИНИМАЕТ:
 * - ActionRequest: объект с именем команды и параметрами
 * - CancellationToken: для отмены операций
 * 
 * ЧТО ВОЗВРАЩАЕТ:
 * - ActionResult: результат выполнения (успех/ошибка, данные, сообщение)
 * - List<FunctionDefinition>: список команд, которые плагин поддерживает
 * 
 * ПРИМЕРЫ ПЛАГИНОВ:
 * 1. KlipperPlugin: start_print, stop_print, get_status, take_photo
 * 2. PCControlPlugin: wake_pc, shutdown_pc, run_steam, execute_cmd
 * 3. SmartHomePlugin (будущее): turn_on_light, set_temperature
 */

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyTgBot2.Core.Models;

namespace MyTgBot2.Core.Interfaces
{
    public interface IPlugin
    {
        /// <summary>
        /// Уникальное имя плагина (например, "KlipperPlugin", "PCControlPlugin")
        /// </summary>
        string PluginName { get; }
        
        /// <summary>
        /// Версия плагина (для совместимости)
        /// </summary>
        string Version { get; }
        
        /// <summary>
        /// Описание плагина для пользователя
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Доступен ли плагин (проверка подключений, конфигурации)
        /// </summary>
        Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Получить список всех команд, которые плагин поддерживает
        /// Эти команды передаются AI для использования
        /// </summary>
        List<FunctionDefinition> GetAvailableFunctions();
        
        /// <summary>
        /// Выполнить конкретное действие плагина
        /// </summary>
        /// <param name="request">Запрос с именем действия и параметрами</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат выполнения действия</returns>
        Task<ActionResult> ExecuteActionAsync(ActionRequest request, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Инициализация плагина при загрузке
        /// </summary>
        Task InitializeAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Освобождение ресурсов при выгрузке плагина
        /// </summary>
        Task DisposeAsync();
    }
}
