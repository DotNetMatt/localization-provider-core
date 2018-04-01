﻿using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.AdminUI.AspNetCore;
using DbLocalizationProvider.AspNetCore;
using DbLocalizationProvider.Core.AspNetSample.Data;
using DbLocalizationProvider.Core.AspNetSample.Models;
using DbLocalizationProvider.Core.AspNetSample.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.Core.AspNetSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization();

            services.AddDbContext<ApplicationDbContext>(options =>
                                                            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc()
                    .AddViewLocalization()
                    .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(opts =>
                                                           {
                                                               var supportedCultures = new List<CultureInfo>
                                                                                       {
                                                                                           new CultureInfo("en"),
                                                                                           new CultureInfo("no")
                                                                                       };

                                                               opts.DefaultRequestCulture = new RequestCulture("en");
                                                               opts.SupportedCultures = supportedCultures;
                                                               opts.SupportedUICultures = supportedCultures;
                                                           });

            services.AddDbLocalizationProvider();
            services.AddDbLocalizationProviderAdminUI();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
                       {
                           routes.MapRoute(
                                           name: "default",
                                           template: "{controller=Home}/{action=Index}/{id?}");
                       });

            app.UseDbLocalizationProvider();
            app.UseDbLocalizationProviderAdminUI();
        }
    }
}
