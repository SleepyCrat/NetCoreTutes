using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSocketAndNetCore.Web
{
    public class Square
    {
        public int Id { get; set; }
        public string Color { get; set; }

        public static IEnumerable<Square> GetInitialSquares()
        {
            var colors = new string[] { "red", "green", "blue" };

            for (int i = 0; i < 10; i++)
            {
                var random = new Random();
                yield return new Square()
                {
                    Id = i,
                    Color = colors[random.Next(0, 2)]
                };
            }
        }
    }

    public class SquareChangeRequest
    {
        public int Id { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }

        public static SquareChangeRequest FromJson(string json)
        {
            return JsonConvert.DeserializeObject<SquareChangeRequest>(json);
        }
    }
}
