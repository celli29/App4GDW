using System.Collections.Generic;
using System.Threading.Tasks;

namespace App4GDW
{
    public class DatabaseManager
    {
        IRestService restService;

        public DatabaseManager(IRestService service)
        {
            restService = service;
        }

        public Task<List<SimpleCourse>> GetCourseTasksAsync()
        {
            return restService.RefreshCourseDataAsync();
        }

        public Task<List<SimpleTee>> GetTeeTasksAsync(int gcid)
        {
            return restService.RefreshTeeDataAsync(gcid);
        }

        public Task<List<SimpleCoordinates>> GetCoordinatesTasksAsync(int gcid)
        {
            return restService.RefreshCoordinatesDataAsync(gcid);
        }

        public Task<List<TeeCommonInfoes>> GetTeeInfoTasksAsync(int gcid, string name, string gender)
        {
            return restService.RefreshTeeInfoDataAsync(gcid, name, gender);
        }
    }
}
