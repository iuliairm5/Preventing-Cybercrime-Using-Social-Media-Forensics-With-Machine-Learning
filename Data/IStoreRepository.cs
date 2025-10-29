using DisertatieIRIMIA.Models;

namespace DisertatieIRIMIA.Data
{
    public interface IStoreRepository
    {
        IQueryable<PREDICTION> Predictions { get; }
        

        Task SavePredictionAsync(PREDICTION prediction);

        Task<PREDICTION> DeletePredictionAsync(int predictionID);

        IQueryable<PREDICTION> PredictionsForUser(string userId);

        /////////////////////
        Task SavePredictionAsync2(PREDICTION prediction);

    }
}
