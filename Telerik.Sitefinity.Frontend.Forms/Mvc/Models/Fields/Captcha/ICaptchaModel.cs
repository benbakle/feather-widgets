namespace Telerik.Sitefinity.Frontend.Forms.Mvc.Models.Fields.Captcha
{
    /// <summary>
    /// This interface provides API for form Captcha field.
    /// </summary>
    public interface ICaptchaModel : IFormElementModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether to display only for unauthenticated users.
        /// </summary>
        /// <value>
        /// <c>true</c> if will be visible only for unauthenticated users; otherwise, <c>false</c>.
        /// </value>
        bool DisplayOnlyForUnauthenticatedUsers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Enable audio code checkbox is displayed.
        /// </summary>
        /// <value>
        /// <c>true</c> if Enable audio code checkbox will be displayed; otherwise, <c>false</c>.
        /// </value>
        bool EnableCaptchaAudio { get; set; }

        /// <summary>
        /// Should the render captcha.
        /// </summary>
        /// <returns></returns>
        bool ShouldRenderCaptcha();
    }
}
