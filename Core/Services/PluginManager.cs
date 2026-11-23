/*
 * TICKET: Plugin Manager Service
 * 
 * ОПИСАНИЕ:
 * Менеджер плагинов - отвечает за загрузку, управление и выполнение плагинов.
 * Поддерживает динамическую загрузку DLL плагинов из папки Plugins.
 * 
 * НАЗНАЧЕНИЕ:
 * - Загрузка плагинов при старте или по требованию
 * - Динамическая загрузка DLL файлов (reflection)
 * - Регистрация доступных команд от всех плагинов
 * - Маршрутизация ActionRequest к нужному плагину
 * 
 * ЧТО ПРИНИМАЕТ:
 * - string pluginsDirectory: путь к папке с плагинами
 * - ActionRequest: запрос на выполнение действия
 * 
 * ЧТО ВОЗВРАЩАЕТ:
 * - List<IPlugin>: список загруженных плагинов
 * - List<FunctionDefinition>: все доступные команды от всех плагинов
 * - ActionResult: результат выполнения действия
 * 
 * АРХИТЕКТУРА ЗАГРУЗКИ:
 * 1. Сканирование папки Plugins/*.dll
 * 2. Поиск классов, реализующих IPlugin через Reflection
 * 3. Создание экземпляров и вызов InitializeAsync()
 * 4. Регистрация в словаре _loadedPlugins
 * 
 * ПРИМЕР ИСПОЛЬЗОВАНИЯ:
 * var functions = pluginManager.GetAllAvailableFunctions();
 * var result = await pluginManager.ExecuteActionAsync(actionRequest);
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MyTgBot2.Core.Interfaces;
using MyTgBot2.Core.Models;

namespace MyTgBot2.Core.Services
{
    public class PluginManager
    {
        // TODO для джуна:
        // 1. Загрузить все DLL файлы из папки Plugins
        // 2. Использовать Assembly.LoadFrom() для загрузки DLL
        // 3. Найти все типы, реализующие IPlugin: assembly.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t))
        // 4. Создать экземпляры плагинов через Activator.CreateInstance()
        // 5. Вызвать InitializeAsync() для каждого плагина
        // 6. Сохранить в _loadedPlugins
        
        private readonly Dictionary<string, IPlugin> _loadedPlugins = new Dictionary<string, IPlugin>();
        private readonly string _pluginsDirectory;
        
        public PluginManager(string pluginsDirectory)
        {
            _pluginsDirectory = pluginsDirectory;
        }
        
        /// <summary>
        /// Загрузить все плагины из папки
        /// </summary>
        public async Task LoadPluginsAsync(CancellationToken cancellationToken = default)
        {
            // TODO: Реализация загрузки плагинов
            // 1. Получить список .dll файлов из _pluginsDirectory
            // 2. Загрузить каждую DLL через Assembly.LoadFrom()
            // 3. Найти классы, реализующие IPlugin
            // 4. Создать экземпляры и инициализировать
            // 5. Добавить в _loadedPlugins
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Получить все доступные функции от всех плагинов
        /// Используется для передачи AI списка команд
        /// </summary>
        public List<FunctionDefinition> GetAllAvailableFunctions()
        {
            // TODO: Реализация
            // 1. Пройти по всем плагинам в _loadedPlugins
            // 2. Вызвать GetAvailableFunctions() для каждого
            // 3. Собрать все в один список
            // 4. Вернуть результат
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Выполнить действие через плагин
        /// </summary>
        public async Task<ActionResult> ExecuteActionAsync(ActionRequest request, CancellationToken cancellationToken = default)
        {
            // TODO: Реализация
            // 1. Найти плагин по request.PluginName в _loadedPlugins
            // 2. Если плагин не найден, вернуть ActionResult с ошибкой
            // 3. Вызвать plugin.ExecuteActionAsync(request, cancellationToken)
            // 4. Вернуть результат
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Получить плагин по имени
        /// </summary>
        public IPlugin GetPlugin(string pluginName)
        {
            // TODO: Реализация
            // Вернуть плагин из _loadedPlugins или null
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Получить список всех загруженных плагинов
        /// </summary>
        public List<IPlugin> GetAllPlugins()
        {
            // TODO: Реализация
            // Вернуть список всех плагинов из _loadedPlugins.Values
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Выгрузить все плагины
        /// </summary>
        public async Task UnloadAllPluginsAsync()
        {
            // TODO: Реализация
            // 1. Пройти по всем плагинам
            // 2. Вызвать DisposeAsync() для каждого
            // 3. Очистить _loadedPlugins
            throw new NotImplementedException();
        }
    }
}
