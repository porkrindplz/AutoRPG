using System;
using System.Collections.Generic;
using System.Linq;

namespace _Scripts.Utilities
{
    
    public class WeightedRouletteWheel
    {
        private Random random = new();

        // Initialize the random number generator

        public T SelectItem<T>(List<T> items, List<double> weights)
        {
            if (items == null || weights == null || items.Count != weights.Count || items.Count == 0)
            {
                throw new ArgumentException("Items and weights must be non-null and of the same length.");
            }

            // Calculate the total weight sum
            double totalWeight = weights.Sum();

            // Generate a random number between 0 and totalWeight
            double randomValue = random.NextDouble() * totalWeight;

            // Iterate through the items and accumulate their weights
            double cumulativeWeight = 0.0;
            for (int i = 0; i < items.Count; i++)
            {
                cumulativeWeight += weights[i];

                // If the random value falls within the cumulative weight range, select the item
                if (randomValue <= cumulativeWeight)
                {
                    return items[i];
                }
            }

            // In case of rounding errors, return the last item
            return items[items.Count - 1];
        }
    }
}