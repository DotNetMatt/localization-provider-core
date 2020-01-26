﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DbLocalizationProvider.NetCore.Storage.SqlServer
{
    public class ResourceRepository
    {
        public IEnumerable<LocalizationResource> GetAll()
        {
            using(var conn = new SqlConnection(Settings.DbContextConnectionString))
            {
                conn.Open();

                var cmd = new SqlCommand(@"
                    SELECT
                        r.Id,
                        ResourceKey,
                        Author,
                        FromCode,
                        IsHidden,
                        IsModified,
                        ModificationDate,
                        Notes,
                        t.Id as TranslationId,
                        t.Value as Translation,
                        t.Language
                    FROM [dbo].[LocalizationResources] r
                    INNER JOIN [dbo].[LocalizationResourceTranslations] t ON r.Id = t.ResourceId",
                    conn);

                var reader = cmd.ExecuteReader();
                var lookup = new Dictionary<string, LocalizationResource>();

                while(reader.Read())
                {
                    var key = reader.GetString(reader.GetOrdinal(nameof(LocalizationResource.ResourceKey)));
                    if(lookup.TryGetValue(key, out var resource))
                    {
                        // add translation to already loaded resource
                        resource.Translations.Add(new LocalizationResourceTranslation
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("TranslationId")),
                            ResourceId = resource.Id,
                            Value = reader.GetStringSafe("Translation"),
                            Language = reader.GetString(reader.GetOrdinal("Language")),
                            LocalizationResource = resource
                        });
                    }
                    else
                    {
                        // create resource
                        var result = new LocalizationResource(key)
                        {
                            Id = reader.GetInt32(reader.GetOrdinal(nameof(LocalizationResource.Id))),
                            Author = reader.GetString(reader.GetOrdinal(nameof(LocalizationResource.Author))),
                            FromCode = reader.GetBoolean(reader.GetOrdinal(nameof(LocalizationResource.FromCode))),
                            IsHidden = reader.GetBoolean(reader.GetOrdinal(nameof(LocalizationResource.IsHidden))),
                            IsModified = reader.GetBoolean(reader.GetOrdinal(nameof(LocalizationResource.IsModified))),
                            ModificationDate = reader.GetDateTime(reader.GetOrdinal(nameof(LocalizationResource.ModificationDate))),
                            Notes = reader.GetStringSafe(nameof(LocalizationResource.Notes)),
                        };

                        result.Translations.Add(new LocalizationResourceTranslation
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("TranslationId")),
                            ResourceId = result.Id,
                            Value = reader.GetStringSafe("Translation"),
                            Language = reader.GetString(reader.GetOrdinal("Language")),
                            LocalizationResource = result
                        });

                        lookup.Add(key, result);
                    }
                }

                return lookup.Values;
            }
        }

        public LocalizationResource GetByKey(string resourceKey)
        {
            if(resourceKey == null) throw new ArgumentNullException(nameof(resourceKey));

            using(var conn = new SqlConnection(Settings.DbContextConnectionString))
            {
                conn.Open();

                var cmd = new SqlCommand(@"
                    SELECT
                        r.Id,
                        Author,
                        FromCode,
                        IsHidden,
                        IsModified,
                        ModificationDate,
                        Notes,
                        t.Id as TranslationId,
                        t.Value as Translation,
                        t.Language
                    FROM [dbo].[LocalizationResources] r
                    INNER JOIN [dbo].[LocalizationResourceTranslations] t ON r.Id = t.ResourceId
                    WHERE ResourceKey = @key",
                    conn);
                cmd.Parameters.AddWithValue("key", resourceKey);

                var reader = cmd.ExecuteReader();

                if(!reader.Read()) return null;

                var result = new LocalizationResource(resourceKey)
                {
                    Id = reader.GetInt32(reader.GetOrdinal(nameof(LocalizationResource.Id))),
                    Author = reader.GetString(reader.GetOrdinal(nameof(LocalizationResource.Author))),
                    FromCode = reader.GetBoolean(reader.GetOrdinal(nameof(LocalizationResource.FromCode))),
                    IsHidden = reader.GetBoolean(reader.GetOrdinal(nameof(LocalizationResource.IsHidden))),
                    IsModified = reader.GetBoolean(reader.GetOrdinal(nameof(LocalizationResource.IsModified))),
                    ModificationDate = reader.GetDateTime(reader.GetOrdinal(nameof(LocalizationResource.ModificationDate))),
                    Notes = reader.GetStringSafe(nameof(LocalizationResource.Notes))
                };

                // read 1st translation
                result.Translations.Add(new LocalizationResourceTranslation
                {
                    Id = reader.GetInt32(reader.GetOrdinal("TranslationId")),
                    ResourceId = result.Id,
                    Value = reader.GetStringSafe("Translation"),
                    Language = reader.GetString(reader.GetOrdinal("Language")),
                    LocalizationResource = result
                });

                while(reader.Read())
                {
                    result.Translations.Add(new LocalizationResourceTranslation
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("TranslationId")),
                        ResourceId = result.Id,
                        Value = reader.GetStringSafe("Translation"),
                        Language = reader.GetString(reader.GetOrdinal("Language")),
                        LocalizationResource = result
                    });
                }

                return result;
            }
        }

        public void AddTranslationForResource(LocalizationResource resource, LocalizationResourceTranslation translation)
        {
            if(resource == null) throw new ArgumentNullException(nameof(resource));
            if(translation == null) throw new ArgumentNullException(nameof(translation));

            using(var conn = new SqlConnection(Settings.DbContextConnectionString))
            {
                conn.Open();

                var cmd = new SqlCommand("INSERT INTO [dbo].[LocalizationResourceTranslations] ([Language], [ResourceId], [Value]) VALUES (@language, @resourceId, @translation)", conn);
                cmd.Parameters.AddWithValue("language", translation.Language);
                cmd.Parameters.AddWithValue("resourceId", translation.ResourceId);
                cmd.Parameters.AddWithValue("translation", translation.Value);

                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateTranslation(LocalizationResource resource, LocalizationResourceTranslation translation)
        {
            if(resource == null) throw new ArgumentNullException(nameof(resource));
            if(translation == null) throw new ArgumentNullException(nameof(translation));

            using(var conn = new SqlConnection(Settings.DbContextConnectionString))
            {
                conn.Open();

                var cmd = new SqlCommand("UPDATE [dbo].[LocalizationResourceTranslations] SET [Value] = @translation WHERE [Id] = @id", conn);
                cmd.Parameters.AddWithValue("translation", translation.Value);
                cmd.Parameters.AddWithValue("id", translation.Id);

                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateResource(LocalizationResource resource)
        {
            if(resource == null) throw new ArgumentNullException(nameof(resource));

            using(var conn = new SqlConnection(Settings.DbContextConnectionString))
            {
                conn.Open();

                var cmd = new SqlCommand("UPDATE [dbo].[LocalizationResources] SET [IsModified] = @isModified, [ModificationDate] = @ModificationDate, [Notes] = @notes WHERE [Id] = @id", conn);
                cmd.Parameters.AddWithValue("id", resource.Id);
                cmd.Parameters.AddWithValue("modificationDate", resource.ModificationDate);
                cmd.Parameters.AddWithValue("isModified", resource.IsModified);
                cmd.Parameters.AddWithValue("notes", resource.Notes);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
