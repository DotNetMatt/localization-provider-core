﻿// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.AdminUI.AspNetCore.Models;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Queries;
using Microsoft.AspNetCore.Mvc;

namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    [AuthorizeRoles]
    public class ServiceController : Controller
    {
        private readonly UiConfigurationContext _config;

        public ServiceController(UiConfigurationContext config)
        {
            _config = config;
        }

        [HttpGet]
        public JsonResult Get()
        {
            return Json(PrepareViewModel());
        }

        [HttpGet]
        public JsonResult GetTree()
        {
            return Json(PrepareTreeViewModel());
        }

        private LocalizationResourceApiTreeModel PrepareTreeViewModel()
        {
            var (resources, languages, isAdmin) = GetResources();
            var result = new LocalizationResourceApiTreeModel(resources,
                languages,
                _config.MaxResourceKeyPopupTitleLength,
                _config.MaxResourceKeyDisplayLength)
            {
                Options =
                {
                    AdminMode = isAdmin,
                    ShowInvariantCulture = _config.ShowInvariantCulture,
                    ShowHiddenResources = _config.ShowHiddenResources
                }
            };

            return result;
        }

        [HttpPost]
        public JsonResult Save([FromBody] CreateOrUpdateTranslationRequestModel model)
        {
            var cmd = new CreateOrUpdateTranslation.Command(model.Key, new CultureInfo(model.Language), model.Translation);
            cmd.Execute();

            return ServiceOperationResult.Ok;
        }

        [HttpPost]
        public JsonResult Remove([FromBody] RemoveTranslationRequestModel model)
        {
            var cmd = new RemoveTranslation.Command(model.Key, new CultureInfo(model.Language));
            cmd.Execute();

            return ServiceOperationResult.Ok;
        }

        private LocalizationResourceApiModel PrepareViewModel()
        {
            var (resources, languages, isAdmin) = GetResources();
            var result = new LocalizationResourceApiModel(
                resources,
                languages,
                _config.MaxResourceKeyPopupTitleLength,
                _config.MaxResourceKeyDisplayLength)
            {
                Options =
                {
                    AdminMode = isAdmin,
                    ShowInvariantCulture = _config.ShowInvariantCulture,
                    ShowHiddenResources = _config.ShowHiddenResources
                }
            };

            return result;
        }

        private (List<LocalizationResource>, IEnumerable<CultureInfo>, bool) GetResources()
        {
            var availableLanguagesQuery = new AvailableLanguages.Query {IncludeInvariant = true};
            var languages = availableLanguagesQuery.Execute();

            var getResourcesQuery = new GetAllResources.Query(true);
            var resources = getResourcesQuery.Execute().OrderBy(_ => _.ResourceKey).ToList();

            var user = Request.HttpContext.User;
            var isAdmin = false;

            if(user != null)
                isAdmin = user.Identity.IsAuthenticated && _config.AuthorizedAdminRoles.Any(_ => user.IsInRole(_));

            return (resources, languages, isAdmin);
        }
    }
}
