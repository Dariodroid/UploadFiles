using Microsoft.AspNetCore.Mvc;

namespace UploadFiles.Controllers
{
    public class FilesController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public FilesController(IWebHostEnvironment env)
        {
            this._env = env;
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files, [FromForm] string folderName)
        {
            try
            {
                string ruta = _env.ContentRootPath+ "wwwroot\\" + folderName;
                if (!Directory.Exists(ruta))
                {
                    // Se crea la carpeta con el nombre dado si no existe
                    Directory.CreateDirectory(ruta);

                }

                // Se crea una variable para almacenar las URLs de los archivos
                var fileUrls = "";
                var filePath = "";
                // Se recorre la lista de archivos y se guardan en la carpeta
                foreach (var file in files)
                {
                    // Se obtiene el nombre del archivo
                    var fileName = Path.GetFileName($"{file.FileName}");

                    // Se crea el path completo del archivo con la carpeta y el nombre
                    filePath = Path.Combine(ruta, fileName);

                    // Se copia el contenido del archivo al path creado
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Se construye la URL del archivo usando el nombre del host y el path relativo
                    var fileUrl = $"{Request.Scheme}://{Request.Host}/{filePath}";

                    // Se agrega la URL del archivo a la variable, separada por un signo |
                    fileUrls += fileUrl + "|";
                }

                // Se elimina el último signo | de la variable
                fileUrls = fileUrls.TrimEnd('|');

                // Se devuelve un mensaje de éxito con las URLs de los archivos
                return Ok("Los archivos se han guardado correctamente en la carpeta " + filePath + ". Las URLs de los archivos son: " + fileUrls);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
