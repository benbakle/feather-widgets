using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Modules.Pages.Web.Services.Model;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Sitefinity.Web.UI.ControlDesign;

namespace Telerik.Sitefinity.Frontend.Navigation.Mvc.Controllers
{
    public class ExtendableController : Controller
    {
        public ExtendableController()
        {
            //if (this.CustomFields == null)
            //{
            //    this.CustomFields = new JavaScriptSerializer().Deserialize<List<FieldInfo>>(this.SerializedCustomFields);
            //}

            if (this.DynamicCustomFields == null)
            {
                this.DynamicCustomFields = new ExpandoObject();
                ((IDictionary<string, object>)this.DynamicCustomFields).Add("TopSpeed", 180);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), TypeConverter(typeof(GenericCollectionConverter))]
        public IList<FieldInfo> CustomFields
        {
            get
            {
                if (this.customFields == null)
                {
                    this.customFields = new JavaScriptSerializer().Deserialize<List<FieldInfo>>(this.SerializedCustomFields);
                }

                return this.customFields;
            }

            set 
            {
                this.customFields = value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), TypeConverter(typeof(CollectionJsonTypeConverter<FieldInfo>))]
        public Collection<FieldInfo> CustomFieldsJson
        {
            get
            {
                if (this.customFieldsJson == null)
                {
                    this.customFieldsJson = new JavaScriptSerializer().Deserialize<Collection<FieldInfo>>(this.SerializedCustomFields);
                }

                return this.customFieldsJson;
            }

            set
            {
                this.customFieldsJson = value;
            }
        }


        public string SerializedCustomFields
        {
            get
            {
                if (this.serializedCustomFields == null)
                {
                    var expectedFieldsFileName = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/Mvc/Views/Breadcrumb/custom-fields.json";

                    if (VirtualPathManager.FileExists(expectedFieldsFileName))
                    {
                        var fileStream = VirtualPathManager.OpenFile(expectedFieldsFileName);

                        using (var streamReader = new StreamReader(fileStream))
                        {
                            this.serializedCustomFields = streamReader.ReadToEnd();
                        }
                    }
                }

                return this.serializedCustomFields;
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public dynamic DynamicCustomFields { get; set; }

        private IList<FieldInfo> customFields;
        private string serializedCustomFields;
        private Collection<FieldInfo> customFieldsJson;

    }
}
