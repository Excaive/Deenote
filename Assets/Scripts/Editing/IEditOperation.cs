namespace Deenote
{
    public interface IEditOperation
    {
        void Execute(); // Called when added to the list, or when redo
        void Revert(); // Called when undo
    }
}
