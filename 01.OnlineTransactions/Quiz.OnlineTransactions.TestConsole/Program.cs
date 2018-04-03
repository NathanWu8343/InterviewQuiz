﻿using Quiz.OnlineTransactions.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quiz.OnlineTransactions.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // skip DI, 建立指定的 account 實做機制
            //AccountBase bank = new Practices.LockAccount();
            //AccountBase bank = new Practices.WithoutLockAccount();
            //AccountBase bank = new Practices.TransactionAccount() { Name = "andrew" };
            AccountBase bank = new Practices.DistributedLockAccount() { Name = "andrew" };


            long concurrent_threads = 20;
            long repeat_count = 1000;
            decimal origin_balance = bank.GetBalance();

            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < concurrent_threads; i++)
            {
                Thread t = new Thread(() => {
                    for (int j = 0; j < repeat_count; j++)
                    {
                        bank.ExecTransaction(1);
                    }
                });
                threads.Add(t);
            }

            Stopwatch timer = new Stopwatch();

            timer.Restart();
            foreach (Thread t in threads) t.Start();
            foreach (Thread t in threads) t.Join();


            decimal expected_balance = origin_balance + concurrent_threads * repeat_count;
            decimal actual_balance = bank.GetBalance();

            Console.WriteLine( "Test Result for {1}: {0}!", (expected_balance == actual_balance)?("PASS"):("FAIL"), bank.GetType().Name);
            Console.WriteLine($"- Expected Balance: {expected_balance}");
            Console.WriteLine($"- Actual Balance: {actual_balance}");
            Console.WriteLine($"- Performance: {concurrent_threads * repeat_count * 1000 / timer.ElapsedMilliseconds} trans/sec");
        }
    }
}
