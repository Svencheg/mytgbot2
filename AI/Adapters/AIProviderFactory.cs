/*
 * TICKET: AI Provider Factory
 * 
 * ОПИСАНИЕ:
 * Фабрика для создания AI провайдеров на основе конфигурации.
 * Поддерживает выбор между Gemini, кастомным AI или NoAI режимом.
 * 
 * НАЗНАЧЕНИЕ:
 * - Единая точка создания AI провайдеров
 * - Автоматический fallback на NoAI при недоступности основного провайдера
 * - Поддержка переключения провайдеров в runtime
 * 
 * ЧТО ПРИНИМАЕТ:
 * - AppSettings: конфигурация приложения
 * - Secrets: секретные ключи
 * - string providerName: имя провайдера (опционально)
 * 
 * ЧТО ВОЗВРАЩАЕТ:
 * - IAIProvider: экземпляр AI провайдера
 * 
 * ПРИОРИТЕТ ВЫБОРА:
 * 1. Если указан providerName - использовать его
 * 2. Если нет - использовать AI.DefaultProvider из конфигурации
 * 3. Если провайдер недоступен - fallback на NoAI
 */

using System;
using System.Threading.Tasks;
using MyTgBot2.Core.Interfaces;
using MyTgBot2.Core.Models;

namespace MyTgBot2.AI.Adapters
{
    public class AIProviderFactory
    {
        // TODO для джуна:
        // 1. В CreateProvider() проверить имя провайдера
        // 2. Если "Gemini" - создать GeminiProvider с параметрами из config
        // 3. Если "NoAI" - создать NoAIProvider
        // 4. Если "CustomAI" - создать кастомный провайдер (если реализован)
        // 5. Вызвать IsAvailableAsync() для проверки
        // 6. Если недоступен - fallback на NoAI
        // 7. Вернуть созданный провайдер
        
        private readonly AppSettings _appSettings;
        private readonly Secrets _secrets;
        
        public AIProviderFactory(AppSettings appSettings, Secrets secrets)
        {
            _appSettings = appSettings;
            _secrets = secrets;
        }
        
        /// <summary>
        /// Создать AI провайдер
        /// </summary>
        /// <param name="providerName">Имя провайдера (Gemini, NoAI, CustomAI) или null для использования DefaultProvider</param>
        public async Task<IAIProvider> CreateProviderAsync(string providerName = null)
        {
            // TODO: Реализация
            // 1. Определить имя провайдера (из параметра или конфигурации)
            // 2. Создать экземпляр провайдера
            // 3. Вызвать InitializeAsync()
            // 4. Проверить IsAvailableAsync()
            // 5. Если недоступен - создать NoAIProvider
            // 6. Вернуть результат
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Создать Gemini провайдер с настройками из конфигурации
        /// </summary>
        private GeminiProvider CreateGeminiProvider()
        {
            // TODO: Реализация
            // Создать GeminiProvider используя:
            // - _secrets.GeminiApiKey
            // - _appSettings.AI.Gemini.Model
            // - _appSettings.AI.Gemini.ApiEndpoint
            // - _appSettings.AI.Gemini.Temperature
            // - _appSettings.AI.Gemini.MaxOutputTokens
            // - _appSettings.AI.Gemini.UseFunctionCalling
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Создать NoAI провайдер
        /// </summary>
        private NoAIProvider CreateNoAIProvider()
        {
            return new NoAIProvider();
        }
    }
}
