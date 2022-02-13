using System;
using System.Collections;
using Tababular;
// ReSharper disable UnusedMember.Global

namespace Testy.Extensions;

/// <summary>
/// Extensions for formatting rows of data in a pretty way
/// </summary>
public static class TableExtensions
{
    static readonly TableFormatter TableFormatter = new TableFormatter(new Hints { CollapseVerticallyWhenSingleLine = true, MaxTableWidth = 200 });

    /// <summary>
    /// Renders a string that displays data from the properties of the given <paramref name="rows"/>
    /// in tabular form
    /// </summary>
    public static string ToTable(this IEnumerable rows)
    {
        if (rows == null) throw new ArgumentNullException(nameof(rows));
            
        return TableFormatter.FormatObjects(rows);
    }

    /// <summary>
    /// Outputs the given <paramref name="rows"/> in tabular form to the console
    /// </summary>
    public static void DumpTable(this IEnumerable rows)
    {
        if (rows == null) throw new ArgumentNullException(nameof(rows));

        Console.WriteLine(rows.ToTable());
    }
}