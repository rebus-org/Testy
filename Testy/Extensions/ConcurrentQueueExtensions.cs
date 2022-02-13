using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

// ReSharper disable UnusedMember.Global

namespace Testy.Extensions;

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
}