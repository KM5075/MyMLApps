using MyMLApps_JP;
// Add input data
var sampleData = new SentimentModel_JP.ModelInput()
{
    Col0 = "なぜこの悲しい小さな野菜はそんなに加熱しすぎているのでしょうか？"
};

// Load model and predict output of sample data
var result = SentimentModel_JP.Predict(sampleData);

// If Prediction is 1, sentiment is "Positive"; otherwise, sentiment is "Negative"
var sentiment = result.PredictedLabel == 1 ? "Positive" : "Negative";
Console.WriteLine($"Text: {sampleData.Col0}\nSentiment: {sentiment}");