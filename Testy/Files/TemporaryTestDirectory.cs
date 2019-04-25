using System;
using System.IO;
using System.Threading;
using Testy.Internals;

namespace Testy.Files
{
    /// <summary>
    /// Represents (and possibly creates) a test directory, which is automatically cleaned up when <see cref="TemporaryTestDirectory"/> instance is disposed
    /// </summary>
    public class TemporaryTestDirectory : IDisposable
    {
        static int _counter = 1;

        readonly string _directoryPath;

        /// <summary>
        /// Creates the temp test directory. If <paramref name="rootDirectory"/> is specified, the directory
        /// will be created as a subdirectory of that root. Otherwise, the base directory of the current
        /// appdomain/appcontest is used.
        /// The directory will automatically be created if <paramref name="automaticallyCreate"/> is true.
        /// </summary>
        public TemporaryTestDirectory(string rootDirectory = null, bool automaticallyCreate = true)
        {
            while (true)
            {
                var number = Interlocked.Increment(ref _counter);

                _directoryPath = Path.Combine(rootDirectory ?? Shims.CurrentBaseDirectory(), $"testdirectory-{number}");

                if (!Directory.Exists(_directoryPath)) break;
            }

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

        /// <summary>
        /// Implicitly casts this temp test dir to a string
        /// </summary>
        public static implicit operator string(TemporaryTestDirectory temporaryTestDirectory) =>
            temporaryTestDirectory._directoryPath;

        /// <summary>
        /// Returns the path of the test dir
        /// </summary>
        public override string ToString() => _directoryPath;

        /// <summary>
        /// Deletes the temporary test directory
        /// </summary>
        public void Dispose()
        {
            if (!Directory.Exists(_directoryPath)) return;

            try
            {
                Directory.Delete(_directoryPath, true);

                Console.WriteLine($"Deleted test directory {_directoryPath}");
            }
            catch(Exception exception)
            {
                if (Directory.Exists(_directoryPath))
                {
                    throw new IOException($"Could not delete directory {_directoryPath}", exception);
                }
            }
        }
    }
}