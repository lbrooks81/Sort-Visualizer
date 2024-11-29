using System.Diagnostics;

namespace SortVisualizer.Algorithms
{
    public class LSDRadixSort : ISortable
    {
        public async Task SortAsync(int[] array, Func<int?, Task> Repaint, CancellationToken cancellationToken)
        {
            int max = array.Max();
            
            for (int exponent = 1; Convert.ToDouble(max) / Convert.ToDouble(exponent) > 0; exponent *= 10)
            {
                // Create a list to represent the occurrences of integers 0 through 9
                List<int> occurrences = Enumerable.Repeat(0, 10).ToList();

                // Increment the value for each significant digit
                for (int i = 0; i < array.Length; i++)
                {
                    int sigDig = array[i] / exponent % 10;
                    occurrences[sigDig]++;

                    cancellationToken.ThrowIfCancellationRequested();
                }

                // Iterate through the occurrences, adding the previous number to the current one
                for (int i = 0; i < occurrences.Count - 1; i++)
                {
                    occurrences[i + 1] += occurrences[i];
                    cancellationToken.ThrowIfCancellationRequested();
                }

                // Create a copy of the array to use for transferring items
                int[] newArray = new int[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    newArray[i] = array[i];
                }

                // Place items sorted by the current place value into the new array
            
                for (int i = array.Length - 1; i >= 0; i--)
                {
                    try
                    {
                        int sigDig = newArray[i] / exponent % 10;

                        array[occurrences[sigDig] - 1] = newArray[i];
                        occurrences[sigDig]--;

                        await Repaint(50);
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}
