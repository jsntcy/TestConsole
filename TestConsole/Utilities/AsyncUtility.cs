namespace TestConsole.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class AsyncAwaitDemo
    {
        int i = 0;

        public static Task CreateFile(string s)
        {
            return Task.Run(() => File.WriteAllText(@"C:\TestFiles\" + s + ".txt", s));
        }

        public static async Task ResolveCoreAsync(List<string> list)
        {
            await list.ForEachInParallelAsync(CreateFile);
        }

        public static Task ResolveAsync(List<string> list)
        {
            return ResolveCoreAsync(list);
        }

        private static async Task<int> RunAsync()
        {
            List<string> list = new List<string>
            {
                "1",
                "2",
                "3"
            };
            await ResolveAsync(list);
            return 1;
        }

        public async Task DoStuff()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(5000);
            });
            Console.WriteLine("sleep done: {0}", i++);
        }

        private static Task<string> LongRunningOperation()
        {
            int counter;

            for (counter = 0; counter < 50000; counter++)
            {
                //Console.WriteLine(counter);
            }

            return Task.FromResult("Counter = " + counter);
        }

        public async Task Loop()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("before await {0}", i);
                await DoStuff();
                Console.WriteLine("after await {0}", i);
            }
            Console.WriteLine("end of loop");
        }

        static async Task DownloadPageAsync()
        {
            // ... Target page.
            string page = "http://www.baidu.com/";

            // ... Use HttpClient.
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();

                // ... Display the result.
                if (result != null &&
                result.Length >= 50)
                {
                    Console.WriteLine(result.Substring(0, 50) + "...");
                }
            }
        }

        private static async Task<DateTime> CountToAsync(int num = 10)
        {
            for (int i = 0; i < num; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                Console.WriteLine("test");
            }

            return DateTime.Now;
        }


        public static async Task<IReadOnlyList<string>> handleasync(string s)
        {
            var httpClient = new HttpClient();

            var result = await httpClient.GetAsync("http://stackoverflow.com");

            Thread.Sleep(10000);

            return new List<string> { s };
        }

        private static async Task<bool> foo()
        {
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("Before await: {0}", i);
                var time = await CountToAsync();
                Console.WriteLine("After await: {0}", time);
            }

            return true;
        }

        public static async Task<IReadOnlyList<string>> Get()
        {
            List<string> ls = new List<string>();
            List<string> first = new List<string> { "1" };
            List<string> second = new List<string> { "1" };

            var diff = second.Except(first);
            //ls.Add("abc");
            //ls.Add("efg");
            //ls.Add("hij");
            var subMessagesToDispatch =
               await diff.SelectInParallelAsync(
                   async handler =>
                   {
                       return await handleasync(handler);
                   });

            int i = 8;
            i++;
            return null;
        }

        private static StringBuilder sb = new StringBuilder();
        private static async void StartButton_Click()
        {
            // ExampleMethodAsync returns a Task<int>, which means that the method
            // eventually produces an int result. However, ExampleMethodAsync returns
            // the Task<int> value as soon as it reaches an await.
            sb.Append("\n");
            try
            {
                await ExampleMethodAsync();
                // Note that you could put "await ExampleMethodAsync()" in the next line where
                // "length" is, but due to when '+=' fetches the value of ResultsTextBox, you
                // would not see the global side effect of ExampleMethodAsync setting the text.
                sb.Append(String.Format("Length: {0}\n", 0));
            }
            catch (Exception)
            {
                // Process the exception if one occurs.
            }
        }

        public static async Task ExampleMethodAsync()
        {
            Console.WriteLine("start get string");
            var httpClient = new HttpClient();
            await DownloadPageAsync();
            Console.WriteLine("end get string");
            sb.Append("Preparing to finish ExampleMethodAsync.\n");
            // After the following return statement, any method that's awaiting
            // ExampleMethodAsync (in this case, StartButton_Click) can get the 
            // integer result.
        }

        private static Task<JO1> fff(List<JO1> list)
        {
            return Task.Run(() => { List<JO1> own = list; Thread.Sleep(100); return new JO1(); });
        }

        private static async Task<JO1> SetupAsyncWithPerfScope(Task<JO1> setupTask)
        {
            return await setupTask;
        }

        private static Task<int> ff()
        {
            throw new Exception("ff's exception");
        }

        private static async Task<int> Cff()
        {
            try
            {
                return await ff();
            }
            catch (Exception ex)
            {
                Console.WriteLine("cff catch ff's exception; call stack: " + ex.ToString());
                return 0;
            }
        }

        public static async Task WhenAllWithExceptions(IEnumerable<Task> tasks)
        {
            Task whenAllTask = null;
            try
            {
                whenAllTask = Task.WhenAll(tasks);
                await whenAllTask;
            }
            catch (Exception ex)
            {
                throw whenAllTask.Exception;
            }
            finally
            {
                Console.WriteLine("come to finally.");
            }
        }

        private static async Task TaskThrowException()
        {
            Console.WriteLine("throw exception from TaskThrowException");
            throw new Exception("exception from TaskThrowException");
        }

        public static Task TaskThrowException2()
        {
            return Task.Run(() =>
            {
                Thread.Sleep(5000);
                Console.WriteLine("throw exception from TaskThrowException2");
            });
            //throw new Exception("exception from TaskThrowException2");
        }

        private static Task NormalTask()
        {
            return Task.Run(() => { Thread.Sleep(100); Console.WriteLine("Normal task"); });
        }

        public static async Task TestWhenAllWithExceptions()
        {
            var tasks = new List<Task>();
            tasks.Add(TaskThrowException2());
            tasks.Add(TaskThrowException());

            Console.WriteLine("before Test");
            try
            {
                await WhenAllWithExceptions(tasks);
            }
            catch (AggregateException ae)
            {
                var ex = ae.InnerExceptions[0];
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.GetType().ToString());
                Console.WriteLine(ex.ToString());
                throw;
            }
            finally
            {
                Console.WriteLine("finally in test");
            }
            Console.WriteLine("after Test");
        }

        // output:
        // sync1 start
        // sync1 end
        // sync2 start
        // sync2 end
        public static Task TestSyncWhenAll() // run synchonously since "sync1, sync2" are synchrous actually.
        {
            var allTasks = new List<Task> { Sync1(), Sync2() };
            return Task.WhenAll(allTasks);
        }

        // output:
        // Wrapsync1 start
        // Wrapsync2 start
        // Wrapsync1 end
        // Wrapsync2 end
        public static Task TestAsyncWhenAll() // run asynchonously since "sync1, sync2" are asynchrous actually.
        {
            var allTasks = new List<Task> { WrapSync1(), WrapSync2() };
            return Task.WhenAll(allTasks);
        }

        private static Task WrapSync1()
        {
            // using "Run" to wrap synchronus code
            return Task.Run(
                () =>
                {
                    Console.WriteLine("WrapSync1 start");
                    Thread.Sleep(100);
                    Console.WriteLine("WrapSync1 end");
                });
        }

        private static Task WrapSync2()
        {
            return Task.Run(
                () =>
                {
                    Console.WriteLine("WrapSync2 start");
                    Thread.Sleep(100);
                    Console.WriteLine("WrapSync2 end");
                });
        }

        private static Task Sync1()
        {
            Console.WriteLine("Sync1 start");
            Thread.Sleep(100);
            Console.WriteLine("Sync1 end");
            return Task.FromResult(1);
        }

        private static Task Sync2()
        {
            Console.WriteLine("Sync2 start");
            Thread.Sleep(100);
            Console.WriteLine("Sync2 end");
            return Task.FromResult(1);
        }
    }
}
