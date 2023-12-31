﻿namespace TreeAlgorithms
{
    public class AVL : Tree
    {
        public AVL() 
        {
            Type = TreeType.AVL;
        }

        private static Node? RightRotate(Node? y)
        {
            if (y is null) return y;

            Node? x = y.Left;
            Node? T2 = x?.Right;

            if (x is not null)
                x.Right = y;

            y.Left = T2;

            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;

            if (x is not null)
                x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;

            return x;
        }

        private static Node? LeftRotate(Node? x)
        {
            if (x is null) return x;

            Node? y = x.Right;
            Node? T2 = y?.Left;

            if (y is not null)
                y.Left = x;

            x.Right = T2;

            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;
            if (y is not null)
                y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;

            return y;
        }

        private static int GetBalance(Node? N)
        {
            return N is null ? 0 : GetHeight(N.Left) - GetHeight(N.Right);
        }

        private static int GetHeight(Node? N)
        {
            if (N == null)
                return 0;
            return N.Height;
        }

        protected override Node? InsertNode(int key, Node? node = null)
        {
            IncrementKeyComparison();
            if (node is null)
                return new Node(key);

            if (key < node.Key)
                node.Left = InsertNode(key, node.Left);
            else if (key > node.Key)
                node.Right = InsertNode(key, node.Right);
            else
                return node;

            node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));

            int balance = GetBalance(node);

            // Left Left Case 
            if (balance > 1 && key < node.Left?.Key)
            {
                IncrementNumRotations();
                return RightRotate(node);
            }

            // Right Right Case 
            if (balance < -1 && key > node.Right?.Key)
            {
                IncrementNumRotations();
                return LeftRotate(node);
            }

            // Left Right Case 
            if (balance > 1 && key > node.Left?.Key)
            {
                IncrementNumRotations(2);
                node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            // Right Left Case 
            if (balance < -1 && key < node.Right?.Key)
            {
                IncrementNumRotations(2);
                node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }

            return node;
        }

        protected override Node? DeleteNode(int key, Node? root = null)
        {
            if (root is null)
                return root;

            if (key < root.Key)
                root.Left = DeleteNode(key, root.Left);
            else if (key > root.Key)
                root.Right = DeleteNode(key, root.Right);
            else
            {
                if ((root.Left == null) || (root.Right == null))
                {
                    Node? temp = null;
                    if (temp == root.Left)
                        temp = root.Right;
                    else
                        temp = root.Left;

                    if (temp == null)
                    {
                        //temp = root;
                        root = null;
                    }
                    else
                        root = temp;
                }
                else
                {
                    Node? temp = GetMinValueNode(root.Right);

                    if (temp != null)
                    {
                        root.Key = temp.Key;

                        root.Right = DeleteNode(temp.Key, root.Right);
                    }
                }
            }

            if (root == null)
                return root;

            root.Height = Math.Max(GetHeight(root.Left), GetHeight(root.Right)) + 1;

            int balance = GetBalance(root);

            // Left Left Case 
            if (balance > 1 && GetBalance(root.Left) >= 0)
                return RightRotate(root);

            // Left Right Case 
            if (balance > 1 && GetBalance(root.Left) < 0)
            {
                root.Left = LeftRotate(root.Left);
                return RightRotate(root);
            }

            // Right Right Case 
            if (balance < -1 && GetBalance(root.Right) <= 0)
                return LeftRotate(root);

            // Right Left Case 
            if (balance < -1 && GetBalance(root.Right) > 0)
            {
                root.Right = RightRotate(root.Right);
                return LeftRotate(root);
            }

            return root;
        }
    }
}
