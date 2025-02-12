using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Practice2.Model
{
    [MetadataType(typeof(EmployeeMetadata))]
    public partial class Employee
    {
        // Этот класс оставляем пустым, так как он используется только для связи с метаданными.
    }

    public class EmployeeMetadata // класс для валидации данных
    {
        [Required(ErrorMessage = "Имя обязательно для заполнения.")]
        [StringLength(10, ErrorMessage = "Имя не должно превышать 10 символов.")]
        public string First_name { get; set; }

        [Required(ErrorMessage = "Фамилия обязательна для заполнения.")]
        [StringLength(10, ErrorMessage = "Фамилия не должна превышать 10 символов.")]
        public string Second_name { get; set; }

        [StringLength(10, ErrorMessage = "Отчество не должно превышать 10 символов.")]
        public string Patronymic { get; set; }

        [Required(ErrorMessage = "Номер телефона обязателен для заполнения.")]
        [Phone(ErrorMessage = "Номер телефона должен быть действительным.")]
        public string Mobile_number { get; set; }
    }
}
