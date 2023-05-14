using Homework4_17.Data;

namespace Homework4_17.Web.Models
{
    public class ViewImageViewModel
    {
        public List<int> ImageIds { get; set; }
        public Image Image { get; set; }
        public string Message { get; set; }
    }
}
