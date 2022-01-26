
namespace WebApiSchoolManagement.Models
{
    internal class RequireAttribute : Attribute
    {
        public bool AllowEmptyStrings { get; set; }
    }
}