using System.Xml.Linq;
using static TreeAlgorithms.Program;

namespace TreeAlgorithms
{
    public abstract class Tree
    {
        private MyResult MyResult { get; set; }

        protected Tree() 
        {
            MyResult = new MyResult();
        }

        protected void IncrementKeyComparison(long ammount = 1)
        {
            MyResult = SumResults(MyResult, new MyResult()
            {
                NumKeyComparison = ammount,
                NumRotations = 0,
                ExecutionMs = 0,
            });
        }

        protected void IncrementNumRotations(long ammount = 1)
        {
            MyResult = SumResults(MyResult, new MyResult()
            {
                NumKeyComparison = 0,
                NumRotations = ammount,
                ExecutionMs = 0,
            });
        }

        public void IncrementExecutionMs(long executionMs)
        {
            MyResult = SumResults(MyResult, new MyResult()
            {
                NumKeyComparison = 0,
                NumRotations = 0,
                ExecutionMs = executionMs,
            });
        }

        public void ResetMyResult()
        {
            MyResult = new MyResult();
        }

        public MyResult GetMyResult()
        {
            return MyResult;
        }

        private Node? Root { get; set; }

        protected TreeType Type { get; set; }

        public enum TreeType
        {
            BST,
            AVL
        }

        protected abstract Node? InsertNode(int key, Node? node = null);

        protected abstract Node? DeleteNode(int key, Node? root = null);

        public void Insert(int key)
        {
            Root = InsertNode(key, Root);
        }

        public void Delete(int key)
        {
            Root = DeleteNode(key, Root);
        }

        protected static Node? GetMinValueNode(Node? node)
        {
            Node? current = node;

            if (current == null) return current;

            while (current.Left is not null)
                current = current.Left;

            return current;
        }

        public Node? GetMinValueNode()
        {
            return GetMinValueNode(Root);
        }

        public void PreOrder()
        {
            PreOrder(Root);
            Console.WriteLine("\n");
        }

        public void ShowStructure()
        {
            ShowStructure(Root);
            Console.WriteLine();
        }

        public void PreOrder(Node? node)
        {
            if (node is not null)
            {
                Console.Write(node.Key + " ");
                PreOrder(node.Left);
                PreOrder(node.Right);
            }
        }

        public void ShowStructure(Node? node)
        {
            string none = "-";
            if (node is not null)
            {
                Console.WriteLine($"{node.Key}: [{(node.Left is not null ? node.Left.Key : none)}, {(node.Right is not null ? node.Right.Key : none)}]");
                ShowStructure(node.Left);
                ShowStructure(node.Right);
            }
        }
    }
}
