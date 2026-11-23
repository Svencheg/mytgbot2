/*
 * TICKET: Gemini AI Provider
 * 
 * ОПИСАНИЕ:
 * Реализация IAIProvider для работы с Gemini API (Google AI).
 * Поддерживает оба режима: simple JSON response и function calling.
 * 
 * НАЗНАЧЕНИЕ:
 * - Отправка запросов к Gemini API
 * - Обработка ответов AI (текст + команды)
 * - Поддержка истории диалогов для контекста
 * - Парсинг function calls из Gemini response
 * 
 * ЧТО ПРИНИМАЕТ:
 * - string userMessage: запрос пользователя
 * - List<Message> history: история диалога
 * - List<FunctionDefinition> functions: доступные команды
 * 
 * ЧТО ВОЗВРАЩАЕТ:
 * - AIResponse: текст для пользователя + список ActionRequest
 * 
 * API GEMINI:
 * - Endpoint: https://generativelanguage.googleapis.com/v1beta/models/gemini-3.0-flash:generateContent
 * - Headers: { "x-goog-api-key": "YOUR_KEY", "Content-Type": "application/json" }
 * - Body: { contents: [...messages], tools: [...functions] }
 * 
 * ВАЖНО:
 * - Использовать HttpClient для запросов
 * - Обрабатывать таймауты и ошибки сети
 * - Парсить JSON ответ от Gemini
 * - Поддерживать retry при ошибках
 */

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MyTgBot2.Core.Interfaces;
using MyTgBot2.Core.Models;

namespace MyTgBot2.AI.Providers
{
    public class GeminiProvider : IAIProvider
    {
        // TODO для джуна:
        // 1. Инициализировать HttpClient
        // 2. В ProcessMessageAsync():
        //    - Собрать массив messages для Gemini (из history + новое сообщение)
        //    - Преобразовать FunctionDefinitions в формат Gemini tools
        //    - Отправить POST запрос к API
        //    - Распарсить ответ JSON
        //    - Извлечь текст ответа и function calls (если есть)
        //    - Преобразовать function calls в ActionRequest
        //    - Вернуть AIResponse
        // 3. Обработать ошибки (timeout, network, API errors)
        
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly string _apiEndpoint;
        private readonly double _temperature;
        private readonly int _maxOutputTokens;
        private readonly bool _useFunctionCalling;
        
        public string ProviderName => "Gemini";
        
        public GeminiProvider(
            string apiKey,
            string model = "gemini-3.0-flash",
            string apiEndpoint = "https://generativelanguage.googleapis.com/v1beta",
            double temperature = 0.7,
            int maxOutputTokens = 2048,
            bool useFunctionCalling = true)
        {
            _apiKey = apiKey;
            _model = model;
            _apiEndpoint = apiEndpoint;
            _temperature = temperature;
            _maxOutputTokens = maxOutputTokens;
            _useFunctionCalling = useFunctionCalling;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("x-goog-api-key", _apiKey);
        }
        
        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
        {
            // TODO: Реализация
            // Проверить что API key не пустой
            // Опционально: отправить тестовый запрос к API
            throw new NotImplementedException();
        }
        
        public async Task<AIResponse> ProcessMessageAsync(
            string userMessage,
            List<Message> conversationHistory,
            List<FunctionDefinition> availableFunctions,
            CancellationToken cancellationToken = default)
        {
            // TODO: Реализация
            // 1. Подготовить messages для Gemini API
            //    Формат: { role: "user"/"model", parts: [{ text: "..." }] }
            // 2. Подготовить tools (если _useFunctionCalling = true)
            //    Формат Gemini: { functionDeclarations: [...] }
            // 3. Создать request body:
            //    {
            //      contents: [...messages],
            //      tools: [...],
            //      generationConfig: { temperature, maxOutputTokens }
            //    }
            // 4. Отправить POST к {_apiEndpoint}/models/{_model}:generateContent
            // 5. Распарсить response:
            //    response.candidates[0].content.parts[] может содержать:
            //    - { text: "..." } - текстовый ответ
            //    - { functionCall: { name: "...", args: {...} } } - вызов функции
            // 6. Собрать AIResponse
            // 7. Обработать ошибки
            throw new NotImplementedException();
        }
        
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            // TODO: Реализация
            // Можно сделать тестовый запрос к API для проверки
            throw new NotImplementedException();
        }
        
        public async Task DisposeAsync()
        {
            // TODO: Реализация
            // Закрыть HttpClient
            _httpClient?.Dispose();
        }
        
        /// <summary>
        /// Преобразовать FunctionDefinition в формат Gemini
        /// </summary>
        private object ConvertToGeminiFunctionDeclaration(FunctionDefinition func)
        {
            // TODO: Реализация
            // Формат Gemini:
            // {
            //   name: "function_name",
            //   description: "description",
            //   parameters: {
            //     type: "object",
            //     properties: {
            //       param1: { type: "string", description: "..." },
            //       param2: { type: "integer", description: "..." }
            //     },
            //     required: ["param1"]
            //   }
            // }
            throw new NotImplementedException();
        }
    }
}
