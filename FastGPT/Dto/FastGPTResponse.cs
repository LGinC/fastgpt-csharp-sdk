namespace FastGPT.Dto
{
    public class FastGPTResponse
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 状态文本
        /// </summary>
        public string? StatusText { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string? Message { get; set; }
    }

    /// <summary>
    /// 基础响应
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class FastGPTResponse<T> : FastGPTResponse
    {
        
        /// <summary>
        /// 数据
        /// </summary>
        public T? Data { get; set; }
    }

    /// <summary>
    /// 列表响应
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FastGPTListResponse<T>
    {
        /// <summary>
        /// 列表数据
        /// </summary>
        public List<T> List { get; set; } = [];

        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }
    }
}
