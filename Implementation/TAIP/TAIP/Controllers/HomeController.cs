using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Text;
using System.Net;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Web.Mvc.Async;


namespace TAIP.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public async Task<ActionResult> Index()
        {
            string[] titles; int contor = 0; 
            HtmlDocument resultHtml = new HtmlDocument(); 
            string website = System.Configuration.ConfigurationManager.AppSettings["AnnalsJournal"] + "Archive";
            await GetBodyContent(website,resultHtml);
            List<HtmlNode> toftitle = resultHtml.DocumentNode.Descendants()
                .Where (x => (x.Name == "span" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("wikilink"))).ToList();
            titles = new string[toftitle.Count];
            foreach (HtmlNode node in toftitle)
            {
                titles[contor] = node.FirstChild.Attributes["href"].Value.ToString();
                contor++;
            }
            //Models.Volumes ob = new Models.Volumes();
            Models.Volumes.volms = titles;
            return View(Models.Volumes.volms);
        }

        public async Task<ActionResult> GetAllPapers()
        {
            List<String> links = new List<String>();
            foreach (var item in Models.Volumes.volms)
            {
                HtmlDocument resultHtml = new HtmlDocument();
                await GetBodyContent(System.Configuration.ConfigurationManager.AppSettings["infoiasi"] + item, resultHtml);
                List<HtmlNode> bibtex = resultHtml.DocumentNode.Descendants()
               .Where(x => (x.Name == "a" && x.InnerHtml == "BibTeX")).ToList();
                foreach (HtmlNode node in bibtex)
                {
                    links.Add(node.Attributes["href"].Value.ToString());
                }
            }
            Models.BibtexScs.bibteXs = links;
            return View();
        }

        public async Task DownloadInCsv()
        {
            HttpClient http = new HttpClient();
            StringBuilder myCsv = new StringBuilder();
            int pubNumbr = 0;
            myCsv.AppendFormat("\"{0}\",{1},{2},{3},{4},{5}","Pno","Title","Authors","Volume", "Year","Pages");
            foreach (var bibtex in Models.BibtexScs.bibteXs)
            {
                Models.BibtexScs.pubNmbr = pubNumbr;
                var response = await http.GetByteArrayAsync(System.Configuration.ConfigurationManager.AppSettings["infoiasi"] + bibtex);
                String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
                WriteInCSV(myCsv,source,pubNumbr);
                pubNumbr++;
            }
            HttpContext.Response.ContentType = "application/csv";
            HttpContext.Response.AddHeader("content-disposition", "attachment; filename=Papers.csv");
            HttpContext.Response.Write(myCsv.ToString());
        }

       
        public void WriteInCSV(StringBuilder _csv,string _response,int pubNumbr)
        {
            _csv.AppendLine(Environment.NewLine);
            _csv.AppendFormat("\"{0}\",{1},{2},{3},{4},{5}", Models.BibtexScs.pubNmbr, ValueNeeded(_response, "title"), ValueNeeded(_response, "author"), ValueNeeded(_response, "journal"), ValueNeeded(_response, "year"), ValueNeeded(_response, "pages"));
            
        }

        private async Task GetBodyContent(string website, HtmlDocument htmlText = null)
        {
            HttpClient http = new HttpClient();
            var response = await http.GetByteArrayAsync(website);
            String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
            source = WebUtility.HtmlDecode(source);
            if (htmlText != null)
                htmlText.LoadHtml(source);
        }
        public string ValueNeeded(string pattern,string search)
        {
            string searchFor = search + "={";
            int ix = pattern.IndexOf(searchFor);
            if (ix != -1)
            {
                string code = pattern.Substring(ix + searchFor.Length).ToString();
                var x = code.Split('}')[0];
                return x;
            }
            return "";
        }
    }
}
