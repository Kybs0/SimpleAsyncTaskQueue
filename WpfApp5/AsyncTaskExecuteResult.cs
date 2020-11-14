using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp5
{
    public class AsyncTaskExecuteResult<T>
    {
        public AsyncTaskExecuteResult(bool isValid, T result)
        {
            IsValid = isValid;
            Result = result;
        }
        /// <summary>
        /// 异步操作是否有效(多任务时，如果设置了"AutoCancelPreviousTask",只会保留最后一个任务有效)
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// 异步操作结果
        /// </summary>
        public T Result { get; set; }
    }
}
