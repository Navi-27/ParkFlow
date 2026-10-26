
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

            Database.TestConnection(out string mensaje);
            MessageBox.Show(mensaje);

            RepositoryPersonas repo = new RepositoryPersonas();

            // 1. AÑADIR UN RESIDENTE
            Persona residente = Residente.Create();
            residente.Id = "1001234567";
            residente.TipoDocumento = "CC";
            residente.Nombre = "Carlos";
            residente.Apellido = "Perez";
            residente.MovilidadReducida = false;
            residente.Telefono = "3001234567";
            residente.Correo = "carlos@gmail.com";

            if (!repo.Exist(residente.Id))
            {
                repo.Add(residente);
                MessageBox.Show("Residente añadido correctamente");
            }
            else
            {
                MessageBox.Show("El residente ya existe");
            }

            // 2. AÑADIR UN VISITANTE
            Persona visitante = Visitante.Create();
            visitante.Id = "1007654321";
            visitante.TipoDocumento = "CC";
            visitante.Nombre = "Laura";
            visitante.Apellido = "Gomez";
            visitante.MovilidadReducida = true;
            visitante.Telefono = "3107654321";
            visitante.Correo = "laura@gmail.com";

            if (!repo.Exist(visitante.Id))
            {
                repo.Add(visitante);
                MessageBox.Show("Visitante añadido correctamente");
            }
            else
            {
                MessageBox.Show("El visitante ya existe");
            }

            // 3. ACTUALIZAR UNA PERSONA
            Persona? personaActualizar =
                repo.GetById("1001234567");

            if (personaActualizar != null)
            {
                personaActualizar.Nombre = "Carlos Andres";
                personaActualizar.Apellido = "Rodriguez";
                personaActualizar.Telefono = "3115555555";
                personaActualizar.Correo = "carlosandres@gmail.com";

                repo.Update(personaActualizar);

                MessageBox.Show("Persona actualizada correctamente");
            }
            else
            {
                MessageBox.Show("No existe la persona para actualizar");
            }

            // 4. CONSULTAR TODAS LAS PERSONAS
            string resultado = "";

            foreach (Persona persona in repo.GetAll())
            {
                resultado +=
                    $"{persona.Id} - {persona.Nombre} " +
                    $"{persona.Apellido} - {persona.GetType().Name}\n";
            }

            MessageBox.Show(
                string.IsNullOrEmpty(resultado)
                    ? "No hay personas registradas"
                    : resultado
            );

            // 5. ELIMINAR UNA PERSONA
            string documentoEliminar = "1007654321";

            if (repo.Exist(documentoEliminar))
            {
                repo.Delete(documentoEliminar);
                MessageBox.Show("Visitante eliminado correctamente");
            }
            else
            {
                MessageBox.Show("La persona que quieres eliminar no existe");
            }

            // 6. VERIFICAR LA ELIMINACIÓN
            if (!repo.Exist(documentoEliminar))
            {
                MessageBox.Show("Verificación: la persona ya no existe");
            }

            // Abrir el formulario principal
            Application.Run(new Form1());
        }
    }
}
