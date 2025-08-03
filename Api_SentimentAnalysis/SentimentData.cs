using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Api_SentimentAnalysis;

/// <summary>
/// 入力用データセットクラス
/// </summary>
public class SentimentData
{
    [LoadColumn(0)]
    public string? SentimentText { get; set; }

    [LoadColumn(1),ColumnName("Label")]
    public bool Sentiment { get; set; }

}

public class SentimentPrediction:SentimentData
{
    [ColumnName("PredictedLabel")]
    public bool Prediction { get; set; }
    [ColumnName("Probability")]
    public float Probability { get; set; }
    [ColumnName("Score")]
    public float Score { get; set; }
}
