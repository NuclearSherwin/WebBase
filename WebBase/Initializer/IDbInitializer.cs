using System.Threading.Tasks;

namespace WebBase.Initializer
{
    public interface IDbInitializer
    {
        Task Initialize();
    }
}