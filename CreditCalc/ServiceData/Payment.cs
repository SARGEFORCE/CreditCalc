using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreditCalc.ServiceData
{
    public class Payment
    {
        public int Id;
        public DateTime Date;
        public decimal MainPayment;
        public decimal PaymentAmountByBody;
        public decimal PaymentAmountByInterest;
        public decimal RemainingPrincipalDebt;
    }
}
