
namespace Keystone.Web.Models
{
    public class DraftUrlModel
    {
        public int TemplateId { get; set; }
        public int TemplatePageId { get; set; }
        public string DraftImageUrl { get; set; }
        public string FileImageUrl { get; set; }
        public string DraftImageJson { get; set; }
    }
}