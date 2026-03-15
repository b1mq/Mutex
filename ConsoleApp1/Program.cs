
using System.IO;
using System.Numerics;

namespace Application
{
    class Program
    {
        const int SIZE = 100;
        const int THREAD_COUNT = 4;
        static int[,] A = new int[SIZE, SIZE];
        static int[,] B = new int[SIZE, SIZE];
        static long[,] C = new long[SIZE, SIZE];
        const int Size2 = 200000;
        static long[] Arr2 = new long[Size2];

        const int part = 5;
        const int PartSize = Size2 / part;
        public static Lock Locker = new Lock();
        static long[] a1 = new long[PartSize];
        static long[] a2 = new long[PartSize];
        static long[] a3 = new long[PartSize];
        static long[] a4 = new long[PartSize];
        static long[] a5 = new long[PartSize];
        static List<long> result = new List<long>();
        public class Player
        {
            public int Age { get; set; }
            public string Name { get; set; }
            public int Count { get; set; }
        }
        static List<Player> players = new List<Player>
        {
            new Player { Age = 21, Name = "Alice", Count = 0 },
            new Player { Age = 22, Name = "Bob", Count = 1 },
            new Player { Age = 23, Name = "Charlie", Count = 2 },
            new Player { Age = 24, Name = "David", Count = 3 },
            new Player { Age = 25, Name = "Eva", Count = 4 },
            new Player { Age = 26, Name = "Frank", Count = 5 },
            new Player { Age = 27, Name = "Grace", Count = 6 },
            new Player { Age = 28, Name = "Henry", Count = 7 },
            new Player { Age = 29, Name = "Ivy", Count = 8 },
            new Player { Age = 30, Name = "Jack", Count = 9 }
        };
        static List<(string, int)> scenes = new()
    {
        ("Forest", 2), ("City", 4), ("Ocean", 1), ("Mountain", 5), ("Space", 3),
        ("Jungle", 2), ("Desert", 4), ("River", 1), ("Volcano", 5), ("Cave", 3),
        ("Sky", 2), ("Underwater", 4), ("Castle", 1), ("Robot", 5), ("Dragon", 3)
    };

        static List<object[]> renderResults = new();
        static Semaphore gpuSem = new(4, 4);
        static object lk = new();
        static int gpuCnt = 0;

        static void Render((string name, int diff) s)
        {
            gpuSem.WaitOne();

            int g;
            lock (lk)
            {
                g = (gpuCnt % 4) + 1;
                gpuCnt++;
            }

            int t = s.diff;
            Thread.Sleep(t * 1000);

            gpuSem.Release();

            lock (lk)
            {
                renderResults.Add(new object[] { s.name, s.diff, t, g });
            }
        }

        static List<(string, int, string)> jobsHigh = new()
    {
        ("Contract.pdf", 12, "high"), ("UrgentMemo.docx", 5, "high"),
        ("Presentation.pptx", 20, "high"), ("ReportQ1.xlsx", 8, "high"),
        ("Invoice1.pdf", 3, "high"), ("ApprovalForm.doc", 2, "high")
    };

        static List<(string, int, string)> jobsLow = new()
    {
        ("Newsletter.html", 15, "low"), ("BackupData.zip", 30, "low"),
        ("OldLog.txt", 7, "low"), ("DraftNotes.odt", 4, "low"),
        ("SpamMail.eml", 1, "low"), ("Archive.rar", 25, "low")
    };

        static List<string> printOrder = new();
        static List<double> waitHigh = new();
        static List<double> waitLow = new();
        static Semaphore printer = new(1, 1);
        static object printLk = new();

        static void Print(string name, int pages, string prio)
        {
            double q = DateTime.UtcNow.Ticks / 10_000_000.0;
            printer.WaitOne();
            double st = DateTime.UtcNow.Ticks / 10_000_000.0;
            Thread.Sleep(pages * 50);
            printer.Release();
            double w = st - q;

            lock (printLk)
            {
                printOrder.Add(name);
                if (prio == "high") waitHigh.Add(w);
                else waitLow.Add(w);
            }
        }

        public static Mutex mutex = new();

