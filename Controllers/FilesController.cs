using Microsoft.AspNetCore.Mvc;

namespace UploadFiles.Controllers
{
    public class FilesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files, [FromForm] string folderName)
        {
            try
            {
                // Se crea la carpeta con el nombre dado si no existe
                Directory.CreateDirectory(folderName);

                // Se crea una variable para almacenar las URLs de los archivos
                var fileUrls = "";

                // Se recorre la lista de archivos y se guardan en la carpeta
                foreach (var file in files)
                {
                    // Se obtiene el nombre del archivo
                    var fileName = Path.GetFileName(file.FileName);

                    // Se crea el path completo del archivo con la carpeta y el nombre
                    var filePath = Path.Combine(folderName, fileName);

                    // Se copia el contenido del archivo al path creado
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Se construye la URL del archivo usando el nombre del host y el path relativo
                    var fileUrl = $"{Request.Scheme}://{Request.Host}/{folderName}/{fileName}";

                    // Se agrega la URL del archivo a la variable, separada por un signo |
                    fileUrls += fileUrl + "|";
                }

                // Se elimina el último signo | de la variable
                fileUrls = fileUrls.TrimEnd('|');

                // Se devuelve un mensaje de éxito con las URLs de los archivos
                return Ok("Los archivos se han guardado correctamente en la carpeta " + folderName + ". Las URLs de los archivos son: " + fileUrls);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
