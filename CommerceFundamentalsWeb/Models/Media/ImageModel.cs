using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;

namespace CommerceFundamentalsWeb.Models.Media
{
    [ContentType(DisplayName = "ImageModel", GUID = "cb418237-9254-4192-b79d-1de0661151bf", Description = "")]
    [MediaDescriptor(ExtensionString = "jpg,jpeg,jpe,ico,gif,bmp,png")]
    public class ImageModel : ImageData
    {
        public virtual string imageDescription { get; set; }

    }
}