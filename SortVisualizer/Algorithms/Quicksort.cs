using System.Diagnostics;
using System.Drawing;

namespace SortVisualizer.Algorithms
{
    public class Quicksort : ISortable
    {
        public async Task SortAsync(int[] array, Func<int?, Task> Repaint, CancellationToken cancellationToken)
        {
            int pivotIndex = 0;

            await Quicksort(0, array.Length - 1);

            async Task Quicksort(int leftIndex, int rightIndex)
            {
                if (rightIndex - leftIndex <= 0) return;

                await Partition(leftIndex, rightIndex);

                await Quicksort(leftIndex, pivotIndex - 1);

                await Quicksort(pivotIndex + 1, rightIndex);

            }

            async Task Partition(int leftPointer, int rightPointer)
            {
                pivotIndex = rightPointer;
                int pivot = array[pivotIndex];

                rightPointer--;

                while (true)
                {
                    while (array[leftPointer] < pivot)
                    {
                        leftPointer++;
                        await Repaint(null);
                    }

                    while (rightPointer >= 0
                        && array[rightPointer] > pivot)
                    {
                        rightPointer--;
                        await Repaint(null);
                    }

                    if (leftPointer >= rightPointer) break;

                    int temp = array[leftPointer];
                    array[leftPointer] = array[rightPointer];
                    array[rightPointer] = temp;

                    leftPointer++;
                    cancellationToken.ThrowIfCancellationRequested();
                }

                array[pivotIndex] = array[leftPointer];
                array[leftPointer] = pivot;

                pivotIndex = leftPointer;
            }

        }
    }
}
