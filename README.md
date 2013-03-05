##SharpFinn
#### AFINN-based sentiment analysis for `C#`
This is a library that utilises the [AFINN](http://www2.imm.dtu.dk/pubdb/views/publication_details.php?id=6010) sentiment lexicon to make a simple [sentiment analysis](http://en.wikipedia.org/wiki/Sentiment_analysis).

### Usage

Grab the newest word list from http://www2.imm.dtu.dk/pubdb/views/publication_details.php?id=6010 before you begin and put it
in the db folder in the project, remember to change the constructor of Sentiment.cs to match your AFINN file name.

Simply reference SharpFinn in your project and you are able to make simple sentiment analysis:

```csharp
var sentiment = Sentiment.Instance;
var score = sentiment.GetScore("Baboons are totally redicuoulsy looking with their ugly red bottoms");
Console.WriteLine("Score: {0}", score.Sentiment);
Console.WriteLine("Average Tokens: {0}", score.AverageSentimentTokens);
Console.WriteLine("Average Words: {0}", score.AverageSentimentWords);
```

License
-------
* AFINN-111.txt is subject to the ODbl-1.0 License
* Sentiment.cs is subject to AGPL-3.0 License or higher
