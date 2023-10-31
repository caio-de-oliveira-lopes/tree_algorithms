namespace TreeAlgorithms
{
    public abstract class Tree
    {
        protected Tree() { }

        private Node? Root { get; set; }

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
