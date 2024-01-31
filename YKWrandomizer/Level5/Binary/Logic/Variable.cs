namespace YKWrandomizer.Level5.Binary.Logic
{
    public class Variable
    {
        public Type Type;
        public object Value;

        public Variable()
        {

        }

        public Variable(Type type, object value)
        {
            Type = type;
            Value = value;
        }

        public Variable(Variable variable)
        {
            Type = variable.Type;
            Value = variable.Value;
        }
    }
}
