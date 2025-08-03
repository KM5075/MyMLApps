using Microsoft.ML;
using Microsoft.ML.Data;
using static Microsoft.ML.DataOperationsCatalog;

using Api_SentimentAnalysis;

string _dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "yelp_labelled.txt");

MLContext mlContext = new MLContext();
TrainTestData splitDataView = LoadData(mlContext);
ITransformer model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);
Evaluate(mlContext, model, splitDataView.TestSet);
UserModelWithSingleItem(mlContext, model);
UseModelWithBatchItems(mlContext, model);



TrainTestData LoadData(MLContext mLContext)
{
    IDataView dataView = mLContext.Data.LoadFromTextFile<SentimentData>(
        _dataPath,
        hasHeader: false,
        separatorChar: '\t');

    TrainTestData splitDataview = mLContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

    return splitDataview;
}

ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitDataView)
{
    var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
        .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
    Console.WriteLine("=============== Create and Train the Model ===============");
    var model = estimator.Fit(splitDataView);
    Console.WriteLine("=============== End of training ===============");
    Console.WriteLine();
    return model;
}

void Evaluate(MLContext mlContext, ITransformer model, IDataView testDataView)
{
    Console.WriteLine("=============== Evaluating Model ===============");
    IDataView predictions = model.Transform(testDataView);
    var metrics = mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label", scoreColumnName: "Score");
    Console.WriteLine($"Model accuracy: {metrics.Accuracy:P2}");
    Console.WriteLine($"AUC: {metrics.AreaUnderRocCurve:P2}");
    Console.WriteLine($"F1 Score: {metrics.F1Score:P2}");
    Console.WriteLine("=============== End of Model Evaluation ===============");
}

void UserModelWithSingleItem(MLContext mlContext, ITransformer model)
{
    Console.WriteLine("=============== Predicting a single item ===============");
    PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
    var sampleStatement = new SentimentData
    {
        SentimentText = "This is a great product!"
    };
    var resultPrediction = predictionFunction.Predict(sampleStatement);

    Console.WriteLine($"Sentiment: {resultPrediction.Prediction}, Probability: {resultPrediction.Probability:P2}");
    Console.WriteLine("=============== End of Prediction ===============");
}

void UseModelWithBatchItems(MLContext mlContext, ITransformer model)
{
    IEnumerable<SentimentData> sentiments = new[]
    {
        new SentimentData
        {
            SentimentText = "This was a horrible meal"
        },
        new SentimentData
        {
            SentimentText = "I love this spaghetti."
        }
    };

    IDataView batchComments = mlContext.Data.LoadFromEnumerable(sentiments);
    IDataView predictions = model.Transform(batchComments);

    IEnumerable<SentimentPrediction> predictedResults = mlContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);

    Console.WriteLine();

    Console.WriteLine("=============== Prediction Test of loaded model with multiple samples ===============");

    foreach (SentimentPrediction prediction in predictedResults)
    {
        Console.WriteLine($"Sentiment: {prediction.SentimentText} | Prediction: {(Convert.ToBoolean(prediction.Prediction) ? "Positive" : "Negative")} | Probability: {prediction.Probability} ");
    }
}


