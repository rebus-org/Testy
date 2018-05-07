using System;
using System.IO;
using System.Threading;

namespace Testy
{
    /// <summary>
    /// 
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

        public void Dispose()
        {
            if (Directory.Exists(_directoryPath))
            {
                Directory.Delete(_directoryPath, true);

                Console.WriteLine($"Deleted test directory {_directoryPath}");
            }
        }
    }
}