using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysMM1
{
    class Pakiet
    {
        public double dlugosc { get; set; }
        public double lambda;
        public double czas_oczekiwania_w_kolejce { get; set; }
        public double h = 1.0; //na stałe od MS podany parametr(!)
        Random generator = new Random();

        public Pakiet(double dlugosc, double lambda, double czas_oczekiwania_w_kolejce)
        {
            this.dlugosc = dlugosc;
            this.lambda = lambda;
            this.czas_oczekiwania_w_kolejce = czas_oczekiwania_w_kolejce;
            
        }

        public Pakiet(double lambda, double czas_oczekiwania_w_kolejce)
        {
            this.lambda = lambda;
            this.czas_oczekiwania_w_kolejce = czas_oczekiwania_w_kolejce;
            dlugosc = zwrocDlugoscPakietu();
        }



        public double zwrocDlugoscPakietu()
        {
            double x = generator.NextDouble();
            return -Math.Log(1.0 - x) / h;

        }

        
    }
}
