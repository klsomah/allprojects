using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EducationalFundingCo.Utilities
{
    public class CreateContract
    {
        public string TemplatePath { get; set; }
        public string SavedPath { get; set; }
        public string SavedPathHtml { get; set; }
        public string SavedPathPdf { get; set; }
        public string ObligorName { get; set; }
        // COIN Education Services Center 
        public string SchoolName { get; set; } 
        public string ProgramName { get; set; }
        public string AuthorityName { get; set; }
        public string AuthorityPosition { get; set; }
        public string ObligorAddress { get; set; }
        public string ObligorEmail { get; set; }
        public string CurrentDate { get; set; }
        [BindProperty]
        public int RecordStatus { get; set; }

        public async Task SaveContractPerObligator()
        {
            using WordprocessingDocument wordDoc = WordprocessingDocument.Open(TemplatePath, true);
            {
                if (!File.Exists(SavedPath))
                {
                    wordDoc.SaveAs(SavedPath).Dispose();
                }
            }

            wordDoc.Dispose();
            await Task.Delay(1000);
        }

        public async Task SearchAndReplace()
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(SavedPath, true))
            {
                string docText = null;
                using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                Regex regexText = new Regex("ObligorName");
                docText = regexText.Replace(docText, ObligorName);

                Regex regexText1 = new Regex("SchoolName");
                docText = regexText1.Replace(docText, SchoolName);

                Regex regexText2 = new Regex("ProgramName");
                docText = regexText2.Replace(docText, ProgramName);

                Regex regexText3 = new Regex("AuthorityName");
                docText = regexText3.Replace(docText, AuthorityName);

                Regex regexText4 = new Regex("AuthorityPosition");
                docText = regexText4.Replace(docText, AuthorityPosition);

                Regex regexText7 = new Regex("ObligorAddress");
                docText = regexText7.Replace(docText, ObligorAddress);

                Regex regexText8 = new Regex("ObligorEmail");
                docText = regexText8.Replace(docText, ObligorEmail);

                Regex regexText9 = new Regex("CurrentDate");
                docText = regexText9.Replace(docText, CurrentDate);

                using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }

                wordDoc.Dispose();
                await Task.Delay(1000);
            }
        }

        public async Task DisposeFile(string file)
        {
            using WordprocessingDocument wordDoc = WordprocessingDocument.Open(file, true);
            {
               
            }

            wordDoc.Dispose();
            await Task.Delay(1000);
        }

    }
}
