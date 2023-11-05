using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Office.Interop.Excel;

namespace TreeAlgorithms
{
    public class Program
    {
        static readonly Random Random = new();
        static readonly int[] ArraySizes = new int[4] { 100, 1000, 10000, 100000 };
        static readonly bool LogTree = false;

        // Este dicionário será usado para compilar o resultado das análises para posterior escrita
        private static Dictionary<MyKey, MyResult> MyResults { get; set; } = new();

        // Struct para chave do dicionário de resultados
        public struct MyKey
        {
            public int ArrayLength { get; set; }
            public OrderCondition OrderCondition { get; set; }
            public Tree.TreeType Tree { get; set; }
            public bool IsSearch { get; set; }
        }

        // Struct para o valor do dicionário de resultados
        public struct MyResult
        {
            public long NumKeyComparison { get; set; }
            public long NumRotations { get; set; }
            public long ExecutionMs { get; set; }
        }

        public enum OrderCondition
        {
            Random,
            Ascending,
            Descending
        }

        public static bool AddToMyResults(MyKey key, MyResult result)
        {
            return MyResults.TryAdd(key, result);
        }

        public static MyResult SumResults(MyResult result1, MyResult result2)
        {
            return new MyResult()
            {
                NumKeyComparison = result1.NumKeyComparison + result2.NumKeyComparison,
                NumRotations = result1.NumRotations + result2.NumRotations,
                ExecutionMs = result1.ExecutionMs + result2.ExecutionMs
            };
        }

        public static int[] InitializeArray(int count)
        {
            // generate count random values.
            HashSet<int> candidates = new();
            while (candidates.Count < count)
            {
                // May strike a duplicate.
                candidates.Add(Random.Next(0, count));
            }

            // load them in to a list.
            List<int> result = new();
            result.AddRange(candidates);

            // shuffle the results:
            int i = result.Count;
            while (i > 1)
            {
                i--;
                int k = Random.Next(i + 1);
                (result[i], result[k]) = (result[k], result[i]);
            }
            return result.ToArray();
        }

        static Tree? GetNewTree(Tree.TreeType type)
        {
            if (type == Tree.TreeType.BST)
                return new BST();
            else if (type == Tree.TreeType.AVL)
                return new AVL();
            else
                return null;
        }

        public static T[] GetRandomElements<T>(IEnumerable<T> array, int elementsCount)
        {
            return array.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToArray();
        }

        static void Main(string[] args)
        {
            string outputPath = AppDomain.CurrentDomain.BaseDirectory;
            if (args.Length > 0)
                outputPath = args[0];

            MyResults.Clear();

            // Vetores de 100, 1000, 10000 e 100000 elementos
            int[] vet100 = InitializeArray(ArraySizes[0]);
            int[] vet1000 = InitializeArray(ArraySizes[1]);
            int[] vet10000 = InitializeArray(ArraySizes[2]);
            int[] vet100000 = InitializeArray(ArraySizes[3]);

            int[] elementsToSearch = GetRandomElements(vet100, 10);

            ArrayList myArrays = new() { vet100, vet1000/*, vet10000, vet100000 */};
            foreach (int[] array in myArrays)
            {
                // Três variações, array randômico, array ordenado de forma crescente e outro inversamente ordenado
                int[] randomArray = array;
                int[] ascendingArray = array.OrderBy(x => x).ToArray();
                int[] descendingArray = array.OrderByDescending(x => x).ToArray();

                Dictionary<OrderCondition, int[]> variations = new Dictionary<OrderCondition, int[]>
                {
                    { OrderCondition.Random, randomArray },
                    { OrderCondition.Ascending, ascendingArray },
                    { OrderCondition.Descending, descendingArray }
                };

                foreach (Tree.TreeType treeType in Enum.GetValues<Tree.TreeType>())
                {
                    foreach (OrderCondition orderingCondition in Enum.GetValues<OrderCondition>())
                    {
                        Tree? tree = GetNewTree(treeType);
                        if (tree == null) continue;

                        var selectedArray = variations[orderingCondition];
                        var insertionKey = new MyKey()
                        {
                            ArrayLength = array.Length,
                            OrderCondition = orderingCondition,
                            Tree = treeType,
                            IsSearch = false,
                        };

                        // Inserindo todos os elementos
                        // Contador de tempo
                        System.Diagnostics.Stopwatch watch = new();
                        watch.Start();
                        foreach (var item in selectedArray)
                        {
                            tree.Insert(item);
                            if (LogTree)
                            {
                                tree.PreOrder();
                                tree.ShowStructure();
                            }
                        }
                        watch.Stop();
                        tree.IncrementExecutionMs(watch.ElapsedMilliseconds);
                        AddToMyResults(insertionKey, tree.GetMyResult());

                        var searchKey = new MyKey()
                        {
                            ArrayLength = array.Length,
                            OrderCondition = orderingCondition,
                            Tree = treeType,
                            IsSearch = true,
                        };
                        tree.ResetMyResult();

                        // Buscando os 10 elementos selecionados
                        // Contador de tempo
                        watch = new();
                        watch.Start();
                        foreach (var item in elementsToSearch)
                        {
                            tree.Insert(item);
                            if (LogTree)
                            {
                                tree.PreOrder();
                                tree.ShowStructure();
                            }
                        }
                        watch.Stop();
                        tree.IncrementExecutionMs(watch.ElapsedMilliseconds);
                        AddToMyResults(searchKey, tree.GetMyResult());
                    }
                }
            }
            MyResultsToExcel(outputPath, elementsToSearch);
        }

