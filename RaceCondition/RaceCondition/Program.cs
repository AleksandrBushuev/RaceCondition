using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RaceCondition
{
    public class Account
    {
        public int Sum { get; private set; }

        object stub = new object();
        public Account(int sum)
        {
            Sum = sum;
        }

        public bool TakeMoney(int sum)
        {
            //lock (stub) //race condition
            //{
                if (Sum - sum >= 0)
                {
                    Sum -= sum;
                    return true;
                }
                return false;
            //}
        }
    }

    public class Purchaser
    {

        private Account _account;

        public int Cost { get; private set; }

        public Purchaser(Account account)
        {
            _account = account;
        }

        public Task Purchase(int sum, int time)
        {
            bool isShopping = true;

            Timer timer = new Timer((obj) =>
            {
                isShopping = false;
            }, null, time, 1000);

                    
            return Task.Run(() =>
            {
                while (isShopping)
                {
                    if (_account.TakeMoney(sum))
                        Cost += sum;
                }
                timer.Dispose();
            });
        }
    }



    class Program
    {
        static async Task Main(string[] args)
        {
            int sum = 100000;
            Console.WriteLine($"Сумма средств на счете {sum}");
            Account account = new Account(sum);

            Purchaser purchaser1 = new Purchaser(account);
            Purchaser purchaser2 = new Purchaser(account);

            Console.WriteLine("Идет покупка товаров...");
            var shoping = new List<Task>()
            {
                purchaser1.Purchase(1, 1000),
                purchaser2.Purchase(1, 5000)
            };


            await Task.WhenAll(shoping.ToArray());

            Console.WriteLine($"Сумма потраченных средст {purchaser1.Cost + purchaser2.Cost } ") ;

            Console.ReadKey();

        }

       

    }
}
