namespace Rivals
{
    public class OwnedLocation
    {
        internal int Owner;
        private int i;
        private object value;
        private int v;

        public OwnedLocation(int i, object value, int v)
        {
            this.i = i;
            this.value = value;
            this.v = v;
        }

        public object Location { get; internal set; }
        public int Distance { get; internal set; }
    }
}