using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MadsKristensen.AddAnyFile
{
    internal static class TemplateMap
    {
        private static readonly string _folder;
        private static readonly string[] _templateFiles;
        private const string _defaultExt = ".txt";

        static TemplateMap()
        {
            var assembly = Assembly.GetExecutingAssembly().Location;
            _folder = Path.Combine(Path.GetDirectoryName(assembly), "Templates");
            _templateFiles = Directory.GetFiles(_folder, "*" + _defaultExt, SearchOption.AllDirectories);
        }

        public static async Task<string> GetTemplateFilePathAsync(Project project, string file, FileNameDialogResult type)
        {
            var extension = Path.GetExtension(file).ToLowerInvariant();
            var name = Path.GetFileName(file);
            var safeName = name.StartsWith(".") ? name : Path.GetFileNameWithoutExtension(file);
            var relative = PackageUtilities.MakeRelative(project.GetRootFolder(), Path.GetDirectoryName(file));

            string templateFile = null;

            // Look for direct file name matches
            if (_templateFiles.Any(f => Path.GetFileName(f).Equals(name + _defaultExt, StringComparison.OrdinalIgnoreCase)))
            {
                templateFile = GetTemplate(name);
            }

            // Look for file extension matches
            else if (_templateFiles.Any(f => Path.GetFileName(f).Equals(extension + _defaultExt, StringComparison.OrdinalIgnoreCase)))
            {
                var tmpl = AdjustForSpecific(safeName, extension, type);
                templateFile = GetTemplate(tmpl);
            }

            var template = await ReplaceTokensAsync(project, safeName, relative, templateFile);
            return NormalizeLineEndings(template);
        }

        private static string GetTemplate(string name)
        {
            return Path.Combine(_folder, name + _defaultExt);
        }

        private static async Task<string> ReplaceTokensAsync(Project project, string name, string relative, string templateFile)
        {
            if (string.IsNullOrEmpty(templateFile))
            {
                return templateFile;
            }

            var rootNs = project.GetRootNamespace();
            var ns = string.IsNullOrEmpty(rootNs) ? "MyNamespace" : rootNs;

            if (!string.IsNullOrEmpty(relative))
            {
                ns += "." + ProjectHelpers.CleanNameSpace(relative);
            }

            using (var reader = new StreamReader(templateFile))
            {
                var content = await reader.ReadToEndAsync();

                return content.Replace("{namespace}", ns)
                              .Replace("{itemname}", name);
            }
        }

        private static string NormalizeLineEndings(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }

            return Regex.Replace(content, @"\r\n|\n\r|\n|\r", "\r\n");
        }

        private static string AdjustForSpecific(string safeName, string extension, FileNameDialogResult type)
        {
            if (type.BaseClass)
            {
                return extension += "-base";
            }

            if (type.Enum)
            {
                return extension += "-enum";
            }

            if (type.Controller)
            {
                return extension += "-controller";
            }

            if (type.Interface)
            {
                return extension += "-interface";
            }

            return extension;
        }
    }
}
