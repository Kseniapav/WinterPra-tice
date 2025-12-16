using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairRequests
{
    public class OperationResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public object Data { get; }

        private OperationResult(bool isSuccess, string message, object data = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
        }
        public static OperationResult Success(string message, object data = null)
            => new OperationResult(true, message, data);
        public static OperationResult Error(string message)
            => new OperationResult(false, message);
    }
}
