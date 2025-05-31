using Document_Intelligence_Task.Domain.Models;
using Document_Intelligence_Task.Services;
using Document_Intelligence_Task.UOW;
using Microsoft.AspNetCore.Mvc;

namespace Document_Intelligence_Task.Controllers
{
    public class IDDocumentController : Controller
    {
        private readonly IDocumentIntelligenceService _documentIntelligenceService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public IDDocumentController(IDocumentIntelligenceService documentIntelligenceService,
                                    IUnitOfWork unitOfWork,
                                    IFileService fileService)
        {
            _documentIntelligenceService = documentIntelligenceService;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        [HttpGet]
        [Route("/passport")]
        public IActionResult UploadPassport()
        {
            return View("PassportForm");
            
        }

        
        [HttpPost]
        [Route("/AnalyzePassport")]
        public async Task<IActionResult> AnalyzePassport(IFormFile file)
        {

            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }
                using var fileStream = file.OpenReadStream(); 

                var operationLocation =
                await _documentIntelligenceService.AnalyzeLocalDocumentAsync(fileStream, file.FileName);

                var jsonResult = await _documentIntelligenceService.GetAnalyzeResultAsync(operationLocation);

                var passportModel = _documentIntelligenceService.Parse_IDDocumentResult(jsonResult);

                if(passportModel != null)
                {
                    passportModel.fileUrl = await _fileService.SaveFileAsync(file);
                    await _unitOfWork.Passports.AddAsync(passportModel);
                    await _unitOfWork.SaveChangesAsync();
                }

                return View("DisplayPassportInfo", passportModel);
                
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

            
        }

        [HttpGet]
        [Route("GetAllPassport")]
        public async Task<IActionResult> GetAllPassports()
        {
            var passports = await _unitOfWork.Passports.GetAllAsync();

            return View("ListPassports", passports);
        }

        [HttpPost]
        [Route("DeletePassport/{id:guid}")]
        public async Task<IActionResult> DeletePassport(Guid id)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var PassportModel = await _unitOfWork.Passports.GetByIdAsync(id);
                if (PassportModel == null)
                {
                    await _unitOfWork.RollBackAsync();
                    return NotFound();
                }
                if(PassportModel.fileUrl != null)
                {
                   _fileService.DeleteFile(PassportModel.fileUrl);
                }
                _unitOfWork.Passports.DeleteAsync(PassportModel);

                await _unitOfWork.CommitAsync();
                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction("GetAllPassports", "IDDocument");
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollBackAsync();
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
