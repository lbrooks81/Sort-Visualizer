using System.Drawing;

namespace SortVisualizer.Algorithms
{
    public class InsertionSort : ISortable
    {
        public async Task SortAsync(int[] array, Func<int?, Task> Repaint, CancellationToken cancellationToken)
        {
            for (int i = 1; i < array.Length; i++)
            {
                int temp = array[i];
                int j = i - 1;

                for (; j >= 0 && array[j].CompareTo(temp) > 0; j--)              
                {
                    array[j + 1] = array[j];
                    await Repaint(null);
                }
                array[j + 1] = temp;
                
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}
