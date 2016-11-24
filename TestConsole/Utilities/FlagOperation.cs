namespace TestConsole.Utilities
{
    using System;

    [Flags]
    public enum MessageGeneratorType
    {
        Publish = 1,
        Sync = 2,
        Dependency = 4,
        All = Publish | Sync | Dependency,
    }

    public class JO1 : IDisposable
    {
        public int Val { get; set; } = 5;

        public void Dispose()
        { }
    }

    public class JO2
    {
        public int Val { get; set; }

        public MessageGeneratorType Type { get; set; } = MessageGeneratorType.All;
    }

    public interface ITest
    {
        void Test(MessageGeneratorType type = MessageGeneratorType.Publish | MessageGeneratorType.Sync | MessageGeneratorType.Dependency);
    }

    public class MyTest : ITest
    {
        public MessageGeneratorType Type { get; set; } = MessageGeneratorType.All;

        public int Value { get; set; }

        public void Test(MessageGeneratorType type)
        {
            if (Type.HasFlag(MessageGeneratorType.Publish))
            {
                Console.WriteLine("has publish");
            }

            if (Type.HasFlag(MessageGeneratorType.Sync))
            {
                Console.WriteLine("has sync");
            }

            if (Type.HasFlag(MessageGeneratorType.Dependency))
            {
                Console.WriteLine("has dependency");
            }
        }
    }
}
