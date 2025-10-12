namespace Rira.Application.Common
{
    /// <summary>
    /// 🔹 مدل استاندارد پاسخ API برای تمامی اکشن‌ها
    /// </summary>
    public class ResponseModel<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public ResponseModel() { }

        public ResponseModel(bool success, string message, T? data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static ResponseModel<T> Ok(string message, T data)
            => new ResponseModel<T>(true, message, data);

        public static ResponseModel<T> Fail(string message)
            => new ResponseModel<T>(false, message, default);
    }
}
