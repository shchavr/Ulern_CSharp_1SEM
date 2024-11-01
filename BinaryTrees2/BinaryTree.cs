using System;
using System.Collections;
using System.Collections.Generic;

namespace BinaryTrees
{
    public class BinaryTree<T> : IEnumerable<T> where T : IComparable
    {
        private T nodeValue;
        private int nodeWeight = 1;
        private BinaryTree<T> leftChild;
        private BinaryTree<T> rightChild;
        private bool nodeInitialized;

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

        /// <summary>
        /// Добавляет новый узел с заданным ключом в двоичное дерево.
        /// </summary>
        /// <param name="currentNode">Текущий узел, в который добавляется новый узел.</param>
        /// <param name="key">Ключ, который нужно добавить в дерево.</param>
        private void AddToNode(BinaryTree<T> currentNode, T key)
        {
            currentNode.nodeWeight++;
            AddNodeToTree(currentNode, key);
        }

        /// <summary>
        /// Определяет, куда необходимо добавить новый узел (левый или правый дочерний узел).
        /// </summary>
        /// <param name="currentNode">Текущий узел, относительно которого определяется, 
        /// куда добавить новый узел.</param>
        /// <param name="key">Ключ нового узла.</param>
        private void AddNodeToTree(BinaryTree<T> currentNode, T key)
        {
            var compareResult = currentNode.nodeValue.CompareTo(key);
            if (compareResult > 0)
                AddToLeftChild(currentNode, key);
            else 
                AddToRightChild(currentNode, key); 
        }

        /// <summary>
        /// Добавляет новый узел с заданным ключом в качестве левого дочернего узла текущего узла.
        /// </summary>
        /// <param name="currentNode">Текущий узел, к которому добавляется новый левый дочерний узел.</param>
        /// <param name="key">Ключ нового левого дочернего узла.</param>
        private void AddToLeftChild(BinaryTree<T> currentNode, T key)
        {
            if (currentNode.leftChild == null)
                currentNode.leftChild = CreateNewNode(key);
            else
                AddToNode(currentNode.leftChild, key);
        }

        /// <summary>
        /// Добавляет новый узел с заданным ключом в качестве правого дочернего узла текущего узла.
        /// </summary>
        /// <param name="currentNode">Текущий узел, к которому добавляется новый правый дочерний узел.</param>
        /// <param name="key">Ключ нового правого дочернего узла.</param>
        private void AddToRightChild(BinaryTree<T> currentNode, T key)
        {
            if (currentNode.rightChild == null)
                currentNode.rightChild = CreateNewNode(key);
            else
                AddToNode(currentNode.rightChild, key);    
        }

        /// <summary>
        /// Создает новый узел двоичного дерева с заданным ключом.
        /// </summary>
        /// <param name="key">Ключ нового узла.</param>
        /// <returns>Новый узел двоичного дерева с заданным ключом.</returns>
        private BinaryTree<T> CreateNewNode(T key)
        {
            return new BinaryTree<T>()
            {
                nodeValue = key,
                nodeInitialized = true
            };
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

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= nodeWeight)
                    throw new ArgumentOutOfRangeException(nameof(index));

                var currentNode = this;
                while (true)
                {
                    var currentNodeIndex = (currentNode!.leftChild?.nodeWeight ?? 0) + 1;
                    if (index == currentNodeIndex - 1)
                        return currentNode.nodeValue;
                    if (index < currentNodeIndex - 1)
                        currentNode = currentNode.leftChild;
                    else
                    {
                        currentNode = currentNode.rightChild;
                        index -= currentNodeIndex;
                    }
                }
            }
        }

        public IEnumerator<T> GetEnumerator() => GetEnumeratorForNode(this);

        /// <summary>
        /// Возвращает перечислитель, который последовательно проходит все узлы двоичного дерева, 
        /// начиная с заданного корневого узла.
        /// </summary>
        /// <param name="rootNode">Корневой узел двоичного дерева, с которого начинается перечисление.</param>
        /// <returns>Перечислитель, который последовательно возвращает элементы двоичного дерева.</returns>
        private IEnumerator<T> GetEnumeratorForNode(BinaryTree<T> rootNode)
        {
            if (rootNode is not { nodeInitialized: true })
                yield break;
            var enumeratorTreeNode = GetEnumeratorForNode(rootNode.leftChild);
            while (enumeratorTreeNode.MoveNext())
                yield return enumeratorTreeNode.Current;
            yield return rootNode.nodeValue;
            enumeratorTreeNode = GetEnumeratorForNode(rootNode.rightChild);
            while (enumeratorTreeNode.MoveNext())
                yield return enumeratorTreeNode.Current;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}