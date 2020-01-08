﻿// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    /// <summary>
    /// Do I really need to document extension classes?
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Use this method if you want to add AdminUI component to your application. This is just a part of the setup. You will also need to mount the component. Use other method (will leave it up to you to figure out which).
        /// </summary>
        /// <param name="services">Collection of the services (Microsoft approach for DI).</param>
        /// <param name="setup">UI setup context will be passed in, so you can do some customization on that object to influence how AdminUI behaves.</param>
        /// <returns>The same service collection - so you can do chaining.</returns>
        public static IServiceCollection AddDbLocalizationProviderAdminUI(this IServiceCollection services, Action<UiConfigurationContext> setup = null)
        {
            setup?.Invoke(UiConfigurationContext.Current);

            services.AddSingleton(_ => UiConfigurationContext.Current);
            services.AddScoped<AuthorizeRolesAttribute>();

            // add support for admin ui razor class library pages
            services.Configure<RazorPagesOptions>(_ =>
                                                  {
                                                      _.Conventions.AuthorizeAreaPage("4D5A2189D188417485BF6C70546D34A1", "/AdminUI");
                                                      _.Conventions.AuthorizeAreaPage("4D5A2189D188417485BF6C70546D34A1", "/AdminUITree");

                                                      _.Conventions.AddAreaPageRoute("4D5A2189D188417485BF6C70546D34A1",
                                                                                     "/AdminUI",
                                                                                     UiConfigurationContext.Current.RootUrl);
                                                      _.Conventions.AddAreaPageRoute("4D5A2189D188417485BF6C70546D34A1",
                                                                                     "/AdminUITree",
                                                                                     UiConfigurationContext.Current.RootUrl + "/tree");
                                                  });
            return services;
        }
    }
}
