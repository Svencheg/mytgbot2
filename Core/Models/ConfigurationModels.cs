/*
 * TICKET: Configuration Models
 * 
 * ОПИСАНИЕ:
 * Модели для загрузки конфигурации из appsettings.json и secrets.txt
 * Эти классы маппятся на JSON структуру конфигурационных файлов
 */

using System.Collections.Generic;

namespace MyTgBot2.Core.Models
{
    /// <summary>
    /// Корневая модель конфигурации (appsettings.json)
    /// </summary>
    public class AppSettings
    {
        public ApplicationSettings Application { get; set; }
        public LoggingSettings Logging { get; set; }
        public DatabaseSettings Database { get; set; }
        public AISettings AI { get; set; }
        public KlipperSettings Klipper { get; set; }
        public PCControlSettings PCControl { get; set; }
        public PluginsSettings Plugins { get; set; }
        public SecuritySettings Security { get; set; }
        public BotSettings Bot { get; set; }
    }
    
    public class ApplicationSettings
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Environment { get; set; }
    }
    
    public class LoggingSettings
    {
        public string MinimumLevel { get; set; }
        public bool EnableFileLogging { get; set; }
        public bool EnableConsoleLogging { get; set; }
        public string LogDirectory { get; set; }
        public int MaxLogFileSizeMB { get; set; }
        public int MaxLogFileCount { get; set; }
    }
    
    public class DatabaseSettings
    {
        public string Type { get; set; }
        public string ConnectionString { get; set; }
        public bool EnableAutoMigration { get; set; }
    }
    
    public class AISettings
    {
        public string DefaultProvider { get; set; }
        public bool EnableConversationHistory { get; set; }
        public int MaxHistoryMessages { get; set; }
        public int RequestTimeoutSeconds { get; set; }
        public int MaxRetries { get; set; }
        public GeminiSettings Gemini { get; set; }
        public AlternativeProviderSettings AlternativeProvider { get; set; }
    }
    
    public class GeminiSettings
    {
        public string Model { get; set; }
        public string ApiEndpoint { get; set; }
        public double Temperature { get; set; }
        public int MaxOutputTokens { get; set; }
        public bool UseFunctionCalling { get; set; }
    }
    
    public class AlternativeProviderSettings
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string Endpoint { get; set; }
    }
    
    public class KlipperSettings
    {
        public string MoonrakerUrl { get; set; }
        public string WebcamUrl { get; set; }
        public bool EnableStatusPolling { get; set; }
        public int StatusPollingIntervalSeconds { get; set; }
        public int CommandTimeoutSeconds { get; set; }
    }
    
    public class PCControlSettings
    {
        public bool EnableWakeOnLan { get; set; }
        public int WolPort { get; set; }
        public int SshConnectionTimeoutSeconds { get; set; }
        public bool EnableAutoReconnect { get; set; }
        public int MaxReconnectAttempts { get; set; }
    }
    
    public class PluginsSettings
    {
        public string PluginsDirectory { get; set; }
        public bool EnableDynamicLoading { get; set; }
        public bool AutoLoadOnStartup { get; set; }
        public List<string> EnabledPlugins { get; set; }
    }
    
    public class SecuritySettings
    {
        public bool EnableUserWhitelist { get; set; }
        public bool EnableCommandRateLimit { get; set; }
        public int RateLimitMaxRequests { get; set; }
        public int RateLimitWindowMinutes { get; set; }
    }
    
    public class BotSettings
    {
        public string CommandPrefix { get; set; }
        public bool EnableInlineMode { get; set; }
        public int MessageChunkSize { get; set; }
        public int PhotoCompressionQuality { get; set; }
    }
    
    /// <summary>
    /// Секреты из secrets.txt
    /// </summary>
    public class Secrets
    {
        public string TelegramBotToken { get; set; }
        public string GeminiApiKey { get; set; }
        public List<long> AuthorizedUserIds { get; set; }
        
        // SSH Settings
        public string PcSshHost { get; set; }
        public int PcSshPort { get; set; }
        public string PcSshUsername { get; set; }
        public string PcSshPassword { get; set; }
        public string PcSshKeyPath { get; set; }
        
        // Wake-on-LAN
        public string PcMacAddress { get; set; }
        public string PcBroadcastIp { get; set; }
        
        // Alternative AI
        public string AlternativeAiApiUrl { get; set; }
        public string AlternativeAiApiKey { get; set; }
    }
}
