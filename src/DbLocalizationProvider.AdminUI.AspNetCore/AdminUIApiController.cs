﻿// Copyright (c) 2018 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using DbLocalizationProvider.AdminUI.AspNetCore.Models;
using DbLocalizationProvider.Queries;
using Microsoft.AspNetCore.Mvc;

namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    public class AdminUIApiController : Controller
    {
        [HttpGet]
        public JsonResult Get()
        {
            return Json(PrepareViewModel());
        }

        private LocalizationResourceApiModel PrepareViewModel()
        {
            var availableLanguagesQuery = new AvailableLanguages.Query { IncludeInvariant = true };
            var languages = availableLanguagesQuery.Execute();

            var getResourcesQuery = new GetAllResources.Query();
            var resources = getResourcesQuery.Execute().OrderBy(r => r.ResourceKey).ToList();

            //var user = RequestContext.Principal;
            var isAdmin = false;

            //if (user != null)
            //    isAdmin = user.Identity.IsAuthenticated && UiConfigurationContext.Current.AuthorizedAdminRoles.Any(r => user.IsInRole(r));

            return new LocalizationResourceApiModel(resources, languages) { AdminMode = isAdmin };
        }
    }
}
