namespace TestConsole.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public static class PerformanceScopeUtility
    {
        public static async Task RunTaskWithPerfScopeAsync(PerformanceScope perfScope, Task task)
        {
            Guard.ArgumentNotNull(perfScope, nameof(perfScope));
            Guard.ArgumentNotNull(task, nameof(task));

            using (perfScope)
            {
                await task;
                Console.WriteLine("runtaskwithperfscopeasync " + perfScope.ElapsedTime.TotalSeconds);
            }
        }

        public static async Task PublishToDocumentHostingServiceAsync()
        {
            var perfScope = new PerformanceScope("PublishToDocumentHostingServiceAsync");
            await RunTaskWithPerfScopeAsync(perfScope, Task.Delay(5000));
            Console.WriteLine("publishtodhs " + perfScope.ElapsedTime.TotalSeconds);
        }

        public static Task PublishToDocumentHostingServiceWithLog()
        {
            return PublishToDocumentHostingServiceAsync();
        }

        public static Task TestNoWait()
        {
            TaskHelper.RunTaskSilentlyNoWaitUsingFunc(PublishToDocumentHostingServiceAsync);
            TaskHelper.RunTaskSilentlyNoWait(PublishToDocumentHostingServiceAsync());
            return PublishToDocumentHostingServiceWithLog();
        }
    }
}
