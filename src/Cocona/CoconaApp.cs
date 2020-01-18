using Cocona.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cocona
{
    public class CoconaApp
    {
        public static CoconaAppHostBuilder Create()
            =>  new CoconaAppHostBuilder(Host.CreateDefaultBuilder());

        public static void Run<T>(string[] args, Action<CoconaAppOptions>? configureOptions = null)
            => Run(args, new[] { typeof(T) }, configureOptions);

        public static Task RunAsync<T>(string[] args, Action<CoconaAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => RunAsync(args, new[] { typeof(T) }, configureOptions, cancellationToken);

        public static void Run(string[] args, Type[] commandTypes, Action<CoconaAppOptions>? configureOptions = null)
            => Create().Run(args, commandTypes, configureOptions);

        public static Task RunAsync(string[] args, Type[] commandTypes, Action<CoconaAppOptions>? configureOptions = null, CancellationToken cancellationToken = default)
            => Create().RunAsync(args, commandTypes, configureOptions, cancellationToken);

    }
}
