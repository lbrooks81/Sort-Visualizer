
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace SortVisualizer.Algorithms
{
    public class HeapSort : ISortable
    {
        public async Task SortAsync(int[] array, Func<int?, Task> Repaint, CancellationToken cancellationToken)
        {
            int length = array.Length;

            // Create a heap with the each starting index from the middle to 0
            // This starting point is chosen because the other half of the array will be the leaf nodes
            for (int i = length / 2 - 1; i >= 0; i--) 
            {
                cancellationToken.ThrowIfCancellationRequested();

                await CreateHeap(array, length, i);
            }

            // Swaps the root element with the last element in the array, which will decrement after each iteration
            for (int i = length - 1; i > 0; i--)
            {
                int temp = array[0];
                array[0] = array[i];
                array[i] = temp;

                cancellationToken.ThrowIfCancellationRequested();
                
                // Create heap with 0 as the new root index
                await CreateHeap(array, i, 0);
            }

            async Task CreateHeap(int[] array, int length, int rootIndex)
            {
                int largestIndex = rootIndex;

                // Setting left and right children of the root node to create a binary heap structure
                int leftIndex = 2 * rootIndex + 1;
                int rightIndex = 2 * rootIndex + 2;

                // Sets the largest index to the left child if it is greater than the current one
                if (leftIndex < length && array[leftIndex] > array[largestIndex])
                {
                    largestIndex = leftIndex;
                    await Repaint(null);
                }

                // Sets the largest index to the right child if it is greater than the current one
                if (rightIndex < length && array[rightIndex] > array[largestIndex])
                {
                    largestIndex = rightIndex;
                    await Repaint(null);
                }

                if (largestIndex != rootIndex)
                {
                    // If the largest index isn't the root one, swap the root element with the largest one
                    int temp = array[rootIndex];
                    array[rootIndex] = array[largestIndex];
                    array[largestIndex] = temp;

                    await Repaint(null);
                    // Recursively call CreateHeap until the largest index is the root index
                    await CreateHeap(array, length, largestIndex);
                }
            }
        }
    }
}
