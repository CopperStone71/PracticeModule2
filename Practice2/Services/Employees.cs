using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice2.Services
{
    ///<summary>
    /// класс для работы с ListView
    ///</summary>
    internal class Employees
    {
        public string Employee_code { get; set; }
        public string First_name { get; set; }
        public string Second_name { get; set; }
        public string Patronymic { get; set; }
        public string FullName { get; set; }
        public DateTime BornDate { get; set; }
        public string Gender { get; set; }
        public string Employee_type_code { get; set; }
        public decimal Wages { get; set; }
        public string PassportSerial { get; set; }
        public string PassportNumber { get; set; }
        public string Registration { get; set; }
        public string Email { get; set; }
        public string Mobile_number { get; set; }
        public string PhotoUrl { get; set; } = "default_photo.png";
    }
}
