using System;

namespace BinaryTrees
{
    public class BinaryTree<T> where T : IComparable
    {
        private T nodeValue;
        private BinaryTree<T> leftChild;
        private BinaryTree<T> rightChild;
        private bool nodeInitialized;

        public BinaryTree() => nodeInitialized = false;

        public BinaryTree(T value)
        {
            nodeValue = value;
            nodeInitialized = true;
        }

        public void Add(T key)
        {
            if (!nodeInitialized)
            {
                nodeValue = key;
                nodeInitialized = true;
                return;
            }

            var currentNode = this;
            AddToNode(currentNode, key);
        }

        public bool Contains(T key)
        {
            if (!nodeInitialized)
                return false;

            var currentNode = this;
            while (currentNode != null)
            {
                var compareResult = currentNode.nodeValue.CompareTo(key);
                if (compareResult == 0)
                    return true;
                currentNode = compareResult > 0 ? currentNode.leftChild : currentNode.rightChild;
            }

            return false;
        }

        /// <summary>
        /// Добавляет новый узел с указанным ключом в двоичное дерево.
        /// </summary>
        /// <param name="currentNode">Текущий узел в двоичном дереве.</param>
        /// <param name="key">Значение ключа для добавления в двоичное дерево.</param>
        private void AddToNode(BinaryTree<T> currentNode, T key)
        {
            AddToLeftChild(currentNode, key);

            if (currentNode.nodeValue.CompareTo(key) <= 0)
                AddToRightChild(currentNode, key);    
        }

        /// <summary>
        /// Добавляет новый узел с указанным ключом в левого потомка текущего узла.
        /// </summary>
        /// <param name="currentNode">Текущий узел в двоичном дереве.</param>
        /// <param name="key">Значение ключа для добавления в двоичное дерево.</param>
        private void AddToLeftChild(BinaryTree<T> currentNode, T key)
        {
            if (currentNode.nodeValue.CompareTo(key) > 0)
            {
                if (currentNode.leftChild != null)
                {
                    currentNode = currentNode.leftChild;
                    AddToNode(currentNode, key);
                }
                else
                    currentNode.leftChild = new BinaryTree<T>(key);
                
            }
        }
        /// <summary>
        /// Добавляет новый узел с указанным ключом в правого потомка текущего узла.
        /// </summary>
        /// <param name="currentNode">Текущий узел в двоичном дереве.</param>
        /// <param name="key">Значение ключа для добавления в двоичное дерево.</param>
        private void AddToRightChild(BinaryTree<T> currentNode, T key)
        {
            if (currentNode.rightChild != null)
            {
                currentNode = currentNode.rightChild;
                AddToNode(currentNode, key);
            }
            else
                currentNode.rightChild = new BinaryTree<T>(key);  
        }
    }
}


