using System.ComponentModel.DataAnnotations;


namespace CreditCalc.Models
{
    
    public class InputData
    {
        [Required(ErrorMessage ="Укажите сумму кредита!")]
        [Display(Name = "Сумма кредита")]
        [Range(10000, 1000000, ErrorMessage = "Вам доступно 10000-1000000.")]
        public decimal sum { get; set; } //сумма кредита

        [Required(ErrorMessage = "Укажите срок кредитования!")]
        [Display(Name = "Срок кредитования")]
        [Range(1, 60, ErrorMessage = "Некорректные данные! (1-60).")]
        public int termCount { get; set; } //срок кредита в днях или месяцах
        public int termId { get; set; } // дни/месяцы

        [Required(ErrorMessage = "Укажите процентную ставку!")]
        [Display(Name = "Процентная ставка")]
        [Range(0.01, 100, ErrorMessage = "Некорректные данные! (0.01-100).")]
        public float interestRate { get; set; } //процентная ставка в день или год
        public int interestRatePeriodId { get; set; } // день/год
        public int periodInDays { get; set; } //платёжный период в днях (10 или 15 дней)
    }
}
