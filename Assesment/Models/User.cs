using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AssesmentProject.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string Mail { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> SkillSets { get; set; }
        public List<string> Hobby { get; set; }
    }
}
