using Blogs.WebApi.Filters;
using Blogs.WebApi.Filters.Authorization;
using Blogs.Factory;
using Blogs.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Blogs.WebApi
{
    [ExcludeFromCodeCoverage]
    public class StartUp
    {
        public IConfiguration Configuration { get; }

        public StartUp(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            RegisterMvc(services);
            RegisterCors(services);
            RegisterSwagger(services);
            RegisterConfiguration(services);

            services.AddControllers(options => options.Filters.Add(typeof(ExceptionFilter)));
            services.AddScoped<AuthenticationFilter>();

            IServiceFactory factory = new ServiceFactory(services);
            factory.RegisterLogic();
            factory.RegisterDataAccess();
        }
            

        // Configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
     
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {endpoints.MapControllers();});
        }

        private void RegisterMvc(IServiceCollection services)
        {
            services.AddMvcCore().AddApiExplorer();
            services.AddMvc().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        private void RegisterCors(IServiceCollection services)
        {
            services.AddCors(options => {
                options.AddPolicy("AllowEverything", builder =>
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
        }

        private void RegisterSwagger(IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        private void RegisterConfiguration(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            Directory.CreateDirectory(Configuration.GetSection("ImportersAssemblys").Value);
            Directory.CreateDirectory(Configuration.GetSection("JSONFilesToImport").Value);
            Directory.CreateDirectory(Configuration.GetSection("XMLFilesToImport").Value);
        }

    }
}
