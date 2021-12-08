using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

[assembly: FunctionsStartup(typeof(MongoMusicAPI.Helpers.Startup))]
namespace MongoMusicAPI.Helpers
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });

            var config = builder.GetContext().Configuration;

            builder.Services.AddSingleton((s) =>
            {
                MongoClient client = new MongoClient(config[Settings.MONGO_CONNECTION_STRING]);

                return client;
            });
        }
    }

}
