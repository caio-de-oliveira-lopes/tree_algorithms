namespace TreeAlgorithms
{
    public class Node
    {
        public int Key { get; set; }
        public int Height { get; set; }
        public Node? Left { get; set; }
        public Node? Right { get; set; }

        public Node(int d)
        {
            Key = d;
            Height = 1;
        }
    }
}
