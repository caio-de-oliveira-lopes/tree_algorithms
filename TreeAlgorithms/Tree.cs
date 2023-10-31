namespace TreeAlgorithms
{
    public class Tree
    {
        readonly Node? Root;

        public void PreOrder(Node? node)
        {
            if (node is not null)
            {
                Console.Write(node.Key + " ");
                PreOrder(node.Left);
                PreOrder(node.Right);
            }
        }
    }
}
