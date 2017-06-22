using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysMM1
{
    class Symulacja
    {
        double czas_symulacji;
        int ilosc_zrodel = 0;
        Zrodlo[] zrodlo;
        //Pakiet[] pakiet;
        double[] lambda_zrodel;
        Serwer serwer;
        int [] dlugosci_pakietow;
        
        //zienne do statystyk
        double suma_pakietow = 0;
        double suma_czasow_przebywania_w_kolejce = 0;


        //Zmienne do głównej symulacji
        double najkrotszy_czas = 0;
        double obecny_czas = 0;
        double[] przechowywane_czasy;// = new double[ilosc_zrodel];
        double czas_serwera = 0;
        
        int j = 0; //zmienna do ustawiania "flagi" na którym źródle/pakiecie się znajdujemy (potrzebne w głównej symulacji)
        Pakiet pierwszy_w_kolejce = null;

        double dlugosc_kolejki = 0;
        double ilosc_odrzuconych_pakietow = 0;

        Queue<Pakiet> kolejka_fifo = new Queue<Pakiet>(); //nasza kolejka w serwerze dziala na zasadzie FIFO, więc używam Queue

        List<double> lista_czasow = new List<double>();



        public Symulacja()
        {
            Console.WriteLine("Podaj czas symulacji:");
            czas_symulacji = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Podaj maksymalna długość kolejki: ");
            dlugosc_kolejki = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Podaj ilosc zrodel:");
            ilosc_zrodel = Int32.Parse(Console.ReadLine());
            //pakiet = new Pakiet[ilosc_zrodel];
            zrodlo = new Zrodlo[ilosc_zrodel];
            serwer = new Serwer();
            lambda_zrodel = new double[ilosc_zrodel];
            dlugosci_pakietow = new int[ilosc_zrodel];
            przechowywane_czasy = new double[ilosc_zrodel];




            for (int i = 0; i < ilosc_zrodel; i++)
            {
                Console.WriteLine("Podaj lambde dla {0} źródła: ", i + 1);
                lambda_zrodel[i] = Double.Parse(Console.ReadLine());
                //Console.WriteLine("Podaj długość pakietu dla {0} żródła: ", i + 1);
                //dlugosci_pakietow[i] = Int32.Parse(Console.ReadLine());

                zrodlo[i] = new Zrodlo(lambda_zrodel[i], dlugosci_pakietow[i]);

            }

        }

        


        public void glownaSymulacja()
        {
            //CALE WYKONANIE PROGRAMU:

            //poniższe rzeczy wykonuja sie tylko raz


            for (int i = 0; i < ilosc_zrodel; i++)
            {
                //pobieramy najpierw pierwsze czasy:
                przechowywane_czasy[i] = zrodlo[i].zwrocCzasNastepnegoPakietu();
            }

            najkrotszy_czas = przechowywane_czasy[0];
            //zmienna zeby zapamietac ktore zrodlo poda najkrotszy czas


            for (int i = 0; i < ilosc_zrodel - 1; i++)
            {


                if (najkrotszy_czas > przechowywane_czasy[i + 1])
                {
                    najkrotszy_czas = przechowywane_czasy[i + 1];
                    //najkrotszy_czas = przechowywane_czasy[i];
                    j = i + 1;
                }


            } // po przejsciu tej petli mamy pierwsze czasy i najkrotszy czas kiedy trafi pierwszy pakiet do kolejki

            obecny_czas = obecny_czas + najkrotszy_czas; //przechodzimy do tego czasu
          //  kolejka_fifo.Enqueue(zrodlo[j].pakiet);

            kolejka_fifo.Enqueue(new Pakiet(zrodlo[j].pakiet.zwrocDlugoscPakietu(), zrodlo[j].pakiet.lambda, 0));
            suma_pakietow=suma_pakietow + 1; //zwiekszamy liczbe pakietow do statystyk
            //Po dodaniu losujemy dla najkrótszego źródła nowy czas kolejnego pakietu:

            zmniejszPrzechowywaneCzasy();
            /*
            for (int i = 0; i < ilosc_zrodel; i++)
            {
                przechowywane_czasy[i] = przechowywane_czasy[i] - najkrotszy_czas;
            }
            */
            przechowywane_czasy[j] = zrodlo[j].zwrocCzasNastepnegoPakietu();

            pierwszy_w_kolejce = kolejka_fifo.Peek();
            czas_serwera = pierwszy_w_kolejce.dlugosc;
            //czas_serwera = serwer.zwrocCzasObslugiObecnegoPakietu(lambda_zrodel[j]);
            
            kolejka_fifo.Dequeue();



            for (; ; )
            {

                if (obecny_czas >= czas_symulacji) //sprawdzamy czy nie konczy sie czas
                {

                    Console.WriteLine("Średni czas przebywania pakietu w kolejce: ");
                    Console.WriteLine("{0} ", suma_czasow_przebywania_w_kolejce / suma_pakietow);
                    Console.WriteLine("Poziom strat pakietów: ");
                    Console.WriteLine("{0} ", ilosc_odrzuconych_pakietow / suma_pakietow);

                    Console.WriteLine("Koniec symulacji!");
                    Console.WriteLine("Wciśnij dowolny przycisk aby wyjść.");

                    Console.ReadKey();
                    Environment.Exit(0);
                }



                najkrotszy_czas = przechowywane_czasy[0];
                j = 0;

                for (int i = 0; i < ilosc_zrodel; i++)
                {     //tutaj musi dzialac sprawdzanie i przechodzenie po czasach ładnie (nie zapomniec o serwerze!);

                    //najpierw sprawdzmy ktore zrodlo ma najmniejszy czas, pozniej porownajmy z serwerem

                    if (najkrotszy_czas > przechowywane_czasy[i])
                    {
                        najkrotszy_czas = przechowywane_czasy[i];
                        j = i;
                    }


                }


                if (najkrotszy_czas > czas_serwera) //porównanie najkrótszego czasu z serwerem
                {
                    najkrotszy_czas = czas_serwera;
                    pierwszy_w_kolejce = null;

                    if (kolejka_fifo.Count > 0)
                    {
                        
                        kolejkaDodatniaNajkrotszySerwer();
                        
                    }
                    else // tutaj jest ta skomplikowana, rzadka opcja, gdy kolejka = 0
                    {    // wtedy musimy ustalic najkrotszy czas jednego ze zrodel i przejsc do tego czasu, pomijajac zerowy czas serwera, bo ten czeka
                         //to wykonuje poniższa funkcja
                        kolejkaZeroNajkrotszySerwer();
                    }


                }
                else
                {
                    kolejkaDodatniaNajkrotszeJednoZeZrodel(); 
                    suma_pakietow = suma_pakietow + 1.0;
                }

                //a++;

            }
        }


        //Funkcje, które obsługują główną symulacje
        //Trzeba jeszcze jakoś dopisać zbieranie czasów tylko nie do końca wiadomo jakich i dlaczego akurat takich.


        public void kolejkaDodatniaNajkrotszeJednoZeZrodel()
        {
            //kolejka_fifo.Enqueue(zrodlo[j].pakiet);
            dodajCzasyObslugiDoPakietow();
            if (kolejka_fifo.Count < dlugosc_kolejki)
            {
                kolejka_fifo.Enqueue(new Pakiet(zrodlo[j].pakiet.zwrocDlugoscPakietu(), zrodlo[j].pakiet.lambda, 0));
            }
            else
            {
                ilosc_odrzuconych_pakietow += 1;
            }

            zmniejszPrzechowywaneCzasy(); //leci po czasach które mają źródła i je zmniejsza o najkrótszy czas
                                            //w wyniku tego przechodzimy odpowiednio po czasach

            czas_serwera = czas_serwera - najkrotszy_czas;
            obecny_czas = obecny_czas + najkrotszy_czas;
            przechowywane_czasy[j] = zrodlo[j].zwrocCzasNastepnegoPakietu();
        }


        public void kolejkaDodatniaNajkrotszySerwer()
        {
            pierwszy_w_kolejce = kolejka_fifo.Peek();
            czas_serwera = pierwszy_w_kolejce.dlugosc; //ZNACZNIK!!!
            lista_czasow.Add(pierwszy_w_kolejce.czas_oczekiwania_w_kolejce);
            suma_czasow_przebywania_w_kolejce += pierwszy_w_kolejce.czas_oczekiwania_w_kolejce;
            kolejka_fifo.Dequeue();

            dodajCzasyObslugiDoPakietow();
            
            obecny_czas = obecny_czas + najkrotszy_czas;
            // ZNACZNIK!!! czas_serwera = serwer.zwrocCzasObslugiObecnegoPakietu(pierwszy_w_kolejce.lambda);

            zmniejszPrzechowywaneCzasy();
            /*
            for (int i = 0; i < ilosc_zrodel; i++)
            {
                przechowywane_czasy[i] = przechowywane_czasy[i] - najkrotszy_czas;
            }*/
        }



        public void kolejkaZeroNajkrotszySerwer()
        {
            obecny_czas = obecny_czas + najkrotszy_czas;
            czas_serwera = czas_serwera - najkrotszy_czas; // czas serwera sie wyzeruje tutaj, bo byl najkrotszym czasem 
            // i nie mial co wziac z kolejki...

            zmniejszPrzechowywaneCzasy();

            najkrotszy_czas = przechowywane_czasy[0]; //z tad znowu szukamy najkrotszego czasu, tym razem pomijajac czas serwera bo = 0
            j = 0;

            for (int i = 0; i < ilosc_zrodel; i++)
            {       //tutaj musi dzialac sprawdzanie i przechodzenie po czasach ładnie;

                //najpierw sprawdzmy ktore zrodlo ma najmniejszy czas

                if (najkrotszy_czas > przechowywane_czasy[i])
                {
                    najkrotszy_czas = przechowywane_czasy[i];
                    j = i;
                }

                obecny_czas += najkrotszy_czas; //przechodzimy do chwili gdy jakies zrodlo podrzuca pakiet

                zmniejszPrzechowywaneCzasy();



                kolejka_fifo.Enqueue(zrodlo[j].pakiet); //pojawia sie pakiet w kolejce

                //pamietamy o zrodle, musi dostac nowy czas nastepnego pakietu
                przechowywane_czasy[j] = zrodlo[j].zwrocCzasNastepnegoPakietu();


                pierwszy_w_kolejce = kolejka_fifo.Peek(); //tutaj przechodzi ten paket od razu do zrodla (mamy te sama chwile czasu)
                czas_serwera = pierwszy_w_kolejce.dlugosc;
                lista_czasow.Add(pierwszy_w_kolejce.czas_oczekiwania_w_kolejce);
                suma_czasow_przebywania_w_kolejce += pierwszy_w_kolejce.czas_oczekiwania_w_kolejce;
                
                kolejka_fifo.Dequeue(); //usuwany z kolejki
                dodajCzasyObslugiDoPakietow();
                //czas_serwera = serwer.zwrocCzasObslugiObecnegoPakietu(pierwszy_w_kolejce.lambda); //czas obslugi nowego pakietu przez serwer

            }
        }


        public void zmniejszPrzechowywaneCzasy()
        {
            for (int i = 0; i < ilosc_zrodel; i++)
            {
                przechowywane_czasy[i] = przechowywane_czasy[i] - najkrotszy_czas;
            }
        }


        public void dodajCzasyObslugiDoPakietow()
        {
            foreach (Pakiet element in kolejka_fifo)
            {
                element.czas_oczekiwania_w_kolejce += najkrotszy_czas;

            }

        }



    }
}
