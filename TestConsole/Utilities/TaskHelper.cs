namespace TestConsole.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public static class TaskHelper
    {
        /// <summary>
        /// WithFinally
        /// </summary>
        /// <param name="tryCode">the task contains try part</param>
        /// <param name="finallyCode">the task contains finally part</param>
        /// <returns>the task included try part and finally part</returns>
        public static async Task WithFinally(this Task tryCode, Func<Task, Task> finallyCode)
        {
            await await tryCode.ContinueWith(finallyCode);
            await tryCode;
        }

        /// <summary>
        /// WithFinally
        /// </summary>
        /// <typeparam name="TResult">TResult</typeparam>
        /// <param name="tryCode">the task contains try part</param>
        /// <param name="finallyCode">the task contains finally part</param>
        /// <returns>the task included try part and finally part</returns>
        public static async Task<TResult> WithFinally<TResult>(this Task<TResult> tryCode, Func<Task<TResult>, Task> finallyCode)
        {
            await await tryCode.ContinueWith(finallyCode);
            return await tryCode;
        }

        /// <summary>
        /// Task.WhenAll, and re-throw AggregateException containing exceptions from all failed tasks
        /// </summary>
        /// <typeparam name="TResult">task result type</typeparam>
        /// <param name="tasks">the list of tasks</param>
        /// <returns>array of task result</returns>
        /// <exception>AggregationException of all failed tasks</exception>
        public static async Task<TResult[]> WhenAllAndThrowAggregateExceptionOnErrorAsync<TResult>(IEnumerable<Task<TResult>> tasks)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException("tasks");
            }

            Task<TResult[]> whenAllTask = null;
            try
            {
                whenAllTask = Task.WhenAll(tasks);
                return await whenAllTask;
            }
            catch
            {
                throw whenAllTask.Exception;
            }
        }

        /// <summary>
        /// Provide parallel version for ForEach
        /// </summary>
        /// <typeparam name="T">The type for the enumerable</typeparam>
        /// <param name="source">The enumerable to control the foreach loop</param>
        /// <param name="body">The task body</param>
        /// <param name="maxParallelism">The max parallelism allowed</param>
        /// <returns>The task</returns>
        public static async Task ForEachInParallelAsync<T>(this IEnumerable<T> source, Func<T, Task> body, int maxParallelism)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }

            using (var semaphore = new SemaphoreSlim(maxParallelism))
            {
                // warning "access to disposed closure" around "semaphore" could be ignored as it is inside Task.WhenAll
                await Task.WhenAll(from s in source select ForEachCoreAsync(body, semaphore, s));
            }
        }

        private static async Task ForEachCoreAsync<T>(Func<T, Task> body, SemaphoreSlim semaphore, T s)
        {
            await semaphore.WaitAsync();
            try
            {
                await body(s);
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// Provide parallel version for ForEach
        /// </summary>
        /// <typeparam name="T">The type for the enumerable</typeparam>
        /// <param name="source">The enumerable to control the foreach loop</param>
        /// <param name="body">The task body</param>
        /// <returns>The task</returns>
        /// <remarks>The max parallelism is 64.</remarks>
        public static Task ForEachInParallelAsync<T>(this IEnumerable<T> source, Func<T, Task> body)
        {
            return ForEachInParallelAsync(source, body, 64);
        }

        /// <summary>
        /// Provide parallel version for Select that each element will map to a result
        /// </summary>
        /// <typeparam name="TSource">The type for the enumerable</typeparam>
        /// <typeparam name="TResult">The type for the result</typeparam>
        /// <param name="source">The enumerable to control the select</param>
        /// <param name="body">The select body</param>
        /// <param name="maxParallelism">The max parallelism allowed</param>
        /// <returns>The task</returns>
        public static async Task<IReadOnlyList<TResult>> SelectInParallelAsync<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<TResult>> body, int maxParallelism)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }

            using (var semaphore = new SemaphoreSlim(maxParallelism))
            {
                // warning "access to disposed closure" around "semaphore" could be ignored as it is inside Task.WhenAll
                return await Task.WhenAll(from s in source select SelectCoreAsync(body, semaphore, s));
            }
        }

        private static async Task<TResult> SelectCoreAsync<TSource, TResult>(Func<TSource, Task<TResult>> body, SemaphoreSlim semaphore, TSource s)
        {
            await semaphore.WaitAsync();
            try
            {
                return await body(s);
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// Provide parallel version for Select that each element will map to a result
        /// </summary>
        /// <typeparam name="TSource">The type for the enumerable</typeparam>
        /// <typeparam name="TResult">The type for the result</typeparam>
        /// <param name="source">The enumerable to control the select</param>
        /// <param name="body">The select body</param>
        /// <returns>The task</returns>
        /// <remarks>The max parallelism is 64.</remarks>
        public static Task<IReadOnlyList<TResult>> SelectInParallelAsync<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<TResult>> body)
        {
            return SelectInParallelAsync(source, body, 64);
        }

        /// <summary>
        /// A completed task
        /// </summary>
        public static readonly Task Completed = Task.FromResult(1);

        public static async Task<T> FirstOrDefaultAsync<T>(this IEnumerable<T> source, Func<T, Task<bool>> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            foreach (var item in source)
            {
                if (await predicate(item))
                {
                    return item;
                }
            }
            return default(T);
        }

        public static async Task<T> FirstAsync<T>(this IEnumerable<T> source, Func<T, Task<bool>> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            foreach (var item in source)
            {
                if (await predicate(item))
                {
                    return item;
                }
            }
            throw new InvalidOperationException("Sequence contains no matching element.");
        }

        public static async Task<IEnumerable<T>> WhereAsync<T>(this IReadOnlyList<T> source, Func<T, Task<bool>> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            var conditions = await source.SelectInParallelAsync(predicate);
            return source.Where((x, i) => conditions[i]);
        }

        public static Task<IEnumerable<T>> WhereAsync<T>(this IEnumerable<T> source, Func<T, Task<bool>> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            return WhereAsync(source.ToList(), predicate);
        }

        public static async Task<Dictionary<TKey, TValue>> ToDictionaryAsync<T, TKey, TValue>(this IReadOnlyList<T> source, Func<T, TKey> keySelector, Func<T, Task<TValue>> valueSelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            if (valueSelector == null)
            {
                throw new ArgumentNullException("valueSelector");
            }
            var values = await source.SelectInParallelAsync(valueSelector);
            var result = new Dictionary<TKey, TValue>();
            for (int i = 0; i < source.Count; i++)
            {
                result.Add(keySelector(source[i]), values[i]);
            }
            return result;
        }

        public static Task<Dictionary<TKey, TValue>> ToDictionaryAsync<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, Task<TValue>> valueSelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            if (valueSelector == null)
            {
                throw new ArgumentNullException("valueSelector");
            }
            return ToDictionaryAsync(source.ToList(), keySelector, valueSelector);
        }

        public static async Task<bool> RunTaskWithCancellationTokenAsync(Task task, CancellationToken cancellationToken, string taskDescription = null)
        {
            Guard.ArgumentNotNull(task, nameof(task));

            try
            {
                int returnedTask = Task.WaitAny(new[] { task }, cancellationToken);
                if (returnedTask == 0)
                {
                    await task;
                    return true;
                }
            }
            catch (OperationCanceledException)
            {
            }

            RunTaskSilentlyNoWait(task, taskDescription);
            return false;
        }

        public static async Task<bool> RunTaskWithTimeoutAsync(Task task, TimeSpan timeout, string taskDescription = null)
        {
            Guard.ArgumentNotNull(task, nameof(task));
            Guard.Argument(() => timeout >= TimeSpan.Zero, nameof(timeout), $"{nameof(timeout)} cannot be a negative time interval.");

            var returnedTask = await Task.WhenAny(Task.Delay(timeout), task);

            if (returnedTask == task)
            {
                await task;
                return true;
            }
            else
            {
                RunTaskSilentlyNoWait(task, taskDescription);
                return false;
            }
        }

        public static async void RunTaskSilentlyNoWait(Task task, string taskDescription = null)
        {
            Guard.ArgumentNotNull(task, nameof(task));

            try
            {
                Console.WriteLine(task.Status + ": RunTaskSilentlyNoWait");
                await task;
                Console.WriteLine(task.Status + ": RunTaskSilentlyNoWait");
            }
            catch (Exception ex)
            {
                TraceEx.TraceError(taskDescription != null
                    ? $"{taskDescription} failed: {ex}"
                    : $"Running task failed: {ex}");
            }
        }

        // The same behavior as "RunTaskSilentlyNoWait".
        // but it is bad because it returns Task,
        // and the caller has chance to wait "RunTaskSilentlyNoWaitBad" to complete.
        public static async Task RunTaskSilentlyNoWaitBad(Task task, string taskDescription = null)
        {
            Guard.ArgumentNotNull(task, nameof(task));

            try
            {
                Console.WriteLine(task.Status + ": RunTaskSilentlyNoWait1");
                await task;
                Console.WriteLine(task.Status + " : RunTaskSilentlyNoWait1");
            }
            catch (Exception ex)
            {
                TraceEx.TraceError(taskDescription != null
                    ? $"{taskDescription} failed: {ex}"
                    : $"Running task failed: {ex}");
            }
        }

        // The same as RunTaskSilentlyNoWait.
        public static async void RunTaskSilentlyNoWaitUsingFunc(Func<Task> task, string taskDescription = null)
        {
            Guard.ArgumentNotNull(task, nameof(task));

            try
            {
                Task t = task();
                Console.WriteLine(t.Status);
                await t;
                Console.WriteLine(t.Status);
            }
            catch (Exception ex)
            {
                TraceEx.TraceError(taskDescription != null
                    ? $"{taskDescription} failed: {ex}"
                    : $"Running task failed: {ex}");
            }
        }

        public static async Task WhenAll(this IEnumerable<Task> tasks, int maxParallelism)
        {
            var workers = new Task[maxParallelism];
            for (int i = 0; i < workers.Length; i++)
            {
                workers[i] = Task.CompletedTask;
            }
            foreach (var task in tasks)
            {
                var completed = await Task.WhenAny(workers);
                await completed;
                var index = Array.IndexOf(workers, completed);
                workers[index] = task;
            }
            await Task.WhenAll(tasks);
        }
    }
}
