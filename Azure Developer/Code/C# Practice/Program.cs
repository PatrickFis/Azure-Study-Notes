using System;

namespace MyApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            LambdaPractice();
            LinqPractice();
            await AsyncPractice.Run();
        }

        public static void LambdaPractice()
        {
            Func<int, int> square = x => x * x;
            Console.WriteLine(square(5));

            List<string> list = new List<string>();
            list.Add("String 1");
            list.Add("String 2");
            list.Add("String 3");
            list.ForEach(s => Console.WriteLine(s));
        }

        public static void LinqPractice()
        {
            int[] values = { 1, 2, 3, 4, 5 };

            IEnumerable<int> valuesQuery =
            from i in values
            where i > 3
            select i;

            foreach (int i in valuesQuery)
            {
                Console.Write(i + " ");
            }

            Console.WriteLine();
        }
    }

    public class AsyncPractice
    {
        public static async Task Run()
        {
            Task<int> firstTask = Task1();
            Task<int> secondTask = Task2();
            Task<int> thirdTask = Task3();

            var allTasks = new List<Task> { firstTask, secondTask, thirdTask };
            while (allTasks.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(allTasks);
                if (finishedTask == firstTask)
                {
                    Console.WriteLine("First task finished");
                }
                else if (finishedTask == secondTask)
                {
                    Console.WriteLine("Second task finished");
                }
                else if (finishedTask == thirdTask)
                {
                    Console.WriteLine("Third task finished");
                }
                allTasks.Remove(finishedTask);
            }
        }

        private static async Task<int> Task1()
        {
            Console.WriteLine("First task started...");
            await Task.Delay(3000);
            return 1;
        }

        private static async Task<int> Task2()
        {
            Console.WriteLine("Second task started...");
            await Task.Delay(2000);
            return 2;
        }

        private static async Task<int> Task3()
        {
            Console.WriteLine("Third task started...");
            await Task.Delay(1000);
            return 3;
        }
    }
}