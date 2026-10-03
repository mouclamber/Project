using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp4;

class Program
{
    internal class Edge
    {
        public int To { get; set; }
        public int Weight { get; set; }
        public string Name { get; set; }

        public Edge(int to, int weight, string name)
        {
            To = to;
            Weight = weight;
            Name = name;
        }
    }

    internal static Dictionary<int, List<Edge>> graph = new Dictionary<int, List<Edge>>();

    internal static Dictionary<int, string> names = new Dictionary<int, string>
    {
        { 0, "Вена руки (место инъекции)" },
        { 1, "Правое предсердие" },
        { 2, "Правый желудочек" },
        { 3, "Лёгкие (малый круг)" },
        { 4, "Левое предсердие" },
        { 5, "Левый желудочек" },
        { 6, "Аорта" },
        { 7, "Сонная артерия" },
        { 8, "Головной мозг" },
        { 9, "Печень" },
        { 10, "Почки" },
        { 11, "Опухоль в мозге" }
    };

    static void Main(string[] args)
    {
        if (args.Contains("test"))
        {
            Tests.RunAll();
            Console.WriteLine();
            RunDemo(1, 11);
            Console.ReadKey();
            return;
        }

        if (args.Contains("demo"))
        {
            RunDemo(1, 11);
            return;
        }

        Tests.RunAll();
        Console.WriteLine();
        RunDemo(1, 11);
        Console.ReadKey();
    }

    static void RunDemo(int start, int target)
    {
        graph.Clear();

        AddEdge(0, 1, 2, "Плечевая вена → верхняя полая вена");
        AddEdge(1, 2, 1, "Правое предсердие → правый желудочек");
        AddEdge(2, 3, 2, "Лёгочная артерия");
        AddEdge(3, 4, 2, "Лёгочные вены");
        AddEdge(4, 5, 1, "Левое предсердие → левый желудочек");
        AddEdge(5, 6, 1, "Аорта");
        AddEdge(6, 7, 1, "Сонная артерия");
        AddEdge(7, 8, 1, "Сосуды мозга");
        AddEdge(6, 9, 2, "Печёночная артерия");
        AddEdge(6, 10, 2, "Почечная артерия");
        AddEdge(8, 11, 1, "Сосуды опухоли");

        var (distances, previous) = Dijkstra(start);

        if (!distances.ContainsKey(target) || distances[target] == int.MaxValue)
        {
            Console.WriteLine("Препарат не достигнет цели.");
            return;
        }

        Console.WriteLine($"Препарат достигнет цели за {distances[target]} сек.\n");
        var path = RestorePath(previous, start, target);

        Console.WriteLine("Маршрут препарата:");
        for (int i = 0; i < path.Count; i++)
        {
            Console.WriteLine(names[path[i]]);
            if (i < path.Count - 1)
            {
                var edge = graph[path[i]].First(e => e.To == path[i + 1]);
                Console.WriteLine($"   --[{edge.Name}, {edge.Weight} с]-->");
            }
        }
    }

    internal static void AddEdge(int from, int to, int weight, string name)
    {
        if (!graph.ContainsKey(from)) graph[from] = new List<Edge>();
        if (!graph.ContainsKey(to)) graph[to] = new List<Edge>();

        graph[from].Add(new Edge(to, weight, name));
    }

    internal static (Dictionary<int, int> distances, Dictionary<int, int> previous) Dijkstra(int start)
    {
        var distances = new Dictionary<int, int>();
        var previous = new Dictionary<int, int>();
        var visited = new HashSet<int>();
        var priorityQueue = new PriorityQueue<int, int>();

        foreach (var vertex in graph.Keys)
        {
            distances[vertex] = int.MaxValue;
        }
        distances[start] = 0;

        priorityQueue.Enqueue(start, 0);

        while (priorityQueue.Count > 0)
        {
            int current = priorityQueue.Dequeue();

            if (visited.Contains(current)) continue;
            visited.Add(current);

            foreach (var edge in graph[current])
            {
                int newDistance = distances[current] + edge.Weight;

                if (newDistance < distances[edge.To])
                {
                    distances[edge.To] = newDistance;
                    previous[edge.To] = current;
                    priorityQueue.Enqueue(edge.To, newDistance);
                }
            }
        }

        return (distances, previous);
    }

    internal static List<int> RestorePath(Dictionary<int, int> previous, int start, int end)
    {
        var path = new List<int>();
        int current = end;

        while (current != start)
        {
            path.Add(current);
            if (!previous.ContainsKey(current)) return new List<int>();
            current = previous[current];
        }

        path.Add(start);
        path.Reverse();
        return path;
    }
}