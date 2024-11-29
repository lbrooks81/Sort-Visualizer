using System.Diagnostics;
using System.Drawing;

namespace SortVisualizer.Algorithms
{
    public class MergeSort : ISortable
    {
        public async Task SortAsync(int[] array, Func<int?, Task> Repaint, CancellationToken cancellationToken)
        {
            await Sort();

            async Task Merge(int left, int middle, int right)
            {
                // Size of left sub-array
                int n1 = middle - left + 1;

                // Size of right sub-array
                int n2 = right - middle;


                // Creating temp arrays
                int[] leftArray = new int[n1];
                int[] rightArray = new int[n2];

                // Copying contents of the original arrays into the temp arrays
                for (int i = 0; i < n1; i++)
                {
                    leftArray[i] = array[left + i];
                    await Repaint(null);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                for (int i = 0; i < n2; i++)
                {
                    rightArray[i] = array[middle + 1 + i];
                    await Repaint(null);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                int leftIndex = 0;
                int rightIndex = 0;

                int mergeIndex = left;


                // This while loop will be exited with the contents of one array being empty
                // Once the left or right index exceeds their size, the while loop is exited
                while (leftIndex < n1 && rightIndex < n2)
                {
                    // The element in the left sub-array is smaller
                    if (leftArray[leftIndex].CompareTo(rightArray[rightIndex]) < 0)
                    {
                        array[mergeIndex] = leftArray[leftIndex];
                        leftIndex++;
                        await Repaint(null);
                    }
                    // The element in the right sub-array is smaller
                    else
                    {
                        array[mergeIndex] = rightArray[rightIndex];
                        rightIndex++;
                        await Repaint(null);
                    }

                    // Increment mergeIndex so it doesn't constantly reassign over the same index
                    mergeIndex++;
                    cancellationToken.ThrowIfCancellationRequested();

                }

                // Copy remaining elements of the left sub-array into merged array
                while (leftIndex < n1)
                {
                    array[mergeIndex] = leftArray[leftIndex];
                    leftIndex++;
                    mergeIndex++;
                    await Repaint(null);
                }

                while (rightIndex < n2)
                {
                    array[mergeIndex] = rightArray[rightIndex];
                    rightIndex++;
                    mergeIndex++;
                    await Repaint(null);
                }


            }

            async Task MergeSort(int left, int right)
            {
                // Base case
                if (left >= right) return;

                int middle = (left + right) / 2;

                // Breaking down
                await MergeSort(left, middle);
                await MergeSort(middle + 1, right);

                // Merging
                await Merge(left, middle, right);

            }

            async Task Sort()
            {
                await MergeSort(0, array.Length - 1);
            }
        }

        
    }
}
