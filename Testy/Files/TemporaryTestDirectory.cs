using System;
using System.IO;
using System.Threading;
using Testy.Internals;

namespace Testy
{
    /// <summary>
    /// Represents (and possibly creates) a test directory, which is automatically cleaned up when <see cref="TemporaryTestDirectory"/> instance is disposed
    /// </summary>
    public class TemporaryTestDirectory : IDisposable
    {
        static int _counter = 1;

        readonly string _directoryPath;

        public TemporaryTestDirectory(string rootDirectory = null, bool automaticallyCreate = true)
        {
            var number = Interlocked.Increment(ref _counter);

            _directoryPath = Path.Combine(rootDirectory ?? Shims.CurrentBaseDirectory(), $"tetdirectory-{number}");

            if (automaticallyCreate)
            {
                try
                {
                    Directory.CreateDirectory(_directoryPath);

                    Console.WriteLine($"Created test directory {_directoryPath}");
                }
                catch
                {
                    if (!Directory.Exists(_directoryPath))
                    {
                        throw;
                    }
                }
            }
        }

        public static implicit operator string(TemporaryTestDirectory temporaryTestDirectory) =>
            temporaryTestDirectory._directoryPath;

        public override string ToString() => _directoryPath;

        public void Dispose()
        {
            if (!Directory.Exists(_directoryPath)) return;

            try
            {
                Directory.Delete(_directoryPath, true);

                Console.WriteLine($"Deleted test directory {_directoryPath}");
            }
            catch
            {
                if (Directory.Exists(_directoryPath))
                {
                    throw;
                }
            }
        }
    }
}