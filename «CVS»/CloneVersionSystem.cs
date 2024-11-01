using System.Collections.Generic;

namespace Clones
{
    public class CloneVersionSystem : ICloneVersionSystem
    {
        private List<Clone> clonesList = new List<Clone>();

        public string Execute(string commandQuery)
        {
            if (clonesList.Count == 0) clonesList.Add(new Clone());
            var commandParts = commandQuery.Split(' ');
            var actionCommand = commandParts[0];
            var targetCloneIndex = int.Parse(commandParts[1]) - 1;
            switch (actionCommand)
            {
                case "learn":
                    clonesList[targetCloneIndex].Learn(commandParts[2]);
                    break;
                case "rollback":
                    clonesList[targetCloneIndex].Rollback();
                    break;
                case "relearn":
                    clonesList[targetCloneIndex].Relearn();
                    break;
                case "clone":
                    var clonedClone = new Clone(clonesList[targetCloneIndex]);
                    clonesList.Add(clonedClone);
                    break;
                case "check":
                    return clonesList[targetCloneIndex].Check();
            }
            return null;
        }
    }

    public class Clone
    {
        private ItemsStack<string> studiedProgramsStack;
        private ItemsStack<string> discontinuedProgramsStack;

        public Clone()
        {
            studiedProgramsStack = new ItemsStack<string>();
            discontinuedProgramsStack = new ItemsStack<string>();
        }

        public Clone(Clone sourceClone)
        {
            studiedProgramsStack = new ItemsStack<string>()
            {
                TopOperation = sourceClone.studiedProgramsStack.TopOperation,
                TotalPrograms = sourceClone.studiedProgramsStack.TotalPrograms
            };

            discontinuedProgramsStack = new ItemsStack<string>()
            {
                TopOperation = sourceClone.discontinuedProgramsStack.TopOperation,
                TotalPrograms = sourceClone.discontinuedProgramsStack.TotalPrograms
            };
        }

        public void Learn(string program)
        {
            studiedProgramsStack.Push(program);
        }

        public void Rollback()
        {
            discontinuedProgramsStack.Push(studiedProgramsStack.Pop());
        }

        public void Relearn()
        {
            studiedProgramsStack.Push(discontinuedProgramsStack.Pop());
        }

        public string Check()
        {
            if (studiedProgramsStack.TotalPrograms == 0)
                return "basic";
            

            var temp = studiedProgramsStack.Pop();
            studiedProgramsStack.Push(temp);
            return temp;
        }
    }

    public class Items<T>
    {
        public T Value { get; set; }
        public Items<T> PreviousOperation { get; set; }
    }

    public class ItemsStack<T>
    {
        public Items<T> TopOperation { get; set; }
        public int TotalPrograms { get; set; }

        public void Push(T item)
        {
            var newItem = new Items<T> { Value = item, PreviousOperation = TopOperation };
            TopOperation = newItem;
            TotalPrograms++;
        }

        public T Pop()
        {
            var result = TopOperation.Value;
            TopOperation = TopOperation.PreviousOperation;
            TotalPrograms--;
            return result;
        }
    }
}