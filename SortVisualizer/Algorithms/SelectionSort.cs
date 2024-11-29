using System.Drawing;

namespace SortVisualizer.Algorithms
{
    public class SelectionSort : ISortable
    {
        public async Task SortAsync(int[] array, Func<int?, Task> Repaint, CancellationToken cancellationToken)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                int minIndex = i;

                for (int j = i + 1; j < array.Length; j++)
                {
                    // Swaps the current minIndex with the new one if a smaller value is found
                    if (array[j].CompareTo(array[minIndex]) < 0)
                    {
                        minIndex = j;
                        await Repaint(null);
                    }
                }
                
                // If the value of minIndex has changed, swap [i] with [minIndex]
                if (minIndex != i)
                {
                    int temp = array[i];
                    array[i] = array[minIndex];
                    array[minIndex] = temp;
                }

            }
        }
    }
}
