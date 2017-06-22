using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysMM1
{
    class Zrodlo
    {
        
        double lambda { get; set; }
        Random generator = new Random();
        public Pakiet pakiet;

        public Zrodlo(double _lambda, int dlugosc_pakietu)
        {
            
            lambda = _lambda;
            pakiet = new Pakiet(lambda, 0);
        }

        public double zwrocCzasNastepnegoPakietu()
        {
            double x = generator.NextDouble();
            return -Math.Log(1.0 - x) / lambda;
        }
    }
}
