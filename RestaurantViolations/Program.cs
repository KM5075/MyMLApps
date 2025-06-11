internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var sampleData = new RestaurantViolations.RestaurantViolations.ModelInput()
        {
            InspectionType = "Routine",
            ViolationDescription = "Improper food storage",
        };

        var predictedLabels = RestaurantViolations.RestaurantViolations.PredictAllLabels(sampleData);
        Console.WriteLine("Predicted Labels:");
        foreach (var label in predictedLabels)
        {
            Console.WriteLine($"{label.Key}: {label.Value}");
        }
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}