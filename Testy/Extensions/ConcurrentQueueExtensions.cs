using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
// ReSharper disable UnusedMember.Global

namespace Testy.Extensions
{
    /// <summary>
    /// Extensions for <see cref="ConcurrentQueue{T}"/>
    /// </summary>
    public static class ConcurrentQueueExtensions
    {
        /// <summary>
        /// Enqueues the <paramref name="items"/> sequence
        /// </summary>
        public static void EnqueueRange<TItem>(this ConcurrentQueue<TItem> queue, IEnumerable<TItem> items)
        {
            if (queue == null) throw new ArgumentNullException(nameof(queue));
            if (items == null) throw new ArgumentNullException(nameof(items));
            
            foreach (var item in items)
            {
                queue.Enqueue(item);
            }
        }

        /// <summary>
        /// Waits for the completion expression <paramref name="completionExpression"/> to become true, defaulting to a 5 s timeout.
        /// The timeout can be specified by passing in another value for <paramref name="timeoutSeconds"/>.
        /// Optionally, a failure expression specified by <paramref name="failExpression"/> can cause an <see cref="AssertionException"/>
        /// to be thrown if it becomes true while waiting.
        /// If additional details are desired in the thrown exceptions, the <paramref name="failureDetailsFunction"/> can be passed in
        /// to provide those additional details.
        /// Please note that the passed-in expressions are – in fact – EXPRESSIONS, so there's limit as to how complex they can be.
        /// </summary>
        /// <returns></returns>
        public static async Task WaitOrDie<T>(this ConcurrentQueue<T> queue,
            Expression<Func<ConcurrentQueue<T>, bool>> completionExpression,
            Expression<Func<ConcurrentQueue<T>, bool>> failExpression = null,
            int timeoutSeconds = 5, 
            Func<string> failureDetailsFunction = null)
        {
            if (queue == null) throw new ArgumentNullException(nameof(queue));
            if (completionExpression == null) throw new ArgumentNullException(nameof(completionExpression));

            failExpression = failExpression ?? (_ => false);
            
            var completionPredicate = completionExpression.Compile();
            var failPredicate = failExpression.Compile();

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

                var stopwatch = Stopwatch.StartNew();
                var cancellationToken = cancellationTokenSource.Token;

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        if (failPredicate(queue))
                        {
                            throw new AssertionException($@"Waiting for

    {completionExpression}

on queue failed, because the failure expression

    {failExpression}

was satisfied after {stopwatch.Elapsed.TotalSeconds:0.0} s.

Details:

{failureDetailsFunction?.Invoke() ?? "NONE"}");
                        }

                        if (completionPredicate(queue)) return;

                        await Task.Delay(153, cancellationToken);
                    }
                }
                catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested)
                {
                }

                throw new TimeoutException($@"Waiting for

    {completionExpression}

on queue did not complete in {timeoutSeconds} s

Details:

{failureDetailsFunction?.Invoke() ?? "NONE"}");
            }
        }
    }
}