# Getting Started (Asp.Net Core)

## Install Package

```
PM> Install-Package LocalizationProvider.AspNetCore
```

## Configure Services

In your `Startup.cs` class you need to add stuff related to Mvc localization (to get required services into DI container - service collection).

And then `services.AddDbLocalizationProvider()`. You can pass in configuration settings class and setup provider's behavior.

```csharp
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // add basic localization support
        services.AddLocalization();

        // add localization to Mvc
        services.AddMvc()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

        services.AddDbLocalizationProvider(cfg =>
        {
            cfg...
        });
    }
}
```

After then you will need to make sure that you start using the provider:

```csharp
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        ...
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        ...

        app.UseDbLocalizationProvider();
    }
}
```

Using localization provider will make sure that resources are discovered and registered in the database (if this process will not be disabled via `AddDbLocalizationProvider()` method by setting `ConfigurationContext.DiscoverAndRegisterResources` to `false`).

## Working with [LocalizedResource] & [LocalizedModel] Attributes

For more information on how localized resources and localized models are working - please read [docs in main package repo](https://github.com/valdisiljuconoks/LocalizationProvider/blob/master/docs/resource-types.md).

## Adding Additional Cultures

Localization is all about translations into multiple languages. So it's often required to add more supported languages to the application. LocalizationProvider uses `RequestLocalizationOptions` to understand what languages application is supporting. You can configure this setting using `ConfigureServices` startup method.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    ...
    // just adding English and Latvian support
    services.Configure<RequestLocalizationOptions>(opts =>
    {
        var supportedCultures = new List<CultureInfo>
                                {
                                    new CultureInfo("en"),
                                    new CultureInfo("lv")
                                };

        opts.DefaultRequestCulture = new RequestCulture("en");
        opts.SupportedCultures = supportedCultures;
        opts.SupportedUICultures = supportedCultures;
    });
}
```

## Add AdminUI

For adding AdminUI to your application - refer to instructions [here](getting-started-adminui.md).
