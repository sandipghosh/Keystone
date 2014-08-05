
namespace Keystone.Web.Models
{
    using System.Collections.Generic;
    public class OrderedImageModel
    {
        public OrderedImageModel()
        {
            this.OrderedImages = new List<string>();
        }
        public int OrderedItemCode { get; set; }
        public List<string> OrderedImages { get; set; }
    }
}