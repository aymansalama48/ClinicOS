using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ClinicOS.Infrastructure.External.Email
{
    public class EmailTemplateEngine
    {
        public async Task<string> RenderTemplateAsync<TModel>(string templateName, TModel model)
        {
            var template = await LoadTemplateAsync(templateName);
            return ReplacePlaceholders(template, model);
        }

        private async Task<string> LoadTemplateAsync(string templateName)
        {
            var templatePath = Path.Combine(
                AppContext.BaseDirectory,
                "External",
                "Email",
                "EmailTemplates",
                $"{templateName}.html");

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"قالب البريد الإلكتروني '{templateName}' غير موجود.", templatePath);

            // ✅ مهم جداً: UTF-8 صريح
            return await File.ReadAllTextAsync(templatePath, Encoding.UTF8);
        }
        private string ReplacePlaceholders<TModel>(string template, TModel model)
        {
            if (model == null) return template;

            var properties = typeof(TModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                var placeholder = $"{{{{{prop.Name}}}}}";

                // ✅ لو الخاصية NULL يتم استبدالها بنص فارغ لتجنب ظهور {{Placeholder}} في الإيميل
                var value = prop.GetValue(model)?.ToString() ?? string.Empty;
                template = template.Replace(placeholder, value);
            }

            return template;
        }
    }
}
