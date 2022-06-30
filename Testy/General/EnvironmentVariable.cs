using System;
// ReSharper disable UnusedMember.Global

namespace Testy.General;

/// <summary>
/// Temporarily sets an environment variable, restoring the original value when it is disposed
/// </summary>
public class EnvironmentVariable : IDisposable
{
    readonly string _name;
    readonly string? _previousValue;

    public EnvironmentVariable(string name, string value)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _previousValue = Environment.GetEnvironmentVariable(name);
        Environment.SetEnvironmentVariable(name, value);
    }

    public void Dispose() => Environment.SetEnvironmentVariable(_name, _previousValue);
}