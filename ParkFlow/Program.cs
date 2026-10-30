
using ParkFlow.Infraestructure;
using ParkFlow.models;
using ParkFlow.Persistence;

namespace ParkFlow
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            RepositoryEstancias repo = new RepositoryEstancias();
            Estancia e = Estancia.Create();
            e.VehiculoPlaca = "ABC123";
            repo.Add(e);
            MessageBox.Show("Estancia creada con id " + e.Id + ". Activas: " + repo.GetActivas().Count());
            repo.Delete(e.Id);
            Application.Run(new Form1());
        }
    }
}
