
namespace SortVisualizer.Algorithms
{
    public class CombSort : ISortable
    {
        public async Task SortAsync(int[] array, Func<int?, Task> Repaint, CancellationToken cancellationToken)
        {
            // Divides the gap by 1.3 each time until it gets to 1, where the array will be sorted
            for (double i = array.Length / 1.3; i >= 1; i /= 1.3)
            {
                int gap = Convert.ToInt32(i);

                for (int j = 0; j + gap < array.Length; j++)
                {
                    // Compares the value at the index of J and J + the gap
                    // Swaps the two if they're out of order
                    if (array[j] > array[j + gap])
                    {
                        int temp = array[j];
                        array[j] = array[j + gap];
                        array[j + gap] = temp;

                        await Repaint(null);
                    }
                    cancellationToken.ThrowIfCancellationRequested();
                }
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}
