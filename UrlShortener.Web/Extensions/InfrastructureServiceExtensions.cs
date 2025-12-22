namespace UrlShortener.Web.Extensions;

using FluentValidation;
using FluentValidation.AspNetCore;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection RegisterInfrastructureServices(
        this IServiceCollection services)
    {
        services.AddOptions();
        
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                options.JsonSerializerOptions.Converters.Add(NodaConverters.IntervalConverter);
                options.JsonSerializerOptions.Converters.Add(NodaConverters.InstantConverter);
                options.JsonSerializerOptions.Converters.Add(NodaConverters.LocalDateConverter);
                options.JsonSerializerOptions.Converters.Add(NodaConverters.LocalTimeConverter);
            });

        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters(); 
        
        services.AddValidatorsFromAssemblyContaining<Program>();

        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        
        return services;
    }
}