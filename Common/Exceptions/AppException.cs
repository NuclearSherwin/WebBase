using System;
using Common.Constants;

namespace Common.Exceptions
{
    public class AppException : Exception
    {
        public StringEnums.AppErrorCode ErrorCode { get; set; }

        public AppException(string message, StringEnums.AppErrorCode errorCode = StringEnums.AppErrorCode.Error) :
            base(message)
        {
            ErrorCode = errorCode;
        }

        public AppException(string message, Exception ex,
            StringEnums.AppErrorCode errorCode = StringEnums.AppErrorCode.Error) : base(message, ex)
        {
            ErrorCode = errorCode;
        }
    }
}