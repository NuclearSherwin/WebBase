using Common.Constants;

namespace Data.ViewModels
{
    public class AppErrorModel
    {
        public string Message { get; set; }
        public string Detail { get; set; }

        public StringEnums.AppErrorCode ErrorCode { get; set; }

        public AppErrorModel()
        {
        }

        public AppErrorModel(string message, StringEnums.AppErrorCode errorCode = StringEnums.AppErrorCode.Error)
        {
            Message = message;
            ErrorCode = errorCode;
        }

        public AppErrorModel(string message, string detail, StringEnums.AppErrorCode errorCode = StringEnums.AppErrorCode.Error) : this(message)
        {
            Detail = detail;
            ErrorCode = errorCode;
        }
    }
}