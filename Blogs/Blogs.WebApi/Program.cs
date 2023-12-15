namespace Blogs.WebApi 
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args).
                    ConfigureWebHostDefaults(webBuilder => {webBuilder.UseStartup<StartUp>();});
            hostBuilder.Build().Run();
        }
    }
} 

