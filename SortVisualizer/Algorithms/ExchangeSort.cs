
namespace SortVisualizer.Algorithms
{
    public class ExchangeSort : ISortable
    {
        public async Task SortAsync(int[] array, Func<int?, Task> Repaint, CancellationToken cancellationToken)
        {
            // Outer iteration to get main index
            for (int i = 0; i < array.Length - 1; i++) 
            {
                // Inner iteration sets secondary index to one more than the main index
                for (int j = i + 1; j < array.Length; j++)
                {
                    // Swaps the two if they're out of order
                    if (array[i] > array[j])
                    {
                        int temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                        await Repaint(null);
                    }
                    cancellationToken.ThrowIfCancellationRequested();
                }
                cancellationToken.ThrowIfCancellationRequested();
            }

        }
    }
}
