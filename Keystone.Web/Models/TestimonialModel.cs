
namespace Keystone.Web.Models
{
    using Keystone.Web.Models.Base;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class TestimonialModel : BaseModel
    {
        public int TestimonialId { get; set; }

        [Display(Name = "Writer Name")]
        public string WriterName { get; set; }

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Posted On")]
        public DateTime PostedOn { get; set; }

        [Display(Name = "Testimonial Content")]
        public string TestimonialContent { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}