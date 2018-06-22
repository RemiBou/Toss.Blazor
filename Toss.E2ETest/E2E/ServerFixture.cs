using System;
using System.IO;
using System.Threading;

namespace Toss.Tests.E2E
{
    public abstract class ServerFixture : IDisposable
    {
        public Uri RootUri => _rootUriInitializer.Value;

        private readonly Lazy<Uri> _rootUriInitializer;

        public ServerFixture()
        {
            _rootUriInitializer = new Lazy<Uri>(() =>
                new Uri(StartAndGetRootUri()));
        }

        public abstract void Dispose();

        protected abstract string StartAndGetRootUri();

        protected static string FindSolutionDir()
        {
            return FindClosestDirectoryContaining(
                "Toss.sln",
                Path.GetDirectoryName(typeof(ServerFixture).Assembly.Location));
        }

        protected static string FindSampleOrTestSitePath(string projectName)
        {
            var solutionDir = FindSolutionDir();
            return Path.Combine(solutionDir, "Toss", projectName);
        }

        private static string FindClosestDirectoryContaining(
            string filename,
            string startDirectory)
        {
            var dir = startDirectory;
            while (true)
            {
                if (File.Exists(Path.Combine(dir, filename)))
                {
                    return dir;
                }

                dir = Directory.GetParent(dir)?.FullName;
                if (string.IsNullOrEmpty(dir))
                {
                    throw new FileNotFoundException(
                        $"Could not locate a file called '{filename}' in " +
                        $"directory '{startDirectory}' or any parent directory.");
                }
            }
        }

        protected static void RunInBackgroundThread(Action action)
        {
            var isDone = new ManualResetEvent(false);

            new Thread(() =>
            {
                action();
                isDone.Set();
            }).Start();

            isDone.WaitOne();
        }
    }
}