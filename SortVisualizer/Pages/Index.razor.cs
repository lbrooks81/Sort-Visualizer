using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SortVisualizer.Algorithms;
using System.Diagnostics;

namespace SortVisualizer.Pages
{
	public enum Algorithm
	{ 
		BubbleSort,
		SelectionSort,
		InsertionSort,
        Quicksort,
        Mergesort,
        LSDRadixSort,
        ExchangeSort,
        CombSort,
        HeapSort,
        CocktailSort,

        // TODO: Add your additional sorting algorithms here
    }

    public enum Complexity
    {
        O_1,        // Constant
        O_logN,     // Logarithmic
        O_N,        // Linear
        O_NlogN,    // Linearithmic
        O_N2,       // Quadratic
        O_N3,       // Cubic
        O_2N,       // Exponential
        O_NFact,    // Factorial
        O_NM,             
    }

	public partial class Index : ComponentBase
	{
        #region Properties (you should not change these)
        [Inject]
        public IJSRuntime? JS { get; set; }

        private CancellationTokenSource? cancellationSource;
        private bool displayResults = false;
        private const int LARGEST_NUM = 1000;

        private int[]? originalArray;//Do not modify this!
        #endregion

        public record AlgorithmDetails(
            Algorithm AlgorithmType, 
            Complexity BestCaseComplexity,
            Complexity AverageCaseComplexity,
            Complexity WorstCaseComplexity,
            ISortable AlgorithmInstance
            );

        //---------Properties for the UI--------------
        private int Seed { get; set; } = 42;        // Default to 42
        public int ArraySize { get; set; } = 600;   // Default to 600
        //--------------------------------------------

        // The array to sort (do not modify this)
        public int[] Array { get; set; } = null!;

        // Increase this to make sorts faster; decrease this to make visual sort more pronounced
        // Feel free to change this value per sorting algorithm for the best visual experience
        private int INSTRUCTIONS_UNTIL_REPAINT = 20;
        public int instructionsUntilRepaint;

        //The algorithm to use for sorting
        public Algorithm AlgorithmToUse { get; set; } = Algorithm.BubbleSort; //Default value

        public AlgorithmDetails[] SortingAlgorithmInstances = [
            new AlgorithmDetails(Algorithm.BubbleSort, Complexity.O_N, Complexity.O_N2, Complexity.O_N2, new BubbleSort()),
            new AlgorithmDetails(Algorithm.SelectionSort, Complexity.O_N2, Complexity.O_N2, Complexity.O_N2, new SelectionSort()),
            new AlgorithmDetails(Algorithm.InsertionSort, Complexity.O_N, Complexity.O_N2, Complexity.O_N2, new InsertionSort()),
            new AlgorithmDetails(Algorithm.Quicksort, Complexity.O_NlogN, Complexity.O_N2, Complexity.O_N2, new Quicksort()),
            new AlgorithmDetails(Algorithm.Mergesort, Complexity.O_NlogN, Complexity.O_NlogN, Complexity.O_NlogN, new MergeSort()),
            new AlgorithmDetails(Algorithm.LSDRadixSort, Complexity.O_NM, Complexity.O_NM, Complexity.O_NM, new LSDRadixSort()),
            new AlgorithmDetails(Algorithm.ExchangeSort, Complexity.O_N2, Complexity.O_N2, Complexity.O_N2, new ExchangeSort()),
            new AlgorithmDetails(Algorithm.CombSort, Complexity.O_NlogN, Complexity.O_N2, Complexity.O_N2, new CombSort()),
            new AlgorithmDetails(Algorithm.HeapSort, Complexity.O_NlogN, Complexity.O_NlogN, Complexity.O_NlogN, new HeapSort()),
            new AlgorithmDetails(Algorithm.CocktailSort, Complexity.O_N, Complexity.O_N2, Complexity.O_N2, new CocktailSort())
            // TODO: Add the rest of the sorting algorithms
            ];

        public AlgorithmDetails? CurrentAlgorithm { get; set; }

