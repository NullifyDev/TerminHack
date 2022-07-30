namespace TerminHack
{
    public interface ICommandHandler
    {
        public bool AppliesTo(string command);
        public void Handle(string[] arguments);
    }
} 