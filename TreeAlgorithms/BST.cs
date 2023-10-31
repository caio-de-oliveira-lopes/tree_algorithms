using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeAlgorithms
{
    public class BST : Tree
    {
        public BST() { }

        protected override Node? InsertNode(int key, Node? node = null)
        {
            if (node is null)
                return new Node(key);

            if (key < node.Key)
                return node.Left = InsertNode(key, node.Left);
            else if (key > node.Key)
                return node.Right = InsertNode(key, node.Right);
            else
                return node;
        }

        protected override Node? DeleteNode(int key, Node? root = null)
        {
            if (root is null)
                return root;

            if (key < root.Key)
                return root.Left = DeleteNode(key, root.Left);
            else if (key > root.Key)
                return root.Right = DeleteNode(key, root.Right);
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

                        return root.Right = DeleteNode(temp.Key, root.Right);
                    }
                }
            }

            return root;
        }
    }
}
