using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace TheRIPPer.Razor
{
    public class Program
    {
        public static void Main(string[] args) {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(o => { o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(300); })
                .Build();
    }
}