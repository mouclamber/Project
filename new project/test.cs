using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp4;

static class Tests
{
    static int passed = 0;
    static int failed = 0;

    public static void RunAll()
    {
        Console.WriteLine("Запуск тестов\n");

        passed = 0;
        failed = 0;

        Test_ShortestTimeToTarget();
        Test_DistanceToStartIsZero();
        Test_PathRestoredCorrectly();
        Test_UnreachableVertex();
        Test_DetourIsShorterThanDirect();
        Test_SameStartAndEnd();
        Test_BranchingToLiverAndKidney();

        Console.WriteLine($"\nИтог: {passed} прошло, {failed} провалено.");
    }

    static void BuildGraph()
    {
        Program.graph.Clear();
        Program.AddEdge(0, 1, 2, "Плечевая вена");
        Program.AddEdge(1, 2, 1, "ПП -> ПЖ");
        Program.AddEdge(2, 3, 2, "Лёгочная артерия");
        Program.AddEdge(3, 4, 2, "Лёгочные вены");
        Program.AddEdge(4, 5, 1, "ЛП -> ЛЖ");
        Program.AddEdge(5, 6, 1, "Аорта");
        Program.AddEdge(6, 7, 1, "Сонная артерия");
        Program.AddEdge(7, 8, 1, "Сосуды мозга");
        Program.AddEdge(6, 9, 2, "Печёночная артерия");
        Program.AddEdge(6, 10, 2, "Почечная артерия");
        Program.AddEdge(8, 11, 1, "Сосуды опухоли");
    }

    static void Test_ShortestTimeToTarget()
    {
        BuildGraph();
        var (d, _) = Program.Dijkstra(0);
        Check("Кратчайшее время от 0 до 11 = 12", d[11] == 12);
    }

    static void Test_DistanceToStartIsZero()
    {
        BuildGraph();
        var (d, _) = Program.Dijkstra(0);
        Check("Расстояние до старта = 0", d[0] == 0);
    }

    static void Test_PathRestoredCorrectly()
    {
        BuildGraph();
        var (_, p) = Program.Dijkstra(0);
        var path = Program.RestorePath(p, 0, 11);
        var expected = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 11 };
        Check("Путь 0 -> 11 восстановлен верно", path.SequenceEqual(expected));
    }

    static void Test_UnreachableVertex()
    {
        Program.graph.Clear();
        Program.AddEdge(0, 1, 5, "A");
        Program.AddEdge(2, 3, 1, "B");
        var (d, _) = Program.Dijkstra(0);
        Check("Недостижимая вершина = int.MaxValue", d[2] == int.MaxValue);
    }

    static void Test_DetourIsShorterThanDirect()
    {
        Program.graph.Clear();
        Program.AddEdge(0, 2, 10, "Прямой");
        Program.AddEdge(0, 1, 3, "Обход 1");
        Program.AddEdge(1, 2, 3, "Обход 2");
        var (d, _) = Program.Dijkstra(0);
        Check("Выбран обход (6 < 10)", d[2] == 6);
    }

    static void Test_SameStartAndEnd()
    {
        BuildGraph();
        var (_, p) = Program.Dijkstra(0);
        var path = Program.RestorePath(p, 0, 0);
        Check("Путь при start == end = [0]", path.Count == 1 && path[0] == 0);
    }

    static void Test_BranchingToLiverAndKidney()
    {
        BuildGraph();
        var (d, _) = Program.Dijkstra(0);
        Check("До печени = 11", d[9] == 11);
        Check("До почек = 11", d[10] == 11);
    }

    static void Check(string name, bool condition)
    {
        if (condition) { Console.WriteLine($"[OK]   {name}"); passed++; }
        else           { Console.WriteLine($"[FAIL] {name}"); failed++; }
    }
}