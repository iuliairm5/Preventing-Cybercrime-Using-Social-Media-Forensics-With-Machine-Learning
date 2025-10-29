using DisertatieIRIMIA.Data;

namespace DisertatieIRIMIA.Models
{
    public class EFStoreRepository : IStoreRepository
    {
        private ApplicationDbContext context;

        public EFStoreRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<PREDICTION> Predictions
        {
            get
            {
                return context.Predictions;
            }
        }

        public async Task SavePredictionAsync(PREDICTION prediction)
        {
            
            context.Predictions.Add(prediction);
            
           
            await context.SaveChangesAsync();
        }



        // NEW
       
        public async Task SavePredictionAsync2(PREDICTION prediction)
        {
           
            context.Predictions.Update(prediction);
            await context.SaveChangesAsync();
        }

        /////////


        public async Task<PREDICTION> DeletePredictionAsync(int predictionID)
        {
            PREDICTION dbEntry = context.Predictions
                    .FirstOrDefault(p => p.PREDICTIONID == predictionID);

            if (dbEntry != null)
            {
                context.Predictions.Remove(dbEntry);
                await context.SaveChangesAsync();
            }

            return dbEntry;
        }
        
        public IQueryable<PREDICTION> PredictionsForUser(string userId)
        {
            return context.Predictions.Where(p => p.UserId == userId);
        }

        ////////////////////////////////////////////////////
        ///

        
    }
}
