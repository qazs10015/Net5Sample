using webapi1.Models;

namespace webapi1.ViewModels
{
    public class vDepartment
    {
        public string Name { get; set; }
        public decimal Budget { get; set; }
        public virtual Person Instructor { get; set; }


    }
}