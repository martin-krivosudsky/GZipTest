using Microsoft.Extensions.DependencyInjection;
using System;
using GZipTest.Compress;
using GZipTest.Helpers;

namespace GZipTest
{
    internal class Program
    {
        private static ServiceProvider _serviceProvider;

        private static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Invalid number of arguments");
                return 1;
            }

            RegisterServices();
            var scope = _serviceProvider.CreateScope();
            var app = scope.ServiceProvider.GetRequiredService<App>();

            try
            {
                switch (args[0]?.ToLower())
                {
                    case "compress":
                        app.Compress(args[1], args[2]);
                        break;
                    case "decompress":
                        app.Decompress(args[1], args[2]);
                        break;
                    default:
                        Console.WriteLine("Invalid command");
                        return 1;
                }
            }
            catch
            {
                return 1;
            }

            DisposeServices();
            return 0;
        }

        private static void RegisterServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton(typeof(ICompressor), typeof(Compressor));
            services.AddSingleton(typeof(IDecompressor), typeof(Decompressor));
            services.AddSingleton(typeof(IFileHelper), typeof(FileHelper));
            services.AddSingleton<App>();

            _serviceProvider = services.BuildServiceProvider(true);
        }

        private static void DisposeServices()
        {
            switch (_serviceProvider)
            {
                case null:
                    return;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }
    }
}
