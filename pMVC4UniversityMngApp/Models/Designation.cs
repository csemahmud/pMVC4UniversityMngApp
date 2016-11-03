using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace pMVC4UniversityMngApp.Models
{
    [Table("Designation")]
    public class Designation
    {
        public int DesignationID { set; get; }
        public string DsgName { set; get; }
        public virtual List<Teacher> TeacherList { set; get; }
    }
}
