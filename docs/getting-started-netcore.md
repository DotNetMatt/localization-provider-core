# Getting Started (Asp.Net Core)

## Install Package

```
PM> Install-Package LocalizationProvider.AspNetCore
```

## Configure Services

In your `Startup.cs` class you need to add stuff related to Mvc localization (to get required services into DI container - service collection).

And then `services.AddDbLocalizationProvider()`. You can pass in configuration settings class (parameter name `cfg`) and setup provider's behavior.

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // add localization to Mvc
        services.AddControllersWithViews()
                .AddMvcLocalization();

        services.AddDbLocalizationProvider(cfg =>
        {
            cfg...
        });

        services.AddRouting();
    }
}
```

Following configuration options are available:

| Option | Description |
|------|------|
| CacheManager | Gets or sets cache manager used to store resources and translations (`InMemory` by default) |
| Connection | Gets or sets the *name* of the database connection (e.g. `"DefaultConnection"`). |
| CustomAttributes | Gets or sets list of custom attributes that should be discovered and registered during startup scanning. |
| DefaultResourceCulture | Gets or sets the default resource culture to register translations for newly discovered resources. |
| DiagnosticsEnabled | Gets or sets value enabling or disabling diagnostics for localization provider (e.g. missing keys will be written to log file). |
| DiscoverAndRegisterResources | Gets or sets the flag to control localized models discovery and registration during app startup. |
| EnableInvariantCultureFallback | Gets or sets flag to enable or disable invariant culture fallback (to use resource values discovered & registered from code). |
| EnableLocalization | Gets or sets the callback function for enabling or disabling localization. If this returns `false` - requested resource key will be returned as translation. |
| Export | Gets or sets settings used for export of the resources. |
| FallbackCultures | Using this list you can configure language fallback settings. |
| ForeignResources | Gets or sets collection of foreign resources. Foreign resource descriptors are used to include classes without `[LocalizedResource]` or `[LocalizedModel]` attributes. |
| Import | Gets or sets settings to be used during resource import. |
| ModelMetadataProviders | Settings for model metadata providers. |
| PopulateCacheOnStartup | Gets or sets a value indicating whether cache should be populated during startup (default = `true`). |
| ScanAllAssemblies | Forces type scanner to load all referenced assemblies. When enabled, scanner is not relying on current `AppDomain.GetAssemblies` but checks referenced assemblies recursively (default `false`). |
| TypeFactory | Returns type factory used internally for creating new services or handlers for commands. |
| TypeScanners | Gets list of all known type scanners. |


Following `ModelMetadataProviders` configuration options are available:

| Option | Description |
|------|------|
| MarkRequiredFields | Set `true` to add translation returned from `RequiredFieldResource` for required fields. |
| ReplaceProviders | Gets or sets a value to replace ModelMetadataProvider to use new db localization system. |
| RequiredFieldResource | If `MarkRequiredFields` is set to `true`, return of this method will be used to indicate required fields (added at the end of label). |
| UseCachedProviders | Gets or sets a value to use cached version of ModelMetadataProvider. |


After then you will need to make sure that you start using the provider:

```csharp
public class Startup
{
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

### Configure Fallback Languages
LocalizationProvider gives you option to configure fallback languages for the library.
It means that provider will try to get translation in requested language. And if it does not exist in that language, fallback language list is used to decide which language to try next until either succeeds or fails with no translation found.

To configure fallback languages use code below:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbLocalizationProvider(_ =>
        {
            ...
            _.FallbackCultures
                .Try(new CultureInfo("sv"))
                .Then(new CultureInfo("no"))
                .Then(new CultureInfo("en"));
        });
    }
}
```

This means that following logic will be used during translation lookup:

1) Developer requests translation in Swedish culture (`"sv"`) using `ILocalizationProvider.GetString(() => ...)` method.
2) If translation does not exist -> provider is looking for translation in Norwegian language (`"no"` - second language in the fallback list).
3) If translation is found - one is returned; if not - provider continues process and is looking for translation in English (`"en"`).
4) If there is no translation in English -> depending on `ConfigurationContext.EnableInvariantCultureFallback` setting -> translation in InvariantCulture may be returned.

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
