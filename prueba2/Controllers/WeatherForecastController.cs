using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
// using Newtonsoft.J]

namespace prueba2.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult Post([FromBody] requestBody data)
    {

            // string requestBody = new StreamReader(req.Body).ReadToEnd();
            // SomeHelper helper  = new SomeHelper();
            // requestBody data = (requestBody)helper.Deserialize<requestBody>(requestBody);

            int[] dimensions = Array.ConvertAll(data.dimensions.ToString().Split(','), int.Parse);
            int[] start = Array.ConvertAll(data.start.ToString().Split(','), int.Parse);
            int[] end = Array.ConvertAll(data.end.ToString().Split(','), int.Parse);

            string[] obstaclesStr = data.obstacles.ToString().Split(';');
            List<int[]> obstacles = new List<int[]>();
            foreach (string obstacle in obstaclesStr)
            {
                obstacles.Add(Array.ConvertAll(obstacle.Split(','), int.Parse));
            }

            
            // llamar a la función BFS para encontrar el camino óptimo
            (List<int[]> optimalPath, List<int[]> visited, int optimalCost, int[,] board) = BFS(dimensions, start, end, obstacles);

            // mostrar el resultado
            Response response = new Response() {
                caminosRealizados = visited.Count,
                caminosBuenos = optimalPath.Count,
                caminosOptimos = (optimalPath.Any() ? 1 : 0),
                pesoCaminoOptimo = (optimalPath.Any() ? optimalCost : 0)
            };

            // mostrar el tablero con el camino óptimo
            if (optimalPath.Any())
            {
                foreach (int[] pos in optimalPath)
                {
                    board[pos[0],pos[1]] = 4;    // 4 para representar el camino óptimo
                }
                board[start[0], start[1]] = 2;   // 2 para representar la posición inicial
                board[end[0], end[1]] = 3;       // 3 para representar la posición final
            }

            string boardString = "";

            response.board = new List<List<int>>(dimensions[0]);

            for (int i = 0; i < dimensions[0]; i++)
            {
                response.board.Add(new List<int>(dimensions[1]));
                for (int j = 0; j < dimensions[1]; j++)
                {
                    boardString = boardString + board[i,j].ToString() + ' ';
                    
                    response.board[i].Add(board[i, j]);
                }
                boardString = boardString + '\n';
            }

            response.printedBoard = boardString;

             return new OkObjectResult(response);
        // return null;
        // return Enumerable.Range(1, 5).Select(index => new Response
        // {
        //     Date = DateTime.Now.AddDays(index),
        //     TemperatureC = Random.Shared.Next(-20, 55),
        //     Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        // })
        // .ToArray();
    }


    private static (List<int[]>, List<int[]>, int, int[,]) BFS(int[] dimensions, int[] start, int[] end, List<int[]> obstacles)
        {
            int[,] board = new int[dimensions[0], dimensions[1]];
            foreach (int[] obs in obstacles)
            {
                board[obs[0], obs[1]] = 1;   // 1 para representar obstáculos
            }
            board[start[0], start[1]] = 2;   // 2 para representar la posición inicial
            board[end[0], end[1]] = 3;       // 3 para representar la posición final

            Queue<List<int[]>> queue = new Queue<List<int[]>>();
            queue.Enqueue(new List<int[]> { start });

            List<int[]> visited = new List<int[]> { start };
            List<int[]> optimalPath = new List<int[]>();
            int optimalCost = int.MaxValue;

            while (queue.Count > 0)
            {
                List<int[]> path = queue.Dequeue();
                int[] current = path.Last();

                if (current.SequenceEqual(end))
                {
                    int pathCost = path.Count - 1;   // no contar la posición inicial
                    if (pathCost < optimalCost)
                    {
                        optimalPath = path;
                        optimalCost = pathCost;
                    }
                }

                List<int[]> neighbors = GetNeighbors(current, board);
                foreach (int[] neighbor in neighbors)
                {
                    if (!visited.Any(x => x.SequenceEqual(neighbor)))
                    {
                        List<int[]> newPath = new List<int[]>(path);
                        newPath.Add(neighbor);
                        queue.Enqueue(newPath);
                        visited.Add(neighbor);
                    }
                }
            }

            return ( optimalPath, visited, optimalCost, board);
        }

        private static List<int[]> GetNeighbors(int[] pos, int[,] board)
        {
            List<int[]> neighbors = new List<int[]>();

            int x = pos[0];
            int y = pos[1];

            if (x > 0 && board[x - 1, y] != 1)   // arriba
            {
                neighbors.Add(new int[] { x - 1, y });
            }
            if (x < board.GetLength(0) - 1 && board[x + 1, y] != 1)   // abajo
            {
                neighbors.Add(new int[] { x + 1, y });
            }
            if (y > 0 && board[x, y - 1] != 1)   // izquierda
            {
                neighbors.Add(new int[] { x, y - 1 });
            }
            if (y < board.GetLength(1) - 1 && board[x, y + 1] != 1)   // derecha
            {
                neighbors.Add(new int[] { x, y + 1 });
            }

            return neighbors;
        }
}
