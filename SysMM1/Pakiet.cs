using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysMM1
{
    class Pakiet
    {
        public int dlugosc { get; set; }
        public double lambda;

        public Pakiet(int dlugosc, double lambda)
        {
            this.dlugosc = dlugosc;
            this.lambda = lambda;
        }

        
    }
}
