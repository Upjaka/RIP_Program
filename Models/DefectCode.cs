using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication2.Models
{
    public class DefectCode
    {
        public string Code { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public bool IsPouring { get; set; }

        public DefectCode(string code, string fullName, string shortName, bool isPouring)
        {
            Code = code;
            FullName = fullName;
            ShortName = shortName;
            IsPouring = isPouring;
        }
    }
}
