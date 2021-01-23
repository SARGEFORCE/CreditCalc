using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CreditCalc.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using CreditCalc.ServiceData;

namespace CreditCalc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        IEnumerable<Term> terms = new List<Term>
        {
            new Term { Id = 1, Name = "Дни" },
            new Term { Id = 2, Name = "Месяцы" }
        };

        IEnumerable<InterestRatePeriod> interestRatePeriods = new List<InterestRatePeriod>
        {
            new InterestRatePeriod { Id = 1, Name = "В день" },
            new InterestRatePeriod { Id = 2, Name = "В год" }
        };

        IEnumerable<PeriodInDays> periodInDays = new List<PeriodInDays>
        {
            new PeriodInDays { Days = 10, Name = "10 дней" },
            new PeriodInDays { Days = 15, Name = "15 дней" }
        };

        public IActionResult Index()
        {
            ViewBag.Terms = new SelectList(terms, "Id", "Name");
            ViewBag.InterestRatePeriods = new SelectList(interestRatePeriods, "Id", "Name");
            ViewBag.PeriodInDays = new SelectList(periodInDays, "Days", "Name");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Result(InputData inputForm)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            decimal sum = inputForm.sum;
            int termCount = inputForm.termCount;
            int termId = inputForm.termId;
            float interestRate = inputForm.interestRate;
            int interestRatePeriodId = inputForm.interestRatePeriodId;

            decimal P;
            if (inputForm.interestRatePeriodId == 1) 
                P = (decimal)(interestRate / 3000); 
            else P = (decimal)(interestRate / 1200);
            decimal K = (decimal)(Convert.ToDouble(P) + (Convert.ToDouble(P) / (Math.Pow((1 + Convert.ToDouble(P)), termCount) - 1)));
            decimal A = K * sum;

            PaymentsTable table = new PaymentsTable();

            table.payments.Add(new Payment
            {
                Id = 0,
                Date = DateTime.Now,
                MainPayment = A,
                RemainingPrincipalDebt = sum,
                PaymentAmountByInterest = sum * P,
                PaymentAmountByBody = A - (sum - A) * P
            });
            
            if(termCount > 1)
            {
                if(inputForm.interestRatePeriodId == 1)//в день 
                {
                    var days = inputForm.periodInDays;
                    var r = sum;
                    for (int i = 1; i < termCount; i++)
                    {
                        r = table.payments.FirstOrDefault(p => p.Id == i - 1).RemainingPrincipalDebt -
                            table.payments.FirstOrDefault(p => p.Id == i - 1).PaymentAmountByBody;
                        table.payments.Add(new Payment
                        {
                            Id = i,
                            Date = DateTime.Now.AddDays(i*days),
                            MainPayment = A,
                            RemainingPrincipalDebt = r,
                            PaymentAmountByInterest = r * P,
                            PaymentAmountByBody = A - r * P,
                        });
                    }
                }
                if (inputForm.interestRatePeriodId == 2)//в год
                {
                    var r = sum;
                    for (int i = 1; i < termCount; i++)
                    {
                        r = table.payments.FirstOrDefault(p => p.Id == i - 1).RemainingPrincipalDebt -
                            table.payments.FirstOrDefault(p => p.Id == i - 1).PaymentAmountByBody;
                        table.payments.Add(new Payment
                        {
                            Id = i,
                            Date = DateTime.Now.AddMonths(i),
                            MainPayment = A,
                            RemainingPrincipalDebt = r,
                            PaymentAmountByInterest = r * P,
                            PaymentAmountByBody = A - r * P,
                        });
                    }
                }
            }

            return View(table);
        }
    }
}