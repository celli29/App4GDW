using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace App4GDW
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
            database.CreateTableAsync<LocalCoordinates>().Wait();
            database.CreateTableAsync<LocalScoreCard>().Wait();
            database.CreateTableAsync<LocalTeeInfo>().Wait();
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

        // basic queries for LOCAL COORDINATES
        public Task<List<LocalCoordinates>> GetLocalCoordAsync()
        {
            return database.Table<LocalCoordinates>().ToListAsync();
        }

        public Task<LocalCoordinates> GetLocalCoordAsync(int hole)
        {
            return database.Table<LocalCoordinates>().Where(t => t.Hole == hole).FirstOrDefaultAsync();
        }

        public Task<int> SaveCoordAsync(LocalCoordinates coord)
        {
            if (coord.Hole != 0)
            {
                return database.UpdateAsync(coord);
            }
            else
            {
                return database.InsertAsync(coord);
            }
        }

        public Task<int> DeleteCoordAsync(LocalCoordinates coord)
        {
            return database.DeleteAsync(coord);
        }

        // basic queries for LOCAL SCORECARD
        public Task<List<LocalScoreCard>> GetLocalScoreCardAsync()
        {
            return database.Table<LocalScoreCard>().ToListAsync();
        }

        public Task<LocalScoreCard> GetLocalScoreCardAsync(int hole)
        {
            return database.Table<LocalScoreCard>().Where(t => t.Hole == hole).FirstOrDefaultAsync();
        }

        public Task<int> SaveLocalScoreCardAsync(LocalScoreCard card)
        {
            if (card.Hole != 0)
            {
                return database.UpdateAsync(card);
            }
            else
            {
                return database.InsertAsync(card);
            }
        }

        public Task<int> DeleteLocalScoreCardAsync(LocalScoreCard coord)
        {
            return database.DeleteAsync(coord);
        }

        // basic queries for LOCAL TEEINFO
        public Task<List<LocalTeeInfo>> GetLocalTeeInfoAsync()
        {
            return database.Table<LocalTeeInfo>().ToListAsync();
        }

        public Task<LocalTeeInfo> GetLocalTeeInfoAsync(int hole)
        {
            return database.Table<LocalTeeInfo>().Where(t => t.Hole == hole).FirstOrDefaultAsync();
        }

        public Task<int> SaveLocalTeeInfoAsync(LocalTeeInfo info)
        {
            if (info.Hole != 0)
            {
                return database.UpdateAsync(info);
            }
            else
            {
                return database.InsertAsync(info);
            }
        }

        public Task<int> DeleteLocalTeeInfoAsync(LocalTeeInfo info)
        {
            return database.DeleteAsync(info);
        }
    }
}
