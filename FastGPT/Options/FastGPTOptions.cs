namespace FastGPT.Options
{
    public sealed class FastGPTOptions
    {
        /// <summary>
        /// 不同应用的api key
        /// </summary>
        public required Dictionary<string, AppInfo> AppApiKeys { get; set; }

        /// <summary>
        /// 全局api key
        /// </summary>
        public string? GlobalApiKey { get; set; }

        /// <summary>
        /// fastGPT服务器地址
        /// </summary>
        public string? Host { get; set; }

        /// <summary>
        /// 应用信息
        /// </summary>
        public sealed class AppInfo
        {
            /// <summary>
            /// 应用ID
            /// </summary>
            public required string AppId { get; set; }
            /// <summary>
            /// API Key
            /// </summary>
            public required string ApiKey { get; set; }
        }
    }
}
