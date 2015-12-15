using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;

namespace FeatherWidgets.TestIntegration.ResourcePackages
{
    [TestFixture]
    [Description("This is a class with tests related to feather resource packages.")]
    public class PackagesTests
    {
        [Test]
        [Category(TestCategories.Packages)]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Compares the embedded resource templates with the Minimal package templates and verifies that their count is equal.")]
        public void ResourcePackage_VerifyEmbeddedTemplatesCountAndMinimalPackageTemplatesCountAreEqual()
        {
            var featherAssemblyResources = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.FullName.Contains(FrontendAssemblyName))
                .SelectMany(x => x.GetManifestResourceNames());

            featherAssemblyResources = this.FilterEmbeddedResources(featherAssemblyResources);

            var embededResourceTemplatesCount = featherAssemblyResources.Count();

            var packageDirectory = FeatherServerOperations.ResourcePackages().GetResourcePackagesDestination(MinimalPackageName);

            var packageDirectoryExists = Directory.Exists(packageDirectory);

            Assert.IsTrue(packageDirectoryExists, "The '{0}' package directory does not exist", MinimalPackageName);

            var packageDirectoryInfo = new DirectoryInfo(packageDirectory);

            var packageTemplateFiles = packageDirectoryInfo.GetFiles("*.cshtml", SearchOption.AllDirectories)
                .Where(x => !this.excludePackageResourceFilters.Any(y => y == x.Directory.Name));

            var packageTemplatesCount = packageTemplateFiles.Count();

            Assert.AreEqual(embededResourceTemplatesCount, packageTemplatesCount, "The embedded templates count and the package templates count are not equal");
        }

        [Test]
        [Category(TestCategories.Packages)]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Verifies that each embedded resource template exists as a template in the Minimal package.")]
        public void ResourcePackage_VerifyEmbeddedTemplateExistsInMinimalPackage()
        {
            var frontendAssemblyResources = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.FullName.Contains(FrontendAssemblyName))
                .SelectMany(x => x.GetManifestResourceNames());

            frontendAssemblyResources = this.FilterEmbeddedResources(frontendAssemblyResources);

            var packageDirectory = FeatherServerOperations.ResourcePackages().GetResourcePackagesDestination(MinimalPackageName);

            var packageDirectoryExists = Directory.Exists(packageDirectory);

            Assert.IsTrue(packageDirectoryExists, "The '{0}' package directory does not exist", MinimalPackageName);

            var packageDirectoryInfo = new DirectoryInfo(packageDirectory);

            var packageTemplateFiles = packageDirectoryInfo.GetFiles("*.cshtml", SearchOption.AllDirectories)
                .Where(x => !this.excludePackageResourceFilters.Any(y => y == x.Directory.Name));

            foreach (var assemblyResource in frontendAssemblyResources)
            {
                var widgetName = Regex.Match(assemblyResource, MvcViewsResourcePattern).Groups[1].ToString();
                var viewFileName = Regex.Match(assemblyResource, MvcViewsResourcePattern).Groups[2].ToString();

                var resourceExists = packageTemplateFiles.Any(x => x.Directory.Name == widgetName && x.Name == viewFileName);

                Assert.IsTrue(resourceExists, "The assembly resource '{0}' does not exists in the '{1}' package", assemblyResource, MinimalPackageName);
            }
        }

        [Test]
        [Category(TestCategories.Packages)]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Compares the embedded resource templates with the Minimal package templates and verifies that their contents are equal.")]
        public void ResourcePackage_VerifyEmbeddedTemplatesContentAndMinimalPackageTemplatesContentAreEqual()
        {
            var frontendAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.FullName.Contains(FrontendAssemblyName));

            foreach (var frontendAssembly in frontendAssemblies)
            {
                var assemblyResources = frontendAssembly.GetManifestResourceNames().AsEnumerable();
                assemblyResources = this.FilterEmbeddedResources(assemblyResources);

                foreach (var assemblyResource in assemblyResources)
                {
                    var widgetName = Regex.Match(assemblyResource, MvcViewsResourcePattern).Groups[1].ToString();
                    var viewFileName = Regex.Match(assemblyResource, MvcViewsResourcePattern).Groups[2].ToString();

                    var filePath = FeatherServerOperations.ResourcePackages().GetResourcePackageMvcViewDestinationFilePath(MinimalPackageName, widgetName, viewFileName);

                    var minimalPackageResourceExists = File.Exists(filePath);

                    Assert.IsTrue(minimalPackageResourceExists, "The '{0}' package template '{1}' does not exist", MinimalPackageName, filePath);

                    var embededResourceContent = this.GetContentFromEmbededResource(frontendAssembly, assemblyResource).Trim();
                    var minimalPackageResourceContent = this.GetContentFromPackageResource(filePath).Trim();

                    Assert.AreEqual(embededResourceContent, minimalPackageResourceContent, "The embedded template content and the package template content are not equal");
                }
            }
        }

        #region Helper methods

        private IEnumerable<string> FilterEmbeddedResources(IEnumerable<string> resourceNames)
        {
            var filteredResourceNames = resourceNames;

            foreach (var filter in this.includeEmbeddedResourceFilters)
            {
                filteredResourceNames = filteredResourceNames
                     .Where(x => Regex.IsMatch(x, filter, RegexOptions.IgnoreCase));
            }

            foreach (var filter in this.excludeEmbeddedResourceFilters)
            {
                filteredResourceNames = filteredResourceNames
                     .Where(x => !Regex.IsMatch(x, filter, RegexOptions.IgnoreCase));
            }

            return filteredResourceNames;
        }

        private string GetContentFromEmbededResource(Assembly assembly, string resourceName)
        {
            var result = string.Empty;

            using (var reader = new StreamReader(assembly.GetManifestResourceStream(resourceName), Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        private string GetContentFromPackageResource(string filePath)
        {
            var result = File.ReadAllText(filePath, Encoding.UTF8);

            return result;
        }

        #endregion

        #region Private members

        private readonly string[] includeEmbeddedResourceFilters = new string[] { "cshtml", "Mvc.Views" };
        private readonly string[] excludeEmbeddedResourceFilters = new string[] { "designer", "TestUtilities", "ContentPager", "sf-html-field.sf-cshtml" };
        private readonly string[] excludePackageResourceFilters = new string[] { "Layout" };
        private const string MinimalPackageName = "Minimal";
        private const string FrontendAssemblyName = "Telerik.Sitefinity.Frontend";
        private const string MvcViewsResourcePattern = @"Mvc\.Views\.(.+?)\.(.+)";

        #endregion
    }
}