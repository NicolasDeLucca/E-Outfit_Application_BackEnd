using Blogs.BusinessLogic.ResourceManagers;
using Blogs.DataAccess.Contexts;
using Blogs.DataAccess.Repositories;
using Blogs.Domain.BusinessEntities;
using Blogs.Domain.BusinessEntities.Mentions;
using Blogs.Importing.DTOs;
using Blogs.Importing.Importers.JSON;
using Blogs.Importing.ImportService;
using Blogs.Importing.Parameters;
using Blogs.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Blogs.Factory
{
    [ExcludeFromCodeCoverage]
    public class ServiceFactory : IServiceFactory
    {
        private readonly IServiceCollection _services;

        public ServiceFactory(IServiceCollection services)
        {
            _services = services;
        }

        public void RegisterLogic()
        {
            RegisterManagers();
            RegisterImportServices();
        }

        public void RegisterDataAccess()
        {
            RegisterDbContexts();
            RegisterRepositories();
        }

        #region RegisterHelpers

        private void RegisterManagers()
        {
            _services.AddScoped<IManager<User>, UserManager>();
            _services.AddScoped<IManager<Mention>, MentionManager>();
            _services.AddScoped<IManager<Notification<Mention>>, NotificationManager<Mention>>();
            _services.AddScoped<IManager<Notification<Article>>, NotificationManager<Article>>();
            _services.AddScoped<IManager<Article>, ArticleManager>();
            _services.AddScoped<IManager<Session>, SessionManager>();
            _services.AddScoped<IManager<Offense>, OffenseManager>();
        }

        private void RegisterImportServices()
        {
            _services.AddScoped<EntityMapper>();
            _services.AddScoped<ParameterImplementation<ParameterType>>();
            _services.AddScoped<IParameter<ParameterType>, ParameterAdapter<ParameterType>>();
            _services.AddScoped<IImporterService<Article, ParameterType>, ArticleImporterService>();
            RegisterImporters();
        }

        private void RegisterImporters()
        {
            _services.AddScoped<IImporter<Article, ParameterType>, ArticleImporterAdapter>();
            _services.AddScoped<IImporter<ImportedArticle, ParameterType>, ArticleImporter>();
        }

        private void RegisterDbContexts()
        {
            _services.AddDbContext<DbContext, DataContext>();
        }   

        private void RegisterRepositories()
        {
            _services.AddScoped<IRepository<User>, UserRepository>();
            _services.AddScoped<IRepository<Mention>, MentionRepository>();
            _services.AddScoped<IRepository<Notification<Mention>>, NotificationRepository<Mention>>();
            _services.AddScoped<IRepository<Notification<Article>>, NotificationRepository<Article>>();
            _services.AddScoped<IRepository<Article>, ArticleRepository>();
            _services.AddScoped<IRepository<Session>, SessionRepository>();
            _services.AddScoped<IRepository<Offense>, OffenseRepository>();
        }

        #endregion

    }
}