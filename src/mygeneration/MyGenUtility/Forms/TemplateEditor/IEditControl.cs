namespace MyGeneration
{
    public interface IEditControl 
    {
        string Mode { get; }
        string Language { get; }
        string Text { get; set; }
        void GrabFocus();
        void Activate();
    }
}