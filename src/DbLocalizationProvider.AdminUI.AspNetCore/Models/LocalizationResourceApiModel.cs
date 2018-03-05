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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Models
{
    public class LocalizationResourceApiModel
    {
        public LocalizationResourceApiModel(ICollection<LocalizationResource> resources, IEnumerable<CultureInfo> languages)
        {
            if(resources == null)
                throw new ArgumentNullException(nameof(resources));

            if(languages == null)
                throw new ArgumentNullException(nameof(languages));

            Resources = resources.Select(r =>
                                         {
                                             return new ResourceListItemApiModel(r.ResourceKey,
                                                                                 r.Translations.Select(t => new ResourceItemApiModel(r.ResourceKey,
                                                                                                                                     t.Value,
                                                                                                                                     t.Language)).ToList(),
                                                                                 r.FromCode);
                                         }).ToList();

            Resources.ForEach(r =>
                              {
                                  var trimmed = new string(r.Key.Take(80).ToArray());
                                  r.DisplayKey = r.Key.Length <= 80 ? trimmed : $"{trimmed}...";
                              });

            Languages = languages.Select(l => new CultureApiModel(l.Name, l.EnglishName));
        }

        public List<ResourceListItemApiModel> Resources { get; }

        public IEnumerable<CultureApiModel> Languages { get; }

        public bool AdminMode { get; set; }
    }
}
