﻿using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    [LocalizedResource]
    [Hidden]
    public class Resources
    {
        public string Title = "Admin UI";
        public string Header = "Localized Resource Editor";
        public string Export = "Export";
        public string Import = "Import";
        public string Languages = "Languages";
        public string Settings = "Settings";
        public string Save = "Ok, Save!";
        public string Cancel = "Cancel";
        public string SearchPlaceholder = "if it gets too noisy, type filter here...";
        public string ResourceKeyColumn = "Key";
        public string InvariantCultureColumn = "Invariant";
        public string HiddenColumn = "Is Hidden?";
    }
}
