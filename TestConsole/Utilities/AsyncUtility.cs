namespace TestConsole.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class AsyncAwaitDemo
    {
        int i = 0;
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
                int length = await ExampleMethodAsync();
                // Note that you could put "await ExampleMethodAsync()" in the next line where
                // "length" is, but due to when '+=' fetches the value of ResultsTextBox, you
                // would not see the global side effect of ExampleMethodAsync setting the text.
                sb.Append(String.Format("Length: {0}\n", length));
            }
            catch (Exception)
            {
                // Process the exception if one occurs.
            }
        }

        public static async Task<int> ExampleMethodAsync()
        {
            var httpClient = new HttpClient();
            int exampleInt = (await httpClient.GetStringAsync("http://msdn.microsoft.com")).Length;
            sb.Append("Preparing to finish ExampleMethodAsync.\n");
            // After the following return statement, any method that's awaiting
            // ExampleMethodAsync (in this case, StartButton_Click) can get the 
            // integer result.
            return exampleInt;
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

    }
}