        static void Main(string[] args)
        {
            //Random rnd = new Random(42);
            //for (int i = 0; i < SIZE; i++)
            //    for (int j = 0; j < SIZE; j++)
            //    {
            //        A[i, j] = rnd.Next(1, 10);
            //        B[i, j] = rnd.Next(1, 10);
            //    }
            //int rowsPerThread = SIZE / THREAD_COUNT;
            //Thread[] threads = new Thread[THREAD_COUNT];
            //for (int t = 0; t < THREAD_COUNT; t++)
            //{
            //    int startRow = t * rowsPerThread;
            //    int endRow = (t == THREAD_COUNT - 1) ? SIZE : startRow + rowsPerThread;

            //    threads[t] = new Thread(() =>
            //    {
            //        long[,] localRows = new long[endRow - startRow, SIZE];

            //        for (int i = 0; i < endRow - startRow; i++)
            //            for (int j = 0; j < SIZE; j++)
            //            {
            //                long sum = 0;
            //                for (int k = 0; k < SIZE; k++)
            //                    sum += A[startRow + i, k] * B[k, j];
            //                localRows[i, j] = sum;
            //            }

            //        mutex.WaitOne();
            //        try
            //        {
            //            for (int i = 0; i < endRow - startRow; i++)
            //                for (int j = 0; j < SIZE; j++)
            //                    C[startRow + i, j] = localRows[i, j];
            //        }
            //        finally
            //        {
            //            mutex.ReleaseMutex();
            //        }
            //    });

            //    threads[t].Start();
            //}

            //foreach (var thread in threads)
            //    thread.Join();

            //Array.Copy(Arr2, 0 * PartSize, a1, 0, PartSize);
            //Array.Copy(Arr2, 1 * PartSize, a2, 0, PartSize);
            //Array.Copy(Arr2, 2 * PartSize, a3, 0, PartSize);
            //Array.Copy(Arr2, 3 * PartSize, a4, 0, PartSize);
            //Array.Copy(Arr2, 4 * PartSize, a5, 0, PartSize);
            //var t1 = new Thread(() => result = Find7ToVector(a1));
            //var t2 = new Thread(() => result = Find7ToVector(a2));
            //var t3 = new Thread(() => result = Find7ToVector(a3));
            //var t5 = new Thread(() => result = Find7ToVector(a5));
            //var t4 = new Thread(() => result = Find7ToVector(a4));
            //t1.Start();
            //t2.Start();
            //t3.Start();
            //t4.Start();
            //t5.Start();
            //t1.Join();
            //t2.Join();
            //t3.Join();
            //t4.Join();
            //t5.Join();
            //DisplayList(result);
            var ts = new List<Thread>();

            foreach (var sc in scenes)
            {
                var t = new Thread(() => Render(sc));
                ts.Add(t);
                t.Start();
            }

            foreach (var t in ts) t.Join();
            ts.Clear();

            renderResults.Sort((a, b) => string.Compare((string)a[0], (string)b[0]));

            foreach (var r in renderResults)
                Console.WriteLine($"{r[0]}\t{r[1]}\t{r[2]}\tGPU{r[3]}");

            foreach (var j in jobsHigh)
            {
                var t = new Thread(() => Print(j.Item1, j.Item2, j.Item3));
                ts.Add(t);
                t.Start();
            }

            Thread.Sleep(300);

            foreach (var j in jobsLow)
            {
                var t = new Thread(() => Print(j.Item1, j.Item2, j.Item3));
                ts.Add(t);
                t.Start();
            }

            foreach (var t in ts) t.Join();

            foreach (var doc in printOrder)
                Console.WriteLine(doc);

            double ah = waitHigh.Any() ? waitHigh.Average() : 0;
            double al = waitLow.Any() ? waitLow.Average() : 0;

            Console.WriteLine(ah);
            Console.WriteLine(al);
        }
        public static Player RandomCountsPlayer(Player player)
        {
            lock (Locker) 
            {
                var random = new Random();

                for (int i = 0; i < 5; i++)
                {
                    player.Count += random.Next(0, 10);
                    Thread.Sleep(2000);
                }
                return player;
            }

        }
        //public static void DisplayList(List<long> a) => a.ForEach(x => Console.WriteLine(x));
        //public static List<long> Find7ToVector(long[] arr)
        //{
        //    var res = new List<long>();

        //    res.AddRange(arr.Where(x => x % 7 == 0));
        //    return res;
        //}
        //public static long[] FullArr(long[] arr)
        //{
        //    for (int i = 0; i < arr.Length; i++)
        //    {
        //        arr[i] = new Random().Next();
        //    }
        //    return arr;
        //}


    }
}