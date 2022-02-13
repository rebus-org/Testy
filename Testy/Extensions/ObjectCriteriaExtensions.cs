using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Testy.Extensions;

public static class ObjectCriteriaExtensions
{
    /// <summary>
    /// Waits for the completion expression <paramref name="completionExpression"/> to become true, defaulting to a 5 s timeout.
    /// The timeout can be specified by passing in another value for <paramref name="timeoutSeconds"/>.
    /// Optionally, a failure expression specified by <paramref name="failExpression"/> can cause an <see cref="AssertionException"/>
    /// to be thrown if it becomes true while waiting.
    /// If additional details are desired in the thrown exceptions, the <paramref name="failureDetailsFunction"/> can be passed in
    /// to provide those additional details.
    /// Please note that the passed-in expressions are – in fact – EXPRESSIONS, so there's limit as to how complex they can be.
    /// </summary>
    public static async Task WaitOrDie<TObject>(
        this TObject obj,
        Expression<Func<TObject, bool>> completionExpression,
        Expression<Func<TObject, bool>> failExpression = null,
        int timeoutSeconds = 5,
        Func<string> failureDetailsFunction = null
        )

    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        if (completionExpression == null) throw new ArgumentNullException(nameof(completionExpression));

        var completionPredicate = completionExpression.Compile();
        var failPredicate = failExpression?.Compile() ?? (_ => false);
        var timeout = TimeSpan.FromSeconds(timeoutSeconds);

        using var cancellationTokenSource = new CancellationTokenSource(timeout);

        var stopwatch = Stopwatch.StartNew();

        while (!cancellationTokenSource.IsCancellationRequested)
        {
            if (failPredicate(obj))
            {
                throw new AssertionException($@"Waiting for

    {completionExpression}

on {obj} failed, because the failure expression

    {failExpression}

was satisfied after {stopwatch.Elapsed.TotalSeconds:0.0} s.

Details:

{failureDetailsFunction?.Invoke() ?? "NONE"}");
            }

            if (completionPredicate(obj)) return;

            await Task.Delay(153, CancellationToken.None);
        }

        throw new TimeoutException($@"Waiting for

    {completionExpression}

on {obj} did not complete in {timeoutSeconds} s

Details:

{failureDetailsFunction?.Invoke() ?? "NONE"}");
    }
}