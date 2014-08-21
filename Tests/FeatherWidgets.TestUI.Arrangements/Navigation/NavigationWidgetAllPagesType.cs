﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUI.Arrangements.Framework.Attributes;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace FeatherWidgets.TestUI.Arrangements
{
    /// <summary>
    /// Sample arrangement that Creates and deletes a page.
    /// </summary>
    public class NavigationWidgetAllPagesType : ITestArrangement
    {
        /// <summary>
        /// Sets up.
        /// </summary>
        [ServerSetUp]
        public void SetUp()
        {
            Guid templateId = ServerOperations.Templates().GetTemplateIdByTitle(PageTemplateName);
            Guid parentPageId = ServerOperations.Pages().CreatePage(PageName, templateId);
            Guid pageNodeId = ServerOperations.Pages().GetPageNodeId(parentPageId);
            Guid childPage1Id = Guid.NewGuid();
            ServerOperations.Pages().CreatePage(ChildPage1, childPage1Id, pageNodeId);
            
            ServerOperations.Pages().CreateInternalRedirectPage(Page2Redirect, pageNodeId);

            ServerOperations.Pages().CreateExternalRedirectPage(Page1Redirect, ExternalUrl);

            Guid unpublisId = ServerOperations.Pages().CreatePage(UnpublishPage);
            ServerOperations.Pages().UnpublishPage(unpublisId);

            ServerOperations.Pages().CreateDraftPage(PageDraft);

            Guid pageGroup2Id = Guid.NewGuid();
            ServerOperations.Pages().CreatePageGroup(pageGroup2Id, Guid.Empty, Page2Group);

            Guid pageGroupId = Guid.NewGuid();
            ServerOperations.Pages().CreatePageGroup(pageGroupId, Guid.Empty, PageGroup);
            Guid childPage2Id = Guid.NewGuid();
            ServerOperations.Pages().CreatePage(ChildPage2, childPage2Id, pageGroupId);
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [ServerTearDown]
        public void TearDown()
        {
            ServerOperations.Pages().DeleteAllPages();
        }

        private const string PageName = "ParentPage";
        private const string PageTemplateName = "Bootstrap.default";
        private const string ChildPage1 = "ChildPage1";
        private const string ChildPage2 = "ChildPage2";
        private const string Page2Redirect = "Page2Redirect";
        private const string Page1Redirect = "Page1Redirect";
        private const string ExternalUrl = "http://www.weather.com/";
        private const string UnpublishPage = "UnpublishPage";
        private const string PageDraft = "PageDraft";
        private const string Page2Group = "Page2Group";
        private const string PageGroup = "PageGroup";
    }
}
