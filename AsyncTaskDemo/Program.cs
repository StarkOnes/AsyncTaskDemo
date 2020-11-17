using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncTaskDemo
{
    class Program
    {
        private static readonly Random RndDelay = new Random();
        private const int MaxRandomValue = 10000;
        private const int TaskDelay = 1000;

        static async Task Main(string[] args)
        {
            var count = 0;
            if (args != null)
            {
                var strCount = args.FirstOrDefault(arg => !string.IsNullOrWhiteSpace(arg) && int.TryParse(arg, out var num) && num > 0);
                if (!string.IsNullOrWhiteSpace(strCount))
                    count = int.Parse(strCount);
            }
            while (count <= 0)
            {
                Console.WriteLine("请输入要测试的并发任务数！");
                Console.WriteLine("直接回车则使用默认值：200");
                var strCount = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(strCount))
                    count = 200;
                else
                    int.TryParse(strCount, out count);
            }
            await RunTask(count);
            Console.ReadKey();
        }

        private static async Task RunTask(int taskCount, int delay = TaskDelay)
        {
            var lstTaskData = new List<TaskInfo>();
            var sw = new Stopwatch();
            sw.Start();
            long t1 = 0, t2 = 0, t3 = 0;
            try
            {
                //1、准备测试数据
                Console.WriteLine("准备并发任务数据！");
                for (var i = 0; i < taskCount; i++)
                    lstTaskData.Add(new TaskInfo(RndDelay.Next(MaxRandomValue), RndDelay.Next(MaxRandomValue)));

                sw.Stop();
                t1 = sw.ElapsedMilliseconds;
                sw.Start();

                //2、生成任务列表
                Console.WriteLine("生成并发任务列表！");
                var lstTask = lstTaskData.Select(item => item.DoWorkAsync(delay)).ToList();

                sw.Stop();
                t2 = sw.ElapsedMilliseconds - t1;
                sw.Start();

                //3、并发执行所有任务，并等待任务完成
                Console.WriteLine("开始并发执行任务！");
                await Task.WhenAll(lstTask); //等待所有任务执行完成（此处任务会并发执行）

                sw.Stop();
                t3 = sw.ElapsedMilliseconds - t2 - t1;
                sw.Start();

                //4、分析所有任务的执行结果
                foreach (var result in lstTask.Select(task => task.Result).ToList())
                    Console.WriteLine($"【线程ID：{result?.WorkThreadId}】\t任务执行“{(result != null && result.Success ? "成功" : "失败")}”：\t{result?.Value1}\t+\t{result?.Value2}\t=\t{result?.Result}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"并发执行任务时发生异常：{e.Message}{Environment.NewLine}{e.StackTrace}");
            }
            finally
            {
                sw.Stop();
                var total = sw.ElapsedMilliseconds;
                var t4 = total - t3 - t2 - t1;
                Console.WriteLine($"并发任务执行完成！{Environment.NewLine}" +
                                  $"当前配置的每个任务延时：{TaskDelay}毫秒！{Environment.NewLine}" +
                                  $"任务数量：{lstTaskData.Count}{Environment.NewLine}" +
                                  $"准备任务数据耗时：{t1 / 1000.0:F8}秒{Environment.NewLine}" +
                                  $"生成任务列表耗时：{t2 / 1000.0:F8}秒{Environment.NewLine}" +
                                  $"执行并发任务耗时：{t3 / 1000.0:F8}秒{Environment.NewLine}" +
                                  $"分析执行结果耗时：{t4 / 1000.0:F8}秒{Environment.NewLine}" +
                                  $"总耗时：{total / 1000.0:F8}秒");
            }
        }
    }
}