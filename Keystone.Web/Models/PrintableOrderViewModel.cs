
namespace Keystone.Web.Models
{
    public class PrintableOrderViewModel
    {
        public int DraftId { get; set; }
        public int TemplateId { get; set; }
        public int TemplatePageId { get; set; }
        public string TemplateTitle { get; set; }
        public int OrderIndex { get; set; }
        public string FinalImageUrl { get; set; }
    }
}