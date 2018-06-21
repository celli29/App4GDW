using System.Collections.Generic;
using System.Threading.Tasks;

namespace App4GDW
{
    public interface IRestService
    {
        Task<List<SimpleCourse>> RefreshCourseDataAsync();
        Task<List<SimpleTee>> RefreshTeeDataAsync(int id);
        Task<List<SimpleCoordinates>> RefreshCoordinatesDataAsync(int id);
        Task<List<TeeCommonInfoes>> RefreshTeeInfoDataAsync(int id, string n, string g);
    }
}
