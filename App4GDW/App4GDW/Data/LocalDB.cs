using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace App4GDW.Data
{
    public class LocalDB
    {
        readonly SQLiteAsyncConnection database;

        public LocalDB(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Round>().Wait();
            database.CreateTableAsync<ScoreCard>().Wait();
            database.CreateTableAsync<TeeCommonInfoes>().Wait();
        }

        // basic queries for ROUND
        public Task<List<Round>> GetRoundsAsync()
        {
            return database.Table<Round>().ToListAsync();
        }

        public Task<Round> GetRoundAsync(int id)
        {
            return database.Table<Round>().Where(r => r.RID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveRoundAsync(Round round)
        {
            if(round.RID != 0)
            {
                return database.UpdateAsync(round);
            }
            else
            {
                return database.InsertAsync(round);    
            }
        }

        public Task<int> DeleteRoundAsync(Round round)
        {
            return database.DeleteAsync(round);
        }

        // basic queries for SCORECARD
        public Task<List<ScoreCard>> GetScoreCardsAsync()
        {
            return database.Table<ScoreCard>().ToListAsync();
        }

        public Task<ScoreCard> GetScoreCardAsync(int id)
        {
            return database.Table<ScoreCard>().Where(s => s.SCrowID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveScoreCardAsync(ScoreCard scoreCard)
        {
            if (scoreCard.SCrowID != 0)
            {
                return database.UpdateAsync(scoreCard);
            }
            else
            {
                return database.InsertAsync(scoreCard);
            }
        }

        public Task<int> DeleteScoreCardAsync(ScoreCard scoreCard)
        {
            return database.DeleteAsync(scoreCard);
        }

        // basic queries for TEECOMMONINFOES
        public Task<List<TeeCommonInfoes>> GetTeeInfosAsync()
        {
            return database.Table<TeeCommonInfoes>().ToListAsync();
        }

        public Task<TeeCommonInfoes> GetTeeInfoAsync(int id)
        {
            return database.Table<TeeCommonInfoes>().Where(t => t.TCID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveTeeInfoAsync(TeeCommonInfoes teeInfo)
        {
            if (teeInfo.TCID != 0)
            {
                return database.UpdateAsync(teeInfo);
            }
            else
            {
                return database.InsertAsync(teeInfo);
            }
        }

        public Task<int> DeleteTeeInfoAsync(TeeCommonInfoes teeInfo)
        {
            return database.DeleteAsync(teeInfo);
        }
    }
}
