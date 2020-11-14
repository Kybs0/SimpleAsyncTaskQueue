using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TransYeekit.Utils
{
    public class SimpleAsyncTaskQueue : IAsyncTaskQueue
    {
        #region 执行

        /// <summary>
        /// 执行异步操作
        /// </summary>
        /// <typeparam name="T">返回结果类型</typeparam>
        /// <param name="func">异步操作</param>
        /// <returns>IsValid:异步操作是否有效；result:异步操作结果</returns>
        public async Task<AsyncTaskExecuteResult<T>> ExecuteAsync<T>(Func<Task<T>> func)
        {
            var task = GetExecutableTask(func);
            if (UseSingleThread)
            {
                task.RunSynchronously();
            }
            else
            {
                task.Start();
            }
            var result = await await task;
            var isValid = _lastTask != null && !string.IsNullOrEmpty(_lastTask.Id) && _lastTask.Id == task.Id;
            if (!isValid)
            {
                result = default(T);
            }
            return new AsyncTaskExecuteResult<T>(isValid, result);
        }

        /// <summary>
        /// 执行异步操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<bool> ExecuteAsync<T>(Func<Task> func)
        {
            var task = GetExecutableTask(func);
            if (UseSingleThread)
            {
                task.RunSynchronously();
            }
            else
            {
                task.Start();
            }
            await await task;
            var isValid = _lastTask != null && !string.IsNullOrEmpty(_lastTask.Id) && _lastTask.Id == task.Id;
            return isValid;
        }

        #endregion

        #region 添加任务

        /// <summary>
        /// 获取待执行任务
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private AwaitableTask GetExecutableTask(Action action)
        {
            var awaitableTask = new AwaitableTask(new Task(action), Guid.NewGuid().ToString());
            SetLastTask(awaitableTask);
            return awaitableTask;
        }

        /// <summary>
        /// 获取待执行任务
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        private AwaitableTask<TResult> GetExecutableTask<TResult>(Func<TResult> function)
        {
            var awaitableTask = new AwaitableTask<TResult>(new Task<TResult>(function), Guid.NewGuid().ToString());
            SetLastTask(awaitableTask);
            return awaitableTask;
        }

        /// <summary>
        /// 设置最后一个task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private void SetLastTask(AwaitableTask task)
        {
            //添加队列，加锁。
            lock (_lockObject)
            {
                _lastTask = task;
            }
        }

        #endregion

        #region 属性及字段

        private readonly object _lockObject = new object();
        private AwaitableTask _lastTask = null;
        /// <summary>
        /// 是否使用单线程完成任务.
        /// </summary>
        public bool UseSingleThread { get; set; } = true;

        #endregion

    }
    public interface IAsyncTaskQueue
    {
        Task<bool> ExecuteAsync<T>(Func<Task> func);
        Task<AsyncTaskExecuteResult<T>> ExecuteAsync<T>(Func<Task<T>> func);
    }
}
