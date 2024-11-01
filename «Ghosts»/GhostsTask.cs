using System;
using System.Text;

namespace hashes
{
    public class GhostsTask :
        IFactory<Document>, IFactory<Vector>, IFactory<Segment>, IFactory<Cat>, IFactory<Robot>,
        IMagic
    {
        private readonly byte[] encodingBytes = { 1, 2, 3 };
        private readonly Vector vector;
        private readonly Segment segment;
        private readonly Document document;
        private readonly Cat cat;
        private readonly Robot robot;

        public GhostsTask()
        {
            vector = new Vector(0, 0);
            segment = new Segment(vector, vector);
            document = new Document("title", Encoding.Unicode, encodingBytes);
            cat = new Cat("Polina", "house", DateTime.Now);
            robot = new Robot("15032004");
        }

        public void DoMagic()
        {
            encodingBytes[0] = 4;
            vector.Add(new Vector(1, 1));
            cat.Rename("Lucifer");
            Robot.BatteryCapacity++;
        }

        Vector IFactory<Vector>.Create() => vector;

        Segment IFactory<Segment>.Create() => segment;

        Document IFactory<Document>.Create() => document;

        Cat IFactory<Cat>.Create() => cat;

        Robot IFactory<Robot>.Create() => robot;
    }
}

