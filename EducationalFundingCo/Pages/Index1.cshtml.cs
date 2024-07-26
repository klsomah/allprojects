using System.IO;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace EducationalFundingCo.Pages
{
    public class Index1Model : PageModel
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Index1Model(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public void OnGet()
        {
            //var mypath = Path.Combine(_hostingEnvironment.WebRootPath, "Contracts/Kloteh-Somah-2007.docx");
            //Document document = new Document();
            //document.LoadFromFile(mypath);
            ////Convert Word to PDF  
            //document.SaveToFile("toPDF.PDF", FileFormat.PDF);

            //CreateContract createContract = new CreateContract();

            //createContract.TemplatePath = Path.Combine(_hostingEnvironment.WebRootPath, "Contracts/IncomeShareAgreement.docx");
            //createContract.SavedPath = Path.Combine(_hostingEnvironment.WebRootPath, "Contracts/KlotehSomah.docx");
            //createContract.ObligorName = "Kloteh E Somah";
            //createContract.ObligorAddress = "308 Baymist Dr, Loganville GA 30084";
            //createContract.ProgramName = "ASP.NET Core Development";
            //createContract.SchoolName = "COIN Education Services Center";
            //createContract.AuthorityName = "Ngozi Enechuku";
            //createContract.AuthorityPosition = "CEO";
            //createContract.ObligorEmail = "kloteh.somah@gmail.com";
            //createContract.CurrentDate = DateTime.Now.ToShortDateString();

            //await createContract.SaveContractPerObligator();
            //await createContract.SearchAndReplace();


            //await SaveContractPerObligator();
            //await SearchAndReplace();
        }

        public async Task SearchAndReplace()
        {
            //var attachment = Path.Combine(_hostingEnvironment.WebRootPath, "Contracts/EFC_ISA_Agreement.docx");
            var savePath = Path.Combine(_hostingEnvironment.WebRootPath, "Contracts/Kloteh.docx");

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(savePath, true))
            {

                string docText = null;
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                Regex regexText = new Regex("ObligorName");
                docText = regexText.Replace(docText, "Kloteh Somah");

                Regex regexText1 = new Regex("SchoolName");
                docText = regexText1.Replace(docText, "COIN Education Services Center");

                Regex regexText2 = new Regex("ProgramName");
                docText = regexText2.Replace(docText, "JAVASCRIPT");

                Regex regexText3 = new Regex("IncomePrecentage");
                docText = regexText3.Replace(docText, "12.00%");

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }

                //wordDoc.SaveAs(savePath);
                //wordDoc.Close();
                wordDoc.Dispose();
                await Task.Delay(1000);
            }
        }

        public async Task SaveContractPerObligator()
        {
            var attachment = Path.Combine(_hostingEnvironment.WebRootPath, "Contracts/EFC_ISA_Agreement.docx");
            var savePath = Path.Combine(_hostingEnvironment.WebRootPath, "Contracts/Kloteh.docx");

            using WordprocessingDocument wordDoc = WordprocessingDocument.Open(attachment, true);
            {
                if (!System.IO.File.Exists(savePath))
                {
                    wordDoc.SaveAs(savePath).Dispose();
                }
            }
        
            wordDoc.Dispose();
            await Task.Delay(3000);
        }

        private void ManipulatePdf()
        {
            //var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Contracts/EFC_ISA_Agreement.pdf");
            //var savePath = Path.Combine(_hostingEnvironment.WebRootPath, "Contracts/Kloteh.pdf");

            //PdfDocument pdfDoc = new PdfDocument(new PdfReader(savePath), new PdfWriter(filePath));
            //PdfPage page = pdfDoc.GetFirstPage();
            //PdfDictionary dict = page.GetPdfObject();

            //PdfObject pdfObject = dict.Get(PdfName.Contents);
            //if (pdfObject is PdfStream)
            //{
            //    PdfStream stream = (PdfStream)pdfObject;
            //    byte[] data = stream.GetBytes();
            //    string replacedData = iText.IO.Util.JavaUtil.GetStringForBytes(data).Replace("[Obligor Name]", "Kloteh Somah");
            //    stream.SetData((Encoding.UTF8.GetBytes(replacedData)));
            //}

            //pdfDoc.Close();
        }

        
    }
}