        // Gets called when the user clicks the "Go!" button
        // There should be no need to modify this method but feel free to do so if needed
        private async Task Sort(CancellationToken cancellationToken)
        {
            try
            {
                CurrentAlgorithm = SortingAlgorithmInstances
                    .First(x => x.AlgorithmType == AlgorithmToUse);
                await CurrentAlgorithm.AlgorithmInstance.SortAsync(Array, RepaintScreen, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                StateHasChanged();
                await ValidateResults();
                displayResults = true;
            }
            catch(OperationCanceledException)
            {
                Console.WriteLine("Sort was cancelled");
            }
            catch(Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                Console.Error.WriteLine(ex.Message);
            }
            finally
            {
                StateHasChanged();
            }
        }

		private void ChangeAlgorithmToRun(ChangeEventArgs e)
		{
			switch (e.Value!.ToString()!)
			{
				case "bubble":
                    AlgorithmToUse = Algorithm.BubbleSort;
                    // Bubble sort is slow, so increase the instructions until repaint
                    INSTRUCTIONS_UNTIL_REPAINT = 48;
                    break;
				case "selection":
					AlgorithmToUse = Algorithm.SelectionSort;
                    INSTRUCTIONS_UNTIL_REPAINT = 16;
                    break;
				case "insertion":
					AlgorithmToUse = Algorithm.InsertionSort;
                    INSTRUCTIONS_UNTIL_REPAINT = 96;
                    break;
                case "quicksort":
                    AlgorithmToUse = Algorithm.Quicksort;
                    INSTRUCTIONS_UNTIL_REPAINT = 16;
                    break;
                case "merge":
                    AlgorithmToUse = Algorithm.Mergesort;
                    INSTRUCTIONS_UNTIL_REPAINT = 32;
                    break;
                case "radix":
                    AlgorithmToUse = Algorithm.LSDRadixSort;
                    INSTRUCTIONS_UNTIL_REPAINT = 24;
                    break;
                case "exchange":
                    AlgorithmToUse = Algorithm.ExchangeSort;
                    INSTRUCTIONS_UNTIL_REPAINT = 196;
                    break;
                case "comb":
                    AlgorithmToUse = Algorithm.CombSort;
                    INSTRUCTIONS_UNTIL_REPAINT = 12;
                    break;
                case "heap":
                    AlgorithmToUse = Algorithm.HeapSort;
                    INSTRUCTIONS_UNTIL_REPAINT = 24;
                    break;
                case "cocktail":
                    AlgorithmToUse = Algorithm.CocktailSort;
                    INSTRUCTIONS_UNTIL_REPAINT = 64;
                    break;

                // TODO: Add your additional sorting algorithms here
            }

            Console.WriteLine($"Algorithm to run: {AlgorithmToUse}");
		}

        #region Helper Methods (you should not change these)
        // ================== Helper Methods (DO NOT MODIFY) ==================

        private async void BeginSort()
        {
            displayResults = false;
            if (cancellationSource != null)
            {
                cancellationSource.Cancel();
                Reset();
            }

            cancellationSource = new CancellationTokenSource();
            instructionsUntilRepaint = INSTRUCTIONS_UNTIL_REPAINT;
            StateHasChanged();
            await Sort(cancellationSource.Token);
        }

        public void Reset()
        {
            displayResults = false;
            if (cancellationSource != null)
            {
                cancellationSource.Cancel();
            }

            Array = originalArray!.ToArray();
            StateHasChanged();
        }

        public async Task RepaintScreen(int? delay)
        {
            if(--instructionsUntilRepaint <= 0)
            {
                instructionsUntilRepaint = INSTRUCTIONS_UNTIL_REPAINT;
                StateHasChanged();
                await Task.Delay(delay ?? 2); // Default time is 2ms
            }
        }

        //Called when the component is initialized
        protected override void OnInitialized()
        {
            PerformArrayPrep();
        }

        private async Task ResizeOrRandomizeArray()
        {
            displayResults = false;
            if (cancellationSource != null)
            {
                cancellationSource.Cancel();
                Reset();
            }

            if (JS != null)
            {
                int elements = await JS.InvokeAsync<int>("getNumberOfElements");
                // Console.WriteLine(elements);
                ArraySize = elements;
            }

            PerformArrayPrep();
        }

        private void PerformArrayPrep()
        {
            //Populate the array with random numbers

            Random rand = new Random(Seed);
            Array = Enumerable.Range(0, ArraySize)
                .Select(x => x = rand.Next(1, 1000))
                .ToArray();
            if (ArraySize <= LARGEST_NUM)
            {
                // Generate unique numbers if ArraySize is within the range of LARGEST_NUMBER
                Array = Enumerable.Range(1, ArraySize)
                    .OrderBy(x => rand.Next()) // Shuffle
                    .Take(ArraySize) // Take only the required amount
                    .ToArray();
            }
            else
            {
                // Only allow duplicates if ArraySize exceeds LARGEST_NUMBER
                Array = Enumerable.Range(0, ArraySize)
                    .Select(x => rand.Next(1, LARGEST_NUM + 1))
                    .ToArray();
            }
            originalArray = Array;
        }

        private async Task ValidateResults()
        {
            Console.WriteLine("Validating results...");
            // Is Array sorted?
            List<int> tempArray = Array.ToList();
            tempArray.Sort();
            if (tempArray.SequenceEqual(Array))
            {
                Console.WriteLine("Array is sorted!");
            }
            else
            {
                if (JS != null)
                {
                    await JS.InvokeVoidAsync("alert", "Array is not sorted!");
                }
                Console.WriteLine("Array is not sorted!");
            }
        }

        private String GetRainbowColor(int value)
        {
            int upperbound = Math.Min(LARGEST_NUM, Array.Length);

            // Clamp the value within the range
            value = Math.Clamp(value, 1, upperbound);

            // Calculate the hue based on value's relative position between min and max
            // Red (0 degrees) to purple (270 degrees)
            double hue = 270.0 * (value - 1) / (upperbound - 1);

            // Convert the hue to an HSL color, keeping saturation and lightness fixed
            return $"hsl({hue}, 100%, 50%)"; // Full saturation and 50% lightness for vibrant colors
        }

        private String ComplexityToHTML(Complexity complexity)
        {
            switch(complexity)
            {
                case Complexity.O_1:
                    return "O(1)";
                case Complexity.O_logN:
                    return "O(log n)";
                case Complexity.O_N:
                    return "O(n)";
                case Complexity.O_NlogN:
                    return "O(n log n)";
                case Complexity.O_N2:
                    return "O(n<sup>2</sup>)";
                case Complexity.O_N3:
                    return "O(n<sup>3</sup>)";
                case Complexity.O_2N:
                    return "O(2<sup>n</sup>)";
                case Complexity.O_NFact:
                    return "O(n!)";
                case Complexity.O_NM:
                    return "O(nm)";
                default:
                    return "Unknown";
            }
        }
        #endregion
    }
}
