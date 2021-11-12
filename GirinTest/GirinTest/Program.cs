using System;
using System.IO;

namespace GirinTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            var listRandom = new ListRandom();

            const int testSize = 100;
            for (var i = 0; i < testSize; i++)
            {
                listRandom.AddToEnd(new ListNode() { Data = $"data {i}" });
            }

            listRandom.Serialize(new FileStream("test.dat", FileMode.Create));

            var otherListRandom = new ListRandom();
            otherListRandom.Deserialize(new FileStream("test.dat", FileMode.Open));

            var testResult = listRandom.IsEqualByState(otherListRandom) ? "success" : "fail";
            Console.WriteLine($"test result: {testResult}");
        }
    }
}