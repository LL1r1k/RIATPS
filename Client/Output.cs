using System;
using System.Linq;

namespace Client
{
    public class Output
    {
        public decimal SumResult { get; set; }
        public int MulResult { get; set; }
        public decimal[] SortedInputs { get; set; }

        public Output()
        {

        }

        public Output(Input input)
        {
            SumResult = 0;
            foreach (var i in input.Sums)
                SumResult += i;
            SumResult *= input.K;

            MulResult = 1;
            foreach (var i in input.Muls)
                MulResult *= i;

            decimal[] decMuls = Array.ConvertAll(input.Muls, x => (decimal)x);
            SortedInputs = new decimal[input.Sums.Length + decMuls.Length];

            input.Sums.CopyTo(SortedInputs, 0);
            decMuls.CopyTo(SortedInputs, input.Sums.Length);

            Array.Sort(SortedInputs);
        }
    }
}
