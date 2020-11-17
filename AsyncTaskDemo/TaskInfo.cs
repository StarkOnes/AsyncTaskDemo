using System.Threading;
using System.Threading.Tasks;

namespace AsyncTaskDemo
{
    public class TaskInfo
    {
        public int Value1 { get; }

        public int Value2 { get; }

        public int Result { get; private set; }

        public bool Success { get; private set; }

        public int WorkThreadId { get; private set; }

        public TaskInfo(int value1, int value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        internal async Task<TaskInfo> DoWorkAsync(int delay)
        {
            //注意：如果多个异步任务可能同时操作同一个共享资源，则需要做好同步，线程同步的操作本示例暂不列举
            await Task.Run(async () =>
            {
                WorkThreadId = Thread.CurrentThread.ManagedThreadId;
                Result = Value1 + Value2;
                Success = true;

                await Task.Delay(delay);
            });
            return this;
        }
    }
}
