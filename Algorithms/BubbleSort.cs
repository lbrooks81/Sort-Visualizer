using System.Drawing;

namespace SortVisualizer.Algorithms
{
    public class BubbleSort : ISortable
    {
        public async Task SortAsync(int[] array, Func<int?, Task> Repaint, CancellationToken cancellationToken)
        {

            bool sorted = false;
            int unsortedUntilIndex = array.Length - 1;
            int startingIndex = 0;
            
            while (sorted == false)
            {
                sorted = true;

                for (int i = startingIndex; i < unsortedUntilIndex; i++)
                {
                    if (array[i] > array[i + 1])
                    {
                        int temp = array[i];
                        array[i] = array[i + 1];
                        array[i + 1] = temp;
                        sorted = false;
                        await Repaint(null);
                    }
                    cancellationToken.ThrowIfCancellationRequested();
                }
                unsortedUntilIndex--;
            }
            
        }
    }
}