        static void MyResultsToExcel(string outputPath, int[] searchedElements)
        {
            Application oXL = new();
            _Workbook oWB = oXL.Workbooks.Add(Missing.Value);

            System.Drawing.Color color = System.Drawing.Color.LightGray;

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            string[] labels;

            _Worksheet oSheet = (_Worksheet)oWB.ActiveSheet;
            _Worksheet first = oSheet;

            oSheet = oWB.Worksheets.Add();
            oSheet.Name = "Buscados";
            for (int i = 0; i < searchedElements.Length; i++)
                oSheet.Cells[1, i + 1].Value = searchedElements[i];

            oSheet.Columns.AutoFit();
            oSheet.Rows.AutoFit();

            oSheet.Rows.Borders.LineStyle = XlLineStyle.xlContinuous;
            oSheet.Columns.Borders.LineStyle = XlLineStyle.xlContinuous;

            oSheet.Columns.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            oSheet.Columns.VerticalAlignment = XlHAlign.xlHAlignCenter;

            oSheet.Rows.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            oSheet.Rows.VerticalAlignment = XlHAlign.xlHAlignCenter;

            foreach (Tree.TreeType treeType in Enum.GetValues<Tree.TreeType>().Reverse())
            {
                foreach (bool isSearch in new List<bool>() { true, false })
                {

                    if (treeType == Tree.TreeType.AVL)
                    {
                        labels = new string[6] { "Tipo de Árvore", "Ordenação Prévia do Vetor", "Tamanho do Vetor", 
                            "Número de Comparações", "Número de Rotações", "Tempo de Execução (ms)" };
                    }
                    else
                    {
                        labels = new string[5] { "Tipo de Árvore", "Ordenação Prévia do Vetor", "Tamanho do Vetor",
                            "Número de Comparações", "Tempo de Execução (ms)" };
                    }

                    oSheet = oWB.Worksheets.Add();
                    int column = 2;
                    int row = 1;
                    oSheet.Name = $"{treeType} - {(isSearch ? "Search" : "Insertion")}";
                    oSheet.Cells[row, column].Value = treeType.ToString();
                    oSheet.Cells[row, column].Interior.Color = System.Drawing.ColorTranslator.ToOle(color);
                    oSheet.Range[oSheet.Cells[row, column], oSheet.Cells[row, column + (ArraySizes.Length * Enum.GetValues<OrderCondition>().Length) - 1]].Merge();
                    row++;
                    foreach (OrderCondition orderCondition in Enum.GetValues<OrderCondition>())
                    {
                        oSheet.Cells[row, column].Value = orderCondition.ToString();
                        oSheet.Cells[row, column].Interior.Color = System.Drawing.ColorTranslator.ToOle(color);

                        oSheet.Range[oSheet.Cells[row, column], oSheet.Cells[row, column + (ArraySizes.Length - 1)]].Merge();
                        foreach (int value in ArraySizes)
                        {
                            oSheet.Cells[row + 1, column].Value = value;
                            oSheet.Cells[row + 1, column].Interior.Color = System.Drawing.ColorTranslator.ToOle(color);
                            MyKey key = new()
                            {
                                ArrayLength = value,
                                OrderCondition = orderCondition,
                                Tree = treeType,
                                IsSearch = isSearch
                            };
                            if (MyResults.TryGetValue(key, out MyResult result))
                            {
                                long[] data;
                                if (treeType == Tree.TreeType.AVL)
                                    data = new long[] { result.NumKeyComparison, result.NumRotations, result.ExecutionMs };
                                else
                                    data = new long[] { result.NumKeyComparison, result.ExecutionMs };

                                for (int i = 0; i < data.Length; i++)
                                    oSheet.Cells[row + i + 2, column].Value = data[i];
                            }
                            column++;
                        }
                    }
                    int start = 1;
                    for (int i = start; i < start + labels.Length; i++)
                    {
                        oSheet.Cells[i, 1].Value = labels[i - start];
                        oSheet.Cells[i, 1].Interior.Color = System.Drawing.ColorTranslator.ToOle(color);
                    }
                    oSheet.Columns.AutoFit();
                    oSheet.Rows.AutoFit();

                    oSheet.Rows.Borders.LineStyle = XlLineStyle.xlContinuous;
                    oSheet.Columns.Borders.LineStyle = XlLineStyle.xlContinuous;

                    oSheet.Columns.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    oSheet.Columns.VerticalAlignment = XlHAlign.xlHAlignCenter;

                    oSheet.Rows.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    oSheet.Rows.VerticalAlignment = XlHAlign.xlHAlignCenter;
                }
            }
            first.Delete();
            oWB.SaveAs(Path.Combine(@outputPath, "PCC534 - Análise De Ordenações.xlsx"));
            oXL.Visible = true;
            oXL.UserControl = true;
        }
    }
}